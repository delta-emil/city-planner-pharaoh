using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace CityPlannerPharaoh;

public class MapModel
{
    public const int DefaultMapSize = 200;

    public MapModel(int mapSideX, int mapSideY, MapTerrain initTerrainType = MapTerrain.Grass, bool hasTooCloseToVoidToBuild = false)
    {
        this.MapSideX = mapSideX;
        this.MapSideY = mapSideY;
        this.HasTooCloseToVoidToBuild = hasTooCloseToVoidToBuild;

        this.Cells = new MapCellModel[mapSideX, mapSideY];

        for (int cellX = 0; cellX < MapSideX; cellX++)
        {
            for (int cellY = 0; cellY < MapSideY; cellY++)
            {
                this.Cells[cellX, cellY] = new MapCellModel { Terrain = initTerrainType };
            }
        }

        this.Buildings = new();
    }

    [JsonConstructor]
    public MapModel(int mapSideX, int mapSideY, bool hasTooCloseToVoidToBuild, MapCellModel[,] cells, List<MapBuilding> buildings)
        : this(mapSideX, mapSideY)
    {
        this.Cells = cells;
        this.Buildings = buildings;
        this.HasTooCloseToVoidToBuild = hasTooCloseToVoidToBuild;

        if (this.HasTooCloseToVoidToBuild)
        {
            this.SetTooCloseToVoidToBuildAfterInit();
        }

        foreach (var mapBuilding in this.Buildings)
        {
            SetBuildingInCells(mapBuilding);
            AddDesirabilityEffect(mapBuilding, 1);
        }
    }

    [JsonIgnore]
    public bool IsChanged { get; private set; }

    public int MapSideX { get; }
    public int MapSideY { get; }
    public bool HasTooCloseToVoidToBuild { get; }

    public MapCellModel[,] Cells { get; }

    public List<MapBuilding> Buildings { get; }

    [JsonIgnore]
    public List<ActionData> UndoStack { get; } = new();

    [JsonIgnore]
    public List<ActionData> RedoStack { get; } = new();

    // how does this affect undo/redo?
    public void ClearChanged()
    {
        IsChanged = false;
    }

    // subject to undo
    public void ChangeTerrain(MapCellModel cellModel, MapTerrain newTerrain)
    {
        this.UndoStack.Add(new ActionData { ActionType = ActionType.ChangeTerrain, OldTerrain = cellModel.Terrain });
        this.IsChanged = true;

        cellModel.Terrain = newTerrain;
    }

    // subject to undo
    public MapBuilding? AddBuilding(int left, int top, MapBuildingType mapBuildingType)
    {
        if (!this.CanAddBuilding(left, top, mapBuildingType))
        {
            return null;
        }

        var mapBuilding = new MapBuilding { Left = left, Top = top, BuildingType = mapBuildingType };

        this.UndoStack.Add(new ActionData { ActionType = ActionType.ChangeBuildings, AddedBuildings = [mapBuilding.GetCopy()] });
        this.IsChanged = true;

        this.Buildings.Add(mapBuilding);

        SetBuildingInCells(mapBuilding);
        AddDesirabilityEffect(mapBuilding, 1);

        return mapBuilding;
    }

    // subject to undo
    public void AddBuildingsWithOffset(HashSet<MapBuilding> buildingsToAdd, int cellX, int cellY)
    {
        var addedBuildings = new List<MapBuilding>(buildingsToAdd.Count);
        foreach (var building in buildingsToAdd)
        {
            var left = building.Left + cellX;
            var top = building.Top + cellY;

            if (!this.CanAddBuilding(left, top, building.BuildingType))
            {
                continue;
            }

            var mapBuilding = new MapBuilding { Left = left, Top = top, BuildingType = building.BuildingType };

            addedBuildings.Add(mapBuilding.GetCopy());

            this.Buildings.Add(mapBuilding);

            SetBuildingInCells(mapBuilding);
            AddDesirabilityEffect(mapBuilding, 1);
        }

        if (addedBuildings.Count > 0)
        {
            this.UndoStack.Add(new ActionData { ActionType = ActionType.ChangeBuildings, AddedBuildings = addedBuildings });
            this.IsChanged = true;
        }
    }

    public bool CanAddBuilding(int left, int top, MapBuildingType mapBuildingType, HashSet<MapBuilding>? ignoredBuildings = null)
    {
        var size = mapBuildingType.GetSize();
        var right = left + size.width - 1;
        var bottom = top + size.height - 1;

        if (left < 0 || top < 0 || right >= this.MapSideX || bottom >= this.MapSideY)
        {
            return false;
        }

        // check for existing building
        var tempMapBuilding = new MapBuilding { Left = left, Top = top, BuildingType = mapBuildingType };
        for (int cellX = left; cellX <= right; cellX++)
        {
            for (int cellY = top; cellY <= bottom; cellY++)
            {
                if (tempMapBuilding.IsEmptyCell(cellX, cellY))
                {
                    continue;
                }

                MapCellModel mapCellModel = this.Cells[cellX, cellY];
                if (mapCellModel.Terrain == MapTerrain.Void
                    || mapCellModel.TooCloseToVoidToBuild)
                {
                    return false;
                }

                var existingBuilding = mapCellModel.Building;
                if (existingBuilding != null && (ignoredBuildings == null || !ignoredBuildings.Contains(existingBuilding)))
                {
                    // plaza can overwrite Road
                    if (mapBuildingType == MapBuildingType.Plaza
                        && existingBuilding.BuildingType == MapBuildingType.Road)
                    {
                        continue;
                    }

                    return false;
                }
            }
        }

        return true;
    }

    private void SetBuildingInCells(MapBuilding mapBuilding)
    {
        var size = mapBuilding.BuildingType.GetSize();
        for (int cellX = mapBuilding.Left; cellX < mapBuilding.Left + size.width; cellX++)
        {
            for (int cellY = mapBuilding.Top; cellY < mapBuilding.Top + size.height; cellY++)
            {
                if (mapBuilding.IsEmptyCell(cellX, cellY))
                {
                    continue;
                }

                var mapCellModel = this.Cells[cellX, cellY];

                // only 1x1 things like roads can be overwritten
                var existingBuilding = mapCellModel.Building;
                if (existingBuilding != null)
                {
                    this.Buildings.Remove(existingBuilding);
                }

                mapCellModel.Building = mapBuilding;
            }
        }
    }

    // subject to undo
    public void RemoveBuilding(MapBuilding mapBuilding)
    {
        if (!this.Buildings.Remove(mapBuilding))
        {
            return;
        }

        this.UndoStack.Add(new ActionData { ActionType = ActionType.ChangeBuildings, RemovedBuildings = [mapBuilding.GetCopy()] });
        this.IsChanged = true;

        RemoveBuildingFromCells(mapBuilding);
        AddDesirabilityEffect(mapBuilding, -1);
    }

    // subject to undo
    public void RemoveBuildings(HashSet<MapBuilding> buildingsToRemove)
    {
        var removedBuildings = new List<MapBuilding>(buildingsToRemove.Count);
        foreach (var mapBuilding in buildingsToRemove)
        {
            if (!this.Buildings.Remove(mapBuilding))
            {
                continue;
            }

            removedBuildings.Add(mapBuilding.GetCopy());

            RemoveBuildingFromCells(mapBuilding);
            AddDesirabilityEffect(mapBuilding, -1);
        }

        if (removedBuildings.Count > 0)
        {
            this.UndoStack.Add(new ActionData { ActionType = ActionType.ChangeBuildings, RemovedBuildings = removedBuildings });
            this.IsChanged = true;
        }
    }

    private void RemoveBuildingFromCells(MapBuilding mapBuilding)
    {
        var size = mapBuilding.BuildingType.GetSize();

        for (int cellX = mapBuilding.Left; cellX < mapBuilding.Left + size.width; cellX++)
        {
            for (int cellY = mapBuilding.Top; cellY < mapBuilding.Top + size.height; cellY++)
            {
                if (mapBuilding.IsEmptyCell(cellX, cellY))
                {
                    continue;
                }

                var mapCellModel = this.Cells[cellX, cellY];

                mapCellModel.Building = null;
            }
        }
    }

    public List<MapBuilding> GetAllBuildingsInRectangle((int x, int y) startCell, (int x, int y) endCell)
    {
        var minX = Math.Min(startCell.x, endCell.x);
        var maxX = Math.Max(startCell.x, endCell.x);
        var minY = Math.Min(startCell.y, endCell.y);
        var maxY = Math.Max(startCell.y, endCell.y);

        var results = new List<MapBuilding>();

        foreach (var building in this.Buildings)
        {
            if (minX <= building.Left && minY <= building.Top)
            {
                var (lenX, lenY) = building.BuildingType.GetSize();
                if (building.Left + lenX - 1 <= maxX && building.Top + lenY - 1 <= maxY)
                {
                    results.Add(building);
                }
            }
        }

        return results;
    }

    public int GetHouseMaxDesirability(MapBuilding building)
    {
        Debug.Assert(building.BuildingType.GetCategory() == MapBuildingCategory.House);

        var size = building.BuildingType.GetSize();

        int maxDesire = int.MinValue;
        for (int cellX = building.Left; cellX < building.Left + size.width; cellX++)
        {
            for (int cellY = building.Top; cellY < building.Top + size.height; cellY++)
            {
                maxDesire = Math.Max(maxDesire, this.Cells[cellX, cellY].Desirability);
            }
        }
        return maxDesire;
    }

    // subject to undo
    public void MoveBuildingsByOffset(HashSet<MapBuilding> selectedBuildings, int offsetX, int offsetY)
    {
        if (offsetX == 0 || offsetY == 0)
        {
            return;
        }

        var removedBuildings = new List<MapBuilding>();
        var addedBuildings = new List<MapBuilding>();

        foreach (var building in selectedBuildings)
        {
            removedBuildings.Add(building.GetCopy());
            
            RemoveBuildingFromCells(building);
            AddDesirabilityEffect(building, -1);
        }

        foreach (var building in selectedBuildings)
        {
            building.Left += offsetX;
            building.Top += offsetY;
            addedBuildings.Add(building.GetCopy());

            SetBuildingInCells(building);
            building.HouseLevel = 0;
            AddDesirabilityEffect(building, 1);
        }

        this.IsChanged = true;
        this.UndoStack.Add(new ActionData { ActionType = ActionType.ChangeBuildings, RemovedBuildings = removedBuildings, AddedBuildings = addedBuildings });
    }

    private void AddDesirabilityEffect(MapBuilding mapBuilding, int multiplier)
    {

        if (DoLogging)
        {
            Console.WriteLine(new string('=', 40));
            Console.WriteLine(new string('=', 40));
            Console.WriteLine(new string('=', 40));
            Console.WriteLine($"-------- {(multiplier > 0 ? "adding" : "removing")} des from {mapBuilding.BuildingType} on {mapBuilding.Left}, {mapBuilding.Top}");
        }

        var affectedHouses = new HashSet<MapBuilding>();

        if (mapBuilding.BuildingType.GetCategory() == MapBuildingCategory.House && multiplier > 0)
        {
            affectedHouses.Add(mapBuilding);
        }
        else
        {
            var desireConfig = GetBuildingDesire(mapBuilding);
            if (desireConfig.Range > 0)
            {
                var oldState = DoLogging ? GetDesireData() : null;

                foreach (var (cell, _, _, distance) in EnumerateAroundBuildingToRange(mapBuilding, desireConfig.Range, includingInside: false))
                {
                    var delta = (desireConfig.Start + (distance - 1) / desireConfig.StepRange * desireConfig.StepDiff) * multiplier;

                    cell.Desirability += delta;

                    if (cell.Building?.BuildingType.GetCategory() == MapBuildingCategory.House)
                    {
                        affectedHouses.Add(cell.Building);
                    }
                }

                if (DoLogging)
                {
                    ShowDesirabilityDiff(oldState!);
                }
            }
        }

        int loopCount = 0;
        while (affectedHouses.Count > 0)
        {
            if (DoLogging)
            {
                Console.WriteLine($"~~~~~~~~ looping on {affectedHouses.Count} affected houses");
            }

            if (++loopCount > 10_000)
            {
                throw new Exception("House desirability loop went on for 10k iterations. Probably buggy.");
            }

            var newlyAffectedHouses = new HashSet<MapBuilding>();

            foreach (var affectedHouse in affectedHouses)
            {
                var oldHouseLevel = affectedHouse.HouseLevel;

                var maxDesire = this.GetHouseMaxDesirability(affectedHouse);
                var newHouseLevel = HouseLevelData.GetHouseLevel(maxDesire);
                affectedHouse.HouseLevel = newHouseLevel;

                if (oldHouseLevel != newHouseLevel)
                {
                    if (DoLogging)
                    {
                        Console.WriteLine($"@@@@@@@@ house on looping on {affectedHouse.Left}, {affectedHouse.Top} chaning level {oldHouseLevel} -> {newHouseLevel}");
                    }

                    var houseSize = affectedHouse.BuildingType.GetSize().width;

                    var oldDesireConfig = HouseLevelData.GetDesire(oldHouseLevel, houseSize);
                    var newDesireConfig = HouseLevelData.GetDesire(newHouseLevel, houseSize);
                    if (!oldDesireConfig.Equals(newDesireConfig))
                    {
                        AdjustHouseDesirabilityEffect(affectedHouse, oldDesireConfig, newDesireConfig, newlyAffectedHouses);
                    }
                }
            }

            affectedHouses = newlyAffectedHouses;
        }

        foreach (var subBuilding in mapBuilding.GetSubBuildings())
        {
            AddDesirabilityEffect(subBuilding, multiplier);
        }
    }

    private void AdjustHouseDesirabilityEffect(MapBuilding mapBuilding, DesireConfig oldDesireConfig, DesireConfig newDesireConfig, HashSet<MapBuilding> newlyAffectedHouses)
    {
        Debug.Assert(mapBuilding.BuildingType.GetCategory() == MapBuildingCategory.House);

        if (DoLogging)
        {
            Console.WriteLine($"++++++++ adjusting des from house on {mapBuilding.Left}, {mapBuilding.Top}");
        }

        var maxRange = Math.Max(oldDesireConfig.Range, newDesireConfig.Range);
        if (maxRange <= 0)
        {
            return;
        }

        var oldState = DoLogging ? GetDesireData() : null;

        foreach (var (cell, _, _, distance) in EnumerateAroundBuildingToRange(mapBuilding, maxRange, includingInside: false))
        {
            var deltaOld = distance <= oldDesireConfig.Range ? (oldDesireConfig.Start + (distance - 1) / oldDesireConfig.StepRange * oldDesireConfig.StepDiff) : 0;
            var deltaNew = distance <= newDesireConfig.Range ? (newDesireConfig.Start + (distance - 1) / newDesireConfig.StepRange * newDesireConfig.StepDiff) : 0;

            var delta = deltaNew - deltaOld;

            cell.Desirability += delta;

            if (cell.Building?.BuildingType.GetCategory() == MapBuildingCategory.House)
            {
                newlyAffectedHouses.Add(cell.Building);
            }
        }

        if (DoLogging)
        {
            ShowDesirabilityDiff(oldState!);
        }
    }

    public bool IsFarmIrrigated(MapBuilding farm)
    {
        const int IrrigationRange = 2;
        foreach (var (cell, _, _, _) in EnumerateAroundBuildingToRange(farm, IrrigationRange, includingInside: false))
        {
            if (cell.Building?.BuildingType == MapBuildingType.Ditch)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerable<(MapCellModel cell, int cellX, int cellY, int distance)> EnumerateAroundBuildingToRange(MapBuilding mapBuilding, int range, bool includingInside)
    {
        var size = mapBuilding.BuildingType.GetSize();
        return EnumerateAroundBuildingToRange(mapBuilding.Left, mapBuilding.Top, size.width, size.height, range, includingInside);
    }

    private IEnumerable<(MapCellModel cell, int cellX, int cellY, int distance)> EnumerateAroundBuildingToRange(int left, int top, int width, int height, int range, bool includingInside)
    {
        var minX = Math.Max(0, left - range);
        var maxX = Math.Min(this.MapSideX - 1, left + width - 1 + range);

        var minY = Math.Max(0, top - range);
        var maxY = Math.Min(this.MapSideY - 1, top + height - 1 + range);

        for (int cellX = minX; cellX <= maxX; cellX++)
        {
            for (int cellY = minY; cellY <= maxY; cellY++)
            {
                var distance = range - Math.Min(
                    Math.Min(Math.Abs(cellX - minX), Math.Abs(cellX - maxX)),
                    Math.Min(Math.Abs(cellY - minY), Math.Abs(cellY - maxY)));
                if (distance > 0 || includingInside)
                {
                    yield return (this.Cells[cellX, cellY], cellX, cellY, distance);
                }
            }
        }
    }

    public void SetTooCloseToVoidToBuildAfterInit()
    {
        for (int cellX = 0; cellX < this.MapSideX; cellX++)
        {
            for (int cellY = 0; cellY < this.MapSideY; cellY++)
            {
                if (this.Cells[cellX, cellY].Terrain == MapTerrain.Void)
                {
                    // check for distance to the edge of the map
                    for (var d = 1; d <= 3; d++)
                    {
                        if (cellX - d >= 0) this.Cells[cellX - d, cellY].TooCloseToVoidToBuild = true;
                        if (cellX + d < this.MapSideX) this.Cells[cellX + d, cellY].TooCloseToVoidToBuild = true;
                        if (cellY - d >= 0) this.Cells[cellX, cellY - d].TooCloseToVoidToBuild = true;
                        if (cellY + d < this.MapSideY) this.Cells[cellX, cellY + d].TooCloseToVoidToBuild = true;
                    }
                }
            }
        }
    }

    public static DesireConfig GetBuildingDesire(MapBuilding mapBuilding)
    {
        if (mapBuilding.BuildingType.GetCategory() == MapBuildingCategory.House)
        {
            return HouseLevelData.GetDesire(mapBuilding.HouseLevel, mapBuilding.BuildingType.GetSize().width);
        }
        else
        {
            return mapBuilding.BuildingType.GetDesire();
        }
    }

    public bool DoLogging { get; set; }

    // for debug
    public int[,] GetDesireData()
    {
        var result = new int[this.MapSideX, this.MapSideY];
        for (int row = 0; row < this.MapSideX; row++)
        {
            for (int col = 0; col < this.MapSideY; col++)
            {
                result[row, col] = this.Cells[row, col].Desirability;
            }
        }
        return result;
    }

    // for debug
    public void ShowDesirabilityState()
    {
        for (int row = 0; row < this.MapSideX; row++)
        {
            Console.Write("        { ");
            for (int col = 0; col < this.MapSideY; col++)
            {
                Console.Write($"{this.Cells[row, col].Desirability,2}, ");
            }
            Console.WriteLine("},");
        }
    }

    // for debug
    public void ShowDesirabilityDiff(int[,] oldState)
    {
        for (int row = 0; row < this.MapSideX; row++)
        {
            Console.Write("        { ");
            for (int col = 0; col < this.MapSideY; col++)
            {
                var diff = this.Cells[row, col].Desirability - oldState[row, col];
                Console.Write($"{diff,3}, ");
            }
            Console.WriteLine("},");
        }
    }
}

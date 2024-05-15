using System;
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
    public bool IsChanged { get; set; }

    public int MapSideX { get; }
    public int MapSideY { get; }
    public bool HasTooCloseToVoidToBuild { get; }

    public MapCellModel[,] Cells { get; }

    public List<MapBuilding> Buildings { get; }

    public MapBuilding? AddBuilding(int left, int top, MapBuildingType mapBuildingType)
    {
        if (!CanAddBuilding(left, top, mapBuildingType))
        {
            return null;
        }

        var mapBuilding = new MapBuilding { Left = left, Top = top, BuildingType = mapBuildingType };

        this.Buildings.Add(mapBuilding);

        SetBuildingInCells(mapBuilding);
        AddDesirabilityEffect(mapBuilding, 1);

        return mapBuilding;
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
        for (int cellX = left; cellX <= right; cellX++)
        {
            for (int cellY = top; cellY <= bottom; cellY++)
            {
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

    public void RemoveBuilding(MapBuilding mapBuilding)
    {
        if (!this.Buildings.Remove(mapBuilding))
        {
            return;
        }

        RemoveBuildingFromCells(mapBuilding);
        AddDesirabilityEffect(mapBuilding, -1);
    }

    private void RemoveBuildingFromCells(MapBuilding mapBuilding)
    {
        var size = mapBuilding.BuildingType.GetSize();

        for (int cellX = mapBuilding.Left; cellX < mapBuilding.Left + size.width; cellX++)
        {
            for (int cellY = mapBuilding.Top; cellY < mapBuilding.Top + size.height; cellY++)
            {
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

    public void MoveBuildingsByOffset(HashSet<MapBuilding> selectedBuildings, int offsetX, int offsetY)
    {
        foreach (var building in selectedBuildings)
        {
            RemoveBuildingFromCells(building);
            AddDesirabilityEffect(building, -1);
        }

        foreach (var building in selectedBuildings)
        {
            building.Left += offsetX;
            building.Top += offsetY;
            SetBuildingInCells(building);
            AddDesirabilityEffect(building, 1);
        }
    }

    private void AddDesirabilityEffect(MapBuilding mapBuilding, int multiplier)
    {
        var desireConfig = mapBuilding.BuildingType.GetDesire();
        if (desireConfig.range > 0)
        {
            foreach (var (cell, _, _, distance) in EnumerateAroundBuildingToRange(mapBuilding, desireConfig.range, includingInside: false))
            {
                var delta = (desireConfig.start + (distance - 1) / desireConfig.stepRange * desireConfig.stepDiff) * multiplier;

                cell.Desirability += delta;
            }
        }

        foreach (var subBuilding in mapBuilding.GetSubBuildings())
        {
            AddDesirabilityEffect(subBuilding, multiplier);
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

    public int MinNotableDesirability => -17;
    public int MaxNotableDesirability => 92;

    private static readonly int[] HouseEvolveBoundsHard = new[] { -10, -5, 0, 4, 8, 12, 16, 20, 25, 32, 40, 48, 53, 58, 63, 68, 74, 80, 90 };

    public string GetMaxHouseLevelLabel(int maxDesire)
    {
        // TODO: this is for Hard, make an option for what difficulty to show
        int level = 1;
        for (int bound = 0; bound < HouseEvolveBoundsHard.Length; bound++)
        {
            if (maxDesire >= HouseEvolveBoundsHard[bound])
            {
                level++;
            }
            else
            {
                break;
            }
        }

        return "H" + level;
    }
}

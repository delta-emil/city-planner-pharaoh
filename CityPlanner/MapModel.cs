using System.Text.Json.Serialization;

namespace CityPlanner;

public class MapModel
{
    public const int DefaultMapSize = 200;

    public MapModel(int mapSideX, int mapSideY)
    {
        this.MapSideX = mapSideX;
        this.MapSideY = mapSideY;

        this.Cells = new MapCellModel[mapSideX, mapSideY];

        for (int cellX = 0; cellX < MapSideX; cellX++)
        {
            for (int cellY = 0; cellY < MapSideY; cellY++)
            {
                this.Cells[cellX, cellY] = new MapCellModel();
            }
        }

        this.Buildings = new();
    }

    [JsonConstructor]
    public MapModel(int mapSideX, int mapSideY, MapCellModel[,] cells, List<MapBuilding> buildings)
        : this(mapSideX, mapSideY)
    {
        this.Cells = cells;
        this.Buildings = buildings;

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
        if (left < 0 || top < 0 || left + size.width > this.MapSideX || top + size.height > this.MapSideY)
        {
            return false;
        }

        // check for existing building
        for (int cellX = left; cellX < left + size.width; cellX++)
        {
            for (int cellY = top; cellY < top + size.height; cellY++)
            {
                var existingBuilding = this.Cells[cellX, cellY].Building;
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
            var size = mapBuilding.BuildingType.GetSize();

            var minX = Math.Max(0, mapBuilding.Left - desireConfig.range);
            var maxX = Math.Min(this.MapSideX - 1, mapBuilding.Left + size.width - 1 + desireConfig.range);

            var minY = Math.Max(0, mapBuilding.Top - desireConfig.range);
            var maxY = Math.Min(this.MapSideY - 1, mapBuilding.Top + size.height - 1 + desireConfig.range);

            for (int cellX = minX; cellX <= maxX; cellX++)
            {
                for (int cellY = minY; cellY <= maxY; cellY++)
                {
                    var distance = desireConfig.range - Math.Min(
                        Math.Min(Math.Abs(cellX - minX), Math.Abs(cellX - maxX)),
                        Math.Min(Math.Abs(cellY - minY), Math.Abs(cellY - maxY)));
                    if (distance > 0)
                    {
                        var delta = (desireConfig.start + (distance - 1) / desireConfig.stepRange * desireConfig.stepDiff) * multiplier;

                        this.Cells[cellX, cellY].Desirability += delta;
                    }
                }
            }
        }

        foreach (var subBuilding in mapBuilding.GetSubBuildings())
        {
            AddDesirabilityEffect(subBuilding, multiplier);
        }
    }
}

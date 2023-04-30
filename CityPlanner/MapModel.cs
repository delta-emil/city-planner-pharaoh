namespace CityPlanner;

public class MapModel
{
    public MapModel(int mapSideLength)
    {
        this.MapSideLength = mapSideLength;

        this.Cells = new MapCellModel[mapSideLength, mapSideLength];

        for (int cellX = 0; cellX < MapSideLength; cellX++)
        {
            for (int cellY = 0; cellY < MapSideLength; cellY++)
            {
                this.Cells[cellX, cellY] = new MapCellModel();
            }
        }

        this.Buildings = new();
    }

    public int MapSideLength { get; }

    public MapCellModel[,] Cells { get; }

    public List<MapBuilding> Buildings { get; }

    public MapBuilding? AddBuilding(int left, int top, MapBuildingType mapBuildingType)
    {
        var (width, height) = mapBuildingType.GetSize();
        if (left < 0 || top < 0 || left + width > this.MapSideLength || top + height > this.MapSideLength)
            return null;

        var mapBuilding = new MapBuilding { Left = left, Top = top, BuildingType = mapBuildingType };

        // check for existing building
        for (int cellX = left; cellX < left + width; cellX++)
        {
            for (int cellY = top; cellY < top + height; cellY++)
            {
                var existingBuilding = this.Cells[cellX, cellY].Building;
                if (existingBuilding != null)
                {
                    // plaza can overwrite Road
                    if (mapBuildingType == MapBuildingType.Plaza
                        && existingBuilding.BuildingType == MapBuildingType.Road)
                        continue;

                    return null;
                }
            }
        }

        this.Buildings.Add(mapBuilding);

        for (int cellX = left; cellX < left + width; cellX++)
        {
            for (int cellY = top; cellY < top + height; cellY++)
            {
                var mapCellModel = this.Cells[cellX, cellY];

                mapCellModel.Building = mapBuilding;
            }
        }

        return mapBuilding;
    }

    public void RemoveBuilding(MapBuilding mapBuilding)
    {
        if (!this.Buildings.Remove(mapBuilding))
            return;

        var (width, height) = mapBuilding.BuildingType.GetSize();

        for (int cellX = mapBuilding.Left; cellX < mapBuilding.Left + width; cellX++)
        {
            for (int cellY = mapBuilding.Top; cellY < mapBuilding.Top + height; cellY++)
            {
                var mapCellModel = this.Cells[cellX, cellY];

                mapCellModel.Building = null;
            }
        }
    }
}

using System.Text.Json;

namespace CityPlannerPharaoh.FileFormat;

internal static class ReaderV3
{
    public static MapModel Read(JsonElement rootElement)
    {
        // --- central properties ---

        int mapSideX = rootElement.GetProperty(nameof(MapModel.MapSideX)).GetInt32();
        int mapSideY = rootElement.GetProperty(nameof(MapModel.MapSideY)).GetInt32();
        bool hasTooCloseToVoidToBuild = rootElement.GetProperty(nameof(MapModel.HasTooCloseToVoidToBuild)).GetBoolean();
        Difficulty? savedDifficulty;
        if (rootElement.TryGetProperty("SavedDifficulty", out var difficultyElement))
        {
            savedDifficulty = Reader.ParseEnum<Difficulty>(difficultyElement);
        }
        else
        {
            savedDifficulty = null;
        }

        // --- cells ---

        JsonElement cellsElement = rootElement.GetProperty(nameof(MapModel.Cells));
        if (cellsElement.GetArrayLength() != mapSideX)
        {
            throw new JsonException($"Unexpected number of rows '{cellsElement.GetArrayLength()}'");
        }
        MapCellModel[,] cells = new MapCellModel[mapSideX, mapSideY];
        for (int x = 0; x < mapSideX; x++)
        {
            JsonElement row = cellsElement[x];
            if (row.GetArrayLength() != mapSideY)
            {
                throw new JsonException($"Unexpected number of columns '{row.GetArrayLength()}'");
            }

            for (int y = 0; y < mapSideY; y++)
            {
                var terrain = Reader.ParseEnum<MapTerrain>(row[y]);
                cells[x, y] = new MapCellModel { Terrain = terrain };
            }
        }

        // --- buildings ---

        JsonElement buildingsElement = rootElement.GetProperty(nameof(MapModel.Buildings));
        List<MapBuilding> buildings = new(buildingsElement.GetArrayLength());
        foreach (JsonElement buildingElement in buildingsElement.EnumerateArray())
        {
            MapBuilding mapBuilding = ReadBuilding(buildingElement);
            buildings.Add(mapBuilding);
        }

        // --- compose and return ---

        return new MapModel(
            mapSideX,
            mapSideY,
            hasTooCloseToVoidToBuild,
            cells,
            buildings,
            savedDifficulty);
    }

    private static MapBuilding ReadBuilding(JsonElement buildingElement)
    {
        int left = buildingElement.GetProperty(nameof(MapBuilding.Left)).GetInt32();
        int top = buildingElement.GetProperty(nameof(MapBuilding.Top)).GetInt32();
        var buildingType = Reader.ParseEnum<MapBuildingType>(buildingElement.GetProperty(nameof(MapBuilding.BuildingType)));
        var mapBuilding = new MapBuilding
        {
            Left = left,
            Top = top,
            BuildingType = buildingType,
        };

        if (buildingType.GetCategory() == MapBuildingCategory.House)
        {
            mapBuilding.MaxHouseLevel = buildingElement.GetProperty(nameof(MapBuilding.MaxHouseLevel)).GetInt32();
        }

        if (buildingElement.TryGetProperty(nameof(MapBuilding.SubBuildings), out var subBuildingsElement))
        {
            mapBuilding.SubBuildings = new(subBuildingsElement.GetArrayLength());
            foreach (JsonElement subBuildingElement in subBuildingsElement.EnumerateArray())
            {
                MapBuilding subBuilding = ReadBuilding(subBuildingElement);
                mapBuilding.SubBuildings.Add(subBuilding);
            }
        }

        return mapBuilding;
    }
}

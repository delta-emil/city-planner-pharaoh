using System.Text.Json;

namespace CityPlannerPharaoh.FileFormat;

internal class Reader
{
    public static MapModel Read(Stream inputStream)
    {
        JsonDocument jsonDocument = JsonDocument.Parse(inputStream, new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });

        var rootElement = jsonDocument.RootElement;

        int version;
        if (rootElement.TryGetProperty("Version", out var versionElement))
        {
            version = versionElement.GetInt32();
        }
        else
        {
            version = 1;
        }

        switch (version)
        {
            case 1: return ReadVersion1(rootElement);

            default: throw new Exception($"Unsupported file version: {version}. The file may have been created by a newer version of this program.");
         }
    }

    private static MapModel ReadVersion1(JsonElement rootElement)
    {
        // --- central properties ---

        int mapSideX = rootElement.GetProperty(nameof(MapModel.MapSideX)).GetInt32();
        int mapSideY = rootElement.GetProperty(nameof(MapModel.MapSideY)).GetInt32();
        bool hasTooCloseToVoidToBuild = rootElement.GetProperty(nameof(MapModel.HasTooCloseToVoidToBuild)).GetBoolean();
        Difficulty? savedDifficulty;
        if (rootElement.TryGetProperty("SavedDifficulty", out var difficultyElement))
        {
            savedDifficulty = ParseEnum<Difficulty>(difficultyElement);
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
                var terrain = ParseEnum<MapTerrain>(row[y]);
                cells[x, y] = new MapCellModel { Terrain = terrain };
            }
        }

        // --- buildings ---

        JsonElement buildingsElement = rootElement.GetProperty(nameof(MapModel.Buildings));
        List<MapBuilding> buildings = new(buildingsElement.GetArrayLength());
        foreach (JsonElement buildElement in buildingsElement.EnumerateArray())
        {
            int left = buildElement.GetProperty(nameof(MapBuilding.Left)).GetInt32();
            int top = buildElement.GetProperty(nameof(MapBuilding.Top)).GetInt32();
            var buildingType = ParseEnum<MapBuildingType>(buildElement.GetProperty(nameof(MapBuilding.BuildingType)));
            var mapBuilding = new MapBuilding
            {
                Left = left,
                Top = top,
                BuildingType = buildingType,
            };
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

    public static T ParseEnum<T>(JsonElement valueElement) where T : struct, Enum
    {
        var rawName = valueElement.ToString();
        return Enum.Parse<T>(rawName, ignoreCase: true);
    }
}

using System.Text.Json;

namespace CityPlannerPharaoh.FileFormat;

internal static class ReaderV1
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
        foreach (JsonElement buildElement in buildingsElement.EnumerateArray())
        {
            int left = buildElement.GetProperty(nameof(MapBuilding.Left)).GetInt32();
            int top = buildElement.GetProperty(nameof(MapBuilding.Top)).GetInt32();

            var buildingTypeV1 = buildElement.GetProperty(nameof(MapBuilding.BuildingType)).GetString() ?? throw new Exception("Missing BuildingType.");
            buildingTypeV1 = buildingTypeV1.ToLowerInvariant();

            var buildingType = ParseBuildingType(buildingTypeV1);
            var subBuildings = GetSubBuildings(buildingTypeV1, left: left, top: top);

            if (buildingType.GetCategory() == MapBuildingCategory.Venue && subBuildings != null)
            {
                AddRoadsUnderVenue(buildings, buildingType, left, top, subBuildings);
            }

            var mapBuilding = new MapBuilding
            {
                Left = left,
                Top = top,
                BuildingType = buildingType,
                SubBuildings = subBuildings,
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

    private static void AddRoadsUnderVenue(List<MapBuilding> buildings, MapBuildingType buildingType, int left, int top, List<MapBuilding> subBuildings)
    {
        var size = buildingType.GetSize();
        int right = left + size.width;
        int bottom = top + size.height;
        for (int cellX = left; cellX < right; cellX++)
        {
            for (int cellY = top; cellY < bottom; cellY++)
            {
                bool occupied = false;
                foreach (var subBuilding in subBuildings)
                {
                    var subBuildingSize = subBuilding.BuildingType.GetSize();
                    if (subBuilding.Left <= cellX && cellX < subBuilding.Left + subBuildingSize.width
                        && subBuilding.Top <= cellY && cellY < subBuilding.Top + subBuildingSize.height)
                    {
                        occupied = true;
                        break;
                    }
                }

                if (!occupied)
                {
                    buildings.Add(new MapBuilding
                    {
                        Left = cellX,
                        Top = cellY,
                        BuildingType = MapBuildingType.Road,
                    });
                }
            }
        }
    }

    private static MapBuildingType ParseBuildingType(string buildingTypeV1)
    {
        return buildingTypeV1 switch
        {
            "booth1" => MapBuildingType.Booth,
            "booth2" => MapBuildingType.Booth,
            "booth3" => MapBuildingType.Booth,
            "booth4" => MapBuildingType.Booth,

            "bandstand1" => MapBuildingType.Bandstand,
            "bandstand2" => MapBuildingType.Bandstand,
            "bandstand3" => MapBuildingType.Bandstand,
            "bandstand4" => MapBuildingType.Bandstand,

            "pavilion1" => MapBuildingType.Pavilion,
            "pavilion2" => MapBuildingType.Pavilion,
            "pavilion3" => MapBuildingType.Pavilion,
            "pavilion4" => MapBuildingType.Pavilion,
            "pavilion5" => MapBuildingType.Pavilion,
            "pavilion6" => MapBuildingType.Pavilion,
            "pavilion7" => MapBuildingType.Pavilion,
            "pavilion8" => MapBuildingType.Pavilion,
            "pavilion9" => MapBuildingType.Pavilion,
            "pavilion10" => MapBuildingType.Pavilion,
            "pavilion11" => MapBuildingType.Pavilion,

            _ => Enum.Parse<MapBuildingType>(buildingTypeV1, ignoreCase: true),
        };
    }

    private static List<MapBuilding>? GetSubBuildings(string buildingTypeV1, int left, int top)
    {
        return buildingTypeV1 switch
        {
            "booth1" => [new() { Left = left,     Top = top,     BuildingType = MapBuildingType.JuggleStage }],
            "booth2" => [new() { Left = left + 1, Top = top,     BuildingType = MapBuildingType.JuggleStage }],
            "booth3" => [new() { Left = left,     Top = top + 1, BuildingType = MapBuildingType.JuggleStage }],
            "booth4" => [new() { Left = left + 1, Top = top + 1, BuildingType = MapBuildingType.JuggleStage }],

            "bandstand1" =>
            [
                new() { Left = left,     Top = top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = left + 1, Top = top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top + 2, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 1, Top = top + 2, BuildingType = MapBuildingType.Garden },
            ],
            "bandstand2" =>
            [
                new() { Left = left,     Top = top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left + 2, Top = top,     BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 2, Top = top + 1, BuildingType = MapBuildingType.Garden },
            ],
            "bandstand3" =>
            [
                new() { Left = left + 1, Top = top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = left + 2, Top = top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = left + 2, Top = top + 2, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 1, Top = top + 2, BuildingType = MapBuildingType.Garden },
            ],
            "bandstand4" =>
            [
                new() { Left = left,     Top = top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top + 2, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left + 2, Top = top + 2, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 2, Top = top + 1, BuildingType = MapBuildingType.Garden },
            ],

            "pavilion1" =>
            [
                new() { Left = left,     Top = top,     BuildingType = MapBuildingType.DanceStage },
                new() { Left = left + 3, Top = top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = left + 3, Top = top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top + 3, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 1, Top = top + 3, BuildingType = MapBuildingType.Garden },
                new() { Left = left + 3, Top = top + 3, BuildingType = MapBuildingType.Garden },
            ],
            "pavilion2" =>
            [
                new() { Left = left + 2, Top = top,     BuildingType = MapBuildingType.DanceStage },
                new() { Left = left,     Top = top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top + 3, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 2, Top = top + 3, BuildingType = MapBuildingType.Garden },
                new() { Left = left + 3, Top = top + 3, BuildingType = MapBuildingType.Garden },
            ],
            "pavilion3" =>
            [
                new() { Left = left + 2, Top = top + 2, BuildingType = MapBuildingType.DanceStage },
                new() { Left = left,     Top = top + 2, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top + 3, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top,     BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 2, Top = top,     BuildingType = MapBuildingType.Garden },
                new() { Left = left + 3, Top = top,     BuildingType = MapBuildingType.Garden },
            ],
            "pavilion4" =>
            [
                new() { Left = left,     Top = top + 2, BuildingType = MapBuildingType.DanceStage },
                new() { Left = left + 3, Top = top + 2, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left + 3, Top = top + 3, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top,     BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 1, Top = top,     BuildingType = MapBuildingType.Garden },
                new() { Left = left + 3, Top = top,     BuildingType = MapBuildingType.Garden },
            ],
            "pavilion5" =>
            [
                new() { Left = left,     Top = top,     BuildingType = MapBuildingType.DanceStage },
                new() { Left = left + 3, Top = top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = left + 3, Top = top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top + 2, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 1, Top = top + 2, BuildingType = MapBuildingType.Garden },
                new() { Left = left + 3, Top = top + 2, BuildingType = MapBuildingType.Garden },
            ],
            "pavilion6" =>
            [
                new() { Left = left + 2, Top = top,     BuildingType = MapBuildingType.DanceStage },
                new() { Left = left,     Top = top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top + 2, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 2, Top = top + 2, BuildingType = MapBuildingType.Garden },
                new() { Left = left + 3, Top = top + 2, BuildingType = MapBuildingType.Garden },
            ],
            "pavilion7" =>
            [
                new() { Left = left + 1, Top = top,     BuildingType = MapBuildingType.DanceStage },
                new() { Left = left + 3, Top = top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = left + 3, Top = top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left + 3, Top = top + 3, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 1, Top = top + 3, BuildingType = MapBuildingType.Garden },
                new() { Left = left + 2, Top = top + 3, BuildingType = MapBuildingType.Garden },
            ],
            "pavilion8" =>
            [
                new() { Left = left + 1, Top = top + 2, BuildingType = MapBuildingType.DanceStage },
                new() { Left = left + 3, Top = top + 2, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left + 3, Top = top + 3, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left + 3, Top = top,     BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 1, Top = top,     BuildingType = MapBuildingType.Garden },
                new() { Left = left + 2, Top = top,     BuildingType = MapBuildingType.Garden },
            ],
            "pavilion9" =>
            [
                new() { Left = left + 2, Top = top + 1, BuildingType = MapBuildingType.DanceStage },
                new() { Left = left,     Top = top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top + 2, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top + 3, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 2, Top = top + 3, BuildingType = MapBuildingType.Garden },
                new() { Left = left + 3, Top = top + 3, BuildingType = MapBuildingType.Garden },
            ],
            "pavilion10" =>
            [
                new() { Left = left,     Top = top + 1, BuildingType = MapBuildingType.DanceStage },
                new() { Left = left + 3, Top = top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left + 3, Top = top + 2, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top + 3, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 1, Top = top + 3, BuildingType = MapBuildingType.Garden },
                new() { Left = left + 3, Top = top + 3, BuildingType = MapBuildingType.Garden },
            ],
            "pavilion11" =>
            [
                new() { Left = left,     Top = top,     BuildingType = MapBuildingType.DanceStage },
                new() { Left = left + 2, Top = top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = left + 2, Top = top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = left,     Top = top + 3, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = left + 1, Top = top + 3, BuildingType = MapBuildingType.Garden },
                new() { Left = left + 2, Top = top + 3, BuildingType = MapBuildingType.Garden },
            ],

            _ => null,
        };
    }
}

using System.Text.Json;

namespace CityPlannerPharaoh.FileFormat;

internal static class Writer
{
    public static void Write(Stream outputStream, MapModel mapModel)
    {
        using Utf8JsonWriter writer = new(outputStream, new JsonWriterOptions { Indented = true });
        writer.WriteStartObject();
        writer.WriteNumber("Version", 2);
        writer.WriteNumber(nameof(MapModel.MapSideX), mapModel.MapSideX);
        writer.WriteNumber(nameof(MapModel.MapSideY), mapModel.MapSideY);
        writer.WriteBoolean(nameof(MapModel.HasTooCloseToVoidToBuild), mapModel.HasTooCloseToVoidToBuild);
        writer.WriteString("SavedDifficulty", EnumToString(mapModel.EffectiveDifficulty));

        writer.WriteStartArray(nameof(MapModel.Cells));
        for (int x = 0; x < mapModel.MapSideX; x++)
        {
            writer.WriteStartArray();
            for (int y = 0; y < mapModel.MapSideY; y++)
            {
                writer.WriteStringValue(EnumToString(mapModel.Cells[x, y].Terrain));
            }
            writer.WriteEndArray();
        }
        writer.WriteEndArray();

        writer.WriteStartArray(nameof(MapModel.Buildings));
        foreach (var building in mapModel.Buildings)
        {
            WriteBuilding(writer, building);
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }

    private static void WriteBuilding(Utf8JsonWriter writer, MapBuilding building)
    {
        writer.WriteStartObject();
        writer.WriteNumber(nameof(MapBuilding.Left), building.Left);
        writer.WriteNumber(nameof(MapBuilding.Top), building.Top);
        writer.WriteString(nameof(MapBuilding.BuildingType), EnumToString(building.BuildingType));
        if (building.SubBuildings != null)
        {
            writer.WriteStartArray(nameof(MapBuilding.SubBuildings));
            foreach (var subBuilding in building.SubBuildings)
            {
                WriteBuilding(writer, subBuilding);
            }
            writer.WriteEndArray();
        }
        writer.WriteEndObject();
    }

    public static string EnumToString(Enum value)
    {
        var rawName = value.ToString();
        return ToCamelCase(rawName);
    }

    public static string ToCamelCase(string value)
    {
        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}

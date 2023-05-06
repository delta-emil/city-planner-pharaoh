using System.Text.Json;
using System.Text.Json.Serialization;

namespace CityPlannerPharaoh;

internal class MapCellsJsonConverter : JsonConverter<MapCellModel[,]>
{
    public override void Write(Utf8JsonWriter writer, MapCellModel[,] value, JsonSerializerOptions options)
    {
        var itemConverter = (JsonConverter<MapTerrain>)options.GetConverter(typeof(MapTerrain));

        writer.WriteStartArray();
        
        for (int i = 0; i < value.GetLength(0); i++)
        {
            writer.WriteStartArray();

            for (int j = 0; j < value.GetLength(1); j++)
            {
                itemConverter.Write(writer, value[i, j].Terrain, options);
            }

            writer.WriteEndArray();
        }

        writer.WriteEndArray();
    }

    public override MapCellModel[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);

        JsonElement root = document.RootElement;

        int rowCount = root.GetArrayLength();
        if (rowCount == 0)
        {
            return new MapCellModel[0, 0];
        }

        int columnCount = root[0].GetArrayLength();

        var result = new MapCellModel[rowCount, columnCount];

        for (int i = 0; i < rowCount; i++)
        {
            JsonElement row = root[i];
            if (row.GetArrayLength() != columnCount)
            {
                throw new JsonException($"Unexpected number of columns '{row.GetArrayLength()}'");
            }

            for (int j = 0; j < columnCount; j++)
            {
                var terrain = JsonSerializer.Deserialize<MapTerrain>(row[j].GetRawText(), options);
                result[i, j] = new MapCellModel { Terrain = terrain };
            }
        }

        return result;
    }
}

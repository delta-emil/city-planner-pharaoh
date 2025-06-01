using System.Text.Json;

namespace CityPlannerPharaoh.FileFormat;

internal static class Reader
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
            case 1: return ReaderV1.Read(rootElement);
            case 2: return ReaderV2.Read(rootElement);
            case 3: return ReaderV3.Read(rootElement);

            default: throw new Exception($"Unsupported file version: {version}. The file may have been created by a newer version of this program.");
         }
    }

    public static T ParseEnum<T>(JsonElement valueElement) where T : struct, Enum
    {
        var rawName = valueElement.ToString();
        return Enum.Parse<T>(rawName, ignoreCase: true);
    }
}

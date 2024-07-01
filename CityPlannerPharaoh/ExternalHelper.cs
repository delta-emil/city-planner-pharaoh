using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CityPlannerPharaoh;

internal static class ExternalHelper
{
    public static JsonSerializerOptions JsonSerializerOptions { get; } = CreateJsonOptions();

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        options.Converters.Add(new MapCellsJsonConverter());
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return options;
    }

    public static void PutTextOnClipboard(string text, Control control)
    {
        try
        {
            Clipboard.SetText(text);
        }
        catch (ExternalException ex)
        {
            MessageBox.Show(control, ex.Message, "Cliptboard error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static void PutJsonOnClipboard<T>(T data, Control control)
    {
        var clipboardString = JsonSerializer.Serialize(data, JsonSerializerOptions);
        PutTextOnClipboard(clipboardString, control);
    }

    public static T? GetFromClipboardJson<T>(Control control) where T : class
    {
        IDataObject? clipboardData;
        try
        {
            clipboardData = Clipboard.GetDataObject();
        }
        catch (ExternalException ex)
        {
            MessageBox.Show(control, ex.Message, "Cliptboard error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }

        if (clipboardData == null || !clipboardData.GetDataPresent(DataFormats.UnicodeText))
        {
            return null;
        }

        string? clipboardString;
        try
        {
            clipboardString = (string?)clipboardData.GetData(DataFormats.UnicodeText);
        }
        catch (InvalidCastException)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(clipboardString))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(clipboardString, JsonSerializerOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}

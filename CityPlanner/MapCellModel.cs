using System.Text.Json.Serialization;

namespace CityPlanner;

public class MapCellModel
{
    public MapTerrain Terrain { get; set; }

    [JsonIgnore]
    public MapBuilding? Building { get; set; }

    public override string ToString()
    {
        return $"MapCellModel({Terrain})";
    }
}

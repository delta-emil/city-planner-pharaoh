using System.Text.Json.Serialization;

namespace CityPlannerPharaoh;

public class MapCellModel
{
    public MapTerrain Terrain { get; set; }

    [JsonIgnore]
    public MapBuilding? Building { get; set; }

    [JsonIgnore]
    public int Desirability { get; set; }

    public override string ToString()
    {
        return $"MapCellModel({Terrain})";
    }
}

namespace CityPlanner;

public record class MapCellModel
{
    public MapTerrain Terrain { get; set; }
    public MapBuilding? Building { get; set; }
}

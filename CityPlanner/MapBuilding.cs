namespace CityPlanner;

public record class MapBuilding
{
    public int Left { get; set; }
    public int Top { get; set; }
    public MapBuildingType BuildingType { get; set; }
}

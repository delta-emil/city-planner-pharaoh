namespace CityPlanner;

public class MapBuilding
{
    public int Left { get; set; }
    public int Top { get; set; }
    public MapBuildingType BuildingType { get; set; }

    public override string ToString()
    {
        return $"MapBuilding({Left},{Top},{BuildingType})";
    }
}

namespace CityPlanner;

public class MapBuilding
{
    public int Left { get; set; }
    public int Top { get; set; }
    public MapBuildingType BuildingType { get; set; }

    public MapBuilding GetCopy()
    {
        return new MapBuilding { Left = Left, Top = Top, BuildingType = BuildingType };
    }

    public override string ToString()
    {
        return $"MapBuilding({Left},{Top},{BuildingType})";
    }
}

namespace CityPlannerPharaoh;

public class Tool
{
    public MapTerrain? Terrain { get; set; }
    public bool IsClearBuilding { get; set; }
    public MapBuildingType? BuildingType { get; set; }
    public int HouseLevel { get; set; }

    public bool IsEmpty =>
        Terrain == null
        && !IsClearBuilding
        && BuildingType == null;

    public bool SupportsDrag =>
        Terrain != null
        || IsClearBuilding
        || (BuildingType is MapBuildingType.Road or MapBuildingType.Plaza);
}

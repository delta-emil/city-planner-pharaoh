namespace CityPlanner;

public enum MapBuildingType
{
    Road = 0,
    Plaza,
    House,
    Bazaar,
}

public static class MapBuildingTypeExtensions
{
    public static (int width, int height) GetSize(this MapBuildingType mapBuildingType)
    {
        return mapBuildingType switch
        {
            MapBuildingType.Road => (1, 1),
            MapBuildingType.Plaza => (1, 1),
            MapBuildingType.House => (1, 1),
            MapBuildingType.Bazaar => (2, 2),
            _ => throw new NotImplementedException(),
        };
    }

    public static MapBuildingCategory GetCategory(this MapBuildingType mapBuildingType)
    {
        return mapBuildingType switch
        {
            MapBuildingType.Road => MapBuildingCategory.Path,
            MapBuildingType.Plaza => MapBuildingCategory.Plaza,
            MapBuildingType.House => MapBuildingCategory.House,
            MapBuildingType.Bazaar => MapBuildingCategory.Commercial,
            _ => throw new NotImplementedException(),
        };
    }

    public static bool HasSoftBorder(this MapBuildingType mapBuildingType)
    {
        return mapBuildingType switch
        {
            MapBuildingType.Road => true,
            MapBuildingType.Plaza => true,
            _ => false,
        };
    }

    public static bool ShowName(this MapBuildingType mapBuildingType)
    {
        return mapBuildingType switch
        {
            MapBuildingType.Road => false,
            MapBuildingType.Plaza => false,
            MapBuildingType.House => false,
            _ => true,
        };
    }
}

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
    public static (int width, int height) GetSize(this MapBuildingType mapBuildingType) => sizes[(int)mapBuildingType];
    private static readonly (int width, int height)[] sizes = new[]
    {
        (1, 1), // Road
        (1, 1), // Plaza,
        (1, 1), // House,
        (2, 2), // Bazaar,
    };

    public static (int range, int start, int stepRange, int stepDiff) GetDesire(this MapBuildingType mapBuildingType) => desires[(int)mapBuildingType];
    private static readonly (int range, int start, int stepRange, int stepDiff)[] desires = new[]
    {
        (0, 0, 0 , 0), // Road
        (2, 4, 1 , -2), // Plaza,
        (0, 0, 0 , 0), // House, // TODO: improve later
        (6, -2, 1 , 1), // Bazaar,
    };

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

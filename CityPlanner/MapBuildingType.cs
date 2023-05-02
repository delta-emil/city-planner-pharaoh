namespace CityPlanner;

public enum MapBuildingType
{
    Road = 0,
    Plaza,
    Garden,
    House,

    Farm,
    Cattle,
    WaterLift,
    Ditch,
    Hunter,
    Fisher,
    WorkCamp,

    QuarryPlainStone,
    QuarryLimestone,
    QuarrySandstone,
    QuarryGranite,
    MineGems,
    MineCopper,
    MineGold,

    Clay,
    Reed,
    Wood,

    Potter,
    Brewer,
    Papyrus,
    Weaver,
    Jeweler,
    Bricks,
    Lamps,
    Paint,
    Shipwright,

    GuildBricklayer,
    GuildCarpenter,
    GuildStonemason,
    GuildArtisan,

    Bazaar,
    Granary,
    StorageYardTower,
    StorageYard,
    Dock,

    JuggleStage,
    MusicStage,
    DanceStage,
    Booth1,
    Booth2,
    Booth3,
    Booth4,
    Bandstand1,
    Bandstand2,
    Bandstand3,
    Bandstand4,
    Pavilion1,
    Pavilion2,
    Pavilion3,
    Pavilion4,
    Pavilion5,
    Pavilion6,
    Pavilion7,
    Pavilion8,
    Pavilion9,
    Pavilion10,
    Pavilion11,
    Senet,
    Zoo,
    JuggleSchool,
    MusicSchool,
    DanceSchool,

    Shrine,
    Temple,
    TempleComplex1,
    TempleComplex2,
    TempleComplexBuilding,
    FestivalSquare,
}

public static class MapBuildingTypeExtensions
{
    public static (int width, int height) GetSize(this MapBuildingType mapBuildingType) => sizes[(int)mapBuildingType];
    private static readonly (int width, int height)[] sizes = new[]
    {
        (1, 1), // Road
        (1, 1), // Plaza,
        (1, 1), // Garden,
        (1, 1), // House,
        
        (3, 3), // Farm,
        (3, 3), // Cattle,
        (2, 2), // WaterLift,
        (1, 1), // Ditch,
        (2, 2), // Hunter,
        (2, 2), // Fisher,
        (2, 2), // WorkCamp,
        
        (2, 2), // QuarryPlainStone,
        (2, 2), // QuarryLimestone,
        (2, 2), // QuarrySandstone,
        (2, 2), // QuarryGranite,
        (2, 2), // MineGems,
        (2, 2), // MineCopper,
        (2, 2), // MineGold,

        (2, 2), // Clay,
        (2, 2), // Reed,
        (2, 2), // Wood,

        (2, 2), // Potter,
        (2, 2), // Brewer,
        (2, 2), // Papyrus,
        (2, 2), // Weaver,
        (2, 2), // Jeweler,
        (2, 2), // Bricks,
        (2, 2), // Lamps,
        (2, 2), // Paint,
        (3, 3), // Shipwright,

        (2, 2), // GuildBricklayer,
        (2, 2), // GuildCarpenter,
        (2, 2), // GuildStonemason,
        (2, 2), // GuildArtisan,

        (2, 2), // Bazaar,
        (4, 4), // Granary,
        (1, 1), // StorageYardTower,
        (3, 3), // StorageYard,
        (3, 3), // Dock,
        
        (1, 1), // JuggleStage,
        (1, 1), // MusicStage,
        (2, 2), // DanceStage,
        (2, 2), // Booth1,
        (2, 2), // Booth2,
        (2, 2), // Booth3,
        (2, 2), // Booth4,
        (3, 3), // Bandstand1,
        (3, 3), // Bandstand2,
        (3, 3), // Bandstand3,
        (3, 3), // Bandstand4,
        (4, 4), // Pavilion1,
        (4, 4), // Pavilion2,
        (4, 4), // Pavilion3,
        (4, 4), // Pavilion4,
        (4, 4), // Pavilion5,
        (4, 4), // Pavilion6,
        (4, 4), // Pavilion7,
        (4, 4), // Pavilion8,
        (4, 4), // Pavilion9,
        (4, 4), // Pavilion10,
        (4, 4), // Pavilion11,
        (4, 4), // Senet,
        (6, 6), // Zoo,
        (2, 2), // JuggleSchool,
        (3, 3), // MusicSchool,
        (4, 4), // DanceSchool,

        (1, 1), // Shrine,
        (3, 3), // Temple,
        (13, 7), // TempleComplex1,
        (7, 13), // TempleComplex2,
        (3, 3), // TempleComplexBuilding,
        (5, 5), // FestivalSquare,
    };

    public static (int range, int start, int stepRange, int stepDiff) GetDesire(this MapBuildingType mapBuildingType) => desires[(int)mapBuildingType];
    private static readonly (int range, int start, int stepRange, int stepDiff)[] desires = new[]
    {
        (0, 0, 0, 0), // Road
        (2, 4, 1, -2), // Plaza,
        (3, 3, 1, -1), // Garden,
        (0, 0, 0, 0), // House, // TODO: improve later
        
        (2, -2, 1, 1), // Farm,
        (4, -4, 1, 1), // Cattle,
        (3, -3, 1, 1), // WaterLift,
        (0, 0, 0, 0), // Ditch,
        (4, -4, 1, 2), // Hunter,
        (4, -8, 2, 2), // Fisher,
        (3, -3, 1, 1), // WorkCamp,
        
        (6, -6, 1, 1), // QuarryPlainStone,
        (6, -6, 1, 1), // QuarryLimestone,
        (6, -6, 1, 1), // QuarrySandstone,
        (6, -6, 1, 1), // QuarryGranite,
        (6, -12, 2, 2), // MineGems,
        (6, -12, 2, 2), // MineCopper,
        (6, -16, 2, 3), // MineGold,

        (2, -3, 1, 1), // Clay,
        (2, -2, 1, 1), // Reed,
        (3, -4, 1, 1), // Wood,

        (4, -4, 1, 1), // Potter,
        (5, -5, 1, 1), // Brewer,
        (4, -4, 1, 1), // Papyrus,
        (3, -3, 1, 1), // Weaver,
        (2, -2, 1, 1), // Jeweler,
        (4, -4, 1, 1), // Bricks,
        (4, -4, 1, 1), // Lamps,
        (4, -4, 1, 1), // Paint,
        (6, -12, 2, 2), // Shipwright,

        (4, -6, 1, 1), // GuildBricklayer,
        (4, -6, 1, 1), // GuildCarpenter,
        (4, -6, 1, 1), // GuildStonemason,
        (4, -6, 1, 1), // GuildArtisan,

        (6, -2, 1, 1), // Bazaar,
        (4, -8, 1, 2), // Granary,
        (3, -5, 2, 2), // StorageYardTower,
        (0, 0, 0, 0), // StorageYard,
        (6, -12, 2, 2), // Dock,

        (2, 2, 1, -1), // JuggleStage,
        (4, 4, 1, -1), // MusicStage,
        (6, 6, 1, -1), // DanceStage,
        (0, 0, 0, 0), // Booth1,
        (0, 0, 0, 0), // Booth2,
        (0, 0, 0, 0), // Booth3,
        (0, 0, 0, 0), // Booth4,
        (0, 0, 0, 0), // Bandstand1,
        (0, 0, 0, 0), // Bandstand2,
        (0, 0, 0, 0), // Bandstand3,
        (0, 0, 0, 0), // Bandstand4,
        (0, 0, 0, 0), // Pavilion1,
        (0, 0, 0, 0), // Pavilion2,
        (0, 0, 0, 0), // Pavilion3,
        (0, 0, 0, 0), // Pavilion4,
        (0, 0, 0, 0), // Pavilion5,
        (0, 0, 0, 0), // Pavilion6,
        (0, 0, 0, 0), // Pavilion7,
        (0, 0, 0, 0), // Pavilion8,
        (0, 0, 0, 0), // Pavilion9,
        (0, 0, 0, 0), // Pavilion10,
        (0, 0, 0, 0), // Pavilion11,
        (3, -6, 1, -2), // Senet,
        (3, -6, 1, -2), // Zoo,
        (2, 2, 1, -1), // JuggleSchool,
        (3, -3, 1, 1), // MusicSchool,
        (3, -3, 1, 1), // DanceSchool,

        (4, 4, 1, -1), // Shrine,
        (6, 6, 2, -2), // Temple,
        (0, 0, 0, 0), // TempleComplex1,
        (0, 0, 0, 0), // TempleComplex2,
        (6, 20, 2, -4), // TempleComplexBuilding,
        (5, 16, 2, -3), // FestivalSquare,
    };

    public static MapBuildingCategory GetCategory(this MapBuildingType mapBuildingType)
    {
        return mapBuildingType switch
        {
            MapBuildingType.Road => MapBuildingCategory.Path,
            MapBuildingType.Plaza => MapBuildingCategory.Plaza,
            MapBuildingType.Garden => MapBuildingCategory.Beauty,
            MapBuildingType.House => MapBuildingCategory.House,

            MapBuildingType.Farm => MapBuildingCategory.Food,
            MapBuildingType.Cattle => MapBuildingCategory.Food,
            MapBuildingType.WaterLift => MapBuildingCategory.Food,
            MapBuildingType.Ditch => MapBuildingCategory.Ditch,
            MapBuildingType.Hunter => MapBuildingCategory.Food,
            MapBuildingType.Fisher => MapBuildingCategory.Food,
            MapBuildingType.WorkCamp => MapBuildingCategory.Food,

            MapBuildingType.QuarryPlainStone => MapBuildingCategory.QuarryMine,
            MapBuildingType.QuarryLimestone => MapBuildingCategory.QuarryMine,
            MapBuildingType.QuarrySandstone => MapBuildingCategory.QuarryMine,
            MapBuildingType.QuarryGranite => MapBuildingCategory.QuarryMine,
            MapBuildingType.MineGems => MapBuildingCategory.QuarryMine,
            MapBuildingType.MineCopper => MapBuildingCategory.QuarryMine,
            MapBuildingType.MineGold => MapBuildingCategory.QuarryMine,

            MapBuildingType.Clay => MapBuildingCategory.RawMaterials,
            MapBuildingType.Reed => MapBuildingCategory.RawMaterials,
            MapBuildingType.Wood => MapBuildingCategory.RawMaterials,

            MapBuildingType.Potter => MapBuildingCategory.Workshop,
            MapBuildingType.Brewer => MapBuildingCategory.Workshop,
            MapBuildingType.Papyrus => MapBuildingCategory.Workshop,
            MapBuildingType.Weaver => MapBuildingCategory.Workshop,
            MapBuildingType.Jeweler => MapBuildingCategory.Workshop,
            MapBuildingType.Bricks => MapBuildingCategory.Workshop,
            MapBuildingType.Lamps => MapBuildingCategory.Workshop,
            MapBuildingType.Paint => MapBuildingCategory.Workshop,
            MapBuildingType.Shipwright => MapBuildingCategory.Workshop,

            MapBuildingType.GuildBricklayer => MapBuildingCategory.Guild,
            MapBuildingType.GuildCarpenter => MapBuildingCategory.Guild,
            MapBuildingType.GuildStonemason => MapBuildingCategory.Guild,
            MapBuildingType.GuildArtisan => MapBuildingCategory.Guild,

            MapBuildingType.Bazaar => MapBuildingCategory.Distribution,
            MapBuildingType.Granary => MapBuildingCategory.Distribution,
            MapBuildingType.StorageYardTower => MapBuildingCategory.Distribution,
            MapBuildingType.StorageYard => MapBuildingCategory.Distribution,
            MapBuildingType.Dock => MapBuildingCategory.Distribution,
            
            MapBuildingType.JuggleStage => MapBuildingCategory.VenueStage,
            MapBuildingType.MusicStage => MapBuildingCategory.VenueStage,
            MapBuildingType.DanceStage => MapBuildingCategory.VenueStage,
            MapBuildingType.Booth1 => MapBuildingCategory.Venue,
            MapBuildingType.Booth2 => MapBuildingCategory.Venue,
            MapBuildingType.Booth3 => MapBuildingCategory.Venue,
            MapBuildingType.Booth4 => MapBuildingCategory.Venue,
            MapBuildingType.Bandstand1 => MapBuildingCategory.Venue,
            MapBuildingType.Bandstand2 => MapBuildingCategory.Venue,
            MapBuildingType.Bandstand3 => MapBuildingCategory.Venue,
            MapBuildingType.Bandstand4 => MapBuildingCategory.Venue,
            MapBuildingType.Pavilion1 => MapBuildingCategory.Venue,
            MapBuildingType.Pavilion2 => MapBuildingCategory.Venue,
            MapBuildingType.Pavilion3 => MapBuildingCategory.Venue,
            MapBuildingType.Pavilion4 => MapBuildingCategory.Venue,
            MapBuildingType.Pavilion5 => MapBuildingCategory.Venue,
            MapBuildingType.Pavilion6 => MapBuildingCategory.Venue,
            MapBuildingType.Pavilion7 => MapBuildingCategory.Venue,
            MapBuildingType.Pavilion8 => MapBuildingCategory.Venue,
            MapBuildingType.Pavilion9 => MapBuildingCategory.Venue,
            MapBuildingType.Pavilion10 => MapBuildingCategory.Venue,
            MapBuildingType.Pavilion11 => MapBuildingCategory.Venue,
            MapBuildingType.Senet => MapBuildingCategory.VenueStage,
            MapBuildingType.Zoo => MapBuildingCategory.VenueStage,
            MapBuildingType.JuggleSchool => MapBuildingCategory.EntSchool,
            MapBuildingType.MusicSchool => MapBuildingCategory.EntSchool,
            MapBuildingType.DanceSchool => MapBuildingCategory.EntSchool,

            MapBuildingType.Shrine => MapBuildingCategory.Religious,
            MapBuildingType.Temple => MapBuildingCategory.Religious,
            MapBuildingType.TempleComplex1 => MapBuildingCategory.Religious,
            MapBuildingType.TempleComplex2 => MapBuildingCategory.Religious,
            MapBuildingType.TempleComplexBuilding => MapBuildingCategory.Religious,
            MapBuildingType.FestivalSquare => MapBuildingCategory.Religious,

            _ => throw new NotImplementedException(),
        };
    }

    public static bool HasSoftBorder(this MapBuildingType mapBuildingType)
    {
        return mapBuildingType switch
        {
            MapBuildingType.Road => true,
            MapBuildingType.Plaza => true,
            MapBuildingType.Garden => true,
            MapBuildingType.Ditch => true,
            _ => false,
        };
    }

    public static bool ShowName(this MapBuildingType mapBuildingType)
    {
        return mapBuildingType switch
        {
            MapBuildingType.Road => false,
            MapBuildingType.Plaza => false,
            MapBuildingType.Garden => false,
            MapBuildingType.House => false,
            MapBuildingType.Ditch => false,
            MapBuildingType.StorageYardTower => false,
            MapBuildingType.JuggleStage => false,
            MapBuildingType.MusicStage => false,
            MapBuildingType.DanceStage => false,
            MapBuildingType.TempleComplexBuilding => false,
            _ => true,
        };
    }
}

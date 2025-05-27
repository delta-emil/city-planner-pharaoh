using System.Text.RegularExpressions;

namespace CityPlannerPharaoh;

public enum MapBuildingType
{
    Road = 0,
    Plaza,
    Garden,
    House,
    House2,
    House3,
    House4,

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
    Booth,
    Bandstand,
    Pavilion,
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

    ScribeSchool,
    Library,

    Well,
    WaterSupply,
    Physician,
    Apothecary,
    Dentist,
    Mortuary,

    Firehouse,
    Architect,
    Police,
    Tax,
    Courthouse,

    Roadblock,
    Bridge,
    FerryLanding,

    StatueSmall,
    StatueMedium,
    StatueLarge,

    PalaceVillage,
    PalaceTown,
    PalaceCity,
    MansionPersonal,
    MansionFamily,
    MansionDynasty,

    Wall,
    Tower,
    GatePath,
    Gate1,
    Gate2,
    Recruiter,
    Academy,
    Weaponsmith,
    Chariot,
    Fort,
    FortBuilding,
    FortYard,
    Warship,
    TransportShip,
}

public static partial class MapBuildingTypeExtensions
{
    public static (int width, int height) GetSize(this MapBuildingType mapBuildingType) => sizes[(int)mapBuildingType];
    private static readonly (int width, int height)[] sizes = new[]
    {
        (1, 1), // Road
        (1, 1), // Plaza,
        (1, 1), // Garden,
        (1, 1), // House,
        (2, 2), // House2,
        (3, 3), // House3,
        (4, 4), // House4,

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
        (2, 2), // Booth,
        (3, 3), // Bandstand,
        (4, 4), // Pavilion,
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

        (2, 2), // ScribeSchool,
        (3, 3), // Library,

        (1, 1), // Well,
        (2, 2), // WaterSupply,
        (2, 2), // Physician,
        (1, 1), // Apothecary,
        (1, 1), // Dentist,
        (2, 2), // Mortuary,

        (1, 1), // Firehouse,
        (1, 1), // Architect,
        (1, 1), // Police,
        (2, 2), // Tax,
        (3, 3), // Courthouse,

        (1, 1), // Roadblock,
        (1, 1), // Bridge,
        (2, 2), // FerryLanding,

        (1, 1), // StatueSmall,
        (2, 2), // StatueMedium,
        (3, 3), // StatueLarge,

        (4, 4), // PalaceVillage,
        (5, 5), // PalaceTown,
        (6, 6), // PalaceCity,
        (3, 3), // MansionPersonal,
        (4, 4), // MansionFamily,
        (5, 5), // MansionDynasty,

        (1, 1), // Wall,
        (2, 2), // Tower,
        (1, 1), // GatePath,
        (5, 2), // Gate1,
        (2, 5), // Gate2,
        (3, 3), // Recruiter,
        (4, 4), // Academy,
        (2, 2), // Weaponsmith,
        (4, 4), // Chariot,
        (7, 4), // Fort,
        (3, 3), // FortBuilding,
        (4, 4), // FortYard,
        (3, 3), // Warship,
        (2, 2), // TransportShip,
    };

    public static DesireConfig GetDesire(this MapBuildingType mapBuildingType)
    {
        return desires[(int)mapBuildingType];
    }

    private static readonly DesireConfig[] desires = new DesireConfig[]
    {
        new(0, 0, 0, 0), // Road
        new(2, 4, 1, -2), // Plaza,
        new(3, 3, 1, -1), // Garden,
        new(0, 0, 0, 0), // House, // TODO: improve later
        new(0, 0, 0, 0), // House2, // TODO: improve later
        new(0, 0, 0, 0), // House3, // TODO: improve later
        new(0, 0, 0, 0), // House4, // TODO: improve later
        
        new(2, -2, 1, 1), // Farm,
        new(4, -4, 1, 1), // Cattle,
        new(3, -3, 1, 1), // WaterLift,
        new(0, 0, 0, 0), // Ditch,
        new(4, -4, 1, 2), // Hunter,
        new(4, -8, 2, 2), // Fisher,
        new(3, -3, 1, 1), // WorkCamp,

        new(6, -6, 1, 1), // QuarryPlainStone,
        new(6, -6, 1, 1), // QuarryLimestone,
        new(6, -6, 1, 1), // QuarrySandstone,
        new(6, -6, 1, 1), // QuarryGranite,
        new(6, -12, 2, 2), // MineGems,
        new(6, -12, 2, 2), // MineCopper,
        new(6, -16, 2, 3), // MineGold,

        new(2, -3, 1, 1), // Clay,
        new(2, -2, 1, 1), // Reed,
        new(3, -4, 1, 1), // Wood,

        new(4, -4, 1, 1), // Potter,
        new(5, -5, 1, 1), // Brewer,
        new(4, -4, 1, 1), // Papyrus,
        new(3, -3, 1, 1), // Weaver,
        new(2, -2, 1, 1), // Jeweler,
        new(4, -4, 1, 1), // Bricks,
        new(4, -4, 1, 1), // Lamps,
        new(4, -4, 1, 1), // Paint,
        new(6, -12, 2, 2), // Shipwright,

        new(4, -6, 1, 1), // GuildBricklayer,
        new(4, -6, 1, 1), // GuildCarpenter,
        new(4, -6, 1, 1), // GuildStonemason,
        new(4, -6, 1, 1), // GuildArtisan,

        new(6, -2, 1, 1), // Bazaar,
        new(4, -8, 1, 2), // Granary,
        new(3, -5, 2, 2), // StorageYardTower,
        new(0, 0, 0, 0), // StorageYard,
        new(6, -12, 2, 2), // Dock,

        new(2, 2, 1, -1), // JuggleStage,
        new(4, 4, 1, -1), // MusicStage,
        new(6, 6, 1, -1), // DanceStage,
        new(0, 0, 0, 0), // Booth,
        new(0, 0, 0, 0), // Bandstand,
        new(0, 0, 0, 0), // Pavilion,
        new(3, -6, 1, 2), // Senet,
        new(3, -6, 1, 2), // Zoo,
        new(2, 2, 1, -1), // JuggleSchool,
        new(3, -3, 1, 1), // MusicSchool,
        new(3, -3, 1, 1), // DanceSchool,

        new(4, 4, 1, -1), // Shrine,
        new(6, 6, 2, -2), // Temple,
        new(0, 0, 0, 0), // TempleComplex1,
        new(0, 0, 0, 0), // TempleComplex2,
        new(6, 20, 2, -4), // TempleComplexBuilding,
        new(5, 16, 2, -3), // FestivalSquare,

        new(4, 4, 1, -1), // ScribeSchool,
        new(6, 8, 2, -2), // Library,

        new(1, 1, 1, -1), // Well,
        new(4, 4, 1, -1), // WaterSupply,
        new(2, 2, 1, -1), // Physician,
        new(1, 1, 1, -1), // Apothecary,
        new(2, 2, 1, -1), // Dentist,
        new(2, -3, 2, 1), // Mortuary,

        new(2, -2, 1, 1), // Firehouse,
        new(0, 0, 0, 0), // Architect,
        new(2, -2, 1, -1), // Police,
        new(3, 3, 1, -1), // Tax,
        new(3, 8, 2, -2), // Courthouse,

        new(0, 0, 0, 0), // Roadblock,
        new(0, 0, 0, 0), // Bridge,
        new(4, -5, 2, 2), // FerryLanding,

        new(3, 3, 1, -1), // StatueSmall,
        new(4, 10, 1, -2), // StatueMedium,
        new(5, 14, 2, -2), // StatueLarge,

        new(4, 20, 2, -4), // PalaceVillage,
        new(5, 22, 2, -5), // PalaceTown,
        new(6, 24, 2, -6), // PalaceCity,
        new(4, 12, 2, -2), // MansionPersonal,
        new(5, 20, 2, -3), // MansionFamily,
        new(6, 28, 2, -4), // MansionDynasty,

        new(0, 0, 0, 0), // Wall,
        new(6, -6, 1, 1), // Tower,
        new(0, 0, 0, 0), // GatePath,
        new(4, -4, 1, 1), // Gate1,
        new(4, -4, 1, 1), // Gate2,
        new(3, -6, 1, 1), // Recruiter,
        new(3, -3, 1, 1), // Academy,
        new(3, -3, 1, 1), // Weaponsmith,
        new(6, -6, 1, 1), // Chariot,
        new(0, 0, 0, 0), // Fort,
        new(6, -20, 2, 2), // FortBuilding,
        new(6, -20, 2, 2), // FortYard,
        new(4, -4, 1, 1), // Warship,
        new(2, -2, 1, 1), // TransportShip,
    };

    public static MapBuildingCategory GetCategory(this MapBuildingType mapBuildingType)
    {
        return mapBuildingType switch
        {
            MapBuildingType.Road => MapBuildingCategory.Path,
            MapBuildingType.Plaza => MapBuildingCategory.Plaza,
            MapBuildingType.Garden => MapBuildingCategory.Beauty,
            MapBuildingType.House => MapBuildingCategory.House,
            MapBuildingType.House2 => MapBuildingCategory.House,
            MapBuildingType.House3 => MapBuildingCategory.House,
            MapBuildingType.House4 => MapBuildingCategory.House,

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
            MapBuildingType.Booth => MapBuildingCategory.Venue,
            MapBuildingType.Bandstand => MapBuildingCategory.Venue,
            MapBuildingType.Pavilion => MapBuildingCategory.Venue,
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

            MapBuildingType.ScribeSchool => MapBuildingCategory.Education,
            MapBuildingType.Library => MapBuildingCategory.Education,

            MapBuildingType.Well => MapBuildingCategory.Water,
            MapBuildingType.WaterSupply => MapBuildingCategory.Water,
            MapBuildingType.Physician => MapBuildingCategory.Health,
            MapBuildingType.Apothecary => MapBuildingCategory.Health,
            MapBuildingType.Dentist => MapBuildingCategory.Health,
            MapBuildingType.Mortuary => MapBuildingCategory.Health,

            MapBuildingType.Firehouse => MapBuildingCategory.Municipal,
            MapBuildingType.Architect => MapBuildingCategory.Municipal,
            MapBuildingType.Police => MapBuildingCategory.Municipal,
            MapBuildingType.Tax => MapBuildingCategory.Municipal,
            MapBuildingType.Courthouse => MapBuildingCategory.Municipal,

            MapBuildingType.Roadblock => MapBuildingCategory.Roadblock,
            MapBuildingType.Bridge => MapBuildingCategory.Bridge,
            MapBuildingType.FerryLanding => MapBuildingCategory.Ferry,

            MapBuildingType.StatueSmall => MapBuildingCategory.Beauty,
            MapBuildingType.StatueMedium => MapBuildingCategory.Beauty,
            MapBuildingType.StatueLarge => MapBuildingCategory.Beauty,

            MapBuildingType.PalaceVillage => MapBuildingCategory.Municipal,
            MapBuildingType.PalaceTown => MapBuildingCategory.Municipal,
            MapBuildingType.PalaceCity => MapBuildingCategory.Municipal,
            MapBuildingType.MansionPersonal => MapBuildingCategory.Municipal,
            MapBuildingType.MansionFamily => MapBuildingCategory.Municipal,
            MapBuildingType.MansionDynasty => MapBuildingCategory.Municipal,

            MapBuildingType.Wall => MapBuildingCategory.Wall,
            MapBuildingType.Tower => MapBuildingCategory.Military,
            MapBuildingType.GatePath => MapBuildingCategory.GatePath,
            MapBuildingType.Gate1 => MapBuildingCategory.Military,
            MapBuildingType.Gate2 => MapBuildingCategory.Military,
            MapBuildingType.Recruiter => MapBuildingCategory.Military,
            MapBuildingType.Academy => MapBuildingCategory.Military,
            MapBuildingType.Weaponsmith => MapBuildingCategory.Military,
            MapBuildingType.Chariot => MapBuildingCategory.Military,
            MapBuildingType.Fort => MapBuildingCategory.Military,
            MapBuildingType.FortBuilding => MapBuildingCategory.Military,
            MapBuildingType.FortYard => MapBuildingCategory.Military,
            MapBuildingType.Warship => MapBuildingCategory.Military,
            MapBuildingType.TransportShip => MapBuildingCategory.Military,

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
            MapBuildingType.Roadblock => true,
            MapBuildingType.Bridge => true,
            MapBuildingType.Wall => true,
            MapBuildingType.GatePath => true,
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
            MapBuildingType.House2 => false,
            MapBuildingType.House3 => false,
            MapBuildingType.House4 => false,
            MapBuildingType.Ditch => false,
            MapBuildingType.StorageYardTower => false,
            MapBuildingType.JuggleStage => true, //false,
            MapBuildingType.MusicStage => false,
            MapBuildingType.DanceStage => false, // true,
            MapBuildingType.Pavilion => true, //false,
            MapBuildingType.TempleComplexBuilding => false,
            MapBuildingType.Roadblock => false,
            MapBuildingType.Bridge => false,
            MapBuildingType.Wall => false,
            MapBuildingType.GatePath => false,
            MapBuildingType.FortBuilding => false,
            MapBuildingType.FortYard => false,
            _ => true,
        };
    }

    public static string GetDisplayString(this MapBuildingType mapBuildingType)
    {
        return mapBuildingType switch
        {
            MapBuildingType.Firehouse => "F",
            MapBuildingType.Architect => "Ar",
            MapBuildingType.Police => "P",
            MapBuildingType.Apothecary => "Ap",
            MapBuildingType.Dentist => "D",
            MapBuildingType.Shrine => "Sh",
            MapBuildingType.Well => "W",
            MapBuildingType.StatueSmall => "St",
            MapBuildingType.Weaponsmith => "Weapon\r\nsmith",
            MapBuildingType.JuggleStage => "J",
            MapBuildingType.DanceStage => "Pavilion",
            _ => RegexWordBumpBorder().Replace(mapBuildingType.ToString(), "$1\r\n$2"),
        };
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex RegexWordBumpBorder();

    public static bool IgnoreMainBuilding(this MapBuildingType mapBuildingType)
    {
        return mapBuildingType switch
        {
            MapBuildingType.Fort => true,
            MapBuildingType.Pavilion => true,
            MapBuildingType.Bandstand => true,
            MapBuildingType.Booth => true,
            _ => false,
        };
    }

    public static bool DrawMainBuildingBackground(this MapBuildingType mapBuildingType)
    {
        return mapBuildingType switch
        {
            MapBuildingType.Fort => false,
            MapBuildingType.Pavilion => true,
            MapBuildingType.Bandstand => true,
            MapBuildingType.Booth => true,
            _ => true,
        };
    }

    public static bool NeedsWater(this MapBuildingType mapBuildingType)
    {
        return mapBuildingType switch
        {
            MapBuildingType.Well => true,
            MapBuildingType.WaterSupply => true,
            MapBuildingType.PalaceVillage => true,
            MapBuildingType.PalaceTown => true,
            MapBuildingType.PalaceCity => true,
            MapBuildingType.MansionPersonal => true,
            MapBuildingType.MansionFamily => true,
            MapBuildingType.MansionDynasty => true,
            _ => false,
        };
    }

    public static int? GetDesireOnNorthBlockNeededToUpgrade(this MapBuildingType mapBuildingType)
    {
        if (mapBuildingType == MapBuildingType.Bazaar)
        {
            return 31;
        }
        else if (mapBuildingType == MapBuildingType.WaterSupply)
        {
            return 30;
        }
        else
        {
            return null;
        }
    }
}

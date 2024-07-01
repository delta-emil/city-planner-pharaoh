namespace CityPlannerPharaoh;

internal static class Glyphs
{
    public static (string Result, string? Error) GetGlyphs(IEnumerable<MapBuilding> buildings)
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        foreach (var building in buildings)
        {
            minX = Math.Min(minX, building.Left);
            minY = Math.Min(minY, building.Top);

            var size = building.BuildingType.GetSize();
            maxX = Math.Max(maxX, building.Left + size.width - 1);
            maxY = Math.Max(maxY, building.Top + size.height - 1);
        }

        var width = maxX - minX + 1;
        var height = maxY - minY + 1;
        if (width > 40 || height > 40)
        {
            return (string.Empty, "The buildings do not fit into 40x40, which is the maximum for glyphs");
        }

        var lines = new char[height][];
        for (var lineIndex = 0; lineIndex < height; lineIndex++)
        {
            lines[lineIndex] = new char[width];
            Array.Fill(lines[lineIndex], '.');
        }

        foreach (var building in buildings)
        {
            if (building.BuildingType == MapBuildingType.Fort)
            {
                foreach (var subBuilding in building.GetSubBuildings())
                {
                    RenderBuilding(minX, minY, lines, subBuilding);
                }
            }
            else if (building.BuildingType.GetCategory() == MapBuildingCategory.Venue)
            {
                RenderBuilding(minX, minY, lines, building, GetForumGlyph(MapBuildingType.Road));
                
                var subBuildings = building.GetSubBuildings();
                
                var musicStages = subBuildings.Where(x => x.BuildingType == MapBuildingType.MusicStage).ToList();
                char musicStageChar = ' ';
                if (musicStages.Count == 2)
                {
                    // 'm' is horizontal, 'µ' is vertical
                    musicStageChar = musicStages[0].Top == musicStages[1].Top ? 'm' : 'µ';
                }

                foreach (var subBuilding in subBuildings)
                {
                    char g;
                    if (subBuilding.BuildingType == MapBuildingType.MusicStage)
                    {
                        g = musicStageChar;
                    }
                    else
                    {
                        g = GetForumGlyph(subBuilding.BuildingType);
                    }
                    RenderBuilding(minX, minY, lines, subBuilding, g);
                }
            }
            else if (building.BuildingType is MapBuildingType.Gate1 or MapBuildingType.Gate2)
            {
                RenderBuilding(minX, minY, lines, building);
                foreach (var subBuilding in building.GetSubBuildings())
                {
                    RenderBuilding(minX, minY, lines, subBuilding);
                }
            }
            else
            {
                RenderBuilding(minX, minY, lines, building);
            }
        }
        return (string.Join(Environment.NewLine, lines.Select(l => new string(l))), null);
    }

    private static void RenderBuilding(int minX, int minY, char[][] lines, MapBuilding building)
    {
        char g = GetForumGlyph(building.BuildingType);
        RenderBuilding(minX, minY, lines, building, g);
    }

    private static void RenderBuilding(int minX, int minY, char[][] lines, MapBuilding building, char g)
    {
        var left = building.Left - minX;
        var top = building.Top - minY;
        var size = building.BuildingType.GetSize();

        for (var x = left; x < left + size.width; x++)
        {
            for (var y = top; y < top + size.height; y++)
            {
                lines[y][x] = g;
            }
        }
    }

    public static char GetForumGlyph(MapBuildingType mapBuildingType)
    {
        return mapBuildingType switch
        {
            MapBuildingType.Road => '=',
            MapBuildingType.Plaza => '+',
            MapBuildingType.Garden => 'g',
            MapBuildingType.House => 'h',
            MapBuildingType.House2 => 'H',
            MapBuildingType.House3 => 'e',
            MapBuildingType.House4 => 'E',

            MapBuildingType.Farm => '8',
            MapBuildingType.Cattle => '7',
            MapBuildingType.WaterLift => '$',
            MapBuildingType.Ditch => 'g', // using garden, had no Ditch
            MapBuildingType.Hunter => 'k', // using Fisher, had no hunter
            MapBuildingType.Fisher => 'k',
            MapBuildingType.WorkCamp => 'z',

            MapBuildingType.QuarryPlainStone => '0',
            MapBuildingType.QuarryLimestone => '0',
            MapBuildingType.QuarrySandstone => '0',
            MapBuildingType.QuarryGranite => '0',
            MapBuildingType.MineGems => '9',
            MapBuildingType.MineCopper => '9',
            MapBuildingType.MineGold => '9',

            MapBuildingType.Clay => '0', // using Quarry, had no clay pit
            MapBuildingType.Reed => 'k', // using Fisher, had no reed
            MapBuildingType.Wood => 'k', // using Fisher, had no wood

            MapBuildingType.Potter => '5',
            MapBuildingType.Brewer => '1',
            MapBuildingType.Papyrus => '4',
            MapBuildingType.Weaver => '6',
            MapBuildingType.Jeweler => '3',
            MapBuildingType.Bricks => '2',
            MapBuildingType.Lamps => '5', // using Potter, had no Lamps
            MapBuildingType.Paint => '1', // using Brewer, had no Paint
            MapBuildingType.Shipwright => '_',

            MapBuildingType.GuildBricklayer => '!',
            MapBuildingType.GuildCarpenter => '@',
            MapBuildingType.GuildStonemason => '#',
            MapBuildingType.GuildArtisan => '@', // using Carpenter, had no Artisan

            MapBuildingType.Bazaar => 'b',
            MapBuildingType.Granary => 'G',
            // MapBuildingType.StorageYardTower => throw, // shouldn't come here
            MapBuildingType.StorageYard => 'Y',
            MapBuildingType.Dock => 'K',

            MapBuildingType.JuggleStage => 'j',
            MapBuildingType.MusicStage => 'j', // looks basically same as juggler stage; TODO: 'm' is horizontal, 'µ' is vertical
            MapBuildingType.DanceStage => 'ð',
            // MapBuildingType.Booth1 => throw, // shouldn't come here
            // MapBuildingType.Booth2 => throw, // shouldn't come here
            // MapBuildingType.Booth3 => throw, // shouldn't come here
            // MapBuildingType.Booth4 => throw, // shouldn't come here
            // MapBuildingType.Bandstand1 => throw, // shouldn't come here
            // MapBuildingType.Bandstand2 => throw, // shouldn't come here
            // MapBuildingType.Bandstand3 => throw, // shouldn't come here
            // MapBuildingType.Bandstand4 => throw, // shouldn't come here
            // MapBuildingType.Pavilion1 => throw, // shouldn't come here
            // MapBuildingType.Pavilion2 => throw, // shouldn't come here
            // MapBuildingType.Pavilion3 => throw, // shouldn't come here
            // MapBuildingType.Pavilion4 => throw, // shouldn't come here
            // MapBuildingType.Pavilion5 => throw, // shouldn't come here
            // MapBuildingType.Pavilion6 => throw, // shouldn't come here
            // MapBuildingType.Pavilion7 => throw, // shouldn't come here
            // MapBuildingType.Pavilion8 => throw, // shouldn't come here
            // MapBuildingType.Pavilion9 => throw, // shouldn't come here
            // MapBuildingType.Pavilion10 => throw, // shouldn't come here
            // MapBuildingType.Pavilion11 => throw, // shouldn't come here
            MapBuildingType.Senet => 'I',
            MapBuildingType.Zoo => 'g', // using garden, had no Zoo
            MapBuildingType.JuggleSchool => 'J',
            MapBuildingType.MusicSchool => 'c',
            MapBuildingType.DanceSchool => 'D',

            MapBuildingType.Shrine => '*',
            MapBuildingType.Temple => 'R', // 'R' is Ra's templa, but we don't distinguish
            MapBuildingType.TempleComplex1 => 'U',
            MapBuildingType.TempleComplex2 => 'U',
            // MapBuildingType.TempleComplexBuilding => throw, // shouldn't come here
            MapBuildingType.FestivalSquare => 'F',

            MapBuildingType.ScribeSchool => 's',
            MapBuildingType.Library => 'L',

            MapBuildingType.Well => 'w',
            MapBuildingType.WaterSupply => 'W',
            MapBuildingType.Physician => 'y',
            MapBuildingType.Apothecary => 'A',
            MapBuildingType.Dentist => 'd',
            MapBuildingType.Mortuary => 'M',

            MapBuildingType.Firehouse => 'f',
            MapBuildingType.Architect => 'a',
            MapBuildingType.Police => 'p',
            MapBuildingType.Tax => 'q',
            MapBuildingType.Courthouse => 'C',

            MapBuildingType.Roadblock => '®',
            MapBuildingType.Bridge => '=', // using Road, had no bridge
            MapBuildingType.FerryLanding => ':',

            MapBuildingType.StatueSmall => '?',
            MapBuildingType.StatueMedium => '>',
            MapBuildingType.StatueLarge => '<',

            MapBuildingType.PalaceVillage => '{',
            MapBuildingType.PalaceTown => '}',
            MapBuildingType.PalaceCity => '[',
            MapBuildingType.MansionPersonal => 'g', // using garden, had no Mansion
            MapBuildingType.MansionFamily => 'g', // using garden, had no Mansion
            MapBuildingType.MansionDynasty => 'g', // using garden, had no Mansion

            MapBuildingType.Wall => 'g', // using garden, had no Wall
            MapBuildingType.Tower => 'g', // using garden, had no Tower
            MapBuildingType.GatePath => '®', // using roadblock to show the gate
            MapBuildingType.Gate1 => 'g', // using garden, had no Gate
            MapBuildingType.Gate2 => 'g', // using garden, had no Gate
            MapBuildingType.Recruiter => 'x',
            MapBuildingType.Academy => 'X',
            MapBuildingType.Weaponsmith => 'v',
            MapBuildingType.Chariot => 'V',
            // MapBuildingType.Fort => throw, // shouldn't come here
            MapBuildingType.FortBuilding => 'g', // using garden, had no Fort
            MapBuildingType.FortYard => 'g', // using garden, had no Fort
            MapBuildingType.Warship => '|',
            MapBuildingType.TransportShip => 'g', // using garden, had no TransportShip

            _ => throw new NotImplementedException(),
        };
    }
}

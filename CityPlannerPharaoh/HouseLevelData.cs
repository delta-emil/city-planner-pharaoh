﻿
namespace CityPlannerPharaoh;

public static class HouseLevelData
{
    public const int MinNotableDesirability = -17;
    public const int MaxNotableDesirability = 92;

    private static readonly int[] HouseEvolveBoundsHard = new[] { -10, -5, 0, 4, 8, 12, 16, 20, 25, 32, 40, 48, 53, 58, 63, 68, 74, 80, 90 };

    public static int GetHouseLevel(int maxDesire)
    {
        // TODO: this is for Hard, make an option for what difficulty to show
        int level = 0;
        for (int bound = 0; bound < HouseEvolveBoundsHard.Length; bound++)
        {
            if (maxDesire >= HouseEvolveBoundsHard[bound])
            {
                level++;
            }
            else
            {
                break;
            }
        }

        return level;
    }

    public static string GetMaxHouseLevelLabel(int maxDesire)
    {
        var level = GetHouseLevel(maxDesire);
        return "H" + (level + 1);
    }

    public static DesireConfig GetDesire(int houseLevel, int houseSize)
    {
        if (houseLevel < 0)
        {
            return new DesireConfig(0, 0, 0, 0);
        }

        int levelForDesire;
        if (houseSize == 1)
        {
            levelForDesire = Math.Min(houseLevel, 10 - 1);
        }
        else if (houseSize == 2)
        {
            levelForDesire = Math.Min(houseLevel, 14 - 1);
        }
        else if (houseSize == 3)
        {
            levelForDesire = Math.Min(houseLevel, 18 - 1);
        }
        else
        {
            levelForDesire = houseLevel;
        }

        return desires[levelForDesire];
    }

    private static readonly DesireConfig[] desires = new DesireConfig[]
    {
        new(2, -2, 1, 1),
        new(2, -2, 1, 1),
        new(2, -2, 1, 1),
        new(2, -2, 1, 1),
        new(2, -2, 1, 1),
        new(2, -2, 1, 1),

        new(1, -1, 1, 1),
        new(1, -1, 1, 1),

        new(0, 0, 0, 0),
        new(0, 0, 0, 0),
        new(0, 0, 0, 0),
        new(0, 0, 0, 0),

        new(2, 2, 1, -1),
        new(2, 2, 1, -1),

        new(3, 3, 1, -1),
        new(3, 3, 1, -1),

        new(6, 4, 2, -1),
        new(6, 4, 2, -1),

        new(6, 5, 2, -1),
        new(6, 5, 2, -1),
    };
}

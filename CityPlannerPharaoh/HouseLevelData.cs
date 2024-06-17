namespace CityPlannerPharaoh;

public enum Difficulty
{
    VeryEasy = 0,
    Easy,
    Normal,
    Hard,
    VeryHard,
}

public static class HouseLevelData
{
    public const int MinNotableDesirability = -17;
    public const int MaxNotableDesirability = 92;

    private static readonly int[][] HouseEvolveBounds =
        [
            [-98, -10, -7, -2, 2, 6, 10, 14, 18, 22, 29, 37, 45, 50, 55, 60, 64, 70, 75, 85], // VeryEasy
            [-98, -10, -6, -1, 3, 7, 11, 15, 19, 23, 30, 38, 46, 51, 56, 61, 66, 72, 77, 87], // Easy
            [-98, -10, -5,  0, 4, 8, 12, 16, 20, 25, 32, 40, 48, 53, 58, 63, 68, 74, 80, 90], // Normal
            [-98, -10, -5,  0, 4, 8, 12, 16, 20, 25, 32, 40, 48, 53, 58, 63, 68, 74, 80, 90], // Hard
            [-98, -10, -5,  0, 4, 9, 13, 17, 21, 26, 33, 41, 50, 55, 60, 65, 70, 76, 82, 92], // VeryHard
        ];

    public static int GetHouseLevel(int maxDesire, Difficulty difficulty)
    {
        var boundsForDifficulty = HouseEvolveBounds[(int)difficulty];
        // TODO: this is for Hard, make an option for what difficulty to show
        int level = 0;
        for (int bound = 0; bound < boundsForDifficulty.Length; bound++)
        {
            if (maxDesire >= boundsForDifficulty[bound])
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

    public static DesireConfig GetDesire(int houseLevel, int houseSize)
    {
        int levelForDesire;
        if (houseSize == 1)
        {
            levelForDesire = Math.Min(houseLevel, 10);
        }
        else if (houseSize == 2)
        {
            levelForDesire = Math.Min(houseLevel, 14);
        }
        else if (houseSize == 3)
        {
            levelForDesire = Math.Min(houseLevel, 18);
        }
        else
        {
            levelForDesire = houseLevel;
        }

        return desires[levelForDesire];
    }

    private static readonly DesireConfig[] desires =
    [
        new(0, 0, 0, 0),

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

        new(2, 1, 2, -1),
        new(2, 2, 1, -1),

        new(3, 3, 1, -1),
        new(3, 3, 1, -1),

        new(6, 4, 2, -1),
        new(6, 4, 2, -1),

        new(6, 5, 2, -1),
        new(6, 5, 2, -1),
    ];
}

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

    public static (int Level, bool Exceedable) GetHouseLevel(int maxDesire, Difficulty difficulty, int maxHouseLevel)
    {
        var boundsForDifficulty = HouseEvolveBounds[(int)difficulty];

        int level = 0;
        for (int bound = 0; bound < boundsForDifficulty.Length; bound++)
        {
            if (maxDesire >= boundsForDifficulty[bound])
            {
                if (level == maxHouseLevel)
                {
                    return (Level: level, Exceedable: true);
                }
                else
                {
                    level++;
                }
            }
            else
            {
                break;
            }
        }

        return (Level: level, Exceedable: false);
    }

    public static int GetNeededDesire(Difficulty difficulty, int maxHouseLevel)
    {
        return HouseEvolveBounds[(int)difficulty][maxHouseLevel - 1];
    }

    public static DesireConfig GetDesire(int houseLevel)
    {
        return desires[houseLevel];
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

    public static readonly string[] LabelsShort =
    [
        string.Empty,
        "CHu",
        "SHu",
        "MSh",
        "CSh",
        "RCo",
        "OCo",
        "MHo",
        "SHo",
        "MAp",
        "SAp",
        "CRe",
        "SRe",
        "ERe",
        "FRe",
        "CMa",
        "SMa",
        "EMa",
        "SMa",
        "MEs",
        "PEs",
    ];

    public static readonly string[] LabelsMid =
    [
        string.Empty,
        "Cr.Hut",
        "St.Hut",
        "Me.Sha",
        "Co.Sha",
        "Ro.Cot",
        "Or.Cot",
        "Mo.Hom",
        "Sp.Hom",
        "Mo.Apa",
        "Sp.Apa",
        "Co.Res",
        "Sp.Res",
        "El.Res",
        "Fa.Res",
        "Co.Man",
        "Sp.Man",
        "El.Man",
        "St.Man",
        "Mo.Est",
        "Pa.Est",
    ];

    public static readonly string[] LabelsFull =
    [
        string.Empty,
        "Crude Hut",
        "Sturdy Hut",
        "Meagre Shanty",
        "Common Shanty",
        "Rough Cottage",
        "Ordinary Cottage",
        "Modest Homestead",
        "Spacious Homestead",
        "Modest Apartment",
        "Spacious Apartment",
        "Common Residence",
        "Spacious Residence",
        "Elegant Residence",
        "Fancy Residence",
        "Common Manor",
        "Spacious Manor",
        "Elegant Manor",
        "Stately Manor",
        "Modest Estate",
        "Palatial Estate",
    ];
}


namespace CityPlannerPharaohTests.Pavilion;

internal static class PavilionAlgo
{
    public static string[]? AddPavilion(string[] input)
    {
        var arr = new char[4][];
        for (int row = 0; row < 4; row++)
        {
            arr[row] = input[row].ToCharArray();
        }

        if (!AddPavilion(arr))
        {
            return null;
        }

        var result = new string[4];
        for (int row = 0; row < 4; row++)
        {
            result[row] = new string(arr[row]);
        }
        return result;
    }

    private static bool AddPavilion(char[][] arr)
    {
        if (!TryAddBuilding(arr, 'D', sizeRows: 2, sizeCols: 2))
        {
            return false;
        }

        if (!TryAddBuilding(arr, 'M', sizeRows: 2, sizeCols: 1))
        {
            if (!TryAddBuilding(arr, 'M', sizeRows: 1, sizeCols: 2))
            {
                return false;
            }
        }

        if (!TryAddBuilding(arr, 'J', sizeRows: 1, sizeCols: 1))
        {
            return false;
        }

        // fill in gardens
        for (var row = 0; row < 4; row++)
        {
            for (var col = 0; col < 4; col++)
            {
                if (arr[row][col] == ' ')
                {
                    arr[row][col] = 'g';
                }
            }
        }

        return true;
    }

    private static readonly (int row, int col)[] PositionPriority = [
        // 1. corners
        (0, 0),
        (0, 3),
        (3, 0),
        (3, 3),
        // 2. sides
        (0, 1),
        (0, 2),
        (1, 0),
        (1, 3),
        (2, 0),
        (2, 3),
        (3, 1),
        (3, 2),
        // 3. inside
        (1, 1),
        (1, 2),
        (2, 1),
        (2, 2),
    ];

    private static bool TryAddBuilding(char[][] arr, char type, int sizeRows, int sizeCols)
    {
        foreach (var (row, col) in PositionPriority)
        {
            if (TryAddBuildingOnPosition(arr, type, sizeRows, sizeCols, row, col))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryAddBuildingOnPosition(char[][] arr, char type, int sizeRows, int sizeCols, int startRow, int startCol)
    {
        // building can't stick out
        int endRow = startRow + sizeRows;
        int endCol = startCol + sizeCols;
        if (endRow > 4 || endCol > 4)
        {
            return false;
        }

        for (var row = startRow; row < endRow; row++)
        {
            for (var col = startCol; col < endCol; col++)
            {
                if (arr[row][col] != ' ')
                {
                    return false;
                }
            }
        }

        for (var row = startRow; row < endRow; row++)
        {
            for (var col = startCol; col < endCol; col++)
            {
                arr[row][col] = type;
            }
        }

        return true;
    }
}

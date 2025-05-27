namespace CityPlannerPharaoh;

public static class StageLayout
{
    public static List<MapBuilding>? GetPavilionSubBuildings(MapCellModel[,] cells, int startRow, int startCol)
    {
        return GetVenueSubBuildings(4, cells, startRow, startCol);
    }

    public static List<MapBuilding>? GetBandstandSubBuildings(MapCellModel[,] cells, int startRow, int startCol)
    {
        return GetVenueSubBuildings(3, cells, startRow, startCol);
    }

    public static List<MapBuilding>? GetBoothSubBuildings(MapCellModel[,] cells, int startRow, int startCol)
    {
        return GetVenueSubBuildings(2, cells, startRow, startCol);
    }

    private static List<MapBuilding>? GetVenueSubBuildings(int venueSize, MapCellModel[,] cells, int startRow, int startCol)
    {
        if (!ValidateCrossroad(venueSize, cells, startRow, startCol))
        {
            return null;
        }

        return ComposeSubBuildings(venueSize, cells, startRow, startCol);
    }

    private static List<MapBuilding>? ComposeSubBuildings(int venueSize, MapCellModel[,] cells, int startRow, int startCol)
    {
        var maxSubBuildings = venueSize switch { 4 => 7, 3 => 5, 2 => 1, _ => throw new Exception($"Invalid venue size {venueSize}") };
        List<MapBuilding> subBuildings = new(maxSubBuildings);

        if (venueSize >= 4)
        {
            if (!TryAddBuilding(venueSize, cells, startRow, startCol, subBuildings, MapBuildingType.DanceStage, sizeRows: 2, sizeCols: 2))
            {
                return null;
            }
        }

        if (venueSize >= 3)
        {
            if (!TryAddBuilding(venueSize, cells, startRow, startCol, subBuildings, MapBuildingType.MusicStage, sizeRows: 2, sizeCols: 1))
            {
                if (!TryAddBuilding(venueSize, cells, startRow, startCol, subBuildings, MapBuildingType.MusicStage, sizeRows: 1, sizeCols: 2))
                {
                    return null;
                }
            }
        }

        if (!TryAddBuilding(venueSize, cells, startRow, startCol, subBuildings, MapBuildingType.JuggleStage, sizeRows: 1, sizeCols: 1))
        {
            return null;
        }

        if (venueSize >= 3)
        {
            // fill in gardens
            for (var row = 0; row < venueSize; row++)
            {
                for (var col = 0; col < venueSize; col++)
                {
                    int left = startCol + col;
                    int top = startRow + row;
                    if (!IsCellOccupied(cells, subBuildings, row: top, col: left))
                    {
                        subBuildings.Add(new MapBuilding { Left = left, Top = top, BuildingType = MapBuildingType.Garden });
                    }
                }
            }
        }

        return subBuildings;
    }

    private static readonly (int row, int col)[][] PositionPrioritySets =
        [
            [],
            [],
            [ // Booth
                (0, 0),
                (0, 1),
                (1, 0),
                (1, 1),
            ],
            [ // Bandstand
                // 1. corners
                (0, 0),
                (0, 2),
                (2, 0),
                (2, 2),
                // 2. sides
                (0, 1),
                (1, 0),
                (1, 2),
                (2, 1),
                // 3. inside
                (1, 1),
            ],
            [ // Pavilion
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
            ]
        ];

    private static bool TryAddBuilding(int venueSize, MapCellModel[,] cells, int startRow, int startCol, List<MapBuilding> subBuildings, MapBuildingType type, int sizeRows, int sizeCols)
    {
        var positionPriority = PositionPrioritySets[venueSize];
        foreach (var (row, col) in positionPriority)
        {
            if (TryAddBuildingOnPosition(venueSize, cells, startRow, startCol, subBuildings, type, sizeRows, sizeCols, row, col))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryAddBuildingOnPosition(int venueSize, MapCellModel[,] cells, int startRow, int startCol, List<MapBuilding> subBuildings, MapBuildingType type, int sizeRows, int sizeCols, int northRow, int northCol)
    {
        // building can't stick out
        int endRow = northRow + sizeRows;
        int endCol = northCol + sizeCols;
        if (endRow > venueSize || endCol > venueSize)
        {
            return false;
        }

        for (var row = northRow; row < endRow; row++)
        {
            for (var col = northCol; col < endCol; col++)
            {
                if (IsCellOccupied(cells, subBuildings, startRow + row, startCol + col))
                {
                    return false;
                }
            }
        }

        subBuildings.Add(new MapBuilding { Top = startRow + northRow, Left = startCol + northCol, BuildingType = type });

        // second MusicStage tile
        if (type == MapBuildingType.MusicStage)
        {
            subBuildings.Add(new MapBuilding { Top = startRow + northRow + (sizeRows - 1), Left = startCol + northCol + (sizeCols - 1), BuildingType = type });
            
        }

        return true;
    }

    private static bool IsCellOccupied(MapCellModel[,] cells, List<MapBuilding> subBuildings, int row, int col)
    {
        if (cells[col, row].Building != null)
        {
            return true;
        }

        foreach (var subBuilding in subBuildings)
        {
            if (row == subBuilding.Top && col == subBuilding.Left)
            {
                return true;
            }

            if (subBuilding.BuildingType == MapBuildingType.DanceStage)
            {
                if (row == subBuilding.Top + 1 && col == subBuilding.Left
                    || row == subBuilding.Top && col == subBuilding.Left + 1
                    || row == subBuilding.Top + 1 && col == subBuilding.Left + 1)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static readonly (int row, int col)[][] ValidCrossroadPositionSets =
        [
            [],
            [],
            [ // Booth
                (0, 0),
                (0, 1),
                (1, 0),
                (1, 1),
            ],
            [ // Bandstand
                (0, 1),
                (1, 0),
                (1, 2),
                (2, 1),
            ],
            [ // Pavilion
                (0, 1),
                (0, 2),
                (1, 0),
                (1, 1),
                (1, 2),
                // (1, 3), // the impossible crossroad
                (2, 0),
                (2, 1),
                (2, 2),
                (2, 3),
                (3, 1),
                (3, 2),
            ]
        ];

    private static bool ValidateCrossroad(int venueSize, MapCellModel[,] cells, int startRow, int startCol)
    {
        var validCrossroadPositions = ValidCrossroadPositionSets[venueSize];
        foreach (var (row, col) in validCrossroadPositions)
        {
            if (IsValidCrossroad(venueSize, cells, startRow, startCol, row, col))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsValidCrossroad(int venueSize, MapCellModel[,] cells, int startRow, int startCol, int crossRow, int crossCol)
    {
        for (int row = 0; row < venueSize; row++)
        {
            for (int col = 0; col < venueSize; col++)
            {
                if (row == crossRow || col == crossCol)
                {
                    var buildingOnCell = cells[startCol + col, startRow + row].Building;
                    if (buildingOnCell == null || buildingOnCell.BuildingType is not (MapBuildingType.Road or MapBuildingType.Plaza))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}

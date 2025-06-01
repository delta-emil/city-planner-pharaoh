using System.Text.Json.Serialization;

namespace CityPlannerPharaoh;

public class MapBuilding
{
    public int Left { get; set; }
    public int Top { get; set; }
    public MapBuildingType BuildingType { get; set; }
    public int MaxHouseLevel { get; set; }
    public List<MapBuilding>? SubBuildings { get; set; }

    [JsonIgnore]
    public int HouseLevel { get; set; }
    [JsonIgnore]
    public bool MaxHouseLevelExceedable { get; set; }
    [JsonIgnore]
    public bool HouseWouldNotDowngrade { get; set; }

    public override string ToString()
    {
        if (BuildingType.GetCategory() == MapBuildingCategory.House)
        {
            return $"MapBuilding({Left},{Top},{BuildingType},{MaxHouseLevel})";
        }
        else
        {
            return $"MapBuilding({Left},{Top},{BuildingType})";
        }
    }

    public MapBuilding GetCopy(bool includeTransientData = false)
    {
        var copy = new MapBuilding
        {
            Left = this.Left,
            Top = this.Top,
            BuildingType = this.BuildingType,
            MaxHouseLevel = this.MaxHouseLevel,
        };

        if (this.SubBuildings != null)
        {
            copy.SubBuildings = new List<MapBuilding>(this.SubBuildings.Count);
            for (int i = 0; i < this.SubBuildings.Count; i++)
            {
                copy.SubBuildings.Add(this.SubBuildings[i].GetCopy());
            }
        }

        if (includeTransientData)
        {
            copy.HouseLevel = this.HouseLevel;
            copy.MaxHouseLevelExceedable = this.MaxHouseLevelExceedable;
            copy.HouseWouldNotDowngrade = this.HouseWouldNotDowngrade;
        }

        return copy;
    }

    public void MoveLocation(int deltaX, int deltaY)
    {
        this.Left += deltaX;
        this.Top += deltaY;

        if (this.SubBuildings != null)
        {
            foreach (var subBuilding in this.SubBuildings)
            {
                subBuilding.Left += deltaX;
                subBuilding.Top += deltaY;
            }
        }
    }

    public void ClearTransientData()
    {
        this.HouseLevel = 0;
        this.MaxHouseLevelExceedable = false;
        this.HouseWouldNotDowngrade = false;
    }

    public List<MapBuilding> GetSubBuildings()
    {
        if (this.SubBuildings != null)
        {
            return this.SubBuildings;
        }

        return this.BuildingType switch
        {
            MapBuildingType.StorageYard => [new() { Left = this.Left, Top = this.Top, BuildingType = MapBuildingType.StorageYardTower }],

            MapBuildingType.TempleComplex1 =>
            [
                new() { Left = this.Left,     Top = this.Top + 2, BuildingType = MapBuildingType.TempleComplexBuilding },
                new() { Left = this.Left + 3, Top = this.Top + 2, BuildingType = MapBuildingType.TempleComplexBuilding },
                new() { Left = this.Left + 6, Top = this.Top + 2, BuildingType = MapBuildingType.TempleComplexBuilding },
            ],
            MapBuildingType.TempleComplex2 =>
            [
                new() { Left = this.Left + 2, Top = this.Top,     BuildingType = MapBuildingType.TempleComplexBuilding },
                new() { Left = this.Left + 2, Top = this.Top + 3, BuildingType = MapBuildingType.TempleComplexBuilding },
                new() { Left = this.Left + 2, Top = this.Top + 6, BuildingType = MapBuildingType.TempleComplexBuilding },
            ],

            MapBuildingType.Gate1 =>
            [
                new() { Left = this.Left,     Top = this.Top,     BuildingType = MapBuildingType.GateNorth },
                new() { Left = this.Left + 2, Top = this.Top,     BuildingType = MapBuildingType.GatePath },
                new() { Left = this.Left + 2, Top = this.Top + 1, BuildingType = MapBuildingType.GatePath },
            ],
            MapBuildingType.Gate2 =>
            [
                new() { Left = this.Left,     Top = this.Top,     BuildingType = MapBuildingType.GateNorth },
                new() { Left = this.Left,     Top = this.Top + 2, BuildingType = MapBuildingType.GatePath },
                new() { Left = this.Left + 1, Top = this.Top + 2, BuildingType = MapBuildingType.GatePath },
            ],
            MapBuildingType.Fort =>
            [
                new() { Left = this.Left,     Top = this.Top + 1, BuildingType = MapBuildingType.FortBuilding },
                new() { Left = this.Left + 3, Top = this.Top,     BuildingType = MapBuildingType.FortYard },
            ],

            _ => [],
        };
    }

    internal bool IsEmptyCell(int cellX, int cellY)
    {
        if (this.BuildingType == MapBuildingType.Fort)
        {
            return cellY == this.Top
                && this.Left <= cellX && cellX  < this.Left + 3;
        }

        if (this.BuildingType.GetCategory() == MapBuildingCategory.Venue && this.SubBuildings != null)
        {
            foreach (var subBuilding in this.SubBuildings)
            {
                var size = subBuilding.BuildingType.GetSize();
                if (subBuilding.Left <= cellX && cellX < subBuilding.Left + size.width
                    && subBuilding.Top <= cellY && cellY < subBuilding.Top + size.height)
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }
}

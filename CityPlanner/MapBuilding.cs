namespace CityPlanner;

public class MapBuilding
{
    public int Left { get; set; }
    public int Top { get; set; }
    public MapBuildingType BuildingType { get; set; }

    public override string ToString()
    {
        return $"MapBuilding({Left},{Top},{BuildingType})";
    }

    public MapBuilding GetCopy()
    {
        return new MapBuilding { Left = Left, Top = Top, BuildingType = BuildingType };
    }

    public MapBuilding[] GetSubBuildings()
    {
        return this.BuildingType switch
        {
            MapBuildingType.StorageYard => new MapBuilding[] { new() { Left = this.Left, Top = this.Top, BuildingType = MapBuildingType.StorageYardTower } },

            MapBuildingType.TempleComplex1 => new MapBuilding[]
            {
                new() { Left = this.Left,     Top = this.Top + 2, BuildingType = MapBuildingType.TempleComplexBuilding },
                new() { Left = this.Left + 3, Top = this.Top + 2, BuildingType = MapBuildingType.TempleComplexBuilding },
                new() { Left = this.Left + 6, Top = this.Top + 2, BuildingType = MapBuildingType.TempleComplexBuilding },
            },
            MapBuildingType.TempleComplex2 => new MapBuilding[]
            {
                new() { Left = this.Left + 2, Top = this.Top,     BuildingType = MapBuildingType.TempleComplexBuilding },
                new() { Left = this.Left + 2, Top = this.Top + 3, BuildingType = MapBuildingType.TempleComplexBuilding },
                new() { Left = this.Left + 2, Top = this.Top + 6, BuildingType = MapBuildingType.TempleComplexBuilding },
            },

            MapBuildingType.Booth1 => new MapBuilding[] { new() { Left = this.Left,     Top = this.Top,     BuildingType = MapBuildingType.JuggleStage } },
            MapBuildingType.Booth2 => new MapBuilding[] { new() { Left = this.Left + 1, Top = this.Top,     BuildingType = MapBuildingType.JuggleStage } },
            MapBuildingType.Booth3 => new MapBuilding[] { new() { Left = this.Left,     Top = this.Top + 1, BuildingType = MapBuildingType.JuggleStage } },
            MapBuildingType.Booth4 => new MapBuilding[] { new() { Left = this.Left + 1, Top = this.Top + 1, BuildingType = MapBuildingType.JuggleStage } },

            MapBuildingType.Bandstand1 => new MapBuilding[]
            {
                new() { Left = this.Left,     Top = this.Top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left + 1, Top = this.Top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top + 2, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 1, Top = this.Top + 2, BuildingType = MapBuildingType.Garden },
            },
            MapBuildingType.Bandstand2 => new MapBuilding[]
            {
                new() { Left = this.Left,     Top = this.Top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left + 2, Top = this.Top,     BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 2, Top = this.Top + 1, BuildingType = MapBuildingType.Garden },
            },
            MapBuildingType.Bandstand3 => new MapBuilding[]
            {
                new() { Left = this.Left + 1, Top = this.Top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left + 2, Top = this.Top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left + 2, Top = this.Top + 2, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 1, Top = this.Top + 2, BuildingType = MapBuildingType.Garden },
            },
            MapBuildingType.Bandstand4 => new MapBuilding[]
            {
                new() { Left = this.Left,     Top = this.Top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top + 2, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left + 2, Top = this.Top + 2, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 2, Top = this.Top + 1, BuildingType = MapBuildingType.Garden },
            },

            MapBuildingType.Pavilion1 => new MapBuilding[]
            {
                new() { Left = this.Left,     Top = this.Top,     BuildingType = MapBuildingType.DanceStage },
                new() { Left = this.Left + 3, Top = this.Top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left + 3, Top = this.Top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top + 3, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 1, Top = this.Top + 3, BuildingType = MapBuildingType.Garden },
                new() { Left = this.Left + 3, Top = this.Top + 3, BuildingType = MapBuildingType.Garden },
            },
            MapBuildingType.Pavilion2 => new MapBuilding[]
            {
                new() { Left = this.Left + 2, Top = this.Top,     BuildingType = MapBuildingType.DanceStage },
                new() { Left = this.Left,     Top = this.Top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top + 3, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 2, Top = this.Top + 3, BuildingType = MapBuildingType.Garden },
                new() { Left = this.Left + 3, Top = this.Top + 3, BuildingType = MapBuildingType.Garden },
            },
            MapBuildingType.Pavilion3 => new MapBuilding[]
            {
                new() { Left = this.Left + 2, Top = this.Top + 2, BuildingType = MapBuildingType.DanceStage },
                new() { Left = this.Left,     Top = this.Top + 2, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top + 3, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top,     BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 2, Top = this.Top,     BuildingType = MapBuildingType.Garden },
                new() { Left = this.Left + 3, Top = this.Top,     BuildingType = MapBuildingType.Garden },
            },
            MapBuildingType.Pavilion4 => new MapBuilding[]
            {
                new() { Left = this.Left,     Top = this.Top + 2, BuildingType = MapBuildingType.DanceStage },
                new() { Left = this.Left + 3, Top = this.Top + 2, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left + 3, Top = this.Top + 3, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top,     BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 1, Top = this.Top,     BuildingType = MapBuildingType.Garden },
                new() { Left = this.Left + 3, Top = this.Top,     BuildingType = MapBuildingType.Garden },
            },
            MapBuildingType.Pavilion5 => new MapBuilding[]
            {
                new() { Left = this.Left,     Top = this.Top,     BuildingType = MapBuildingType.DanceStage },
                new() { Left = this.Left + 3, Top = this.Top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left + 3, Top = this.Top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top + 2, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 1, Top = this.Top + 2, BuildingType = MapBuildingType.Garden },
                new() { Left = this.Left + 3, Top = this.Top + 2, BuildingType = MapBuildingType.Garden },
            },
            MapBuildingType.Pavilion6 => new MapBuilding[]
            {
                new() { Left = this.Left + 2, Top = this.Top,     BuildingType = MapBuildingType.DanceStage },
                new() { Left = this.Left,     Top = this.Top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top + 2, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 2, Top = this.Top + 2, BuildingType = MapBuildingType.Garden },
                new() { Left = this.Left + 3, Top = this.Top + 2, BuildingType = MapBuildingType.Garden },
            },
            MapBuildingType.Pavilion7 => new MapBuilding[]
            {
                new() { Left = this.Left + 1, Top = this.Top,     BuildingType = MapBuildingType.DanceStage },
                new() { Left = this.Left + 3, Top = this.Top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left + 3, Top = this.Top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left + 3, Top = this.Top + 3, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 1, Top = this.Top + 3, BuildingType = MapBuildingType.Garden },
                new() { Left = this.Left + 2, Top = this.Top + 3, BuildingType = MapBuildingType.Garden },
            },
            MapBuildingType.Pavilion8 => new MapBuilding[]
            {
                new() { Left = this.Left + 1, Top = this.Top + 2, BuildingType = MapBuildingType.DanceStage },
                new() { Left = this.Left + 3, Top = this.Top + 2, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left + 3, Top = this.Top + 3, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left + 3, Top = this.Top,     BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 1, Top = this.Top,     BuildingType = MapBuildingType.Garden },
                new() { Left = this.Left + 2, Top = this.Top,     BuildingType = MapBuildingType.Garden },
            },
            MapBuildingType.Pavilion9 => new MapBuilding[]
            {
                new() { Left = this.Left + 2, Top = this.Top + 1, BuildingType = MapBuildingType.DanceStage },
                new() { Left = this.Left,     Top = this.Top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top + 2, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top + 3, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 2, Top = this.Top + 3, BuildingType = MapBuildingType.Garden },
                new() { Left = this.Left + 3, Top = this.Top + 3, BuildingType = MapBuildingType.Garden },
            },
            MapBuildingType.Pavilion10 => new MapBuilding[]
            {
                new() { Left = this.Left,     Top = this.Top + 1, BuildingType = MapBuildingType.DanceStage },
                new() { Left = this.Left + 3, Top = this.Top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left + 3, Top = this.Top + 2, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top + 3, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 1, Top = this.Top + 3, BuildingType = MapBuildingType.Garden },
                new() { Left = this.Left + 3, Top = this.Top + 3, BuildingType = MapBuildingType.Garden },
            },
            MapBuildingType.Pavilion11 => new MapBuilding[]
            {
                new() { Left = this.Left,     Top = this.Top,     BuildingType = MapBuildingType.DanceStage },
                new() { Left = this.Left + 2, Top = this.Top,     BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left + 2, Top = this.Top + 1, BuildingType = MapBuildingType.MusicStage },
                new() { Left = this.Left,     Top = this.Top + 3, BuildingType = MapBuildingType.JuggleStage },
                new() { Left = this.Left + 1, Top = this.Top + 3, BuildingType = MapBuildingType.Garden },
                new() { Left = this.Left + 2, Top = this.Top + 3, BuildingType = MapBuildingType.Garden },
            },
            _ => Array.Empty<MapBuilding>(),
        };
    }
}

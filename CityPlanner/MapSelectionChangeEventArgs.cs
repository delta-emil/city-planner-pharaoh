namespace CityPlanner;

public class MapSelectionChangeEventArgs : EventArgs
{
    public int SelectedRoadLength { get; set; }
    public int Selected2x2HouseCount { get; set; }
}

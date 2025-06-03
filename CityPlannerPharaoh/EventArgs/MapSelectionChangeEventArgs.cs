namespace CityPlannerPharaoh;

public class MapSelectionChangeEventArgs : EventArgs
{
    public int SelectedEmpl { get; set; }
    public int SelectedRoadLength { get; set; }
    public int Selected2x2HouseCount { get; set; }
}

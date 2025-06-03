namespace CityPlannerPharaoh;

public class MapUndoStackChangeEventArgs : EventArgs
{
    public bool CanUndo { get; set; }
    public bool CanRedo { get; set; }

    public bool UpdateContentControls { get; set; }
    public Difficulty Difficulty { get; set; }
    public bool SimpleHouseDesire { get; set; }
}

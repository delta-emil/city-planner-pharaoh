namespace CityPlannerPharaoh;

public class MapUndoStackChangeEventArgs : EventArgs
{
    public bool CanUndo { get; set; }
    public bool CanRedo { get; set; }
    public Difficulty Difficulty { get; set; }
}

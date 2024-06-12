namespace CityPlannerPharaoh;

public class ActionData
{
    public ActionType ActionType { get; init; }
    
    public MapTerrain OldTerrain { get; init; }
    public List<MapBuilding>? AddedBuildings { get; init; }
    public List<MapBuilding>? RemovedBuildings { get; init; }
}

public enum ActionType
{
    ChangeTerrain,
    ChangeBuildings,
}
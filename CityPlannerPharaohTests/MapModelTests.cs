using System.Linq;
using CityPlannerPharaoh;

namespace CityPlannerPharaohTests;

[TestFixture]
public class MapModelTests
{
    [Test]
    public void Test0()
    {
        var a1 = new int[2, 2] { { 1, 2 }, { 3, 4 } };

        for (int cellX = 0; cellX < 2; cellX++)
        {
            for (int cellY = 0; cellY < 2; cellY++)
            {
                Console.Write(a1[cellX, cellY]);
                Console.Write(' ');
            }
            Console.WriteLine();
        }

        //var a2 = new int[2, 2] { { 1, 3 }, { 5, 4 } };
        //Assert.That(a1, Is.EqualTo(a2));
    }
        
    [Test]
    public void Test1()
    {
        var mapModel = new MapModel(40, 20);

        for (int cellX = 0; cellX < mapModel.MapSideX; cellX++)
        {
            for (int cellY = 0; cellY < mapModel.MapSideY; cellY++)
            {
                Assert.That(mapModel.Cells[cellX, cellY].Desirability, Is.Zero, $"row: {cellX}, col: {cellY}");
            }
        }

        // ------- add statues ------
        var statues = new List<MapBuilding>();
        statues.Add(AddBuilding(mapModel, 5, 7, MapBuildingType.StatueLarge));
        statues.Add(AddBuilding(mapModel, 5, 10, MapBuildingType.StatueLarge));
        statues.Add(AddBuilding(mapModel, 8, 5, MapBuildingType.StatueLarge));
        statues.Add(AddBuilding(mapModel, 8, 12, MapBuildingType.StatueLarge));
        statues.Add(AddBuilding(mapModel, 11, 7, MapBuildingType.StatueLarge));
        statues.Add(AddBuilding(mapModel, 11, 10, MapBuildingType.StatueLarge));

        //ShowState(mapModel);

        var expectedDesireAfterStatues = new int[40, 20]
        {
            {  0,  0, 10, 10, 10, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 10, 10, 10,  0,  0, },
            {  0,  0, 10, 12, 12, 22, 24, 24, 24, 24, 24, 24, 24, 24, 22, 12, 12, 10,  0,  0, },
            {  0,  0, 10, 12, 12, 22, 24, 24, 24, 24, 24, 24, 24, 24, 22, 12, 12, 10,  0,  0, },
            { 10, 10, 20, 22, 22, 34, 36, 46, 48, 48, 48, 48, 46, 36, 34, 22, 22, 20, 10, 10, },
            { 10, 12, 22, 24, 24, 36, 38, 48, 52, 52, 52, 52, 48, 38, 36, 24, 24, 22, 12, 10, },
            { 10, 12, 22, 24, 24, 36, 38, 34, 38, 38, 38, 38, 34, 38, 36, 24, 24, 22, 12, 10, },
            { 10, 12, 32, 36, 36, 58, 60, 56, 60, 60, 60, 60, 56, 60, 58, 36, 36, 32, 12, 10, },
            { 10, 12, 32, 38, 38, 60, 64, 60, 64, 64, 64, 64, 60, 64, 60, 38, 38, 32, 12, 10, },
            { 10, 12, 32, 38, 38, 46, 50, 60, 78, 78, 78, 78, 60, 50, 46, 38, 38, 32, 12, 10, },
            { 10, 12, 32, 38, 38, 48, 52, 62, 82, 82, 82, 82, 62, 52, 48, 38, 38, 32, 12, 10, },
            { 10, 12, 32, 38, 38, 46, 50, 60, 78, 78, 78, 78, 60, 50, 46, 38, 38, 32, 12, 10, },
            { 10, 12, 32, 38, 38, 60, 64, 60, 64, 64, 64, 64, 60, 64, 60, 38, 38, 32, 12, 10, },
            { 10, 12, 32, 36, 36, 58, 60, 56, 60, 60, 60, 60, 56, 60, 58, 36, 36, 32, 12, 10, },
            { 10, 12, 22, 24, 24, 36, 38, 34, 38, 38, 38, 38, 34, 38, 36, 24, 24, 22, 12, 10, },
            { 10, 12, 22, 24, 24, 36, 38, 48, 52, 52, 52, 52, 48, 38, 36, 24, 24, 22, 12, 10, },
            { 10, 10, 20, 22, 22, 34, 36, 46, 48, 48, 48, 48, 46, 36, 34, 22, 22, 20, 10, 10, },
            {  0,  0, 10, 12, 12, 22, 24, 24, 24, 24, 24, 24, 24, 24, 22, 12, 12, 10,  0,  0, },
            {  0,  0, 10, 12, 12, 22, 24, 24, 24, 24, 24, 24, 24, 24, 22, 12, 12, 10,  0,  0, },
            {  0,  0, 10, 10, 10, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 10, 10, 10,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
        };
        Assert.That(GetDesireData(mapModel), Is.EqualTo(expectedDesireAfterStatues));

        // ------- add house ------

        var house = AddBuilding(mapModel, 8, 9, MapBuildingType.House2);

        Assert.That(house.HouseLevel, Is.EqualTo(19));

        var expectedDesireAfterHouse = new int[40, 20]
        {
            {  0,  0, 10, 10, 10, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 10, 10, 10,  0,  0, },
            {  0,  0, 10, 12, 12, 22, 24, 24, 24, 24, 24, 24, 24, 24, 22, 12, 12, 10,  0,  0, },
            {  0,  0, 10, 12, 12, 22, 24, 24, 24, 24, 24, 24, 24, 24, 22, 12, 12, 10,  0,  0, },
            { 10, 10, 20, 22, 22, 34, 36, 46, 48, 48, 48, 48, 46, 36, 34, 22, 22, 20, 10, 10, },
            { 10, 12, 22, 24, 24, 36, 38, 48, 52, 52, 52, 52, 48, 38, 36, 24, 24, 22, 12, 10, },
            { 10, 12, 22, 24, 24, 36, 38, 34, 38, 38, 38, 38, 34, 38, 36, 24, 24, 22, 12, 10, },
            { 10, 12, 32, 36, 36, 58, 60, 57, 61, 61, 61, 61, 57, 60, 58, 36, 36, 32, 12, 10, },
            { 10, 12, 32, 38, 38, 60, 64, 61, 66, 66, 66, 66, 61, 64, 60, 38, 38, 32, 12, 10, },
            { 10, 12, 32, 38, 38, 46, 50, 61, 80, 78, 78, 80, 61, 50, 46, 38, 38, 32, 12, 10, },
            { 10, 12, 32, 38, 38, 48, 52, 63, 84, 82, 82, 84, 63, 52, 48, 38, 38, 32, 12, 10, },
            { 10, 12, 32, 38, 38, 46, 50, 61, 80, 80, 80, 80, 61, 50, 46, 38, 38, 32, 12, 10, },
            { 10, 12, 32, 38, 38, 60, 64, 61, 65, 65, 65, 65, 61, 64, 60, 38, 38, 32, 12, 10, },
            { 10, 12, 32, 36, 36, 58, 60, 56, 60, 60, 60, 60, 56, 60, 58, 36, 36, 32, 12, 10, },
            { 10, 12, 22, 24, 24, 36, 38, 34, 38, 38, 38, 38, 34, 38, 36, 24, 24, 22, 12, 10, },
            { 10, 12, 22, 24, 24, 36, 38, 48, 52, 52, 52, 52, 48, 38, 36, 24, 24, 22, 12, 10, },
            { 10, 10, 20, 22, 22, 34, 36, 46, 48, 48, 48, 48, 46, 36, 34, 22, 22, 20, 10, 10, },
            {  0,  0, 10, 12, 12, 22, 24, 24, 24, 24, 24, 24, 24, 24, 22, 12, 12, 10,  0,  0, },
            {  0,  0, 10, 12, 12, 22, 24, 24, 24, 24, 24, 24, 24, 24, 22, 12, 12, 10,  0,  0, },
            {  0,  0, 10, 10, 10, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 10, 10, 10,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
            {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, },
        };
        Assert.That(GetDesireData(mapModel), Is.EqualTo(expectedDesireAfterHouse));

        // ------- move all ------
        mapModel.DoLogging = true;

        Console.WriteLine(new string('*', 40));
        mapModel.ShowDesirabilityState();
        Console.WriteLine(new string('*', 40));

        mapModel.MoveBuildingsByOffset([.. statues, house], 20, 0);

        Console.WriteLine(new string('*', 40));
        mapModel.ShowDesirabilityState();
        Console.WriteLine(new string('*', 40));
    }

    [Test]
    public void Test2()
    {
        var mapModel = new MapModel(6, 6);
        AddBuilding(mapModel, 1, 4, MapBuildingType.Road);
        AddBuilding(mapModel, 2, 1, MapBuildingType.Road);
        AddBuilding(mapModel, 2, 2, MapBuildingType.Road);
        AddBuilding(mapModel, 2, 3, MapBuildingType.Road);
        AddBuilding(mapModel, 2, 4, MapBuildingType.Road);
        AddBuilding(mapModel, 3, 4, MapBuildingType.Road);
        AddBuilding(mapModel, 4, 4, MapBuildingType.Road);

        mapModel.ShowBuildings();

        Assert.That(mapModel.CanAddBuilding(1, 1, MapBuildingType.Pavilion, null), Is.True);
    }

    [Test]
    public void Test3()
    {
        var mapModel = new MapModel(6, 6);

        Assert.That(mapModel.CanAddBuilding(1, 1, MapBuildingType.Pavilion, null), Is.False);
    }

    private static MapBuilding AddBuilding(MapModel mapModel, int left, int top, MapBuildingType mapBuildingType)
    {
        return mapModel.AddBuilding(left, top, mapBuildingType, null) ?? throw new Exception("Building could not be added");
    }

    private static int[,] GetDesireData(MapModel mapModel)
    {
        var result = new int[mapModel.MapSideX, mapModel.MapSideY];
        for (int row = 0; row < mapModel.MapSideX; row++)
        {
            for (int col = 0; col < mapModel.MapSideY; col++)
            {
                result[row, col] = mapModel.Cells[row, col].Desirability;
            }
        }
        return result;
    }
}
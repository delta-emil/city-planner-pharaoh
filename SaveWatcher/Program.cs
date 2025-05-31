namespace SaveWatcher;

using CityPlannerPharaoh.FileDataExtraction;

internal class Program
{
    private const string SavePath = """e:\games_gog\Pharaoh Gold\Save\Shannan\test.sav""";

    private static void Main(string[] args)
    {
        Console.WriteLine("Watching: " + SavePath);
        Console.WriteLine("Press q to quit.");
        
        var lastModDate = DateTime.MinValue;
        while(true)
        {
            var modDate = File.GetLastWriteTime(SavePath);
            if (modDate > lastModDate)
            {
                if (ReportFile(SavePath))
                {
                    lastModDate = modDate;
                }
            }

            var quitRequested = false;
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q)
                {
                    quitRequested = true;
                    break;
                }
            }
            if (quitRequested)
            {
                break;
            }

            Thread.Sleep(100);
        }
        
        Console.WriteLine("Bye!");
    }

    private static bool ReportFile(string savePath)
    {
        PharaohFile.Building[] buildings;
        List<PharaohFile.TerrainTile> tiles;
        uint[,] building_grid_data;
        try
        {
            using var ff = new PharaohFile(savePath);

            (var buildingsReturned, tiles, var building_grid_data_Ret) = ff.GetBuildings();
            if (buildingsReturned == null)
            {
                Console.WriteLine("Failed to read file");
                return false;
            }

            buildings = buildingsReturned;
            building_grid_data = building_grid_data_Ret!;
        }
        catch (IOException)
        {
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }

        Console.WriteLine("|------|-------------|------|----------|-------|-------|");
        Console.WriteLine("| idx  | type / hex  | size | rotation |   x   |   y   |");
        Console.WriteLine("|------|-------------|------|----------|-------|-------|");
        for (int i = 0; i < buildings.Length; i++)
        {
            if (buildings[i].type != 0)
            {
                Console.WriteLine($"| {i,4} | {buildings[i].type,4} / {buildings[i].type,4:X} | {buildings[i].size,4} | {buildings[i].rotation,8} | {buildings[i].x,5} | {buildings[i].y,5} |");
            }
        }
        Console.WriteLine("|------|-------------|------|----------|-------|-------|");

        Console.WriteLine("BUILDINGS GRID DATA:");
        Console.WriteLine("----------------------------------");
        for (int y = 63; y <= 78; y++)
        {
            for (int x = 44; x <= 56; x++)
            {
                Console.Write(building_grid_data[x, y].ToString("X4"));
                Console.Write(' ');
            }
            Console.WriteLine();
        }
        Console.WriteLine("----------------------------------");

        //Console.WriteLine("TILES:");
        //Console.WriteLine("|----------|----------|-------|-------|");
        //Console.WriteLine("| flags    | b.flags  |   x   |   y   |");
        //Console.WriteLine("|----------|----------|-------|-------|");
        //for (int i = 0; i < tiles.Count; i++)
        //{
        //    Console.WriteLine($"| {tiles[i].flags,8:X} | {tiles[i].bflags,8:X} | {tiles[i].x,5} | {tiles[i].y,5} |");
        //}
        //Console.WriteLine("|----------|----------|-------|-------|");

        return true;
    }
}

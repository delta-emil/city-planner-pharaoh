// This class is a C# port (modified to produce a different result) of a C++ class found here:
// http://pecunia.nerdcamp.net/downloads/citybuilding
// With the following license:

/*   Citybuilding Mappers - create minimaps from citybuilding game files
 *   Copyright (C) 2007, 2008  Bianca van Schaik
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License along
 *   with this program; if not, write to the Free Software Foundation, Inc.,
 *   51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System.Text;
using System.Diagnostics;

namespace CityPlannerPharaoh.FileDataExtraction;

public class PharaohFile : GameFile
{
    public PharaohFile(string fileName)
        : base(fileName)
    {
        MAX_MAPSIZE = 228;
    }

    public MapModel? GetMapModelFromFile()
    {
        ok = true;

        // Init grids for use
        uint[,]? building_grid = null, terrain = null;
        byte[,]? edges = null, random = null;
        Walker[]? walkers = null;
        Building[]? buildings = null;
        int mapsize;
        bool is_scenario = false;
        var fourcc = new byte[4];
        InStream.Read(fourcc);
        if (Encoding.ASCII.GetString(fourcc) == "MAPS")
        {
            is_scenario = true;
        }

        if (is_scenario)
        {
            // Read scenario info
            InStream.Seek(0x177c, SeekOrigin.Begin);
            building_grid = readIntGrid();
            edges = readByteGrid();
            terrain = readIntGrid();
            skipBytes(51984);
            random = readByteGrid();
		    InStream.Seek(0x99C78, SeekOrigin.Begin);
            mapsize = (int)readInt();
        }
        else
        {
            InStream.Seek(0x177c, SeekOrigin.Begin);
            building_grid = readCompressedIntGrid();
            edges = readCompressedByteGrid();
            skipCompressed(); // building IDs
            terrain = readCompressedIntGrid();
            random = getRandomData();
            walkers = getWalkers();
            buildings = getBuildings();
            mapsize = getMapsize();
        }

        // Lil' sanity check
        if (!ok || mapsize > MAX_MAPSIZE)
        {
            return null;
        }

        // Transform it to something useful

        var mapModel = new MapModel(mapsize, mapsize, MapTerrain.Void, hasTooCloseToVoidToBuild: true);

        int half = MAX_MAPSIZE / 2;
        int border = (MAX_MAPSIZE - mapsize) / 2;
        int max = border + mapsize;
        int coordX, coordY;
        int start, end;
        uint t_terrain;

        var unknownCodes = new Dictionary<uint, uint>();

        for (int y = border; y < max; y++)
        {
            start = (y < half) ? (border + half - y - 1) : (border + y - half);
            end = (y < half) ? (half + y + 1 - border) : (3 * half - y - border);
            for (int x = start; x < end; x++)
            {
                t_terrain = terrain![x, y];
                var mapTerrain = translateTerrain(t_terrain, unknownCodes);

                // Set pixel colours
                coordX = x - border;
                coordY = y - border;
                // getBitmapCoordinates(x - border, y - border, mapsize, out coordX, out coordY);

                if (0 <= coordX && coordX < mapsize && 0 <= coordY && coordY < mapsize)
                {
                    mapModel.Cells[coordX, coordY].Terrain = mapTerrain;
                }
            }
        }

        foreach (var u in unknownCodes.Keys.OrderBy(x => x))
        {
            Debug.WriteLine($"Unknown terrain: {u:X} encountered {unknownCodes[u]} times.");
        }

        CalculateWaterEdges(mapModel);

        mapModel.SetTooCloseToVoidToBuildAfterInit();

        return mapModel;
    }

    private void CalculateWaterEdges(MapModel mapModel)
    {
        var cells = mapModel.Cells;
        for (int x = 0; x < mapModel.MapSideX; x++)
        {
            for (int y = 0; y < mapModel.MapSideY; y++)
            {
                var terrain = cells[x, y].Terrain;
                if (terrain == MapTerrain.Void) continue;
                if (terrain == MapTerrain.Water)
                {
                    if (   IsLand(cells, x - 1, y - 1)
                        || IsLand(cells, x - 1, y)
                        || IsLand(cells, x - 1, y + 1)
                        || IsLand(cells, x,     y - 1)
                        || IsLand(cells, x,     y + 1)
                        || IsLand(cells, x + 1, y - 1)
                        || IsLand(cells, x + 1, y)
                        || IsLand(cells, x + 1, y + 1))
                    {
                        cells[x, y].Terrain = MapTerrain.WaterEdge;
                    }
                }
                else if (terrain == MapTerrain.Floodpain)
                {
                    SetClearLandTo(cells, x - 1, y - 1, MapTerrain.FloodpainEdge);
                    SetClearLandTo(cells, x - 1, y    , MapTerrain.FloodpainEdge);
                    SetClearLandTo(cells, x - 1, y + 1, MapTerrain.FloodpainEdge);
                    SetClearLandTo(cells, x,     y - 1, MapTerrain.FloodpainEdge);
                    SetClearLandTo(cells, x,     y + 1, MapTerrain.FloodpainEdge);
                    SetClearLandTo(cells, x + 1, y - 1, MapTerrain.FloodpainEdge);
                    SetClearLandTo(cells, x + 1, y,     MapTerrain.FloodpainEdge);
                    SetClearLandTo(cells, x + 1, y + 1, MapTerrain.FloodpainEdge);
                }
            }
        }
    }

    private static bool IsLand(MapCellModel[,] cells, int x, int y)
    {
        if (x < 0 || cells.GetLength(0) <= x
         || y < 0 || cells.GetLength(1) <= y)
        {
            return false;
        }
        var terrain = cells[x, y].Terrain;
        return terrain != MapTerrain.Void
            && terrain != MapTerrain.Water
            && terrain != MapTerrain.WaterEdge;
    }

    private static void SetClearLandTo(MapCellModel[,] cells, int x, int y, MapTerrain newTerrain)
    {
        if (x < 0 || cells.GetLength(0) <= x
         || y < 0 || cells.GetLength(1) <= y)
        {
            return;
        }

        var cell = cells[x, y];
        var terrain = cell.Terrain;
        if (terrain == MapTerrain.Grass
            || terrain == MapTerrain.GrassFarmland
            || terrain == MapTerrain.Sand
            || terrain == MapTerrain.SandFarmland)
        {
            cell.Terrain = newTerrain;
        }
    }

    /**
    * Figures out what the terrain colours for this tile are
    */
    private MapTerrain translateTerrain(uint terrain, Dictionary<uint, uint> unknownCodes)
    {
        if ((terrain & 0x8_00_00) != 0)
        {
            return MapTerrain.Void;
        }

        if ((terrain & 0x1) != 0)
        {
            return MapTerrain.Trees;
        }
        else if ((terrain & 0x4_00_00) != 0)
        {
            return MapTerrain.Reeds;
        }
        else if ((terrain & 0x2) != 0)
        {
            if ((terrain & 0x10_00_00) != 0)
            {
                return MapTerrain.RockOre;
            }
            else
            {
                return MapTerrain.Rock;
            }
        }
        else if ((terrain & 0x4) != 0)
        {
            return MapTerrain.Water;
        }
        else if ((terrain & 0x2_00_00_00) != 0)
        {
            return MapTerrain.Dune;
        }
        else if ((terrain & 0x1_00_00) != 0)
        {
            return MapTerrain.Floodpain;
        }
        else if ((terrain & 0x80) != 0)
        {
            if ((terrain & 0x8_00) != 0)
            {
                return MapTerrain.GrassFarmland;
            }
            else
            {
                return MapTerrain.Grass;
            }
        }
        else if ((terrain & 0x8_00) != 0)
        {
            return MapTerrain.SandFarmland;
        }
        // start ignored codes
        else if ((terrain & 0x40) != 0)
        {
            // road
        }
        // log unknown codes
        else
        {
            if (!unknownCodes.TryGetValue(terrain, out var count))
            {
                // Debug.WriteLine($"Unknown terrain: {terrain:X}");
                count = 0;
            }

            unknownCodes[terrain] = count + 1;
        }
        
        // empty land or watered land
        return MapTerrain.Sand;
    }

    /**
    * Reads the random data grid
    */
    private byte[,]? getRandomData()
    {
        // Assume the first 4 chunks of data have been read
        // The next 4 (useless) compressed parts are stored in the same way
        for (int i = 0; i < 4; i++)
        {
            skipCompressed();
        }

        // Here be the random data
        return readByteGrid();
    }

    /**
    * Gets the walker info from the saved game, which is buried after
    * a few "useless" compressed chunks.
    */
    private Walker[]? getWalkers()
    {
        if (!ok) return null;

        // Assume random data has been read already
        // Which is followed by some more compressed data blocks
        for (int i = 0; i < 5; i++)
        {
            skipCompressed();
        }

        // Next come the walkers in compressed format
        var walkers = new Walker[MAX_WALKERS];
        int length = (int)readInt();

        using var pk = new PKWareInputStream(InStream, length, false);

        // Walker entry = 388 bytes
        for (int i = 0; i < MAX_WALKERS; i++)
        {
            if (pk.hasError()) break;

            pk.skip(10);
            walkers[i].type = pk.readShort();
            pk.skip(8);
            walkers[i].x = pk.readShort();
            walkers[i].y = pk.readShort();
            pk.skip(364);
        }
        pk.empty();

        if (pk.hasError())
        {
            ok = false;
            return null;
        }
        return walkers;
    }

    /**
    * Gets the building info from the saved game
    */
    private Building[]? getBuildings()
    {
        if (!ok) return null;

        // Assume walkers have been read already

        // Three compressed blocks
        for (int i = 0; i < 3; i++)
        {
            skipCompressed();
        }

        // Three ints
        skipBytes(12);

        // Compressed block, followed by 72 bytes, followed by walkers
        skipCompressed();
        skipBytes(72);

        var buildings = new Building[MAX_BUILDINGS];
        int length = (int)readInt();

        using var pk = new PKWareInputStream(InStream, length, false);

        // Building entry = 264 bytes
        for (int i = 0; i < MAX_BUILDINGS; i++)
        {
            if (pk.hasError()) break;

            pk.skip(3);
            buildings[i].size = pk.readByte();
            pk.skip(2);
            buildings[i].x = pk.readShort();
            buildings[i].y = pk.readShort();
            pk.skip(6);
            buildings[i].type = pk.readShort();
            pk.skip(152);
            buildings[i].rotation = pk.readByte();
            pk.skip(93);
        }
        pk.empty();

        if (pk.hasError())
        {
            ok = false;
            return null;
        }
        return buildings;
    }

    /**
    * Returns the map size of a saved game. Assumes everything up to
    * and including buildings have been read already.
    */
    private int getMapsize()
    {
        if (!ok) return 0;

        // Assume buildings have been read already

        // 68 bytes, followed by one compressed
        skipBytes(68);
        skipCompressed();

        // Start of data copied from the .map file!
        // 704 bytes to skip
        skipBytes(704);
        // Next int = map size
        int mapsize = (int)readInt();

        return mapsize;
    }

    private class Walker
    {
        public ushort type;
        public ushort x;
        public ushort y;
    }

    private class Building
    {
        public ushort type;
        public ushort x;
        public ushort y;
        public byte size;
        public byte rotation;
    }
}

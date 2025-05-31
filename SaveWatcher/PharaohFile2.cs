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

using System.Diagnostics;
using System.Text;
using static CityPlannerPharaoh.FileDataExtraction.PharaohFile;

namespace CityPlannerPharaoh.FileDataExtraction;

public class PharaohFile : GameFile
{
    public PharaohFile(string fileName)
        : base(fileName)
    {
        MAX_MAPSIZE = 228;
        MAX_WALKERS = 0; // 2000; // I don't need the walkers
        MAX_BUILDINGS = 4000;
    }

    public (Building[]? Building, List<TerrainTile> Tiles, uint[,]? building_grid) GetBuildings()
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
            return (null, [], null);
        }

        int half = MAX_MAPSIZE / 2;
        int border = (MAX_MAPSIZE - mapsize) / 2;
        int max = border + mapsize;
        int coordX, coordY;
        int start, end;
        uint t_terrain, t_building;

        uint[,] building_grid_shifted = new uint[mapsize, mapsize];

        List<TerrainTile> tiles = new();

        for (int y = border; y < max; y++)
        {
            start = (y < half) ? (border + half - y - 1) : (border + y - half);
            end = (y < half) ? (half + y + 1 - border) : (3 * half - y - border);
            for (int x = start; x < end; x++)
            {
                t_terrain = terrain![x, y];
                t_building = building_grid![x, y];

                // Set pixel colours
                coordX = x - border;
                coordY = y - border;

                building_grid_shifted[coordX, coordY] = t_building;
                // getBitmapCoordinates(x - border, y - border, mapsize, out coordX, out coordY);

                if (0 <= coordX && coordX < mapsize && 0 <= coordY && coordY < mapsize)
                {
                    //if ((t_terrain & 0x40) != 0 || (t_terrain & 0x20) != 0 || (t_terrain & 0x8) != 0)
                    //var cleanFlags = (uint)(t_terrain & (~0x80));
                    //if ((cleanFlags & 0x40) != 0 && (cleanFlags & 0x4) == 0 && cleanFlags != 0x40)
                    //{
                    //    tiles.Add(new TerrainTile { flags = cleanFlags, bflags = t_building, x = (ushort)coordX, y = (ushort)coordY });
                    //}

                    //if (
                    //       (coordY == 65 && coordX is 35 or 36 or 37 or 38 or 39 or 40 or 42)
                    //    || (coordY == 53 && coordX is 42 or 43)
                    //    )

                    //if ((coordY is 65 or 66 or 67 && coordX is 89 or 90 or 91)
                    //    || (coordY is 58 && coordX is 37 or 38))
                    if ((102 <= coordY && coordY <= 104 && 15 <= coordX && coordX <= 29))
                    {
                        tiles.Add(new TerrainTile { flags = t_terrain, bflags = t_building, x = (ushort)coordX, y = (ushort)coordY });
                    }
                }
            }
        }

        return (buildings, tiles, building_grid_shifted);
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

    private struct Walker
    {
        public ushort type;
        public ushort x;
        public ushort y;
    }

    public struct Building
    {
        public ushort type;
        public ushort x;
        public ushort y;
        public byte size;
        public byte rotation;
    }

    public struct TerrainTile
    {
        public uint flags;
        public uint bflags;
        public ushort x;
        public ushort y;
    }
}

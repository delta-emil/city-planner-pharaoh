// This class is a C# port of a C++ class found here:
// http://pecunia.nerdcamp.net/downloads/citybuilding
// With the following license:

/*   Citybuilding Mappers - create minimaps from citybuilding game files
 * This class is a C# port of a C++ class found here:
 * http://pecunia.nerdcamp.net/downloads/citybuilding
 * With the following license:
 * 
 *   Citybuilding Mappers - create minimaps from citybuilding game files
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

namespace CityPlanner.FileDataExtraction;

public class GameFile : IDisposable
{
    protected readonly Stream InStream;
    protected int MAX_MAPSIZE;
    protected int MAX_WALKERS;
    protected int MAX_BUILDINGS;
    protected bool ok;

    public GameFile(string filename, bool compressed = false)
    {
        if (compressed)
        {
            // TODO:
            //this.InStream = new ZlibFile(filename);
            //new System.IO.Compression.GZipStream ??
            throw new NotImplementedException();
        }
        else
        {
            this.InStream = File.OpenRead(filename);
        }
    }

    public void Dispose()
    {
        this.InStream?.Dispose();
    }

    /// <summary>
    /// Reads an uncompressed byte grid from the stream.
    /// </summary>
    protected byte[,]? readByteGrid()
    {
        if (!ok) return null;
        if (InStream.Position == InStream.Length)
        {
            ok = false;
            return null;
        }

        var g = new byte[MAX_MAPSIZE, MAX_MAPSIZE];

        for (int y = 0; y < MAX_MAPSIZE; y++)
        {
            for (int x = 0; x < MAX_MAPSIZE; x++)
            {
                g[x, y] = readByte();
            }
        }

        return g;
    }

    /// <summary>
    /// Reads an uncompressed short grid from the stream.
    /// </summary>
    protected ushort[,]? readShortGrid()
    {
        if (!ok) return null;
        if (InStream.Position == InStream.Length)
        {
            ok = false;
            return null;
        }

        var g = new ushort[MAX_MAPSIZE, MAX_MAPSIZE];

        for (int y = 0; y < MAX_MAPSIZE; y++)
        {
            for (int x = 0; x < MAX_MAPSIZE; x++)
            {
                g[x, y] = readShort();
            }
        }

        return g;
    }

    /// <summary>
    /// Reads an uncompressed int grid from the stream.
    /// </summary>
    protected uint[,]? readIntGrid()
    {
        if (!ok) return null;
        if (InStream.Position == InStream.Length)
        {
            ok = false;
            return null;
        }

        var g = new uint[MAX_MAPSIZE, MAX_MAPSIZE];

        for (int y = 0; y < MAX_MAPSIZE; y++)
        {
            for (int x = 0; x < MAX_MAPSIZE; x++)
            {
                g[x, y] = readInt();
            }
        }

        return g;
    }

    /// <summary>
    /// Reads a compressed byte grid from the stream. Compressed chunks
    /// consist of a length followed by a compressed chunk of that length
    /// </summary>
    protected byte[,]? readCompressedByteGrid()
    {
        if (!ok) return null;
        if (InStream.Position == InStream.Length)
        {
            ok = false;
            return null;
        }

        int length = (int)readInt();
        var g = new byte[MAX_MAPSIZE, MAX_MAPSIZE];

        using var pk = new PKWareInputStream(InStream, length, false);
        for (int y = 0; y < MAX_MAPSIZE; y++)
        {
            if (pk.hasError()) break;
            for (int x = 0; x < MAX_MAPSIZE; x++)
            {
                g[x, y] = pk.readByte();
            }
        }

        pk.empty();
        if (pk.hasError())
        {
            return null;
        }

        return g;
    }

    /// <summary>
    /// Reads a compressed byte grid from the stream. Compressed chunks
    /// consist of a length followed by a compressed chunk of that length
    /// </summary>
    protected ushort[,]? readCompressedShortGrid()
    {
        if (!ok) return null;
        if (InStream.Position == InStream.Length)
        {
            ok = false;
            return null;
        }

        int length = (int)readInt();
        var g = new ushort[MAX_MAPSIZE, MAX_MAPSIZE];

        using var pk = new PKWareInputStream(InStream, length, false);
        for (int y = 0; y < MAX_MAPSIZE; y++)
        {
            if (pk.hasError()) break;
            for (int x = 0; x < MAX_MAPSIZE; x++)
            {
                g[x, y] = pk.readShort();
            }
        }

        pk.empty();
        if (pk.hasError())
        {
            return null;
        }

        return g;
    }

    /// <summary>
    /// Reads a compressed byte grid from the stream. Compressed chunks
    /// consist of a length followed by a compressed chunk of that length
    /// </summary>
    protected uint[,]? readCompressedIntGrid()
    {
        if (!ok) return null;
        if (InStream.Position == InStream.Length)
        {
            ok = false;
            return null;
        }

        int length = (int)readInt();
        var g = new uint[MAX_MAPSIZE, MAX_MAPSIZE];

        using var pk = new PKWareInputStream(InStream, length, false);
        for (int y = 0; y < MAX_MAPSIZE; y++)
        {
            if (pk.hasError()) break;
            for (int x = 0; x < MAX_MAPSIZE; x++)
            {
                g[x, y] = pk.readInt();
            }
        }

        pk.empty();
        if (pk.hasError())
        {
            return null;
        }

        return g;
    }

    /// <summary>
    /// Skips some bytes
    /// </summary>
    protected void skipBytes(int bytes)
    {
        if (!ok) return;
	
	    InStream.Seek(bytes, SeekOrigin.Current);
    }

    /// <summary>
    /// Skips a compressed data block
    /// </summary>
    protected void skipCompressed()
    {
        if (!ok) return;

        uint skip = readInt();
        InStream.Seek(skip, SeekOrigin.Current);
    }

    /// <summary>
    /// Reads a byte from the stream
    /// </summary>
    protected byte readByte()
    {
        if (!ok) return 0;

        var data = InStream.ReadByte();
        if (data < 0)
        {
            ok = false;
            return 0;
        }

        return (byte)data;
    }

    /// <summary>
    /// Reads a short from the stream
    /// </summary>
    protected ushort readShort()
    {
        if (!ok) return 0;

        var data = new byte[2];
        if (InStream.Read(data, 0, data.Length) != 2)
        {
            ok = false;
            return 0;
        }
        return (ushort)(data[0] | (data[1] << 8));
    }

    /// <summary>
    /// Reads a short from the stream
    /// </summary>
    protected uint readInt()
    {
        if (!ok) return 0;

        var data = new byte[4];
        if (InStream.Read(data, 0, data.Length) != 4)
        {
            ok = false;
            return 0;
        }

        uint number = 0;
        for (int i = 0; i < 4; i++)
        {
            number |= (uint)(data[i] << (i * 8));
        }

        return number;
    }

    /// <summary>
    /// Returns the bitmap coordinates for a given map coordinate, for a
    /// diamond-shaped minimap (Caesar 3)
    /// </summary>
    protected void getDiamondBitmapCoordinates(int x, int y, int mapsize,
            out int x_out, out int y_out)
    {
        x_out = x + mapsize - y - 1;
        y_out = x + y;
    }
    
    /// <summary>
    /// Returns the bitmap coordinates for a given map coordinate
    /// </summary>
    protected void getBitmapCoordinates(int x, int y, int mapsize,
            out int x_out, out int y_out)
    {
        x_out = mapsize / 2 + x - y - 1;
        y_out = 1 + x + y - mapsize / 2;
    }

    /// <summary>
    /// Searches the stream for a pattern. Returns true if the pattern has been
    /// found; the file pointer is set to the first byte *after* the pattern.
    /// Returns false if the pattern wasn't found; the file pointer remains at the
    /// place where it was before the function call
    /// </summary>
    /// <param name="pattern">Pattern to search for</param>
    /// <param name="length">Pattern length</param>
    protected bool searchPattern(byte[] pattern, int length)
    {
	    if (InStream.Position == InStream.Length || length <= 0 || length > 256)
        {
		    return false;
	    }

        var bad_char_skip = new int[256];
	
	    for (int i = 0; i< 256; i++)
        {
		    bad_char_skip[i] = length;
	    }

        int last = length - 1;
	    for (int i = 0; i<last; i++) {
		    bad_char_skip[pattern[i]] = last - i;
	    }
	
	    long startpos = InStream.Position;
        long returnpos = startpos;
        int bufsize = 16384;
        var buffer = new byte[16384];
        int haystackIndex;
        int buflength;
        do
        {
            buflength = InStream.Read(buffer, 0, bufsize);
            int hlen = buflength;
            haystackIndex = 0;

            while (hlen >= length)
            {
                /* scan from the end of the needle */
                for (int i = last; buffer[haystackIndex + i] == pattern[i]; i--)
                {
                    if (i == 0)
                    {
                        /* If the first byte matches, we've found it. */
                        // Seek to just after the matched pattern
                        startpos += buflength - hlen + length;
					    InStream.Seek(startpos, SeekOrigin.Begin);
                        return true;
                    }
                }

                hlen -= bad_char_skip[buffer[haystackIndex + last]];
                haystackIndex += bad_char_skip[buffer[haystackIndex + last]];
            }

            // Leave "last" bytes for the next block: they might contain a match
            startpos += buflength - last;
		    InStream.Seek(startpos, SeekOrigin.Begin);

        } while (buflength == bufsize);

        InStream.Seek(returnpos, SeekOrigin.Begin);
        return false;
    }
}

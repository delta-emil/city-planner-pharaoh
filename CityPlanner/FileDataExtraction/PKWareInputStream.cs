using System.Diagnostics;

namespace CityPlanner.FileDataExtraction;

public class PKWareInputStream : IDisposable
{
    // Class variables (comments is where they're initialised)
    private Stream input; // ctor
    private int dictSize; // readHeader
    private byte[]? buffer; // fillBuffer
    private int bufOffset; // fillBuffer
    private int bufBit; // init
    private int dictionary_bits; // readHeader
    private PKDictionary? dictionary; // readHeader

    // For the reading of bytes:
    private int read_offset; // init
    private int read_length; // init
    private bool read_copying; // init
    private int file_length; // ctor or init

    // For detecting end of stream:
    private bool eof_reached; // init, fillBuffer
    private int eof_position; // fillBuffer
    private bool close_stream; // ctor

    private bool at_end;
    private bool has_error;
    private string? errorMessage;

    private const int BUFFER_SIZE = 4096;

    /**
    * Constructor
    * @param stream Open input stream to read from
    * @param close_stream Whether to close the stream upon destroying
    * this object. Set to FALSE if you want to continue reading from
    * the stream after decompressing
    * @param file_length Length of the compressed data. If not given, the
    * object tries to figure it out, but may read beyond the end of the
    * compressed block
    */
    public PKWareInputStream(Stream @in, int file_length, bool close_stream)
    {
        input = @in;
        this.close_stream = close_stream;
        this.file_length = file_length;
        init();
    }

    public void Dispose()
    {
        if (close_stream)
        {
            input?.Dispose();
        }
    }

    /**
    * Reads a single byte from the compressed stream
    */
    public byte read()
    {
        if (has_error) return 0;

        if (read_copying)
        {
            read_length--;
            if (read_length <= 0)
            {
                read_copying = false;
            }
            return dictionary.get(read_offset);
        }
        else
        {
            if (readBit() == 0)
            {
                // Copy byte verbatim
                int result = readBits(8);
                dictionary.put((byte)result);
                return (byte)result;
            }
            // Needs to copy stuff from the dictionary
            read_length = getCopyLength();
            if (read_length >= 519)
            {
                has_error = at_end = true;
                return 0;
            }

            read_offset = getCopyOffset(read_length);
            read_length--;
            read_copying = true;
            return dictionary.get(read_offset);
        }
    }

    /**
    * Reads a block of data of maximum length `length' from the stream
    * @param buf Place to put read data
    * @param length Maximum length to read
    * @return int Number of bytes actually read, may be less than
    * `length' if and only if EOF is encountered
    */
    public int read(byte[]? buf, int length)
    {
        if (has_error) return 0;
        if (buf == null)
        {
            return skipBytes(length);
        }

        int current = 0;
        while (current < length && !has_error)
        {
            if (read_copying)
            {
                while (current < length && read_length > 0)
                {
                    read_length--;
                    buf[current++] = dictionary.get(read_offset);
                }
                if (read_length <= 0)
                {
                    read_copying = false;
                }
            }
            else if (readBit() == 0)
            {
                // Copy byte verbatim
                int result = readBits(8);
                dictionary.put((byte)result);
                buf[current++] = (byte)result;
            }
            else
            {
                // Needs to copy stuff from the dictionary
                read_length = getCopyLength();
                if (read_length >= 519)
                {
                    has_error = at_end = true;
                    return current;
                }

                read_offset = getCopyOffset(read_length);
                buf[current++] = dictionary.get(read_offset);
                read_length--;
                read_copying = true;
            }
        }
        return current;
    }

    /**
    * Reads a byte from the input stream. Same as read()
    */
    public byte readByte()
    {
        if (has_error) return 0;
        return read();
    }

    /**
    * Reads a little-endian short (2 bytes) from the input stream.
    */
    public ushort readShort()
    {
        if (has_error) return 0;
        var data = new byte[2];
        read(data, 2);
        return (ushort)(data[0] + (data[1] << 8));
    }

    /**
    * Reads a little-endian int (4 bytes) from the input stream.
    */
    public uint readInt()
    {
        if (has_error) return 0;
        var data = new byte[4];
        uint number = 0;

        read(data, 4);
        for (int i = 0; i < 4; i++)
        {
            number += (uint)(data[i] << (i * 8));
        }
        return number;
    }

    /**
    * Skips over `length' bytes.
    */
    public void skip(int length)
    {
        skipBytes(length);
    }

    private int skipBytes(int length)
    {
        if (has_error) return 0;

        int current = 0;
        while (current < length && !has_error)
        {
            if (read_copying)
            {
                while (current < length && read_length > 0)
                {
                    read_length--;
                    dictionary.get(read_offset);
                    current++;
                }
                if (read_length <= 0)
                {
                    read_copying = false;
                }
            }
            else if (readBit() == 0)
            {
                // Copy byte verbatim
                dictionary.put((byte)readBits(8));
                current++;
            }
            else
            {
                // Needs to copy stuff from the dictionary
                read_length = getCopyLength();
                if (read_length >= 519)
                {
                    has_error = at_end = true;
                    return current;
                }

                read_offset = getCopyOffset(read_length);
                dictionary.get(read_offset);
                current++;
                read_length--;
                read_copying = true;
            }
        }
        return current;
    }

    /**
    * Empties the stream, reading until EOF is encountered
    */
    /**
    * Reads from the stream (and discards) until EOF is encountered
    */
    public void empty()
    {
        if (has_error) return;

        while (!has_error)
        {
            read();
        }
    }

    /**
    * Returns whether end-of-file is reached
    */
    public bool atEnd()
    {
        return at_end;
    }

    /**
    * Returns whether there was an error during decompression
    */
    public bool hasError()
    {
        return has_error && !at_end;
    }

    /**
    * Returns the last error message. Behaviour is undefined if
    * {@link hasError()} returns false
    */
    public string? error()
    {
        return errorMessage;
    }

    /// <summary>
    /// Initialises the stream
    /// </summary>
    private void init()
    {
        // Init the variables
        has_error = false;
        at_end = false;
        eof_reached = false;
        dictionary = null;
        buffer = null;

        bufBit = 0;
        read_offset = 0;
        read_length = 0;
        read_copying = false;

        if (file_length <= 2)
        {
            setError("File too small");
            return;
        }

        readHeader();
        if (has_error)
        {
            return;
        }
        fillBuffer();
    }

    /**
    * Reads the 2-byte header and initialises the dictionary
    */
    private void readHeader()
    {
        // Read the header to decide on the encoding type
        byte c = (byte)input.ReadByte();
        if (c != 0)
        {
            setError("Static dictionary not supported");
            return;
        }

        c = (byte)input.ReadByte();
        dictionary_bits = (int)c;
        switch (dictionary_bits)
        {
            case 4: dictSize = 1024; break;
            case 5: dictSize = 2048; break;
            case 6: dictSize = 4096; break;
            default:
                setError("Unknown dictionary size");
                return;
        }
        dictionary = new PKDictionary(dictSize);
        buffer = new byte[BUFFER_SIZE];
        file_length -= 2; // Subtract two header bytes from total file length
    }

    /**
    * Gets the amount of bytes to copy from the dictionary
    */
    private int getCopyLength()
    {
        int bits;

        bits = readBits(2);
        if (bits == 3)
        { // 11
            return 3;
        }
        else if (bits == 1)
        { // 10x
            return 4 - 2 * readBit();
        }
        else if (bits == 2)
        { // 01
            if (readBit() == 1)
            { // 011
                return 5;
            }
            else
            { // 010x
                return 7 - readBit();
            }
        }
        else if (bits == 0)
        { // 00
            bits = readBits(2);
            if (bits == 3)
            { // 0011
                return 8;
            }
            else if (bits == 1)
            { // 0010
                if (readBit() == 1)
                { // 00101
                    return 9;
                }
                else
                { // 00100x
                    return 10 + readBit();
                }
            }
            else if (bits == 2)
            { // 0001
                if (readBit() == 1)
                { // 00011xx
                    return 12 + readBits(2);
                }
                else
                { // 00010xxx
                    return 16 + readBits(3);
                }
            }
            else if (bits == 0)
            { // 0000
                bits = readBits(2);
                switch (bits)
                {
                    case 3: return 24 + readBits(4); // 000011xxxx
                    case 1: return 40 + readBits(5); // 000010xxxxx
                    case 2: return 72 + readBits(6); // 000001xxxxxx
                    case 0:
                        if (readBit() == 1)
                        {
                            return 136 + readBits(7); // 0000001xxxxxxx
                        }
                        else
                        {
                            return 264 + readBits(8); // 0000000xxxxxxxx
                        }
                }
            }
        }
        // Cannot happen
        return -1;
    }

    /**
    * Gets the offset at which to start copying bytes from the dictionary
    */
    private int getCopyOffset(int length)
    {
        int lower_bits, result;
        if (length == 2)
        {
            lower_bits = 2;
        }
        else
        {
            lower_bits = dictionary_bits;
        }

        result = getCopyOffsetHigh() << lower_bits;
        result |= readBits(lower_bits);
        return result;
    }

    /**
    * Gets the "high" value of the copy offset, the lower N bits
    * are stored verbatim; N depends on the copy length and the
    * dictionary size.
    */
    private int getCopyOffsetHigh()
    {
        int bits;

        bits = readBits(2);
        if (bits == 3)
        { // 11
            return 0;
        }
        else if (bits == 1)
        { // 10
            bits = readBits(2);
            switch (bits)
            {
                case 0: return 0x6 - readBit(); // 1000x
                case 1: return 0x2; // 1010
                case 2: return 0x4 - readBit(); // 1001x
                case 3: return 0x1; // 1011
            }
        }
        else if (bits == 2)
        { // 01
            bits = readBits(4);
            if (bits == 0)
            {
                return 0x17 - readBit();
            }
            else
            {
                return 0x16 - reverse(bits, 4);
            }
        }
        else if (bits == 0)
        { // 00
            bits = readBits(2);
            switch (bits)
            {
                case 3: return 0x1f - reverse(readBits(3), 3);
                case 1: return 0x27 - reverse(readBits(3), 3);
                case 2: return 0x2f - reverse(readBits(3), 3);
                case 0: return 0x3f - reverse(readBits(4), 4);
            }
        }
        // Cannot happen
        return -1;
    }

    /**
    * Reverse the bits in `number', essentially converting it from little
    * endian to big endian or vice versa.
    */
    private int reverse(int number, int length)
    {
        if (length == 3)
        {
            switch (number)
            {
                case 1: return 4;
                case 3: return 6;
                case 4: return 1;
                case 6: return 3;
                default: return number;
            }
        }
        else if (length == 4)
        {
            switch (number)
            {
                case 1: return 8;
                case 2: return 4;
                case 3: return 12;
                case 4: return 2;
                case 5: return 10;
                case 7: return 14;
                case 8: return 1;
                case 10: return 5;
                case 11: return 13;
                case 12: return 3;
                case 13: return 11;
                case 14: return 7;
                default: return number;
            }
        }
        return number;
    }

    /**
    * Fill the internal buffer
    */
    private void fillBuffer()
    {
        bufOffset = 0;
        if (file_length <= BUFFER_SIZE)
        {
            input.Read(buffer, 0, file_length);
            eof_reached = true;
            eof_position = file_length;
        }
        else
        {
            input.Read(buffer, 0, BUFFER_SIZE);
            file_length -= BUFFER_SIZE;
        }
    }

    /**
    * Advances the data pointer one byte, filling the buffer
    * if necessary
    */
    private void advanceByte()
    {
        bufOffset++;
        if (eof_reached && bufOffset >= eof_position)
        {
            setError("Unexpected EOF");
            return;
        }
        if (bufOffset >= BUFFER_SIZE)
        {
            fillBuffer();
        }
        bufBit = 0;
    }

    /**
    * Reads one single bit
    */
    private byte readBit()
    {
        if (bufBit == 8)
        {
            advanceByte();
        }
        byte b = (byte)((buffer[bufOffset] >> bufBit) & 1);
        bufBit++;
        return b;
    }

    /**
    * Reads bits in little endian order
    * @param length Number of bits to read. Should never be more than 8.
    * @return int Value of the bits read
    */
    private int readBits(int length)
    {
        int result;
        if (bufBit == 8)
        {
            advanceByte();
        }
        // Check to see if we span multiple bytes
        if (bufBit + length > 8)
        {
            // First take last remaining bits in this byte & put them in place
            // Do "& 0xff" to prevent a negative character from filling with ff's
            result = ((buffer[bufOffset] & 0xff) >> bufBit);
            int length1 = 8 - bufBit;
            int length2 = length - length1;
            advanceByte();

            // Read length2 bits from the second byte & add them to the result
            result |= ((buffer[bufOffset]) & ((1 << length2) - 1)) << length1;
            bufBit = length2;
        }
        else
        {
            // Same byte, easy!
            result = (buffer[bufOffset] >> bufBit) & ((1 << length) - 1);
            bufBit += length;
        }
        return result;
    }

    private void setError(string message)
    {
        Debug.WriteLine("Setting error to true: " + message);
	    errorMessage = message;
	    has_error = true;
    }

private class PKDictionary
    {
        private int size;
        private int first;
        private byte[] dictionary;

        public PKDictionary(int size)
        {
            this.dictionary = new byte[size];
            this.size = size;
            this.first = -1;
        }

        /// <summary>
        /// Returns the byte at the specified position.
		/// Also does a PUT for this byte since the compression
		/// algorithm requires it
        /// </summary>
        public byte get(int position)
        {
            int index = (size + first - position) % size;
            put(dictionary[index]);
            return dictionary[index];
        }

        /// <summary>
        /// Adds a byte to the dictionary
        /// </summary>
        public void put(byte b)
        {
            first = (first + 1) % size;
            dictionary[first] = b;
        }
    }
}

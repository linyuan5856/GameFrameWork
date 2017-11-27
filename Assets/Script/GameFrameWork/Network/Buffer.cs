// RingBuffer.cs --- This is where you apply your OCD.
//
// Copyright (C) 2016 Damon Kwok
//
// Author: gww <DamonKwok@msn.com>
// Date: 2015-09-01
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
//Code:

using System;

public class FloatUtil
{
    public static float intBitsToFloat(int v)
    {
        byte[] buf = BitConverter.GetBytes(v);
        return BitConverter.ToSingle(buf, 0);
    }
	
    public static int floatToRawIntBits(float v)
    {
        byte[] buf = BitConverter.GetBytes(v);
        return BitConverter.ToInt32(buf, 0);
    }
}

public class DoubleUtil
{
    public static double longBitsToDouble(long v)
    {
        byte[] buf = BitConverter.GetBytes(v);
        return BitConverter.ToDouble(buf, 0);
    }
	
    public static long doubleToRawLongBits(double v)
    {
        byte[] buf = BitConverter.GetBytes(v);
        return BitConverter.ToInt64(buf, 0);
    }
}

public abstract class NetBuffer//  : System.IO.Stream/*implements ByteBuffer*/
{

    #region Interface

    public abstract void Clear();
		
    public abstract bool IsFull{ get; }
		
    public abstract bool IsEmpty{ get; }
		
    // length
    public abstract int BufferSize{ get; }
		
    public abstract int DataSize{ get; }
		
    public abstract int FreeSize{ get; }
		
    public abstract int LineDataSize{ get; }
		
    public abstract int LineFreeSize{ get; }
		

    //Byte
    public abstract byte ReadByte();
	
    public abstract void WriteByte(byte val);

    #endregion

    #region Seek

    void Seek(int pos)
    {

    }

    #endregion

    #region Read/Write Char

    //Char
    public char ReadChar()
    {
        return (char)this.ReadByte();
    }
	
    public void WriteChar(char val)
    {
        this.WriteByte((byte)val);
    }

    #endregion

    #region Read/Write Number

    // Int8
    public int ReadInt8()
    {
        return (int)this.ReadByte();
    }
		
    public void WriteInt8(int val)
    {
        this.WriteByte((byte)val);
    }
	
    // Boolean
    public bool ReadBoolean()
    {
        return this.ReadByte() != 0;
    }
		
    public void WriteBoolean(bool val)
    {
        this.WriteByte(((byte)(val ? 1 : 0)));
    }
		
    // Int16
    public short ReadInt16LE()
    {
        int accum = 0;
			
        accum |= (this.ReadByte() & 0xff);
        accum |= (this.ReadByte() & 0xff) << 8;
			
        return (short)accum;
    }
		
    public void WriteInt16LE(int val)
    {
        this.WriteByte((byte)val);
        this.WriteByte((byte)(val >> 8));
    }
		
    public short ReadInt16BE()
    {
        int accum = 0;
			
        accum |= (this.ReadByte() & 0xff) << 8;
        accum |= (this.ReadByte() & 0xff);
			
        return (short)accum;
    }
		
    public void WriteInt16BE(int val)
    {
        this.WriteByte((byte)(val >> 8));
        this.WriteByte((byte)val);
    }
		
    // Int32
    public int ReadInt32LE()
    {
        int accum = 0;
			
        accum |= (this.ReadByte() & 0xff);
        accum |= (this.ReadByte() & 0xff) << 8;
        accum |= (this.ReadByte() & 0xff) << 16;
        accum |= (this.ReadByte() & 0xff) << 24;
			
        return accum;
    }
		
    public void WriteInt32LE(int val)
    {
        this.WriteByte((byte)(val));
        this.WriteByte((byte)(val >> 8));
        this.WriteByte((byte)(val >> 16));
        this.WriteByte((byte)(val >> 24));
    }
		
    public int ReadInt32BE()
    {
        int accum = 0;
			
        accum |= (this.ReadByte() & 0xff) << 24;
        accum |= (this.ReadByte() & 0xff) << 16;
        accum |= (this.ReadByte() & 0xff) << 8;
        accum |= (this.ReadByte() & 0xff);
			
        return accum;
    }
		
    public void WriteInt32BE(int val)
    {
        this.WriteByte((byte)(val >> 24));
        this.WriteByte((byte)(val >> 16));
        this.WriteByte((byte)(val >> 8));
        this.WriteByte((byte)(val));
    }
		
    // Int64
    public long ReadInt64LE()
    {
        long accum = 0;
			
        accum |= ((long)this.ReadByte() & 0xff);
        accum |= (((long)this.ReadByte() & 0xff) << 8);
        accum |= (((long)this.ReadByte() & 0xff) << 16);
        accum |= (((long)this.ReadByte() & 0xff) << 24);
        accum |= (((long)this.ReadByte() & 0xff) << 32);
        accum |= (((long)this.ReadByte() & 0xff) << 40);
        accum |= (((long)this.ReadByte() & 0xff) << 48);
        accum |= (((long)this.ReadByte() & 0xff) << 56);
			
        return accum;
    }
		
    public void WriteInt64LE(long val)
    {
        this.WriteByte((byte)(val));
        this.WriteByte((byte)(val >> 8));
        this.WriteByte((byte)(val >> 16));
        this.WriteByte((byte)(val >> 24));
        this.WriteByte((byte)(val >> 32));
        this.WriteByte((byte)(val >> 40));
        this.WriteByte((byte)(val >> 48));
        this.WriteByte((byte)(val >> 56));
    }
		
    public long ReadInt64BE()
    {
        long accum = 0;
			
        accum |= ((long)this.ReadByte() & 0xff) << 56;
        accum |= ((long)this.ReadByte() & 0xff) << 48;
        accum |= ((long)this.ReadByte() & 0xff) << 40;
        accum |= ((long)this.ReadByte() & 0xff) << 32;
        accum |= ((long)this.ReadByte() & 0xff) << 24;
        accum |= ((long)this.ReadByte() & 0xff) << 16;
        accum |= ((long)this.ReadByte() & 0xff) << 8;
        accum |= ((long)this.ReadByte() & 0xff);
			
        return accum;
    }

    public void WriteInt64BE(long val)
    {
        this.WriteByte((byte)(val >> 56));
        this.WriteByte((byte)(val >> 48));
        this.WriteByte((byte)(val >> 40));
        this.WriteByte((byte)(val >> 32));
        this.WriteByte((byte)(val >> 24));
        this.WriteByte((byte)(val >> 16));
        this.WriteByte((byte)(val >> 8));
        this.WriteByte((byte)(val));
    }
		
    // Float32
    public float ReadFloat32LE()
    {
        int accum = this.ReadInt32LE();
        return FloatUtil.intBitsToFloat(accum);
    }
		
    public void WriteFloat32LE(float val)
    {
        int accum = FloatUtil.floatToRawIntBits(val);
        this.WriteInt32LE(accum);
    }
		
    public float ReadFloat32BE()
    {
        int accum = this.ReadInt32BE();
        return FloatUtil.intBitsToFloat(accum);
    }
		
    public void WriteFloat32BE(float val)
    {
        int accum = FloatUtil.floatToRawIntBits(val);
        this.WriteInt32BE(accum);
    }
		
    // Float64
    public double ReadFloat64LE()
    {
        long accum = this.ReadInt64LE();
        return DoubleUtil.longBitsToDouble(accum);
    }
		
    public void WriteFloat64LE(double val)
    {
        long accum = DoubleUtil.doubleToRawLongBits(val);
        this.WriteInt64LE(accum);
    }
		
    public double ReadFloat64BE()
    {
        long accum = this.ReadInt64BE();
        return DoubleUtil.longBitsToDouble(accum);
    }
		
    public void WriteFloat64BE(double val)
    {
        long accum = DoubleUtil.doubleToRawLongBits(val);
        this.WriteInt64BE(accum);
    }

    #endregion

    #region Read/Write Line

    // Line
    public string ReadLine(byte lineend)
    {
        int line_size = 1024;
        char[] line = new char[line_size];
        int line_index = 0;
        while (true)
        {
            // 闂傚啯褰冪亸绲搉            if (line_index == line_size)
            {
                int new_line_size = line_size + 1024;
                char[] new_line = new char[new_line_size];
                System.Buffer.BlockCopy(line, 0, new_line, 0, line_index);//System.arraycopy (line, 0, new_line, 0, line_index);
                line_size = new_line_size;
                line = new_line;
            }
            // read
            line[line_index] = (char)this.ReadByte();
            if (line[line_index] == lineend)
                break;
            line_index++;
        }

        return new string(line, 0, line_index); //new string (line, 0, line_index);
        /*
		 * String ret=""; try { ret = new String(bytes, 0, index, "UTF8"); }
		 * catch (UnsupportedEncodingException e) { e.printStackTrace(); } // ,
		 * "Unicode" return ret;
		 */
    }
		
    public void WriteLine(string val, byte lineend)
    {
        char[] bytes = val.ToCharArray();
        for (int i = 0; i < bytes.Length; i++)
        {
            this.WriteChar(bytes[i]);
        }
        this.WriteByte(lineend);
    }
		
    public void ReadLine(NetBuffer dst, byte lineend)
    {
        byte b;
        while (!this.IsEmpty)
        {
            b = this.ReadByte();
            dst.WriteByte(b);
            if (b == lineend)
                break;
        }
    }
		
    public void WriteLine(NetBuffer src, byte lineend)
    {
        src.ReadLine(this, lineend);
    }

    #endregion

    #region Read/Write String

    // string
    public string ReadStringLE()
    {
        return this.ReadUTF8(true);
    }
	
    public void WriteStringLE(string val)
    {
        this.WriteUTF8(val, true);
    }
	
    public void WriteStringLE()
    {
        this.WriteInt16LE(0);
    }

    public string ReadStringBE()
    {
        return this.ReadUTF8(false);
    }

    public void WriteStringBE(string val)
    {
        this.WriteUTF8(val, false);
    }
	
    public void WriteStringBE()
    {
        this.WriteInt16BE(0);
    }
	
    //utf8
    public String ReadUTF8(bool LE = true)
    {
        //int b_length = this.length();
        int utflen = LE ? this.ReadInt16LE() : this.ReadInt16BE();
        if (utflen == 0)
            return "";
        char[] data = this.ReadCharArray(utflen);
        char[] charArray = new char[utflen];
        int count = 0;
        int b1 = 0, b2 = 0, b3 = 0;
        int readPos = 0;
        int endpos = readPos + utflen;
        while (readPos < endpos)
        {
            b1 = data[readPos++] & 0xff;
            if (b1 < 127)
            {
                charArray[count++] = (char)b1;
            }
            else if ((b1 >> 5) == 7)
            {
                b2 = data[readPos++];
                b3 = data[readPos++];
                charArray[count++] = (char)((b1 & 0xf) << 12 | (b2 & 0x3f) << 6 | (b3 & 0x3f));
            }
            else
            {
                b2 = data[readPos++];
                charArray[count++] = (char)((b1 & 0x1f) << 6 | (b2 & 0x3f));
            }
        }
        return new string(charArray, 0, count);
    }

    public void WriteUTF8(string s, bool LE = true)
    {
        if (s == null)
            s = "";
        int strlen = s.Length;
        int utflen = 0;
        for (int i = 0; i < strlen; i++)
        {
            char c = s.ToCharArray()[i];
            if (c < 127)
                utflen++;
            else if (c > 2047)
                utflen += 3;
            else
                utflen += 2;
        }
        //if (utflen > 65535)
        //	throw new IllegalArgumentException("the string is too long:" + strlen);
		
        //ensureCapacity(utflen + 2 + writePos);
        if (LE)
            this.WriteInt16LE(utflen);
        else
            this.WriteInt16BE(utflen);
        for (int i = 0; i < strlen; i++)
        {
            char c = s.ToCharArray()[i];
            if (c < 127)
            {
                this.WriteChar(c);//data[writePos++] = (byte)c;
            }
            else if (c > 2047)
            {
                this.WriteByte((byte)(0xE0 | ((c >> 12) & 0x0F)));
                this.WriteByte((byte)(0x80 | ((c >> 6) & 0x3F)));
                this.WriteByte((byte)(0x80 | ((c >> 0) & 0x3F)));
            }
            else
            {
                this.WriteByte((byte)(0xC0 | ((c >> 6) & 0x1F)));
                this.WriteByte((byte)(0x80 | ((c >> 0) & 0x3F)));
            }
        }
    }

    #endregion

    #region Read/Write Array

    // CharArray
    public void ReadCharArray(char[] dst, int size)
    {
        for (int i = 0; i < size; i++)
        {
            dst[i] = this.ReadChar();
        }
    }
	
    public char[] ReadCharArray(int size)
    {
        char[] dst = new char[size];
        for (int i = 0; i < size; i++)
        {
            dst[i] = this.ReadChar();
        }
        return dst;
    }

    public void WriteCharArray(char[] src, int size)
    {
        for (int i = 0; i < size; i++)
        {
            this.WriteChar(src[i]);
        }
    }

    //Bytes
    public void ReadByteArray(byte[] dst, int size)
    {
        for (int i = 0; i < size; i++)
        {
            dst[i] = this.ReadByte();
        }
    }
		
    public byte[] ReadByteArray(int size)
    {
        byte[] dst = new byte[size];
        for (int i = 0; i < size; i++)
        {
            dst[i] = this.ReadByte();
        }
        return dst;
    }

    public void WriteByteArray(string val)
    { 
        char[] bytes = val.ToCharArray();
        for (int i = 0; i < bytes.Length; i++)
        {
            this.WriteChar(bytes[i]);
        }
    }
		
    public void WriteByteArray(byte[] src, int size)
    {
        for (int i = 0; i < size; i++)
        {
            this.WriteByte(src[i]);
        }
    }

    #endregion

    #region Read/Write Buffer

    //Buffer
    public void ReadBuffer(NetBuffer dst, int size)
    {
        for (int i = 0; i < size; i++)
        {
            dst.WriteByte(this.ReadByte());
        }
    }
		
    public void WriteBuffer(NetBuffer src, int size)
    {
        for (int i = 0; i < size; i++)
        {
            this.WriteByte(src.ReadByte());
        }
    }

    #endregion
}
















#region Old Code

/*
[StructLayout(LayoutKind.Explicit, Size = 8)]
public struct Float
{
	[FieldOffset(0)]
	int i;
	[FieldOffset(0)]
	float f;
	
	public static int floatToRawIntBits(float f)
	{
		Float u;
		u.i = 0;
		u.f = f;
		return u.i;
	}
	public static float intBitsToFloat(int i)
	{
		Float u;
		u.f = 0;
		u.i = i;
		return u.f;
	}
};
using System.Runtime.InteropServices;
*/

/*
	public string ReadStringLE ()
	{
		int size = this.ReadInt16BE ();
		if (size > 0) {
			string val = new string (this.ReadCharArray (size), 0, size);
			return val;
		}
			
		return "";
	}
		
	public void WriteStringLE (string val)
	{
		char[] bytes = val.ToCharArray ();
		this.WriteInt16LE (bytes.Length);

		for (int i = 0; i < bytes.Length; i++) {
			this.WriteChar (bytes [i]);
		}
	}
		
	public void WriteStringLE ()
	{
		this.WriteInt16LE (0);
	}

	public void WriteStringBE (string val)
	{
		char[] bytes = val.ToCharArray ();
		this.WriteInt16LE (bytes.Length);
		
		for (int i = 0; i < bytes.Length; i++) {
			this.WriteChar (bytes [i]);
		}
	}
	
	public void WriteStringBE ()
	{
		this.WriteInt16BE (0);
	}
	*/
#endregion

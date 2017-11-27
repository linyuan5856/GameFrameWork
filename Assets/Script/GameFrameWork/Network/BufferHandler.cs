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

public class BufferHandler : NetBuffer
{
    protected byte[] databuf = null;
    protected int buffer_size;
    protected int read_offset;
    protected int write_offset;

    public BufferHandler(byte[] buffer, int buffer_size, int data_size)
    {
        this.databuf = buffer;
        this.buffer_size = buffer_size;
        this.read_offset = 0;
        this.write_offset = data_size;
    }
	
    //@Override
    public override void Clear()
    {
        this.read_offset = this.write_offset = 0;
    }
	
    //@Override
    public override bool IsFull
    {
        get{ return (this.write_offset == this.buffer_size); }
    }
	
    //@Override
    public override bool IsEmpty
    {
        get{ return (this.write_offset == 0); }
    }
	
    // Size
    //@Override
    public override int BufferSize
    {
        get{ return this.buffer_size; }
    }
	
    //@Override
    public override int DataSize
    {
        get{ return this.write_offset - this.read_offset; }
    }
	
    //@Override
    public override int FreeSize
    {
        get{ return this.buffer_size - this.write_offset; }
    }
	
    //@Override
    public override int LineDataSize
    {
        get{ return this.DataSize; }
    }
	
    //@Override
    public override int LineFreeSize
    {
        get{ return this.FreeSize; }
    }
	
    // Byte
    //@Override
    public override byte ReadByte()
    {
        byte ret = this.databuf[this.read_offset];
        this.read_offset += 1;
        return ret;
    }
	
    //@Override
    public override void WriteByte(byte val)
    {
        this.databuf[this.write_offset] = val;
        this.write_offset += 1;
    }
	
    // Bytes
    public byte[] GetByteArray()
    {
        return this.databuf;
    }
	
    public int GetReadOffset()
    {
        return this.read_offset;
    }
	
    public int GetWriteOffset()
    {
        return this.write_offset;
    }

    //SUM
    public int CalculateSum()
    {
        int sum = 0x77;
        int pos = 0;
        int len = this.DataSize;
        while (pos < len)
        {
            sum += this.GetByteArray()[pos++] & 0xff;
        }
        return (sum) & 0x7F7F;
    }
    
}

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

public class LineBuffer : NetBuffer
{
	
    protected byte[] buffer = null;
    protected int buffer_size;
    protected int read_offset;
    protected int write_offset;
    protected int expand_size;
	
    public LineBuffer(int capacity, int expand_size)
    {
        //super();
        this.buffer = new byte[capacity];
        this.buffer_size = capacity;
        this.read_offset = 0;
        this.write_offset = 0;
        this.expand_size = expand_size;
    }
	
    // clear
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
	
    // size
    //@Override
    public override int BufferSize
    {
        get{ return this.buffer_size; }
    }
	
    public int ExpandSize
    {
        get{ return this.expand_size; }
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
        byte ret = 0;
        if (this.read_offset < this.write_offset)
        {
            ret = this.buffer[this.read_offset];
            this.add_read_offset(1);
        }
		
        return ret;
    }
	
    //@Override
    public override void WriteByte(byte val)
    {
        if ((this.write_offset + 1) > this.buffer_size)
        {
            int new_buff_size = this.buffer_size + this.expand_size;
            byte[] new_databuf = new byte[new_buff_size];
			
            System.Buffer.BlockCopy(this.buffer, 0, new_databuf, 0, this.write_offset);//System.arraycopy
            this.buffer = new_databuf;
            this.buffer_size = new_buff_size;
        }
		
        this.buffer[this.write_offset] = val;
        this.add_write_offset(1);
    }
	
    // offset
    protected int add_read_offset(int offset)
    {
        this.read_offset += offset;
        return this.read_offset;
    }
	
    protected int add_write_offset(int offset)
    {
        this.write_offset += offset;
        return this.write_offset;
    }
	
    // Bytes
    public byte[] GetBytes()
    {
        return this.buffer;
    }
	
    public int GetReadOffset()
    {
        return this.read_offset;
    }
	
    public int GetWriteOffset()
    {
        return this.write_offset;
    }
	
}


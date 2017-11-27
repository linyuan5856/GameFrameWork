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

public class RingBuffer : NetBuffer
{
	
    protected byte[] buffer = null;
    protected int buffer_size;
    protected int read_offset;
    protected int write_offset;
    protected int expand_size;
	
    public RingBuffer(int capacity, int expand_size)
    {
        //super();
        this.buffer = new byte[capacity];
        this.buffer_size = capacity;
        this.read_offset = 0;
        this.write_offset = 0;
        this.expand_size = expand_size;
    }
	
    //clear
    //@Override
    public override void Clear()
    {
        this.read_offset = this.write_offset = 0;
    }
	
    //@Override
    public override bool IsFull
    {
        get
        {
            return (((this.write_offset + 1) %
            this.buffer_size) == this.read_offset);
        }
    }
	
    //@Override
    public override bool IsEmpty
    {
        get{ return (this.read_offset == this.write_offset); }
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
        get
        {
            if (this.write_offset == this.read_offset)
                return 0;
			
            return (this.write_offset > this.read_offset) ? (this.write_offset - this.read_offset)
			: (this.buffer_size - this.read_offset + this.write_offset);
        }
    }
	
    //@Override
    public override int FreeSize
    {
        get
        {
            if (this.write_offset == this.read_offset)
                return this.buffer_size;
			
            return (this.write_offset > this.read_offset) ? (this.buffer_size
            - this.write_offset + this.read_offset)
				: (this.read_offset - this.write_offset);
        }
    }
	
    //@Override
    public override int LineDataSize
    {
        get
        {
            if (this.write_offset == this.read_offset)
                return 0;
			
            return (this.write_offset > this.read_offset) ? (this.write_offset - this.read_offset)
				: (this.buffer_size - this.read_offset);
        }
    }
	
    //@Override
    public override int LineFreeSize
    {
        get
        {
            if (this.write_offset == this.read_offset)
                return this.buffer_size - this.write_offset;
			
            return (this.write_offset > this.read_offset) ? (this.buffer_size - this.write_offset)
				: (this.read_offset - this.write_offset);
        }
    }
	
    // Byte
    //@Override
    public override byte ReadByte()
    {
        byte ret = this.buffer[this.read_offset];
        this.add_read_offset(1);
        return ret;
    }
	
    //@Override
    public override void WriteByte(byte val)
    {
        if (this.IsFull)
        {
            int new_buff_size = this.buffer_size + this.expand_size;
            byte[] new_buffer = new byte[new_buff_size];
            //byte []old_buffer = this.buffer;
            //size_t data_size = this.DataSize();
            int size = 0;
            while (!this.IsEmpty)
            {
                new_buffer[size] = this.ReadByte();
                size++;
            }
            this.buffer = new_buffer;
            this.buffer_size = new_buff_size;
            this.read_offset = 0;
            this.write_offset = size;
        }
		
        this.buffer[this.write_offset] = val;
        this.add_write_offset(1);
    }
	
    // offset
    protected int add_read_offset(int offset)
    {
        this.read_offset = ((this.read_offset + offset) % this.buffer_size);
		
        return this.read_offset;
    }
	
    protected int add_write_offset(int offset)
    {
        this.write_offset = ((this.write_offset + offset) % this.buffer_size);
		
        return this.write_offset;
    }
	
}

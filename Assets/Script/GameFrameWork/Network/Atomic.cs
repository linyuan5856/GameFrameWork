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

using System.Threading;

public class AtomicLong
{
	private long value;
	
	public AtomicLong (long value)
	{
		this.value = value;
	}
	
	public long Get ()
	{
		return Interlocked.Read (ref this.value);
	}
	
	public void Set (long newValue)
	{
		Interlocked.Exchange (ref this.value, newValue);
	}

	public long Add (long delta)
	{
		return this.AddAndGet(delta);
	}
	
	public bool CompareAndSet (long newValue, long comprand)
	{
		return Interlocked.CompareExchange (ref this.value, newValue, comprand) == comprand;
	}
	
	//GetAnd*
	public long GetAndAdd (long delta)
	{
		return Interlocked.Add (ref this.value, delta);
	}
	
	public long GetAndSet (long newValue)
	{
		return Interlocked.Exchange (ref this.value, newValue);
	}
	
	public long GetAndIncrement ()
	{
		return Interlocked.Increment (ref this.value);
	}
	
	public long GetAndDecrement ()
	{
		return Interlocked.Decrement (ref this.value);
	}
	
	//*AndGet
	public long AddAndGet (long delta)
	{
		return Interlocked.Add (ref this.value, delta) + delta;
	}
	
	public long SetAndGet (long newValue)
	{
		Interlocked.Exchange (ref this.value, newValue);
		return newValue;
	}
	
	public long IncrementAndGet ()
	{
		return Interlocked.Increment (ref this.value) + 1;
	}
	
	public long DecrementAndGet ()
	{
		return Interlocked.Decrement (ref this.value) - 1;
	}
	
}

public class AtomicInteger
{
	private long value;
	
	public AtomicInteger (int value)
	{
		this.value = value;
	}
	
	public int Get ()
	{
		return (int)Interlocked.Read (ref this.value);
	}
	
	public void Set (int newValue)
	{
		Interlocked.Exchange (ref this.value, newValue);
	}

	public int Add (int delta)
	{
		return this.AddAndGet(delta);
	}
	
	public bool CompareAndSet (int newValue, int comprand)
	{
		return Interlocked.CompareExchange (ref this.value, newValue, comprand) == comprand;
	}
	
	//GetAnd*
	public int GetAndAdd (int delta)
	{
		return (int)Interlocked.Add (ref this.value, delta);
	}
	
	public int GetAndSet (int newValue)
	{
		return (int)Interlocked.Exchange (ref this.value, newValue);
	}
	
	public int GetAndIncrement ()
	{
		return (int)Interlocked.Increment (ref this.value);
	}
	
	public int GetAndDecrement ()
	{
		return (int)Interlocked.Decrement (ref this.value);
	}
	
	//*AndGet
	public int AddAndGet (int delta)
	{
		return (int)Interlocked.Add (ref this.value, delta) + delta;
	}
	
	public int SetAndGet (int newValue)
	{
		Interlocked.Exchange (ref this.value, newValue);
		return newValue;
	}
	
	public int IncrementAndGet ()
	{
		return (int)Interlocked.Increment (ref this.value) + 1;
	}
	
	public int DecrementAndGet ()
	{
		return (int)Interlocked.Decrement (ref this.value) - 1;
	}
}

// public class AtomicDouble
// {
// 	private double value;
	
// 	public AtomicDouble (double value)
// 	{
// 		this.value = value;
// 	}
	
// 	public double Get ()
// 	{
// 		return (double)Interlocked.Read (ref this.value);
// 	}
	
// 	public void Set (double newValue)
// 	{
// 		Interlocked.Exchange (ref this.value, newValue);
// 	}

// 	public double Add (double delta)
// 	{
// 		return this.AddAndGet(delta);
// 	}
	
// 	public bool CompareAndSet (double newValue, double comprand)
// 	{
// 		return Interlocked.CompareExchange (ref this.value, newValue, comprand) == comprand;
// 	}
	
// 	//GetAnd*
// 	public double GetAndAdd (double delta)
// 	{
// 		return (double)Interlocked.Add (ref this.value, delta);
// 	}
	
// 	public double GetAndSet (double newValue)
// 	{
// 		return (double)Interlocked.Exchange (ref this.value, newValue);
// 	}
	
// 	public double GetAndIncrement ()
// 	{
// 		return (double)Interlocked.Increment (ref this.value);
// 	}
	
// 	public double GetAndDecrement ()
// 	{
// 		return (double)Interlocked.Decrement (ref this.value);
// 	}
	
// 	//*AndGet
// 	public double AddAndGet (double delta)
// 	{
// 		return (double)Interlocked.Add (ref this.value, delta) + delta;
// 	}
	
// 	public int SetAndGet (double newValue)
// 	{
// 		Interlocked.Exchange (ref this.value, newValue);
// 		return newValue;
// 	}
	
// 	public double IncrementAndGet ()
// 	{
// 		return (double)Interlocked.Increment (ref this.value) + 1;
// 	}
	
// 	public double DecrementAndGet ()
// 	{
// 		return (double)Interlocked.Decrement (ref this.value) - 1;
// 	}
// }

public class AtomicBoolean
{
	private long value;
	
	public AtomicBoolean (bool value)
	{
		this.value = value ? 1 : 0;
	}
	
	public bool Get ()
	{
		return Interlocked.Read (ref this.value) == 1;
	}
	
	public void Set (bool newValue)
	{
		Interlocked.Exchange (ref this.value, newValue ? 1 : 0);
	}
	
	public bool CompareAndSet (bool newValue, bool comprand)
	{
		long new_value = newValue ? 1 : 0;
		long comprand_value = comprand ? 1 : 0;
		return Interlocked.CompareExchange (ref this.value, new_value, comprand_value) == comprand_value;
	}
	
	//GetAnd*
	public bool GetAndSet (bool newValue)
	{
		return Interlocked.Exchange (ref this.value, newValue ? 1 : 0) == 1;
	}
	
	//*AndGet
	public bool SetAndGet (bool newValue)
	{
		Interlocked.Exchange (ref this.value, newValue ? 1 : 0);
		return newValue;
	}
}
/*
public class Atomic<TYPE>
{
	private TYPE value;
	
	public TYPE Get ()
	{
		return Interlocked.Read (ref this.value);
	}
	
	public void Set (TYPE newValue)
	{
		Interlocked.Exchange (ref this.value, newValue);
	}
	
	public TYPE CompareAndSet (TYPE newValue, TYPE comprand)
	{
		return Interlocked.CompareExchange (ref this.value, newValue, comprand) == comprand;
	}
	
	//GetAnd*
	public TYPE GetAndSet (TYPE newValue)
	{
		return Interlocked.Exchange (ref this.value, newValue);
	}
	
	//*AndGet
	public TYPE SetAndGet (TYPE newValue)
	{
		Interlocked.Exchange (ref this.value, newValue);
		return newValue;
	}
}
*/

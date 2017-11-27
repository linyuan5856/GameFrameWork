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
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class BlockChannel
{
	private readonly Queue queue = new Queue ();
		private readonly long max_size;
	AtomicBoolean closing = new AtomicBoolean (false);
//	bool blocking = false;

	public BlockChannel (long maxSize = long.MaxValue)
	{
//		this.blocking = true;
		this.max_size = maxSize;
	}

	private void wait ()
	{
		if (this.closing.Get () == false) {
			Monitor.Wait (this.queue);
		}
	}

	private void pulseAll ()
	{
		Monitor.PulseAll (this.queue);
	}

	public void Clear ()
	{
		lock (this.queue) {
			this.queue.Clear ();
		}
	}
	
	public int Count { 
		get { 
			lock (this.queue) {
				int count = this.queue.Count;
				return count; 
			}
		} 
	}
	
	public bool IsEmpry { get { return this.Count <= 0; } }
	
	public void Write (object item)
	{
		lock (this.queue) {
			while (queue.Count >= max_size) {
				Monitor.Wait (queue);
			}
			this.queue.Enqueue (item);
			if (this.queue.Count == 1) {
				// wake up any blocked dequeue
				this.pulseAll ();
			}
		}
	}
	
	public object Read ()
	{
		lock (queue) {
			while (this.queue.Count == 0) {
				this.wait ();
				if (this.closing.Get ()) {
					return null;
				}
			}
			object item = this.queue.Dequeue ();
			if (this.queue.Count == max_size - 1) {
				// wake up any blocked enqueue
				this.pulseAll();
			}
			return item;
		}
	}
	
	public T Read<T> ()
	{
		lock (queue) {
			while (this.queue.Count == 0) {
				this.wait ();
				if (this.closing.Get ()) {
					return default(T);
				}
			}
			T item = (T)this.queue.Dequeue ();
			if (this.queue.Count == this.max_size - 1) {
				// wake up any blocked enqueue
				this.pulseAll();
			}
			return item;
		}
	}

	public bool TryRead<T> (out T value)
	{
		lock (queue) {
			if (this.queue.Count > 0) {
				value = (T)queue.Dequeue ();
				if (queue.Count == max_size - 1) {
					// wake up any blocked enqueue
					this.pulseAll();
				}
				return true;
			}
			value = default(T);
			return false;
		}
	}

	public void Close ()
	{
		if(this.closing.GetAndSet(true))
			return;

		lock (this.queue) {
			this.pulseAll ();
		}
	}
}

public class Channel
{
	private Queue queue = Queue.Synchronized (new Queue ());

	public object Read ()
	{
		if (this.queue.Count > 0)
			return this.queue.Dequeue ();
		return null;
	}

	public T Read<T> ()
	{
		if (this.queue.Count > 0)
			return (T)this.queue.Dequeue ();
		return default(T);
	}

	public void Write (object value)
	{
		this.queue.Enqueue (value);
	}

	public void Clear ()
	{
		this.queue.Clear ();
	}

	public int Count { 
		get { 
			lock (queue) {
				return queue.Count; 
			}
		} 
	}

	public bool IsEmpry { get { return this.Count == 0; } }
	public bool NotEmpry { get { return this.Count > 0; } }
}


/*
public class SizeQueue
{
	private readonly Queue queue = new Queue ();
	private readonly int maxSize;
	bool closing;
    
	public SizeQueue (int maxSize = int.MaxValue)
	{
		this.maxSize = maxSize;
	}

	public void Clear ()
	{
		lock (queue) {
			this.queue.Clear ();
		}
	}
	
	public int Count { 
		get { 
			lock (queue) {
				int count = queue.Count;
				return count; 
			}
		} 
	}
	
	public bool IsEmpry { get { return this.Count <= 0; } }

	public void Write (object item)
	{
		lock (queue) {
			while (queue.Count >= maxSize) {
				Monitor.Wait (queue);
			}
			queue.Enqueue (item);
			if (queue.Count == 1) {
				// wake up any blocked dequeue
				Monitor.PulseAll (queue);
			}
		}
	}

	public T Read<T> ()
	{
		lock (queue) {
			while (queue.Count == 0) {
				Monitor.Wait (queue);
			}
			T item = (T)queue.Dequeue ();
			if (queue.Count == maxSize - 1) {
				// wake up any blocked enqueue
				Monitor.PulseAll (queue);
			}
			return (T)item;
		}
	}

	public void Close ()
	{
		lock (queue) {
			closing = true;
			Monitor.PulseAll (queue);
		}
	}

	public bool TryRead<T> (out T value)
	{
		lock (queue) {
			while (queue.Count == 0) {
				if (closing) {
					value = default(T);
					return false;
				}
				Monitor.Wait (queue);
			}
			value = (T)queue.Dequeue ();
			if (queue.Count == maxSize - 1) {
				// wake up any blocked enqueue
				Monitor.PulseAll (queue);
			}
			return true;
		}
	}
}
*/

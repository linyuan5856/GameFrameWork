using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SingletonGeneric<CLASS_NAME> where CLASS_NAME : new()
{
	private static CLASS_NAME instance__ = default(CLASS_NAME);
	private static object syncRoot__ = new object ();

	protected SingletonGeneric ()
	{
	}

	public static CLASS_NAME Instance {
		get {
			if (instance__ == null) {
				lock (syncRoot__) {
					if (instance__ == null) {
						instance__ = new CLASS_NAME ();
					}
				}
			}
			return instance__;
		}
	}
}

#region Map
public class MapManager<SELF, KEY, VALUE> : SingletonGeneric<SELF> where SELF : new()
{
	protected Dictionary<KEY,VALUE> map = new Dictionary<KEY,VALUE> ();
	
	protected MapManager ()
	{
	}
	public int Count {
		get {
			return this.map.Count;
		}
	}
	public int Clear ()
	{
		int num = this.map.Count;
		this.map.Clear ();
		return num;
	}
	
	public bool Has (KEY key)
	{
		return this.map.ContainsKey(key);
	}
	
	public void Add (KEY key, VALUE value)
	{
		this.Set(key, value);
	}

	public void Set (KEY key, VALUE value)
	{
		if(this.map.ContainsKey(key))
			this.map[key]=value;
		else
			this.map.Add (key,value);
	}

	public VALUE Get (KEY key)
	{
		if(!this.map.ContainsKey(key))
			return default(VALUE);
		return this.map[key];
	}
	
	public void Remove (KEY key)
	{
		this.map.Remove(key);
	}
	
	public void DoIteration (System.Action<KEY,VALUE> callback)
	{
		if (callback != null) {
			var keyArr = this.map.Keys.ToArray();
			for (int i=0; i<keyArr.Length; i++) {
				var key = keyArr[i];
				callback (key, this.map[key]);
			}
		}
	}
}
#endregion


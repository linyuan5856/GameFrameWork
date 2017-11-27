using System.Collections.Generic;
using System.Linq;

namespace GFW
{
    public class KVSTable<KEY, VALUE> : SVO
    {
        protected Dictionary<KEY, VALUE> mMap = new Dictionary<KEY, VALUE>();

        public KVSTable()
        {
        }

        public void Add(KEY key, VALUE value)
        {
            if (this.mMap.ContainsKey(key))
            {
                this.mMap[key] = value;
                DispatchEvent(DataEvent.Event_ProChange, value);
            }
            else
            {
                this.mMap.Add(key, value);
                DispatchEvent(DataEvent.Event_AddChild, value);
            }
        }

        public void Remove(KEY key)
        {
            if (mMap.ContainsKey(key))
            {
                VALUE value = mMap[key];
                this.mMap.Remove(key);
                DispatchEvent(DataEvent.Event_RemoveChild, value);
            }
        }

        public int Count
        {
            get { return this.mMap.Count; }
        }

        public virtual int Clear()
        {
            int num = this.mMap.Count;
            this.mMap.Clear();
            return num;
        }

        public bool Has(KEY key)
        {
            return this.mMap.ContainsKey(key);
        }

        public VALUE Get(KEY key)
        {
            if (!this.mMap.ContainsKey(key))
                return default(VALUE);
            return this.mMap[key];
        }

        public void DoIteration(System.Action<KEY, VALUE> callback)
        {
            if (callback != null)
            {
                var keyArr = this.mMap.Keys.ToArray();
                for (int i = 0; i < keyArr.Length; i++)
                {
                    var key = keyArr[i];
                    callback(key, this.mMap[key]);
                }
            }
        }
    }
}
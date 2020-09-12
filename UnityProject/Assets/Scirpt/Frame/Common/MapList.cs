using System.Collections.Generic;

public class MapList<TKey, TValue>
{
    private readonly Dictionary<TKey, TValue> _map;
    private readonly List<TValue> _list;

    public MapList(int capacity)
    {
        _map = new Dictionary<TKey, TValue>(capacity);
        _list = new List<TValue>(capacity);
    }

    public MapList()
    {
        _map = new Dictionary<TKey, TValue>();
        _list = new List<TValue>();
    }

    public List<TValue> AsList()
    {
        return _list;
    }

    public Dictionary<TKey, TValue> AsDictionary()
    {
        return _map;
    }

    public TValue[] ToArray()
    {
        return _list.ToArray();
    }

    public TValue this[TKey indexKey]
    {
        set
        {
            if (_map.ContainsKey(indexKey))
            {
                TValue v = _map[indexKey];
                _map[indexKey] = value;
                _list.Remove(v);
                _list.Add(value);
            }
            else
            {
                _map.Add(indexKey, value);
                _list.Add(value);
            }
        }
        get
        {
            TValue value = default(TValue);
            _map.TryGetValue(indexKey, out value);
            return value;
        }
    }

    public bool Add(TKey key, TValue value)
    {
        if (_map.ContainsKey(key))
        {
            return false;
        }

        _map.Add(key, value);
        _list.Add(value);
        return true;
    }

    public bool Remove(TKey key)
    {
        if (_map.ContainsKey(key))
        {
            TValue v = _map[key];
            _list.Remove(v);
            return _map.Remove(key);
        }

        return false;
    }

    public bool RemoveAt(int index, TKey key)
    {
        _list.RemoveAt(index);
        return _map.Remove(key);
    }


    public void Clear()
    {
        _map.Clear();
        _list.Clear();
    }

    public int Count => _list.Count;

    public bool ContainsKey(TKey key)
    {
        return _map.ContainsKey(key);
    }
}
using System;

namespace Pandora
{
    public class UIDataProvider
    {
        public UIDataProvider()
        {

        }
        public UIDataProvider(object _SelfData)
        {
            this.SelfData = _SelfData;
        }


        protected object mSelfData;

        public object SelfData
        {
            get { return mSelfData; }
            set { mSelfData = value; }
        }
        protected BetterList<object> mList = new BetterList<object>();

        public void Clear() { mList.size = 0; }


        public virtual void Add(object item)
        {
            mList.Add(item);
        }

        public void Add(params object[] param)
        {
            for (int i = 0, len = param.Length; i < len; i++)
            {
                Add(param[i]);
            }
        }

        public virtual void AddAtIndex(int index, object item)
        {
            mList.Insert(index, item);
        }

        public void SetSize(int newSize)
        {
            mList.SetSize(newSize);
        }

        public object GetItemAt(int i)
        {
            if (i > -1 && i < mList.size)
            {
                return mList[i];
            }
            return null;
        }

        public T GetItemAt<T>(int i)
        {
            return (T)GetItemAt(i);
        }

        public virtual void SetItemAt(int i, object value)
        {
            if (i >= 0 && i < mList.size)
            {
                mList[i] = value;
            }
            else
            {
                GameLogger.LogError("set data out of index:" + i);
            }
        }

        public void RemoveAt(int index)
        {
            mList.RemoveAt(index);
        }

        public void Remove(object item)
        {
            mList.Remove(item);
        }

        public int IndexOf(object item)
        {
            return mList.IndexOf(item);
        }

        public int Size
        {
            get { return mList.size; }
        }
    }


    public enum HMDataProviderEnum
    {
        None,
        VisibleFalse,
        VisibleTrue,
        Disable,
        Enable,
    }
}

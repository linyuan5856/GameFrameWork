using System.Collections.Generic;

namespace Pandora
{
    public class MapStruct
    {
        public struct Map<T, S>
        {
            private T key;
            private S value;

            public Map(T t, S s)
            {
                this.key = t;
                this.value = s;
            }


            public T Key
            {
                get { return key; }

                set { key = value; }
            }

            public S Value
            {
                get { return value; }

                set { this.value = value;}
            }
        }

        public struct MapList<T>
        {
            private readonly List<T> key;
            private readonly List<T> value;

            public MapList(List<T> key, List<T> value)
            {
                this.key = key;
                this.value = value;
            }


            public List<T> Key
            {
                get { return key; }
            }

            public List<T> Value
            {
                get { return value; }
            }
        }
    }
}

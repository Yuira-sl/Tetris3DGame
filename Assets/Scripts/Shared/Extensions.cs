using System;
using System.Collections.Generic;

namespace Octamino
{
    public static class Extensions
    {
        // Pool
        
        public static void PushRange<TKey, TValue>(this Pool<TKey> pool, List<TValue> list) where TValue: IPoolItem<TKey>
        {
            if (list.Count <= 0)
            {
                return;
            }
            
            foreach (var view in list)
            {
                pool.Push(view);
            }
            list.Clear();
        }
        
        public static float GetScreenToWorldHeight
        {
            get
            {
                UnityEngine.Vector2 topRightCorner = new UnityEngine.Vector2(1, 1);
                UnityEngine.Vector2 edgeVector = UnityEngine.Camera.main.ViewportToWorldPoint(topRightCorner);
                var height = edgeVector.y * 2;
                return height;
            }
        }
        public static float GetScreenToWorldWidth
        {
            get
            {
                UnityEngine.Vector2 topRightCorner = new UnityEngine.Vector2(1, 1);
                UnityEngine.Vector2 edgeVector = UnityEngine.Camera.main.ViewportToWorldPoint(topRightCorner);
                var width = edgeVector.x * 2;
                return width;
            }
        }
        
        // Enum Ex
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");
            }

            var array = (T[])Enum.GetValues(src.GetType());
            var j = Array.IndexOf(array, src) + 1;
            return array.Length==j ? array[0] : array[j];            
        }
        
        public static T RandomValue<T>(this T src)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");
            }
            
            var values = Enum.GetValues(src.GetType());
            int random = UnityEngine.Random.Range(0, values.Length);
            return (T)values.GetValue(random);
        }
        
        // ICollection Ex
        public static T FindFirst<T>(this ICollection<T> collection, Func<T, bool> condition)
        {
            foreach (var element in collection)
            {
                if (condition(element)) return element;
            }

            return default;
        }

        public static T[] First<T>(this ICollection<T> collection, int count)
        {
            var result = new T[count];
            var index = 0;
            foreach (var element in collection)
            {
                if (index >= count) break;
                result[index++] = element;
            }

            return result;
        }

        public static void Iterate<T>(this ICollection<T> collection, Action<T, int> action)
        {
            var index = 0;
            foreach (var element in collection) action(element, index++);
        }

        public static U[] Map<T, U>(this ICollection<T> collection, Func<T, U> map)
        {
            var result = new U[collection.Count];
            collection.Iterate((element, index) => result[index] = map(element));
            return result;
        }

        public static T Min<T>(this ICollection<T> colleciton) where T : IComparable
        {
            return colleciton.CompareAll((a, b) => a.CompareTo(b) < 0);
        }

        public static T Max<T>(this ICollection<T> colleciton) where T : IComparable
        {
            return colleciton.CompareAll((a, b) => a.CompareTo(b) > 0);
        }

        static T CompareAll<T>(this ICollection<T> colleciton, Func<T, T, bool> compare) where T : IComparable
        {
            T result = default(T);
            var hasValue = false;
            foreach (var element in colleciton)
            {
                if (!hasValue || compare(element, result))
                {
                    result = element;
                    hasValue = true;
                }
            }

            return result;
        }
        
        // List Ex
        public static void Shuffle<T>(this IList<T> list, Random rnd)
        {
            for (var i = 0; i < list.Count - 1; i++)
            {
                list.Swap(i, rnd.Next(i, list.Count));
            }
        }

        public static List<T> SortByDescending<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    var temp = list[i];
                    list[i] = list[j];
                    list[j] = temp;
                }
            }

            return list;
        }
        
        public static void RemoveFrom<T>(this List<T> lst, int from)
        {
            lst.RemoveRange(from, lst.Count - from);
        }
        
        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static T TakeFirst<T>(this IList<T> list)
        {
            var value = list[0];
            list.RemoveAt(0);
            return value;
        }

        public static void Add<T>(this IList<T> list, T value, int numDuplicates)
        {
            for (int n = 0; n < numDuplicates; n++)
            {
                list.Add(value);
            }
        }

        // Bool Ex
        public static int IntValue(this bool value) => value ? 1 : 0;

        // Int Ex

        public static bool BoolValue(this int value) => value == 1;
        
    }
}
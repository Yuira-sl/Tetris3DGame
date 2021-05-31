﻿using System;
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

        public static T CompareAll<T>(this ICollection<T> colleciton, Func<T, T, bool> compare) where T : IComparable
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

        // Bool Ex
        public static int IntValue(this bool value) => value ? 1 : 0;

        // Int Ex
        public static bool BoolValue(this int value) => value == 1;
    }
}
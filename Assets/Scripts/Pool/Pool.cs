using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Octamino
{
    public class Pool<T>
    { 
        private readonly Dictionary<T, Stack<IPoolItem<T>>> _items = new Dictionary<T, Stack<IPoolItem<T>>>();
        private readonly Dictionary<T, IPoolItem<T>> _originals = new Dictionary<T, IPoolItem<T>>();
        private readonly Transform _root;
        
        public Pool(T key, IPoolItem<T> original, int count, Transform parent)
        {
            _root = parent;
            Populate(key, original, count);
        }

        public bool Contains(T key)
        {
            return _originals.ContainsKey(key);
        }

        public void Populate(T key, IPoolItem<T> original, int count)
        {
            if (!_originals.ContainsKey(key))
            {
                _originals.Add(key, original);
            }

            if (!_items.ContainsKey(key))
            {
                _items.Add(key, new Stack<IPoolItem<T>>());
            }

            for (var j = 0; j < count; ++j)
            {
                var newItem = original.Replicate();
                newItem.GameObject.transform.parent = _root;
                newItem.GameObject.SetActive(false);
                newItem.PoolOwner = this;
                newItem.Key = key;

                _items[key].Push(newItem);
            }
        }

        public T1 Pop<T1>(T key) where T1 : IPoolItem<T>
        {
            if (!_items.ContainsKey(key))
            {
                Debug.LogError("There no " + key + " type inside pool!");
            }

            if (_items[key].Count == 0)
            {
                Populate(key, _originals[key], 1);
            }
            
            var item = (T1)_items[key].Pop();
            item.GameObject.SetActive(true);
            return item;
        }

        public void Push(IPoolItem<T> item)
        {
            if (_root == null)
            {
                Object.Destroy(item.GameObject);
            }
            else
            {
                item.GameObject.transform.parent = _root;
                item.ReturnToPool();
                _items[item.Key].Push(item);
            }
        }
    }
}
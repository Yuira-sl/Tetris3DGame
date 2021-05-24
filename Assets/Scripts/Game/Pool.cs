using System.Collections.Generic;
using UnityEngine;

namespace Octamino
{
    public class Pool<T> where T : Component
    {
        public T[] Items { get; }

        public Pool(GameObject prefab, int size, GameObject parent)
        {
            Items = new T[size];
            for (int i = 0; i < size; ++i)
            {
                var newItem = Object.Instantiate(prefab, parent.transform, true);
                var component = newItem.GetComponent<T>();
                if (!component)
                {
                    newItem.AddComponent<T>();
                }
                Items[i] = newItem.GetComponent<T>();
            }
        }

        public List<T> GetActiveItems()
        {
            List<T> result = new List<T>();   
            for (int i = 0; i < Items.Length; ++i)
            {
                if (Items[i].gameObject.activeInHierarchy)
                {
                    result.Add(Items[i]);
                }
            }
            
            return result;
        }
        
        public T GetAndActivate()
        {
            var result = Items.FindFirst(item => !item.gameObject.activeInHierarchy);
            result.gameObject.SetActive(true);
            return result;
        }
        
        public T GetAndActivate(Vector3 position)
        {
            var result = Items.FindFirst(item => !item.gameObject.activeInHierarchy);
            result.transform.position = position;
            result.gameObject.SetActive(true);
            return result;
        }

        public void DeactivateAll()
        {
            foreach (var item in Items) item.gameObject.SetActive(false);
        }
    }
}
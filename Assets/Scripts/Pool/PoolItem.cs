using UnityEngine;

namespace Octamino
{
    public class PoolItem: MonoBehaviour, IPoolItem<PoolItem>
    {
        public Pool<PoolItem> PoolOwner { get; set; }
        public PoolItem Key { get; set; }
        public GameObject GameObject => gameObject;
        public void ReturnToPool()
        {
            gameObject.SetActive(false);
        }

        public IPoolItem<PoolItem> Replicate()
        {
            return Instantiate(gameObject).GetComponent<PoolItem>();
        }
    }
}
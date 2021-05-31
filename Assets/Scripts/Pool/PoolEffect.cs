using UnityEngine;

namespace Octamino
{
    public class PoolEffect: MonoBehaviour, IPoolItem<PoolEffect>
    {
        public Pool<PoolEffect> PoolOwner { get; set; }
        public PoolEffect Key { get; set; }
        public GameObject GameObject => gameObject;
        public void ReturnToPool()
        {
            gameObject.SetActive(false);
        }

        public IPoolItem<PoolEffect> Replicate()
        {
            return Instantiate(gameObject).GetComponent<PoolEffect>();
        }
    }
}
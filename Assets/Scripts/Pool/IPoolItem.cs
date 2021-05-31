using UnityEngine;

namespace Octamino
{
    public interface IPoolItem<T>
    {
        Pool<T> PoolOwner { get; set; }
        T Key { get; set; }
        GameObject GameObject { get; }

        void ReturnToPool();
        IPoolItem<T> Replicate();
    }
}
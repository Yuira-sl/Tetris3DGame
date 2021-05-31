using UnityEngine;

namespace Octamino
{
    public class BlockView : MonoBehaviour, IPoolItem<BlockView>
    {
        public Renderer Renderer { get; set; }
        
        private void Awake()
        {
            Renderer = GetComponent<Renderer>();
        }
        
        public void SetMaterial(Material material)
        { 
            Renderer.material = material;
        }
        
        public void SetPosition(Vector3 position)
        {
            transform.localPosition = position;
        }

        public Pool<BlockView> PoolOwner { get; set; }
        public BlockView Key { get; set; }
        public GameObject GameObject => gameObject;

        public void ReturnToPool()
        {
            gameObject.SetActive(false);
        }

        public IPoolItem<BlockView> Replicate()
        {
            return Instantiate(gameObject).GetComponent<BlockView>();
        }
    }
}
using UnityEngine;

namespace Octamino
{
    public class BlockView : MonoBehaviour
    {
        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }
        
        public void SetMaterial(Material material)
        { 
            _renderer.material = material;
        }
        
        public void SetPosition(Vector3 position)
        {
            transform.localPosition = position;
        }
    }
}
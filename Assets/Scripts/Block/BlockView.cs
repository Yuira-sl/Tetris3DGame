using UnityEngine;

namespace Octamino
{
    public class BlockView : MonoBehaviour
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
    }
}
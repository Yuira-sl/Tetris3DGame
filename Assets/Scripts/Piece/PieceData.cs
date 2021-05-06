using System.Collections.Generic;
using UnityEngine;

namespace Octamino
{
    [CreateAssetMenu()]
    public class PieceData : ScriptableObject
    {
        public GameObject Block;
        public List<Material> PieceMaterials = new List<Material>();
        public Material GhostPieceMaterial;
    }
}
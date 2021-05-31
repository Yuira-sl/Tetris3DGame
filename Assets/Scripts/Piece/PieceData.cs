using System.Collections.Generic;
using UnityEngine;

namespace Octamino
{
    [CreateAssetMenu()]
    public class PieceData : ScriptableObject
    {
        public BlockView BlockView;
        public List<Material> PieceMaterials = new List<Material>();
        public Material GhostPieceMaterial;
    }
}
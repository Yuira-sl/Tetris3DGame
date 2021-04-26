using UnityEngine;

public class TextureSetter : MonoBehaviour
{
    private Material _material;
    
    private void Awake()
    {
        _material = GetComponent<Renderer>().sharedMaterial;
    }

    public void SetTexture(Texture2D texture)
    {
        _material.mainTexture = texture;
    }
}
using UnityEngine;

public class Block : MonoBehaviour
{
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    
    private Renderer _renderer;
    private Color _color = Color.black;
    private Material _material;
    private bool _paused;
    private bool _clearing;
    private float _clearClock;
    private float _clearTime;
    
    public Renderer Renderer
    {
        get => _renderer;
        set => _renderer = value;
    }
    
    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.enabled = false;
    }

    private void Update()
    {
        if (_clearing && !_paused)
        {
            _clearClock += Time.deltaTime;
            if (_clearClock < _clearTime)
            {
                //при очистке уровня
                //_renderer.sharedMaterial.SetColor(EmissionColorId, Color.Lerp(Color.black, Color.white, (_clearClock / _clearTime)));
            }
            else
            {
                _clearClock = 0f;
                _clearing = false;
            }
        }
    }
    
    public void SetMaterial(Material input)
    {
        _renderer.material = input;
        _material = input;
    }

    public Material GetMaterial()
    {
        return _material;
    }
    
    public void Clear(float time)
    {
        _clearing = true;
        _clearTime = time;
    }

    public void Pause()
    {
        _paused = true;
    }

    public void Resume()
    {
        _paused = false;
    }
}

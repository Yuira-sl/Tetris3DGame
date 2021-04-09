using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private static readonly int Color = Shader.PropertyToID("_Color");

    private Renderer _renderer;
    private float _gradientPosition;
    
    public float MoveSpeed = 1f;
    public float ColorScale = 1f; 
    public Gradient GradientColor;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _gradientPosition = 0f;
    }

    private void Update()
    {
        DoMove(GetMove() * MoveSpeed * Time.deltaTime);
        UpdateColor(transform.position);
    }

    private Vector3 GetMove()
    {
        Vector3 move; 
        if (Input.anyKey)
        {
            move = new Vector3(Input.GetAxisRaw("Axis_X"), Input.GetAxisRaw("Axis_Z"), Input.GetAxisRaw("Axis_Y"));
            move = Vector3.ClampMagnitude(move, 1f);
            return move;
        }
        move = Vector3.zero;
        return move;
    }

    private void DoMove(Vector3 move)
    {
        transform.Translate(move);
    }

    private void UpdateColor(Vector3 inputColor)
    {
        _gradientPosition = Mathf.Pow(Mathf.Sin((inputColor.x + inputColor.y + inputColor.z) * ColorScale),2);
        _renderer.material.SetColor(Color,GradientColor.Evaluate(_gradientPosition));
    }
}

using UnityEngine;

public class Piece
{
    private Vector2 _position;

    public Piece(Vector2 position)
    {
        _position = position;
    }
    public float GetX()
    {
        return _position.x;
    }
    public float GetY()
    {
        return _position.y;
    }
    public void SetX(float x)
    {
        _position.x = x;
    }
    public void SetY(float y)
    {
        _position.y = y;
    }
    
    public void SetPosition(Vector2 position)
    {
        _position = position;
    }
    
    public Vector2 GetPosition()
    {
        return _position;
    }
}

using UnityEngine;

public class Piece
{
    private Vector3 _position;
  
    public Piece(Vector3 position)
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
    public float GetZ()
    {
        return _position.z;
    }
    public void SetX(float x)
    {
        _position.x = x;
    }
    public void SetY(float y)
    {
        _position.y = y;
    }
    public void SetZ(float z)
    {
        _position.z = z;
    }
    
    public void SetPosition(Vector3 position)
    {
        _position = position;
    }
}

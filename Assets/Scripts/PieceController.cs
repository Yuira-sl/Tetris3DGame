using UnityEngine;

public class PieceController
{
    private readonly Piece[] _pieces;
    private Vector3Int _position;
    
    public PieceController()
    {
        _pieces = PieceCreator.PickRandomPiece();
    }

    private PieceController(PieceController input)
    {
        _position.x = input.GetPositionX();
        _position.y = input.GetPositionY();
        _position.z = input.GetPositionZ();

        _pieces = input.GetPieces();
    }
    
    public PieceController Clone()
    {
        return new PieceController(this);
    }
    
    public void SetPosition(int x, int y, int z)
    {
        _position.x = x;
        _position.y = y;
        _position.z = z;
    }
    
    public int GetPositionX()
    {
        return _position.x;
    }
    
    public int GetPositionY()
    {
        return _position.y;
    }

    private int GetPositionZ()
    {
        return _position.z;
    }

    public Piece[] GetPieces()
    {
        Piece[] piecesBuffer = new Piece[_pieces.Length];
        for (var i = 0; i < _pieces.Length; i++)
        {
            piecesBuffer[i] = new Piece(new Vector2(_pieces[i].GetX(), _pieces[i].GetY()));
        }
        return piecesBuffer;
    }
    

    public void RotateXY()
    {
        for (var i = 0; i < _pieces.Length; i++)
        {
            var tempX = _pieces[i].GetX();
            var tempY = _pieces[i].GetY();
            _pieces[i].SetX(tempY);
            _pieces[i].SetY(tempX * -1);
        }
    }
    
    public void FlipVertically()
    {
        for (var i = 0; i < _pieces.Length; i++)
        {
            var tempX = _pieces[i].GetX();
            _pieces[i].SetX(tempX * -1);
        }
    }
    
    public void MoveX(int input)
    {
        _position.x += input;
    }

    public void MoveY(int input)
    {
        _position.y += input;
    }
}

using UnityEngine;

public class PieceController
{
    private readonly Piece[] _randomPieces;
    private Vector3Int _position;
    
    public PieceController()
    {
        _randomPieces = PieceCreator.PickRandomPiece();
    }

    private PieceController(PieceController input)
    {
        _position.x = input.GetPositionX();
        _position.y = input.GetPositionY();
        _position.z = input.GetPositionZ();

        _randomPieces = input.GetPieces();
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

    public int GetPositionZ()
    {
        return _position.z;
    }

    public Piece[] GetPieces()
    {
        Piece[] piecesBuffer = new Piece[_randomPieces.Length];
        for (var i = 0; i < _randomPieces.Length; i++)
        {
            piecesBuffer[i] = new Piece(new Vector3(_randomPieces[i].GetX(), _randomPieces[i].GetY(), _randomPieces[i].GetZ()));
        }
        return piecesBuffer;
    }
    

    public void RotateXY()
    {
        for (var i = 0; i < _randomPieces.Length; i++)
        {
            var tempX = _randomPieces[i].GetX();
            var tempY = _randomPieces[i].GetY();
            _randomPieces[i].SetX(tempY);
            _randomPieces[i].SetY(tempX * -1);
        }
    }
    
    public void FlipVertically()
    {
        for (var i = 0; i < _randomPieces.Length; i++)
        {
            var tempX = _randomPieces[i].GetX();
            _randomPieces[i].SetX(tempX * -1);
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

    public void MoveYToValue(int input)
    {
        for (var i = 0; i < _randomPieces.Length; i++)
        {
            var tempX = _randomPieces[i].GetY();
            _randomPieces[i].SetY(tempX * -1);
        }
    }
}

using UnityEngine;

public class PieceController
{
    private readonly Piece[] _pieces;
    private Vector3Int _position;

    public PieceController()
    {
        _pieces = PieceCreator.PickPiece();
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

    public int GetPositionZ()
    {
        return _position.z;
    }

    public Piece[] GetPieces()
    {
        Piece[] piecesBuffer = new Piece[_pieces.Length];
        for (var i = 0; i < _pieces.Length; i++)
        {
            piecesBuffer[i] = new Piece(new Vector3(_pieces[i].GetX(), _pieces[i].GetY(), _pieces[i].GetZ()));
        }
        return piecesBuffer;
    }

    /// <summary>
    /// Rotates piece 90 degrees counter clockwise on the XZ plane if input is true, otherwise rotates clockwise.
    /// </summary>
    public void RotateXZ(bool сounterClockwise)
    {
        if (сounterClockwise)
        {
            for (int i = 0; i < _pieces.Length; i++)
            {
                var tempX = _pieces[i].GetX();
                var tempZ = _pieces[i].GetZ();
                _pieces[i].SetX(tempZ);
                _pieces[i].SetZ(tempX * -1);
            }
        }
        else
        {
            for (var i = 0; i < _pieces.Length; i++)
            {
                var tempX = _pieces[i].GetX();
                var tempZ = _pieces[i].GetZ();
                _pieces[i].SetX(tempZ * -1);
                _pieces[i].SetZ(tempX);
            }
        }
    }

    /// <summary>
    /// Rotates piece 90 degrees counter clockwise on the XY plane if input is true, otherwise rotates clockwise.
    /// </summary>
    public void RotateXY(bool сounterClockwise)
    {
        if (сounterClockwise)
        {
            for (var i = 0; i < _pieces.Length; i++)
            {
                var tempX = _pieces[i].GetX();
                var tempY = _pieces[i].GetY();
                _pieces[i].SetX(tempY * -1);
                _pieces[i].SetY(tempX);
            }
        }
        else
        {
            for (var i = 0; i < _pieces.Length; i++)
            {
                var tempX = _pieces[i].GetX();
                var tempY = _pieces[i].GetY();
                _pieces[i].SetX(tempY);
                _pieces[i].SetY(tempX * -1);
            }
        }
    }

    /// <summary>
    /// Rotates piece 90 degrees counter clockwise on the YZ plane if input is true, otherwise rotates clockwise.
    /// </summary>
    public void RotateYZ(bool сounterClockwise)
    {
        if (сounterClockwise)
        {
            for (var i = 0; i < _pieces.Length; i++)
            {
                var tempY = _pieces[i].GetY();
                var tempZ = _pieces[i].GetZ();
                _pieces[i].SetY(tempZ * -1);
                _pieces[i].SetZ(tempY);
            }
        }
        else
        {
            for (var i = 0; i < _pieces.Length; i++)
            {
                var tempY = _pieces[i].GetY();
                var tempZ = _pieces[i].GetZ();
                _pieces[i].SetY(tempZ);
                _pieces[i].SetZ(tempY * -1);
            }
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

    public void MoveZ(int input)
    {
        _position.z += input;
    }
}

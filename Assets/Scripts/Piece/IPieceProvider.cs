namespace Octamino
{
    public interface IPieceProvider
    {
        Piece GetPiece();
        Piece GetNextPiece();
    }
}

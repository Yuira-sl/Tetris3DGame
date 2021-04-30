namespace Octamino
{
    public class Block
    {
        public PieceType Type { get; }
        public Position Position { get; private set; }
        
        public Block(Position position, PieceType type)
        {
            Position = position;
            Type = type;
        }
        
        public void MoveTo(int row, int column)
        {
            MoveTo(new Position(row, column));
        }
        
        public void MoveTo(Position position)
        {
            Position = position;
        }
        
        public void MoveByOffset(int rowOffset, int columnOffset)
        {
            MoveTo(Position.Row + rowOffset, Position.Column + columnOffset);
        }
    }
}

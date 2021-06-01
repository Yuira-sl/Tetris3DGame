using System.Collections.Generic;
using System;

namespace Octamino
{
    public class Piece
    {
        public Block[] Blocks;
        public bool CanRotate { get; }
        public PieceType Type { get; }
        
        // Returns number of columns occupied by this piece
        public int Width
        {
            get
            {
                var min = Blocks.Map(block => block.Position.Column).Min();
                var max = Blocks.Map(block => block.Position.Column).Max();
                return Math.Abs(max - min);
            }
        }
        
        // Returns the topmost row in which a block of the piece is positioned
        public int Top => Blocks.Map(block => block.Position.Row).Max();
        
        public Piece(Position[] blockPositions, PieceType type, bool canRotate = true)
        {
            Blocks = blockPositions.Map(position => new Block(position, type));
            Type = type;
            CanRotate = canRotate;
        }
        
        // Returns a mapping of blocks to the positions of blocks
        public Dictionary<Block, Position> GetPositions()
        {
            var positions = new Dictionary<Block, Position>();
            foreach (Block block in Blocks)
            {
                positions[block] = block.Position;
            }
            return positions;
        }
    }
}

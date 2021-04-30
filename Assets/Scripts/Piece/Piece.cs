using System.Collections.Generic;
using System;

namespace Octamino
{
    public class Piece
    {
        private readonly bool _canRotate;
        
        public Block[] blocks;
        public bool CanRotate => _canRotate;
        
        public PieceType Type { get; }
        
        // Returns number of columns occupied by this piece
        public int Width
        {
            get
            {
                var min = blocks.Map(block => block.Position.Column).Min();
                var max = blocks.Map(block => block.Position.Column).Max();
                return Math.Abs(max - min);
            }
        }
        
        // Returns the topmost row in which a block of the piece is positioned
        public int Top => blocks.Map(block => block.Position.Row).Max();
        
        public Piece(Position[] blockPositions, PieceType type, bool canRotate = true)
        {
            blocks = blockPositions.Map(position => new Block(position, type));
            Type = type;
            _canRotate = canRotate;
        }
        
        // Returns a mapping of blocks to the positions of blocks
        public Dictionary<Block, Position> GetPositions()
        {
            var positions = new Dictionary<Block, Position>();
            foreach (Block block in blocks)
            {
                positions[block] = block.Position;
            }
            return positions;
        }
    }
}

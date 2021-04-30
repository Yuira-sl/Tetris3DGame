using System.Collections.Generic;

namespace Octamino
{
    public class Board
    {
        private readonly IPieceProvider _pieceProvider;
        private readonly int _top;
        private Piece Piece { get; set; }
        
        public readonly int Width;
        public readonly int Height;
        
        public Piece NextPiece => _pieceProvider.GetNextPiece();
        public List<Block> Blocks { get; } = new List<Block>();

        public Board(int width, int height) : this(width, height, new BalancedRandomPieceProvider())
        {
        }
        
        public Board(int width, int height, IPieceProvider pieceProvider)
        {
            Width = width;
            Height = height;
            _pieceProvider = pieceProvider;
            _top = height - 1;
        }
        
        // Determines whether blocks on the board collide with board bounds or with themselves.
        public bool HasCollisions()
        {
            return HasBoardCollisions() || HasBlockCollisions();
        }
        
        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var block in Blocks)
            {
                var row = block.Position.Row;
                var column = block.Position.Column;
                var offset = Width * Height * (int)block.Type;
                var blockHash = offset + row * Width + column;
                hash += blockHash;
            }
            return hash;
        }

        /// <summary>
        /// Adds new piece.
        /// </summary>
        public void AddPiece()
        {
            Piece = _pieceProvider.GetPiece();

            var offsetRow = _top - Piece.Top;
            var offsetCol = (Width - Piece.Width) / 2;

            foreach (var block in Piece.blocks)
            {
                block.MoveByOffset(offsetRow, offsetCol);
            }

            Blocks.AddRange(Piece.blocks);
        }
        
        // Returns position of the ghost piece which is the final piece position if it starts
        public Position[] GetGhostPiecePositions()
        {
            var positions = Piece.GetPositions();
            FallPiece();
            var shadowPositions = Piece.GetPositions().Values.Map(p => p);
            RestoreSavedPiecePosition(positions);
            return shadowPositions;
        }
        
        public bool MovePieceLeft() => MovePiece(0, -1);
        public bool MovePieceRight() => MovePiece(0, 1);
        public bool MovePieceDown() => MovePiece(-1, 0);
        
        // Rotates the current piece clockwise
        public bool RotatePiece()
        {
            if (!Piece.CanRotate)
            {
                return false;
            }

            var piecePosition = Piece.GetPositions();
            var offset = Piece.blocks[0].Position;

            foreach (var block in Piece.blocks)
            {
                var row = block.Position.Row - offset.Row;
                var column = block.Position.Column - offset.Column;
                block.MoveTo(-column + offset.Row, row + offset.Column);
            }

            if (HasCollisions() && !ResolveCollisionsAfterRotation())
            {
                RestoreSavedPiecePosition(piecePosition);
                return false;
            }
            return true;
        }

        public int FallPiece()
        {
            var rowsCount = 0;
            while (MovePieceDown())
            {
                rowsCount++;
            }
            return rowsCount;
        }
        
        public int RemoveFullRows()
        {
            var rowsRemoved = 0;
            for (int row = Height - 1; row >= 0; --row)
            {
                var rowBlocks = GetBlocksFromRow(row);
                if (rowBlocks.Count == Width)
                {
                    Remove(rowBlocks);
                    MoveDownBlocksBelowRow(row);
                    rowsRemoved += 1;
                }
            }
            return rowsRemoved;
        }
        
        public void RemoveAllBlocks()
        {
            Blocks.Clear();
        }
        
        private bool HasBlockCollisions()
        {
            var allPositions = Blocks.Map(block => block.Position);
            var uniquePositions = new HashSet<Position>(allPositions);
            return allPositions.Length != uniquePositions.Count;
        }

        private bool HasBoardCollisions()
        {
            return Blocks.Find(CollidesWithBoard) != null;
        }

        private bool CollidesWithBoard(Block block)
        {
            return block.Position.Row < 0 ||
                   block.Position.Row >= Height ||
                   block.Position.Column < 0 ||
                   block.Position.Column >= Width;
        }
        
        private bool MovePiece(int rowOffset, int columnOffset)
        {
            foreach (var block in Piece.blocks)
            {
                block.MoveByOffset(rowOffset, columnOffset);
            }

            if (HasCollisions())
            {
                foreach (var block in Piece.blocks)
                {
                    block.MoveByOffset(-rowOffset, -columnOffset);
                }
                return false;
            }
            return true;
        }
        
        private bool ResolveCollisionsAfterRotation()
        {
            var columnOffsets = new[] { -1, -2, 1, 2 };
            
            foreach (var offset in columnOffsets)
            {
                MovePiece(0, offset);

                if (HasCollisions())
                {
                    MovePiece(0, -offset);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private void RestoreSavedPiecePosition(Dictionary<Block, Position> piecePosition)
        {
            foreach (var block in Piece.blocks)
            {
                block.MoveTo(piecePosition[block]);
            }
        }
        
        private List<Block> GetBlocksFromRow(int row)
        {
            return Blocks.FindAll(block => block.Position.Row == row);
        }

        private void Remove(List<Block> blocksToRemove)
        {
            Blocks.RemoveAll(blocksToRemove.Contains);
        }

        private void MoveDownBlocksBelowRow(int row)
        {
            foreach (var block in Blocks)
            {
                if (block.Position.Row > row)
                {
                    block.MoveByOffset(-1, 0);
                }
            }
        }
    }
}

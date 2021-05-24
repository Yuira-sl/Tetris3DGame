using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Octamino
{
    public class Board
    {
        private readonly IPieceProvider _pieceProvider;
        private readonly int _top;

        public event Action<int, float> OnBoardRowCleared;
        public event Action<List<Block>, float> OnBoardLastRowCleared;
        
        public readonly int Width;
        public readonly int Height;
        
        public Piece NextPiece => _pieceProvider.GetNextPiece();
        public List<Block> Blocks { get; } = new List<Block>();

        public Piece Piece { get; set; }

        public Board(int width, int height) : this(width, height, new RandomPieceProvider())
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

        // Adds new piece.
        public void AddPiece()
        {
            Piece = _pieceProvider.GetPiece();

            var offsetRow = _top - Piece.Top;
            var offsetCol = (Width - Piece.Width) / 2;

            foreach (var block in Piece.Blocks)
            {
                block.MoveByOffset(offsetRow, offsetCol);
            }

            Blocks.AddRange(Piece.Blocks);
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
        public bool RotatePiece(bool counterclockwise)
        {
            if (!Piece.CanRotate)
            {
                return false;
            }

            var piecePosition = Piece.GetPositions();
            var offset = Piece.Blocks[0].Position;

            foreach (var block in Piece.Blocks)
            {
                var row = block.Position.Row - offset.Row;
                var column = block.Position.Column - offset.Column;

                if (!counterclockwise)
                {
                    block.MoveTo(-column + offset.Row, row + offset.Column);
                }
                else
                {
                    block.MoveTo(column + offset.Row, -row + offset.Column);
                }
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
        
        public List<Block> GetBlocksFromRow(int row)
        {
            return Blocks.FindAll(block => block.Position.Row == row);
        }
        
        public void Remove(List<Block> blocksToRemove)
        {
            Blocks.RemoveAll(blocksToRemove.Contains);
        }

        public void MoveDownBlocksBelowRow(int row)
        {
            foreach (var block in Blocks)
            {
                if (block.Position.Row > row)
                {
                    block.MoveByOffset(-1, 0);
                }
            }
        }
        
        public IEnumerator RemoveFullRows(float time)
        {
            yield return new WaitForSeconds(time);

            var rowsRemoved = 0;
            for (int row = Height - 1; row >= 0; --row)
            {
                var rowBlocks = GetBlocksFromRow(row);

                if (rowBlocks.Count == Width)
                {
                    OnBoardRowCleared?.Invoke(row, time);
                    rowsRemoved += 1;
                }
            }
            
            Game.Instance.Score.RowsCleared(rowsRemoved);
            Game.Instance.Level.RowsCleared(rowsRemoved);

            yield return null;
        }
        
        public void RemoveLastRows(int rowsCount)
        {
            var hMax = Height - 1;
            var hCurrent = hMax - rowsCount;
            var blocksToRemove = Blocks.FindAll(block => block.Position.Row > hCurrent && block.Position.Row <= hMax);
            Remove(blocksToRemove);
        }
        
        // public IEnumerator RemoveLastRows(int rowsCount, float time)
        // {
        //     Game.Instance.Resume();
        //     var hMax = Height - 1;
        //     var hCurrent = hMax - rowsCount;
        //     var blocksToRemove = Blocks.FindAll(block => block.Position.Row > hCurrent && block.Position.Row <= hMax);
        //     yield return new WaitForSeconds(time);
        //     OnBoardLastRowCleared?.Invoke(blocksToRemove, time);
        // }
        
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
            foreach (var block in Piece.Blocks)
            {
                block.MoveByOffset(rowOffset, columnOffset);
            }

            if (HasCollisions())
            {
                foreach (var block in Piece.Blocks)
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
            foreach (var block in Piece.Blocks)
            {
                block.MoveTo(piecePosition[block]);
            }
        }
    }
}

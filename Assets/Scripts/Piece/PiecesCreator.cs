namespace Octamino
{
    public static class PiecesCreator
    {
        private static PieceType _pieceType;
        public static Piece GetPiece()
        {
            var pieceType = _pieceType.RandomValue();  
            
            switch (pieceType)
            {
                case PieceType.O: return OPiece();
                case PieceType.T: return TPiece();
                case PieceType.S: return SPiece();
                case PieceType.Z: return ZPiece();
                case PieceType.J: return JPiece();
                case PieceType.L: return LPiece();
                case PieceType.I: return IPiece();
            }

            return null;
        }
      
        public static Piece OPiece()
        {
            var positions = new[] {
                new Position(0, 0),
                new Position(1, 0),
                new Position(1, 1),
                new Position(0, 1)
            };
            return new Piece(positions, PieceType.O, false);
        }

        public static Piece TPiece()
        {
            var positions = new[] {
                new Position(0, 0),
                new Position(1, 0),
                new Position(-1, 0),
                new Position(0, 1)
            };
            return new Piece(positions, PieceType.T);
        }

        public static Piece SPiece()
        {
            var positions = new[] {
                new Position(0, 0),
                new Position(1, 0),
                new Position(0, 1),
                new Position(-1, 1)
            };
            return new Piece(positions, PieceType.S);
        }

        public static Piece ZPiece()
        {
            var positions = new[] {
                new Position(0, 0),
                new Position(-1, 0),
                new Position(0, 1),
                new Position(1, 1)
            };
            return new Piece(positions, PieceType.Z);
        }

        public static Piece JPiece()
        {
            var positions = new[] {
                new Position(0, 0),
                new Position(1, 0),
                new Position(-1, 0),
                new Position(-1, -1)
            };
            return new Piece(positions, PieceType.J);
        }

        public static Piece LPiece()
        {
            var positions = new[] {
                new Position(0, 0),
                new Position(1, 0),
                new Position(-1, 0),
                new Position(-1, 1)
            };
            return new Piece(positions, PieceType.L);
        }

        public static Piece IPiece()
        {
            var positions = new[] {
                new Position(0, 0),
                new Position(1, 0),
                new Position(-1, 0),
                new Position(-2, 0)
            };
            return new Piece(positions, PieceType.I);
        }
    }
}

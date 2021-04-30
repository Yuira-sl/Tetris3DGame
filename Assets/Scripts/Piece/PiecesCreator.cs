namespace Octamino
{
    public static class PiecesCreator
    {
        public static Piece[] All()
        {
            return new[] {
                OPiece(),
                TPiece(),
                SPiece(),
                ZPiece(),
                JPiece(),
                LPiece(),
                IPiece()
            };
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

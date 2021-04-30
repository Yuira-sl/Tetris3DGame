using System;
using System.Collections.Generic;

namespace Octamino
{
    public class BalancedRandomPieceProvider : IPieceProvider
    {
        private Random _random = new Random();
        private List<int> _pool = new List<int>();
        
        private const int NumDuplicates = 4;

        public Piece GetPiece() => PiecesCreator.All()[GetPopulatedPool().TakeFirst()];

        public Piece GetNextPiece() => PiecesCreator.All()[GetPopulatedPool()[0]];

        private List<int> GetPopulatedPool()
        {
            if (_pool.Count == 0)
            {
                PopulatePool();
            }
            return _pool;
        }

        private void PopulatePool()
        {
            for (var index = 0; index < PiecesCreator.All().Length; ++index)
            {
                _pool.Add(index, NumDuplicates);
            }
            _pool.Shuffle(_random);
        }
    }
}
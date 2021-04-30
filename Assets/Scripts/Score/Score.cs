using System.Collections.Generic;

namespace Octamino
{
    public class Score
    {
        private readonly Dictionary<int, int> _scoreForClearedRows = new Dictionary<int, int>
        {
            {1, 100}, 
            {2, 300}, 
            {3, 500}, 
            {4, 800}
        };
        
        public int Value { get; private set; }
        
        public void RowsCleared(int count)
        {
            _scoreForClearedRows.TryGetValue(count, out var valueIncrease);
            Value += valueIncrease;
        }

        public void PieceFinishedFalling(int rowsCount) => Value += rowsCount * 2;

        public void PieceMovedDown() => Value++;
    }
}
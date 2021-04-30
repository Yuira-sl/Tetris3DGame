using UnityEngine;

namespace Octamino
{
    public class PieceView : MonoBehaviour
    {
        private Board _board;
        private Pool<BlockView> _blockViewPool;
        private PieceType? _renderedPieceType;
        private int _blockPoolSize = 10;
        private bool _forceRender;

        // random blocks here
        public GameObject blockPrefab;
        public Sprite[] blockSprites;
        public RectTransform container;

        public void SetBoard(Board board)
        {
            _board = board;
            _blockViewPool = new Pool<BlockView>(blockPrefab, _blockPoolSize, gameObject);
        }

        private void Update()
        {
            if (_renderedPieceType == null || _forceRender || _board.NextPiece.Type != _renderedPieceType)
            {
                DrawPiece(_board.NextPiece);
                _renderedPieceType = _board.NextPiece.Type;
                _forceRender = false;
            }
        }

        private void OnRectTransformDimensionsChange()
        {
            _forceRender = true;
        }

        private void DrawPiece(Piece piece)
        {
            _blockViewPool.DeactivateAll();

            var blockSize = BlockSize(piece);

            foreach (var block in piece.blocks)
            {
                var blockView = _blockViewPool.GetAndActivate();
                blockView.SetSprite(BlockSprite(block.Type));
                blockView.SetSize(blockSize);
                blockView.SetPosition(BlockPosition(block.Position, blockSize));
            }

            var pieceBlocks = _blockViewPool.Items.First(piece.blocks.Length);
            var xValues = pieceBlocks.Map(b => b.transform.localPosition.x);
            var yValues = pieceBlocks.Map(b => b.transform.localPosition.y);

            var halfBlockSize = blockSize / 2.0f;
            var minX = Mathf.Min(xValues) - halfBlockSize;
            var maxX = Mathf.Max(xValues) + halfBlockSize;
            var minY = Mathf.Min(yValues) - halfBlockSize;
            var maxY = Mathf.Max(yValues) + halfBlockSize;
            var width = maxX - minX;
            var height = maxY - minY;
            var offsetX = (-width / 2.0f) - minX;
            var offsetY = (-height / 2.0f) - minY;

            foreach (var block in pieceBlocks)
            {
                block.transform.localPosition += new Vector3(offsetX, offsetY);
            }
        }

        private Vector3 BlockPosition(Position position, float blockSize)
        {
            return new Vector3(position.Column * blockSize, position.Row * blockSize);
        }

        private float BlockSize(Piece piece)
        {
            var width = container.rect.size.x;
            var height = container.rect.size.y;
            var numBlocks = piece.blocks.Length;
            return Mathf.Min(width / numBlocks, height / numBlocks);
        }

        private Sprite BlockSprite(PieceType type)
        {
            return blockSprites[(int) type];
        }
    }
}
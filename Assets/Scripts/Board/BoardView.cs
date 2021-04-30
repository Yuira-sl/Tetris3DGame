using UnityEngine;

namespace Octamino
{
    public class BoardView : MonoBehaviour
    {
        private enum Layer
        {
            Blocks,
            PieceShadow
        }

        private Board _gameBoard;
        private int _renderedBoardHash = -1;
        private bool _forceRender;
        private Pool<BlockView> _blockViewPool;
        private RectTransform _rectTransform;

        // multiple blocks with different meshes and materials
        public GameObject BlockPrefab;
        public Sprite[] BlockSprites;
        public Sprite GhostBlockSprite;
        public TouchInput TouchInput = new TouchInput();

        public void SetBoard(Board board)
        {
            _gameBoard = board;
            var size = board.Width * board.Height + 10;
            _blockViewPool = new Pool<BlockView>(BlockPrefab, size, gameObject);
        }

        private void RenderGameBoard()
        {
            _blockViewPool.DeactivateAll();
            RenderGhostPiece();
            DrawBlocks();
        }

        private void DrawBlocks()
        {
            foreach (var block in _gameBoard.Blocks) RenderBlock(BlockSprite(block.Type), block.Position, Layer.Blocks);
        }

        private void RenderGhostPiece()
        {
            foreach (var position in _gameBoard.GetGhostPiecePositions())
                RenderBlock(GhostBlockSprite, position, Layer.PieceShadow);
        }

        private void RenderBlock(Sprite sprite, Position position, Layer layer)
        {
            var view = _blockViewPool.GetAndActivate();
            view.SetSprite(sprite);
            view.SetSize(BlockSize());
            view.SetPosition(BlockPosition(position.Row, position.Column, layer));
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            TouchInput.BlockSize = BlockSize();

            var hash = _gameBoard.GetHashCode();
            if (_forceRender || hash != _renderedBoardHash)
            {
                RenderGameBoard();
                _renderedBoardHash = hash;
                _forceRender = false;
            }
        }

        private void OnRectTransformDimensionsChange()
        {
            _forceRender = true;
        }

        private Vector3 BlockPosition(int row, int column, Layer layer)
        {
            var size = BlockSize();
            var position = new Vector3(column * size, row * size, (float) layer);
            var offset = new Vector3(size / 2, size / 2, 0);
            return position + offset - PivotOffset();
        }

        private float BlockSize()
        {
            var boardWidth = _rectTransform.rect.size.x;
            return boardWidth / _gameBoard.Width;
        }

        private Sprite BlockSprite(PieceType type)
        {
            return BlockSprites[(int) type];
        }

        private Vector3 PivotOffset()
        {
            var pivot = _rectTransform.pivot;
            var boardSize = _rectTransform.rect.size;
            return new Vector3(boardSize.x * pivot.x, boardSize.y * pivot.y);
        }
    }
}
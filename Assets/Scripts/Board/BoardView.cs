using UnityEngine;

namespace Octamino
{
    public class BoardView : MonoBehaviour
    {
        private Board _gameBoard;
        private int _renderedBoardHash = -1;
        private bool _forceRender;
        private Pool<BlockView> _blockViewPool;
        private RectTransform _rectTransform;

        public PieceData Data;
        public GameObject BlocksContaiter;
        public TouchInput TouchInput = new TouchInput();

        public void SetBoard(Board board)
        {
            _gameBoard = board;
            var size = board.Width * board.Height + 10;
            _blockViewPool = new Pool<BlockView>(Data.Block, size, BlocksContaiter);
        }
        
        private void RenderGameBoard()
        {
            _blockViewPool.DeactivateAll();
            RenderGhostPiece();
            RenderBlocks();
        }

        private void RenderBlocks()
        {
            foreach (var block in _gameBoard.Blocks)
            {
                RenderBlock(BlockMaterial(block.Type), block.Position);
            }
        }

        private void RenderGhostPiece()
        {
            foreach (var position in _gameBoard.GetGhostPiecePositions())
            {
                RenderBlock(Data.GhostPieceMaterial, position);
            }
        }
                
        private void RenderBlock(Material material, Position position)
        {
            var view = _blockViewPool.GetAndActivate();
            view.SetMaterial(material);
            view.SetPosition(BlockPosition(position.Row, position.Column));
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
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

        private Vector3 BlockPosition(int row, int column)
        {
            return new Vector3(column, row, 0);
        }

        private float BlockSize()
        {
            var boardWidth = _rectTransform.rect.size.x;
            return boardWidth / _gameBoard.Width;
        }
       
        private Material BlockMaterial(PieceType type)
        {
            return Data.PieceMaterials[(int) type];
        }
    }
}
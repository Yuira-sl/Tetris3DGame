using UnityEngine;

namespace Octamino
{
    public class BoardView : MonoBehaviour
    {
        private Board _board;
        private int _renderedBoardHash = -1;
        private bool _forceRender;
        
        public Pool<BlockView> BlockViewPool { get; set; }

        
        public PieceData Data;
        public GameObject BlocksContaiter;
        public TouchInput TouchInput = new TouchInput();

        
        public void SetBoard(Board board)
        {
            _board = board;
            _board.Game.OnPieceSettled += OnBlockSettled;
            _board.Game.OnPieceAppeared += OnBlockAppeared;
            
            var size = board.Width * board.Height + 10;
            BlockViewPool = new Pool<BlockView>(Data.Block, size, BlocksContaiter);
        }

        private void OnBlockAppeared()
        {
            
        }
        
        private void OnBlockSettled()
        {

        }

        // public void StartProcessBlockViewRemove(float time)
        // {
        //     StartCoroutine(ProcessBlockViewRemove(time));
        // }
        //
        // public void StartRemoveFullRows(float time)
        // {
        //     StartCoroutine(_board.RemoveFullRows(time));
        // }
        //
        // private IEnumerator ProcessBlockViewRemove(float time)
        // {
        //     for (int row = _board.Height - 1; row >= 0; --row)
        //     {
        //         List<BlockView> views = new List<BlockView>();
        //         foreach (var block in BlockViewPool.GetActiveItems())
        //         {
        //             var blocks = _board.Blocks.FindAll(b => b.Position.Row == row);
        //             if (blocks.Count == _board.Width)
        //             {
        //                 views.Add(block);
        //             }
        //         }
        //         
        //         float elapsedTime = 0;
        //         while (elapsedTime < time)
        //         {
        //             elapsedTime += Time.deltaTime;
        //
        //             foreach (var view in views)
        //             {
        //                 view.Renderer.material.color 
        //                     = Color.Lerp(view.Renderer.material.color, 
        //                         Color.black, elapsedTime / time);
        //             }
        //
        //             yield return null;
        //         }
        //     }
        // }
        
        
        private void RenderGameBoard()
        {
            BlockViewPool.DeactivateAll();
            RenderGhostPiece();
            RenderPiece();
        }

        private void RenderPiece()
        {
            foreach (var block in _board.Blocks)
            {
                RenderBlock(BlockMaterial(block.Type), block.Position);
            }
        }
        
        private void RenderGhostPiece()
        {
            foreach (var position in _board.GetGhostPiecePositions())
            {
                RenderBlock(Data.GhostPieceMaterial, position);
            }
        }
        
        private BlockView RenderBlock(Material material, Position position)
        {
            var view = BlockViewPool.GetAndActivate();
            view.SetMaterial(material);
            view.SetPosition(BlockPosition(position.Row, position.Column));
            return view;
        }
        
        private void Update()
        {
            var hash = _board.GetHashCode();
            if (_forceRender || hash != _renderedBoardHash)
            {
                RenderGameBoard();
                _renderedBoardHash = hash;
                _forceRender = false;
            }
        }
        
        private Vector3 BlockPosition(int row, int column)
        {
            return new Vector3(column, row, 0);
        }
        
        private Material BlockMaterial(PieceType type)
        {
            return Data.PieceMaterials[(int) type];
        }

        private void OnDestroy()
        {
            _board.Game.OnPieceSettled -= OnBlockSettled;
            _board.Game.OnPieceAppeared -= OnBlockAppeared;
        }
    }
}
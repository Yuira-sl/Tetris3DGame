using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Octamino
{
    public class BoardView : MonoBehaviour
    {
        private static readonly int ColorId = Shader.PropertyToID("_BaseColor");

        private Board _board;
        private int _renderedBoardHash = -1;

        private readonly List<BlockView> _currentPiece = new List<BlockView>();
        private readonly List<BlockView> _currentGhostPiece = new List<BlockView>();
        private readonly List<PoolItem> _clearedRowEffects = new List<PoolItem>();
        private readonly List<BlockView> _settledBlocksInRow = new List<BlockView>();
        
        private Pool<BlockView> BlockViewPool { get; set; }
        private Pool<PoolItem> RowEffectPool { get; set; }
        
        public PieceData PieceData;
        public PoolItem RowEffect;
        public Transform BlocksContaiter;
        public Transform EffectsContaiter;
        public AudioPlayer AudioPlayer;

        public void Initialize(Board board)
        {
            _board = board;
            _board.OnRowCleared += OnRowCleared;
            _board.OnLastRowsCleared += OnLastRowsCleared;
            Game.Instance.OnPieceSettled += OnBlockSettled;

            var size = board.Width * board.Height + 10;
            
            BlockViewPool = new Pool<BlockView>(PieceData.BlockView, PieceData.BlockView, size, BlocksContaiter);
            RowEffectPool = new Pool<PoolItem>(RowEffect, RowEffect, board.Height, EffectsContaiter);
            RowEffectPool.PushRange(_clearedRowEffects);
        }

        private void OnRowCleared(List<Block> blocks, int row, float time)
        {
            StartCoroutine(ClearBlocks(blocks, time, true, row));
        }

        private void OnLastRowsCleared(List<Block> blocks, float time)
        { 
            StartCoroutine(ClearBlocks(blocks, time * 1.5f));
        }

        private void OnBlockSettled()
        {
            RowEffectPool.PushRange(_clearedRowEffects);
        }
        
        private IEnumerator ClearBlocks(List<Block> blocks, float time, bool isLastRows = false,  int row = 0)
        {
            yield return null;

            _settledBlocksInRow.AddRange(!isLastRows ? GetNecessaryBlocks(blocks) : GetNecessaryBlocks(row));

            yield return EnableGlowInRows(time);
            
            AudioPlayer.PlayCollectRowClip();

            if (!isLastRows)
            {
                Game.Instance.Resume();
                _board.AddPiece();
            }
            else
            {
                var effect = RowEffectPool.Pop<PoolItem>(RowEffect);
                effect.transform.parent.position = new Vector3(4.5f, row, 0);
                _clearedRowEffects.Add(effect);

                _board.MoveDownBlocksBelowRow(row);
            }
            _board.Remove(blocks);
        }
        
        private List<BlockView> GetNecessaryBlocks(int row)
        {
            return FillViews(new List<BlockView>(), row);
        }
        
        private List<BlockView> GetNecessaryBlocks(List<Block> blocks)
        {
            var views = new List<BlockView>();

            foreach (var block in blocks)
            {
                FillViews(views, block.Position.Row);
            }
            
            return views;
        }

        private List<BlockView> FillViews(List<BlockView> newList, int row)
        {
            foreach (var view in _currentPiece)
            {
                if (view.transform.position.y.Equals(row))
                {
                    newList.Add(view);
                }
            }

            return newList;
        }
        
        
        private IEnumerator EnableGlowInRows(float time)
        {
            if (_settledBlocksInRow.Count > 0)
            {
                float elapsedTime = 0;
                while (elapsedTime < time)
                {
                    elapsedTime += Time.deltaTime;
        
                    foreach (var view in _settledBlocksInRow)
                    {
                        SetColor(view, elapsedTime / time);
                    }
        
                    yield return null;
                }
                _settledBlocksInRow.Clear();
            }
        }
        
        private void SetColor(BlockView view, float time)
        {
            var mat = view.Renderer.material;
            var color = mat.GetColor(ColorId);
            mat.SetColor(ColorId, Color.Lerp(color, color * 1.25f, time * 1f));
        }
        
        private void RenderGameBoard()
        {
            BlockViewPool.PushRange(_currentPiece);
            BlockViewPool.PushRange(_currentGhostPiece);
            RenderGhostPiece();
            RenderPiece();
        }
        
        private void RenderPiece()
        {
            foreach (var block in _board.Blocks)
            {
                var blockView = RenderBlock(BlockMaterial(block.Type), block.Position);
                _currentPiece.Add(blockView);
            }
        }
        
        private void RenderGhostPiece()
        {
            foreach (var position in _board.GetGhostPiecePositions())
            {
                var blockView = RenderBlock(PieceData.GhostPieceMaterial, position);
                if (CheckIntersectsPositions(position))
                {
                    blockView.gameObject.SetActive(false);
                }
                
                _currentGhostPiece.Add(blockView);
            }
        }
        
        private bool CheckIntersectsPositions(Position position)
        {
            foreach (var block in _board.Blocks)
            {
                if (block.Position == position)
                {
                    return true;
                }
            }
            return false;
        }
        
        private BlockView RenderBlock(Material material, Position position)
        {
            var view = BlockViewPool.Pop<BlockView>(PieceData.BlockView);
            view.SetMaterial(material);
            view.SetPosition(BlockPosition(position.Row, position.Column));
            return view;
        }
        
        private void Update()
        {
            var hash = _board.GetHashCode();
            if (hash != _renderedBoardHash)
            {
                RenderGameBoard();
                _renderedBoardHash = hash;
            }
        }
        
        private Vector3 BlockPosition(int row, int column)
        {
            return new Vector3(column, row, 0);
        }
        
        private Material BlockMaterial(PieceType type)
        {
            return PieceData.PieceMaterials[(int) type];
        }
        
        private void OnDestroy()
        {
            Game.Instance.OnPieceSettled -= OnBlockSettled;
            _board.OnRowCleared -= OnRowCleared;
            _board.OnLastRowsCleared -= OnLastRowsCleared;
        }
    }
}
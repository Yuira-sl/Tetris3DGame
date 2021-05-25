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
        private BlockView _currentGhostBlock;
        
        private readonly List<ParticleSystem> _clearedRowEffects = new List<ParticleSystem>();
        private readonly List<BlockView> _settledBlocksInRow = new List<BlockView>();
        
        private Pool<BlockView> BlockViewPool { get; set; }
        private Pool<ParticleSystem> EffectsPool { get; set; }
        
        public PieceData Data;
        public GameObject BlocksContaiter;
        public GameObject EffectsContaiter;
        public ParticleSystem ClearedEffect;
        public AudioPlayer AudioPlayer;

        public void SetBoard(Board board)
        {
            _board = board;
            Game.Instance.OnPieceSettled += OnBlockSettled;
            _board.OnBoardRowCleared += OnBoardRowCleared;
            _board.OnBoardLastRowCleared += OnBoardLastRowCleared;
            var size = board.Width * board.Height + 10;
            BlockViewPool = new Pool<BlockView>(Data.Block, size, BlocksContaiter);
            EffectsPool = new Pool<ParticleSystem>(ClearedEffect.gameObject,board.Height + 10,EffectsContaiter);
            EffectsPool.DeactivateAll();
        }

        private void OnBlockSettled()
        {
            CleanRowEffects();
        }

        private void OnBoardRowCleared(int row, float time)
        {
            StartCoroutine(GetBlocksInRow(row, time));
        }
        
        private void OnBoardLastRowCleared(List<Block> blocks, float time)
        {
            StartCoroutine(GetLastBlocks(blocks, time));
        }

        private IEnumerator GetLastBlocks(List<Block> blocks, float time)
        {
            var temp = new List<BlockView>();
            foreach (var view in BlockViewPool.GetActiveItems())
            {
                foreach (var block in blocks)
                {
                    if (view.transform.position.y.Equals(block.Position.Row))
                    {
                        temp.Add(view);
                    }
                }
            }

            foreach (var r in temp)
            {
                _settledBlocksInRow.Add(r);
            }
            
            if (_settledBlocksInRow.Count > 0)
            {
                float elapsedTime = 0;
                while (elapsedTime < time)
                {
                    elapsedTime += Time.deltaTime;
        
                    foreach (var view in _settledBlocksInRow)
                    {
                        EnableGlowInRows(view, elapsedTime / time);
                    }
        
                    yield return null;
                }
                _settledBlocksInRow.Clear();
            }
            
            Game.Instance.Resume();
            _board.Remove(blocks);
            _board.AddPiece();
            AudioPlayer.PlayCollectRowClip();
        }
        
        private IEnumerator GetBlocksInRow(int row, float time)
        {
            var temp = new List<BlockView>();
            foreach (var view in BlockViewPool.GetActiveItems())
            {
                if (view.transform.position.y.Equals(row))
                {
                    temp.Add(view);
                }
            }

            foreach (var r in temp)
            {
                _settledBlocksInRow.Add(r);
            }
            
            if (_settledBlocksInRow.Count > 0)
            {
                float elapsedTime = 0;
                while (elapsedTime < time)
                {
                    elapsedTime += Time.deltaTime;
        
                    foreach (var view in _settledBlocksInRow)
                    {
                        EnableGlowInRows(view, elapsedTime / time);
                    }
        
                    yield return null;
                }
                _settledBlocksInRow.Clear();
            }

            var effect = EffectsPool.GetAndActivate(new Vector3(4.5f, row, 0));
            _clearedRowEffects.Add(effect);
            _board.Remove(_board.GetBlocksFromRow(row));
            _board.MoveDownBlocksBelowRow(row);
            AudioPlayer.PlayCollectRowClip();
        }

        private void EnableGlowInRows(BlockView view, float time)
        {
            var mat = view.Renderer.material;
            var color = mat.GetColor(ColorId);
            mat.SetColor(ColorId, Color.Lerp(color, color * 1.25f, time * 0.5f));
        }
        
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
                _currentGhostBlock = RenderBlock(Data.GhostPieceMaterial, position);
                if (CheckIntersectsPositions(position))
                {
                    _currentGhostBlock.gameObject.SetActive(false);
                }
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
            var view = BlockViewPool.GetAndActivate();
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
            return Data.PieceMaterials[(int) type];
        }

        private void CleanRowEffects()
        {
            if (_clearedRowEffects.Count <= 0)
            {
                return;
            }
            
            EffectsPool.DeactivateAll();
            _clearedRowEffects.Clear();
        }
        
        private void OnDestroy()
        {
            Game.Instance.OnPieceSettled -= OnBlockSettled;
            _board.OnBoardRowCleared -= OnBoardRowCleared;
            _board.OnBoardLastRowCleared -= OnBoardLastRowCleared;
        }
    }
}
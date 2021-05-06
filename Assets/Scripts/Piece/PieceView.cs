﻿using UnityEngine;
using UnityEngine.UI;

namespace Octamino
{
    public class PieceView : MonoBehaviour
    {
        private Board _board;
        private Pool<BlockView> _blockViewPool;
        private PieceType? _renderedPieceType;
        private int _blockPoolSize = 10;
        private bool _forceRender;
        private RenderTexture _renderTexture;

        private RawImage _image;

        public PieceData Data;
        public Camera NextBlockCameraView;
        public GameObject NextBlockContainer;
        
        private void Awake()
        {
            _image = GetComponent<RawImage>();
        }

        public void SetBoard(Board board)
        {
            _board = board;
            _blockViewPool = new Pool<BlockView>(Data.Block, _blockPoolSize, NextBlockContainer);
            _renderTexture = new RenderTexture(140, 140, 0);
            _image.texture = _renderTexture;
            NextBlockCameraView.targetTexture = _renderTexture;
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
        
        private void DrawPiece(Piece piece)
        {
            _blockViewPool.DeactivateAll();
            
            foreach (var block in piece.Blocks)
            {
                var blockView = _blockViewPool.GetAndActivate();
                blockView.SetMaterial(BlockMaterial(block.Type));
                blockView.SetPosition(BlockPosition(block.Position));
                blockView.gameObject.layer = 10;
            }
            
            var pieceBlocks = _blockViewPool.Items.First(piece.Blocks.Length);
            var xValues = pieceBlocks.Map(b => b.transform.localPosition.x);
            var yValues = pieceBlocks.Map(b => b.transform.localPosition.y);

            var halfBlockSize = 0.5f;
            var minX = Mathf.Min(xValues) - halfBlockSize;
            var maxX = Mathf.Max(xValues) + halfBlockSize;
            var minY = Mathf.Min(yValues) - halfBlockSize;
            var maxY = Mathf.Max(yValues) + halfBlockSize;
            var width = maxX - minX;
            var height = maxY - minY;
            var offsetX = -width * 0.5f - minX;
            var offsetY = -height * 0.5f - minY;

            foreach (var block in pieceBlocks)
            {
                block.transform.localPosition += new Vector3(offsetX, offsetY);
            }
        }

        private Vector3 BlockPosition(Position position)
        {
            return new Vector3(position.Column, position.Row);
        }
        
        private Material BlockMaterial(PieceType type)
        {
            return Data.PieceMaterials[(int)type];
        }

        private void OnDestroy()
        {
            _renderTexture.Release();
        }
    }
}
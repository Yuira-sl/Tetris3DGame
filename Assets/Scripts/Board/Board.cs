using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    private GameObject _blocksContainer;
    private BoardProjection _boardProjection;
    private int[,] _boardMatrix;

    private ParticleSystem _currentEffect;
    private bool _isEffectInited;

    [SerializeField] private AudioManager _audioManager; 

    [SerializeField] private BlockController _blockController;
    [SerializeField] private BoardData _boardData;

    public BlockController BlockController => _blockController;
    public GameObject BlocksContainer => _blocksContainer;

    private void Awake()
    {
        _blockController.OnBlockSettle += OnBlockSettle;
        gameObject.AddComponent<BoardProjection>();
        _boardProjection = GetComponent<BoardProjection>();
    }

    private void Start()
    {
        //Create int array according to board size
        _boardMatrix = new int[_boardData.BoardSize.x, _boardData.BoardSize.y];
        _blocksContainer = new GameObject();
        _blocksContainer.name = "Settled Blocks Container";
        _blocksContainer.transform.SetParent(transform);
    }

    //for temp effect
    private void Update()
    {
        if (_isEffectInited)
        {
            if (!_currentEffect.isPlaying)
            {
                Destroy(_currentEffect.gameObject);
                _isEffectInited = false;
                _currentEffect = null;
            }
        }
    }
    
    private void OnDestroy()
    {
        _blockController.OnBlockSettle -= OnBlockSettle;
    }

    //Store block tiles in boolean matrix
    private void OnBlockSettle(List<Vector2Int> positions, bool speed)
    {
        List<int> possibleRows = new List<int>();

        foreach(var position in positions)
        {
            //Warn editor if an illegal settlement has been made
            if (_boardMatrix[position.x, position.y] == 1)
                Debug.LogWarning("Tile settled in an illegal place!");

            //Store tiles' Y coordinates to check for row clears
            if (!possibleRows.Contains(position.y))
                possibleRows.Add(position.y);

            _boardMatrix[position.x, position.y] = 1;
        }

        CheckForClearedRows(possibleRows);
    }

    //Checks if coordinate has a tile in board matrix
    public bool HasTile(Vector2Int position)
    {
        //Out of bounds check
        Rect boardRect = new Rect(0, 0, _boardData.BoardSize.x, _boardData.BoardSize.y);

        bool outOfBounds = !boardRect.Contains(position);

        if (!outOfBounds)
        {
            //If not out of bounds, check in board matrix
            return _boardMatrix[position.x, position.y] == 1;
        }

        return true;
    }

    private void CheckForClearedRows(List<int> possibleRows)
    {
        //Sort in ascending order, to conform with algorithm logic
        possibleRows.Sort();

        //Accounts for cleared rows, to offset other row values accondingly
        int clearedRows = 0;

        foreach(int row in possibleRows)
        {
            int effectiveRow = row - clearedRows;

            if (IsRowFull(effectiveRow))
            {
                _audioManager.Play(_audioManager.Clips[1]);
                
                StartCoroutine(ProcessToRemove(row));
                StartCoroutine(RemoveRowFromBoard(effectiveRow));
                
                ScoreManager.Instance.AddRowScore();
                clearedRows++;
            }
        }
    }
    
    //Returns true if row is full
    private bool IsRowFull(int row)
    {
        bool successAll = false;
        //Iterate through row in board matrix to see if all elements are 1. If a 0 is found, interrupt loop
        for (int i = 0; i < _boardData.BoardSize.x; i++)
        {
            successAll = HasTile(new Vector2Int(i, row));

            if (!successAll)
                break;
        }

        return successAll;
    }

    private IEnumerator ProcessToRemove(int row)
    {
        var tiles = new List<Transform>();
        tiles.AddRange(_blocksContainer.GetComponentsInChildren<Transform>());
        //As GetComponentsInChildren looks in the parent as well, ignore first element, which is the container itself
        tiles.RemoveAt(0);
        
        var tilesForEffect = new List<Transform>();

        foreach (var tile in tiles)
        {
            if (Mathf.RoundToInt(tile.position.y) == row)
            {
                tilesForEffect.Add(tile);
            }
        }
        
        var elapsedTime = 0.0f;
        
        while (elapsedTime < _boardData.RowCleanEffectTime) 
        {
            elapsedTime += Time.deltaTime;
            
            foreach (var tile in tilesForEffect)
            {
                //temp effect
                // TODO: Need to rework
                var col = tile.gameObject.GetComponentInChildren<Renderer>().material.color;
                tile.gameObject.GetComponentInChildren<Renderer>().material.color 
                    = Color.Lerp(col, Color.black, elapsedTime / _boardData.RowCleanEffectTime);
            }
            yield return null;
        }
    }
    //Removes row from board matrix
    private void RemoveRowFromMatrix(int row)
    {
        //Start from row to be removed, and goes up until the second last row
        for(int j = row; j < _boardData.BoardSize.y - 1; j++)
        {
            for (int i = 0; i < _boardData.BoardSize.x; i++) {
                //Store upper row element value in lower row
                _boardMatrix[i,j] = _boardMatrix[i,j+1];
            }
        }

        //For the last row, fill it with zeroes
        for(int i = 0; i < _boardData.BoardSize.x; i++)
        {
            _boardMatrix[i, _boardData.BoardSize.y - 1] = 0;
        }
    }

    //Remove actual tile game objects from scene
    private IEnumerator RemoveRowFromBoard(int row)
    {
        RemoveRowFromMatrix(row);

        yield return new WaitForSeconds(_boardData.RowCleanEffectTime);
        
        var tiles = new List<Transform>();
        tiles.AddRange(_blocksContainer.GetComponentsInChildren<Transform>());
        //As GetComponentsInChildren looks in the parent as well, ignore first element, which is the container itself
        tiles.RemoveAt(0);
        
        //Get tiles from row to be removed
        var rowTiles = new List<Transform>();

        foreach (var tile in tiles)
        {
            if (Mathf.RoundToInt(tile.position.y) == row)
            {
                rowTiles.Add(tile);
            }
        }
        
        // temp effect
        // TODO: исправить багу если успел закрыть новый слой, когда текущий эффект не закончился. новый не инитися понятно почему 
        if (_currentEffect == null)
        {
            _currentEffect = Instantiate(_boardData.TempCleaningEffect, new Vector3(4, row, 0), Quaternion.identity);
            _isEffectInited = true;
        }
        
        //Destroy all tiles from row
        foreach (var tile in rowTiles)
        {
            Destroy(tile.gameObject);
        }
            
        //Get all tiles from rows bigger than the removed row
        var biggerRowTiles = new List<Transform>();
        
        foreach (var tile in tiles)
        {
            if (Mathf.RoundToInt(tile.position.y) > row)
            {
                biggerRowTiles.Add(tile);
            }
        }
        
        //Lower a row in each bigger row tile
        foreach (var tile in biggerRowTiles)
        {
            tile.position += Vector3.down;
        }
    }
}

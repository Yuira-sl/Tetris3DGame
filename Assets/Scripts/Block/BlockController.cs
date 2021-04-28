using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public delegate void BlockTilesCallback(List<Vector2Int> positions, bool speed);
public delegate void BlockMovementCallback(GameObject controller);
public delegate void BlockCallback(GameObject block, Vector2Int position);

public class BlockController : MonoBehaviour
{
    private List<BlockTile> _tiles = new List<BlockTile>();
    private GameObject _block;

    private Transform _pivot;
    
    //Timer to trigger block descent
    private float _verticalTimer;
    private bool _isRotating;
    private bool _allowRotation = true;
    private bool _hasSpeed;
    private bool _isPaused;
    
    [SerializeField] private AudioManager _audioManager; 

    [SerializeField] private InputController _inputController;
    [SerializeField] private BlockControllerData _blockControllerData;
    [SerializeField] private Board _board;
    [SerializeField] private NextBlock _nextBlock;
    
    public BlockControllerData BlockControllerData => _blockControllerData;

    public bool IsPaused
    {
        get => _isPaused;
        set => _isPaused = value;
    }

    public event BlockTilesCallback OnBlockSettle;
    public event BlockMovementCallback OnMovement;
    public event BlockCallback OnNewBlock;

    public GameObject GetCurrentBlock()
    {
        return _block;
    }
    
    public List<GameObject> GetCurrentChildObjects()
    {
        return _block.GetComponentsInChildren<GameObject>().GetChildOnlyObjects();
    }

    
    private void Start()
    {
        var prefab = GetRandomBlock();
        var randTile = GetRandomTile();

        _block = Instantiate(prefab, transform.position, Quaternion.identity);

        var positions = _block.GetComponent<TileContainer>().Positions;
        
        foreach (var position in positions)
        { 
            var go = Instantiate(randTile, _block.transform, true);
            go.AddComponent<BlockTile>();
            go.transform.localPosition = position;
        }
        
        _block.transform.SetParent(transform);

        var blockTiles = _block.GetComponentsInChildren<BlockTile>();
        _tiles.AddRange(blockTiles);

        transform.position = _blockControllerData.BlockStartingPosition;

        OnNewBlock?.Invoke(_block, Vector2Int.RoundToInt(transform.position));
        
        _inputController.OnSpeedDown += OnSpeedDown;
        
        _inputController.OnSwitchDown += OnSwitchDown;
        _inputController.OnRotateLeftDown += OnRotateLeftDown;
        _inputController.OnRotateRightDown += OnRotateRightDown;
        _inputController.OnHorizontalInputDown += OnHorizontalInputControllerDown;
        _inputController.OnForcedDropDown += ForcedDropDown;
    }
    
    private void OnDestroy()
    {
         _inputController.OnSpeedDown -= OnSpeedDown;
   
        _inputController.OnSwitchDown -= OnSwitchDown;
        _inputController.OnRotateLeftDown -= OnRotateLeftDown;
        _inputController.OnRotateRightDown -= OnRotateRightDown;
        _inputController.OnHorizontalInputDown -= OnHorizontalInputControllerDown;
        _inputController.OnForcedDropDown -= ForcedDropDown;
    }

    private void Update()
    {
        UpdateVertical();
    }

    private void UpdateVertical()
    {
        _verticalTimer += Time.deltaTime;

        //Check for timer value
        if (!_isRotating && _verticalTimer > GameManager.Instance.CurrentBlockSpeed && !_isPaused)
        {
            //Block rotation to avoid bugs
            _allowRotation = false;

            //Try down movement. If it can't be done, settle block
            if (CanBlockMove(new Vector2Int(0, -1)))
            {
                transform.position += Vector3Int.down;
            }
            else
            {
                SettleBlockRoutine();
            }

            //Reset timer and rotation values
            _verticalTimer = 0;
            _allowRotation = true;
        }
    }

    private bool CanBlockMove(Vector2Int moveDirection, int rotation = 0)
    {
        bool successAll = false;

        //All tiles have to be able to move in order to confirm movement
        if (_tiles != null)
        {
            foreach (var tile in _tiles)
            {
                successAll = tile.CanMove(
                    moveDirection,
                    _board,
                    _isRotating,
                    rotation,
                    transform
                );

                //If failed, interrupt loop
                if (!successAll)
                {
                    break;
                }
            }
        }

        return successAll;
    }

    private void OnHorizontalInputControllerDown(int direction)
    {
        if (!_isRotating && CanBlockMove(new Vector2Int(direction, 0)))
        {
            transform.position += Vector3.right * direction;
            TriggerOnMovement();
        }
    }
    
//Rotate block on z-axis
    private void OnRotateLeftDown()
    {
        CheckAndStartRotation(-1);
    }

    private void OnRotateRightDown()
    {
        CheckAndStartRotation(1);
    }

    private void CheckAndStartRotation(int direction)
    {
        _pivot = _block.GetComponent<TileContainer>().Pivot;

        //Check if rotation time is bigger than remaining time to go down. If it is, don't rotate
        float remainingTime = GameManager.Instance.CurrentBlockSpeed - _verticalTimer;
        float rotationTime = (1f / _blockControllerData.BlockTurningSpeed) * Time.fixedDeltaTime;

        bool enoughTime = remainingTime > rotationTime;

        if (!_isRotating && _allowRotation && enoughTime)
        {
            int rotation = -90 * direction;

            //Check if rotation is allowed
            bool allowed = CanBlockMove(Vector2Int.zero, rotation);

            if (allowed)
            {
                var coroutine = Rotate(rotation);
                StartCoroutine(coroutine);
            }
        }
    }
   
    //Rotates block in z-axis with specified angle
    private IEnumerator Rotate(float rotationAngle)
    {
        if (!_isRotating && _allowRotation)
        {
            _isRotating = true;
            _allowRotation = false;

            if (!_pivot)
            {
                Vector3 targetRotation = transform.rotation * new Vector3(0, 0, Mathf.Round(transform.eulerAngles.z + rotationAngle));
                Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
                float progress = 0f;
                while (progress != 1)
                {
                    progress += _blockControllerData.BlockTurningSpeed;
                    if (progress > 1)
                    {
                        progress = 1;
                    }
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, progress);
                    yield return null;
                }
            }
            else
            {
                float step = 0f;
                float smoothStep;
                float lastStep = 0f;
                while(step < 1.0f) 
                {
                    step += _blockControllerData.BlockTurningSpeed;
                    smoothStep = Mathf.SmoothStep(0.0f, 1.0f, step);
                    transform.RotateAround(_pivot.position, Vector3.forward, rotationAngle * (smoothStep - lastStep));
                    lastStep = smoothStep;
                    yield return null;
                }
           
                if(step > 1.0f) 
                {
                    transform.RotateAround(_pivot.position, Vector3.forward, rotationAngle * (1.0f - lastStep));
                }
            }
           
            
            _isRotating = false;
            _allowRotation = true;
            
            TriggerOnMovement();
        }
    }
    
    private void ForcedDropDown()
    {
        _allowRotation = false;
        
        while (true)
        {
            if (CanBlockMove(new Vector2Int(0, -1)))
            {
                transform.position += Vector3Int.down;
            }
            else
            {
                SettleBlockRoutine();
                break;
            }
        }
        
        _allowRotation = true; 
    }
    
    private void OnSwitchDown()
    {
        if (!_isRotating)
        {
            //Check if switch won't cause out of bounds or collisions
            List<Vector2Int> possiblePositions = _nextBlock.GetPossibleNextBlockCoordinates(Vector2Int.RoundToInt(transform.position));

            //For each coordinate, verify if it would have a block. If so, cancel switch
            bool successAll = false;

            foreach (var possiblePosition in possiblePositions)
            {
                successAll = !_board.HasTile(possiblePosition);
                //If failed, interrupt loop
                if (!successAll)
                {
                    break;
                }
            }

            if (successAll)
            {
                GameObject aux = _block;
                GetNextBlock();
                _nextBlock.SwitchBlock(aux);
                TriggerOnNewBlock();
            }
        }
    }
    
    private void OnSpeedDown()
    {
        _hasSpeed = true;
    }
    
    //Triggers OnMovement event from behaviour classes
    public void TriggerOnMovement()
    {
        OnMovement?.Invoke(gameObject);
    }

    public void TriggerOnNewBlock()
    {
        OnNewBlock?.Invoke(_block, Vector2Int.RoundToInt(transform.position));
    }
    
    //Get random block from block pool
    private GameObject GetRandomBlock()
    {
        int randBlock = Random.Range(0, _blockControllerData.BlockTypePool.Count);
        var block = _blockControllerData.BlockTypePool[randBlock];
        return block;
    }
    
    private GameObject GetRandomTile()
    {
        int randTile = Random.Range(0, _blockControllerData.TilePool.Count);
        var tile = _blockControllerData.TilePool[randTile];
        return tile;
    }
    
    //Get next block and place it into current block
    private void GetNextBlock()
    {
        //Reset rotation of this component
        transform.rotation = Quaternion.identity;

        _block = _nextBlock.Block;
        _block.transform.localScale = new Vector3(1, 1, 1);
        _block.transform.position = transform.position;
        _block.transform.SetParent(transform);

        _tiles = new List<BlockTile>();
        _tiles.AddRange(_block.GetComponentsInChildren<BlockTile>());
    }

    //Settle block complete routine
    public void SettleBlockRoutine()
    {
        SettleCurrentBlock();

        //Fetches next block and triggers a new block generation from NextBlock
        GetNextBlock();
        _nextBlock.GenerateNewBlock();

        //Resets position to the top
        transform.position = _blockControllerData.BlockStartingPosition;
        OnNewBlock?.Invoke(_block, Vector2Int.RoundToInt(transform.position));
    }


    //Removes playable components from current blocks
    private void SettleCurrentBlock()
    {
        _audioManager.Play(_audioManager.Clips[3]);

        if (_tiles != null)
        {
            //Get tile positions
            List<Vector2Int> positions = new List<Vector2Int>();

            foreach (var tile in _tiles)
            {
                positions.Add(Vector2Int.RoundToInt(tile.transform.position));

                //Re-parent block tiles to board
                tile.transform.SetParent(_board.BlocksContainer.transform);

                //Destroy tile component to leave static tile only
                Destroy(tile);
            }

            OnBlockSettle.Invoke(positions, _hasSpeed);
            //Reset speed flag after triggering event
            _hasSpeed = false;

            //Destroy block parent game object and references
            Destroy(_block);
            _tiles = null;
        }
    }
}

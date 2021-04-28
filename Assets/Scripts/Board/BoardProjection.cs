using System.Collections.Generic;
using UnityEngine;

public class BoardProjection : MonoBehaviour
{
    private Board _board;
    private GameObject _projectionBlock;
    private BlockTile _collisionTile;

    public GameObject GetCurrentProjection()
    {
        return _projectionBlock;
    }

    public List<GameObject> GetCurrentProjectionChildObjects()
    {
        return _projectionBlock.GetComponentsInChildren<GameObject>().GetChildOnlyObjects();
    }
    
    private void Awake()
    {
        _board = GetComponent<Board>();
        _board.BlockController.OnMovement += OnMovement;
        _board.BlockController.OnNewBlock += OnNewBlock;
    }
    
    private void OnDestroy()
    {
        _board.BlockController.OnMovement -= OnMovement;
        _board.BlockController.OnNewBlock -= OnNewBlock;
    }

    private void OnNewBlock(GameObject block, Vector2Int position)
    {
        //Destroy current projection if exists
        if(_projectionBlock != null)
        {
            Destroy(_projectionBlock);
        }

        //Create projection based on new block
        _projectionBlock = Instantiate(block, Vector3.zero, Quaternion.identity);
        _projectionBlock.name = "Projection";

        //Call first movement update
        OnMovement(_board.BlockController.gameObject);

        //Remove script from projection tiles
        foreach (var blockTile in _projectionBlock.GetComponentsInChildren<BlockTile>())
        {
            Destroy(blockTile);
        }

        //Change color alpha to make block tiles look like a projection

        var renderers = new List<Renderer>();
        renderers.AddRange(_projectionBlock.GetComponentsInChildren<Renderer>());

        foreach (var r in renderers)
        {
            r.sharedMaterial = _board.BlockController.BlockControllerData.GhostMaterial;
        }
    }

    private void OnMovement(GameObject controller)
    {
        //Maintain position and rotation from original block
        _projectionBlock.transform.position = controller.transform.position;
        _projectionBlock.transform.rotation = controller.transform.rotation;

        var tiles = new List<BlockTile>();
        tiles.AddRange(controller.GetComponentsInChildren<BlockTile>());

        //Iterate downwards from original postion until collide with something
        int initialHeight = Mathf.RoundToInt(_projectionBlock.transform.position.y);
        for (int j = initialHeight; j >= 0; j--)
        {
            Vector2Int moveAmount = Vector2Int.down + Vector2Int.down * (initialHeight - j);

            //If projection can move down without colliding with something, do so
            if(CanProjectionMove(tiles, moveAmount))
            {
                _projectionBlock.transform.position += Vector3.down;
            }
            //If not, interrupt loop and exit, projection got as far as it could
            else
            {
                break;
            }
        }
    }

    //Verify all projection tiles movement possibility
    private bool CanProjectionMove(List<BlockTile> tiles, Vector2Int moveDirection)
    {
        bool successAll = false;

        //All tiles have to be able to move in order to confirm movement
        if (tiles != null)
        {
            foreach (var tile in tiles)
            {
                successAll = !_board.HasTile(Vector2Int.RoundToInt(tile.transform.position) + moveDirection);

                //If failed, interrupt loop
                if (!successAll)
                {
                    break;
                }
            }
        }

        return successAll;
    }
}

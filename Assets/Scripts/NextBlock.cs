using System.Collections.Generic;
using UnityEngine;

public class NextBlock : MonoBehaviour
{
    [SerializeField] private BlockControllerData _blockControllerData;
    [SerializeField] private GameObject _panel;

    private GameObject block;
    private GameObject _blockContainer;

    public GameObject Block => block;
    
    private void Start()
    {
        _blockContainer = new GameObject();
        var position = _blockContainer.transform.position;
        position.x = _panel.transform.position.x;
        position.y = _panel.transform.position.y;
        position.z = -15.8f;
        _blockContainer.transform.position = position;
        _blockContainer.name = "Next Block Container";
        _blockContainer.transform.SetParent(transform);

        GenerateNewBlock();
    }

    public void SwitchBlock(GameObject go)
    {
        block = go;

        //Reset position and rotation
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;

        go.transform.SetParent(_blockContainer.transform);
        //Center object pivot in panel
        block.transform.localPosition = Vector3.zero;

        //Lowering block scale to fit panel
        //Scale is proportional to a constant, and also to the screen aspect ratio
        Vector3 newScale = _blockControllerData.NextBlockScale * (Mathf.Sqrt(Camera.main.aspect));
        block.transform.localScale = newScale;

        //Ajust container according to block center, accounting for the scale change
        var offset = FindBlockCenter(block);
        if (offset != null)
        {
            block.transform.localPosition -= new Vector3(offset.Value.x * newScale.x, offset.Value.y * newScale.y);
        }
    }

    //Generates a new block drafted from the block pool
    public void GenerateNewBlock()
    {
        block = Instantiate(GetRandomBlock(), Vector3.zero, Quaternion.identity);
        block.transform.SetParent(_blockContainer.transform);
        //Center object pivot in panel
        block.transform.localPosition = Vector3.zero;

        //Lowering block scale to fit panel
        //Scale is proportional to a constant, and also to the screen aspect ratio
        Vector3 newScale = _blockControllerData.NextBlockScale * (Mathf.Sqrt(Camera.main.aspect));
        block.transform.localScale = newScale;

        //Ajust container according to block center, accounting for the scale change
        var offset = FindBlockCenter(block);
        if (offset != null)
        {
            block.transform.localPosition -= new Vector3(offset.Value.x * newScale.x, offset.Value.y * newScale.y);
        }
    }

    //Returns a list of possible coordinates of the next block in the case of a switch
    public List<Vector2Int> GetPossibleNextBlockCoordinates(Vector2Int currentPosition)
    {
        var tiles = new List<BlockTile>();
        tiles.AddRange(block.GetComponentsInChildren<BlockTile>());

        List<Vector2Int> positions = new List<Vector2Int>();

        foreach(var tile in tiles)
        {
            //Get block local coordinates
            Vector2Int coord = Vector2Int.RoundToInt(tile.transform.localPosition);

            //Add the current position offset to the value
            coord += currentPosition;

            positions.Add(coord);
        }

        return positions;
    }

    //Get random block from block pool
    private GameObject GetRandomBlock()
    {
        int randBlock = Random.Range(0, _blockControllerData.BlockPool.Count);
        int randMaterial = Random.Range(0, _blockControllerData.Materials.Count);
        var mat = _blockControllerData.Materials[randMaterial];
        var block = _blockControllerData.BlockPool[randBlock];
        var rends = block.GetComponentsInChildren<Renderer>();
        foreach (var rend in rends)
        {
            rend.material = mat;
        }
        return block;
    }

    //Finds block true center to help piece exhibition
    private Vector2? FindBlockCenter(GameObject block)
    {
        var tiles = block.GetComponentsInChildren<BlockTile>();

        if (tiles == null)
            return null;

        //Stores minimum and maximum values of each coordinate
        float xMin = 0f, xMax = 0f, yMin = 0f, yMax = 0f;

        //Collect x and y max and min coordinates
        foreach (var tile in tiles)
        {
            float xCoord = tile.transform.localPosition.x;
            float yCoord= tile.transform.localPosition.y;

            xMin = xCoord < xMin ? xCoord : xMin;
            yMin = yCoord < yMin ? yCoord : yMin;

            xMax = xCoord > xMax ? xCoord : xMax;
            yMax = yCoord > yMax ? yCoord : yMax;
        }

        //Getting center point with total width and height

        float width = xMax - xMin;
        float height = yMax - yMin;

        float xCenter = xMin + (width / 2f);
        float yCenter = yMin + (height / 2f);

        return new Vector2(xCenter, yCenter);
    }
}

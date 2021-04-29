using UnityEngine;

public class BlockTile : MonoBehaviour
{
    public bool CanMove(Vector2Int movement, Board board, bool isRotating, int rotation = 0, Transform controllerTransform = null)
    {
        var possiblePosition = Vector2Int.zero;

        //Horizontal / vertical movement
        if (rotation == 0)
        {
            //If already rotating, block movement
            if (isRotating)
            {
                return false;
            }

            //Transforming value to integer to avoid rounding problems
            possiblePosition = Vector2Int.RoundToInt(transform.position) + movement;
        }
        //Rotation
        else
        {
            //Game object to project possible position after rotation
            var projectedGO = gameObject;
            //Equalizing local position to real tile's and simulating rotation to get possible position
            projectedGO.transform.localPosition = transform.localPosition;
            controllerTransform.transform.Rotate(new Vector3(0, 0, rotation));
            possiblePosition = Vector2Int.RoundToInt(projectedGO.transform.position);
        }

        //Check if move is legal according to board matrix
        bool legalMove = !board.HasTile(possiblePosition);

        return legalMove;
    }
}

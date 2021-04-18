using UnityEngine;

[CreateAssetMenu(fileName = "Score Data", menuName = "Score Data")]
public class ScoreData : ScriptableObject
{
    public int PointsWithoutSpeed = 2;
    public int PointsWithSpeed = 4;
    public int PointsPerRow = 40;
}

using UnityEngine;

[CreateAssetMenu(fileName = "Game Data", menuName = "Game Data")]
public class GameData : ScriptableObject
{
    public int GameOverLimitRow = 20;
    public float StartingPeriod = 1f;
    public float DecrementPerLevel = 0.2f;
    public float SpeedPeriod = 0.05f;
}

using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private int _scoreValue;
    
    public GameManager managerController;
    public Text ScoreText;
    
    private void Start()
    {
        ScoreText.text = "Score: " + managerController.GetScore();
    }

    private void Update()
    {
        if (_scoreValue != managerController.GetScore())
        {
            _scoreValue = managerController.GetScore();
            ScoreText.text = "Score: " + _scoreValue;
        }
    }
}

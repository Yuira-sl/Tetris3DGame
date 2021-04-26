using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadScene(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
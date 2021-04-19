using UnityEngine;

public class ImmortalObject : MonoBehaviour
{
    private static ImmortalObject _instance;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }
}

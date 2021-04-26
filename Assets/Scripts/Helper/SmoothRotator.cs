using System.Collections;
using UnityEngine;

public class SmoothRotator: MonoBehaviour
{
    [SerializeField] private float _time = 0.5f;
    
    public void RotateDown(float from)
    {
        StartCoroutine(SmoothRotate(new Vector2(from, from - 100f)));
    }
    
    public void RotateUp(float from)
    {
        StartCoroutine(SmoothRotate(new Vector2(from - 100, from)));
    }
    private IEnumerator SmoothRotate(Vector2 angleRange)
    {
        var elapsedTime = 0f;
        while (elapsedTime < _time)
        {
            transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(0,0, angleRange.x), 
                Quaternion.Euler(0,0,angleRange.y), 
                elapsedTime / _time);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
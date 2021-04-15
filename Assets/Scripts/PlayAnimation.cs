using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    private Animation _animation;
    
    private void Awake()
    {
        _animation = GetComponent<Animation>();
        
    }

    public void Activate()
    {
        _animation.Play();
    }

    public void ResetAnimation()
    {
        _animation.clip.SampleAnimation(gameObject, 0f);
    }
}

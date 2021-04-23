using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    private Animation _animation;
    
    private void Awake()
    {
        _animation = GetComponent<Animation>();
    }

    public void Play()
    {
        _animation.Play();
    }
    
    public void Play(int time)
    {
        _animation[_animation.clip.name].speed = time;
        _animation[_animation.clip.name].time = _animation[_animation.clip.name].length;
        _animation.Play(_animation.clip.name);
    }

    public void ResetAnimation()
    {
        _animation.clip.SampleAnimation(gameObject, 0f);
    }
}

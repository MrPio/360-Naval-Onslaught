using UnityEngine;

public class GlitchEffect : MonoBehaviour
{
    [SerializeField] private float eachSeconds = 3;
    [SerializeField] private float speed = 1;
    [SerializeField] private Animator glitchAnimator;
    private float _acc;
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int Start = Animator.StringToHash("start");

    private void Update()
    {
        if(eachSeconds<0)
            return;
        _acc += Time.deltaTime;
        if (_acc > eachSeconds)
        {
            _acc = 0;
            Animate();
        }
    }

    public void Animate()
    {
        glitchAnimator.SetFloat(Speed,speed);
        glitchAnimator.SetTrigger(Start);
    }
    
}

using UnityEngine;

public class AnimationTimer : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public float GetCurrentCurrentTime()
    {
        return anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
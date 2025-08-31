using UnityEngine;
using UnityEngine.UI;
public class AnimationSpeedController : MonoBehaviour
{
    [Range(0.1f, 2f)]
    public float animationSpeed = 1f;
    private Animation anim;
    void Awake()
    {
        anim = GetComponent<Animation>();
    }

    void Update()
    {
        // Loop through all animations on this component
        foreach (AnimationState state in anim)
        {
            state.speed = animationSpeed;
        }
    }
}


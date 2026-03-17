using UnityEngine;

public class AnimationEndDetector : MonoBehaviour
{
    [SerializeField] private UiAnimations uiAnimations;
    [SerializeField] private int animationIndex;
    
    public void AnimationEnded()
    {
        uiAnimations.SetAnimation(animationIndex);
    }
}

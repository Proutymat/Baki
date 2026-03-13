using UnityEngine;

public class AnimationEndDetector : MonoBehaviour
{
    [SerializeField] private UIAnimations uiAnimations;
    [SerializeField] private int animationIndex;
    
    public void AnimationEnded()
    {
        uiAnimations.SetAnimation(animationIndex);
    }
}

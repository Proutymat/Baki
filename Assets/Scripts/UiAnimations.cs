using UnityEngine;
using System.Collections.Generic;

public class UiAnimations : MonoBehaviour
{
    [SerializeField] private List<Animator> animators;
    [SerializeField] private List<int> animationsOn; // 0 = off, 1 = on

    void Start()
    {
        animationsOn = new List<int>();
        animationsOn.Add(1);
        animationsOn.Add(1);
        animationsOn.Add(0);
        animationsOn.Add(0);
        
        animators[0].SetTrigger("trigger");
        animators[1].SetTrigger("trigger");
    }

    public void SetAnimation(int index)
    {
        animationsOn[index] = 0;
        Debug.Log("Animation " + index + " set to off.");
        
        // Choose a random number between 0 and 3
        int randomIndex = Random.Range(0, animators.Count);
        while (animationsOn[randomIndex] == 1)
            randomIndex = Random.Range(0, animators.Count);

        animators[randomIndex].SetTrigger("trigger");
        animationsOn[randomIndex] = 1;
    }

    public void PauseAnimations()
    {
        foreach (var animator in animators)
            animator.speed = 0;
    }
    
    public void ResumeAnimations()
    {
        foreach (var animator in animators)
            animator.speed = 1;
    }
    
    void Update()
    {
        //Debug.Log("Animations On: " + animationsOn[0] + ", " + animationsOn[1] + ", " + animationsOn[2] + ", " + animationsOn[3]);
    }
}

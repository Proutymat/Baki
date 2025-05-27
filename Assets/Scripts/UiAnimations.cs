using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class UiAnimations : MonoBehaviour
{
    [SerializeField] private List<Animator> animators;
    [SerializeField] private List<int> animationsOn; // 0 = off, 1 = on
    [SerializeField] private Image shaderImage;
    [SerializeField] private float shaderSpeed = 0.8f;
    
    private float shaderValue;
    
    void Start()
    {
        animationsOn = new List<int>();
        animationsOn.Add(0);
        animationsOn.Add(0);
        animationsOn.Add(1);
        animationsOn.Add(1);
        
        animators[2].SetTrigger("trigger");
        animators[3].SetTrigger("trigger");
        
        Initialize();
    }

    void Initialize()
    {
        shaderImage.material.SetFloat("_vitesse", 0);
        shaderSpeed = 0.8f;
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
        Debug.Log("Animation " + randomIndex + " set to on.");
        animationsOn[randomIndex] = 1;
    }
    
    public void SetShaderSpeed(float speed)
    {
        shaderSpeed = speed;
        
        DOTween.To(() => shaderValue, x => {
            shaderValue = x;
            shaderImage.material.SetFloat("_vitesse", shaderValue);
        }, shaderSpeed, 0.5f);
    }

    public void PauseAnimations()
    {
        foreach (var animator in animators)
            animator.speed = 0;
        
        DOTween.To(() => shaderValue, x => {
            shaderValue = x;
            shaderImage.material.SetFloat("_vitesse", shaderValue);
        }, 0f, 0.5f);
    }
    
    public void ResumeAnimations()
    {
        foreach (var animator in animators)
            animator.speed = 1;
        
        DOTween.To(() => shaderValue, x => {
            shaderValue = x;
            shaderImage.material.SetFloat("_vitesse", shaderValue);
        }, shaderSpeed, 0.5f);
    }
}

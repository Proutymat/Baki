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

    public void Initialize()
    {
        shaderImage.material.SetFloat("_vitesse", 0);
        shaderSpeed = 0.8f;
        shaderValue = 0f;
    }

    public void SetAnimation(int index)
    {
        animationsOn[index] = 0;
        
        // Choose a random number between 0 and 3
        int randomIndex = Random.Range(0, animators.Count);
        while (animationsOn[randomIndex] == 1)
            randomIndex = Random.Range(0, animators.Count);

        animators[randomIndex].SetTrigger("trigger");
        animationsOn[randomIndex] = 1;
    }
    
    public void SetShaderSpeed(float speed)
    {
        shaderSpeed = speed;
        PlayShader();
    }
    
    public void StopShader(float stopSpeed = 3f)
    {
        DOTween.To(() => shaderValue, x => {
            shaderValue = x;
            shaderImage.material.SetFloat("_vitesse", shaderValue);
        }, 0f, stopSpeed);
    }
    
    public void PlayShader()
    {
        DOTween.To(() => shaderValue, x => {
            shaderValue = x;
            shaderImage.material.SetFloat("_vitesse", shaderValue);
        }, shaderSpeed, 3f);
    }

    public void PauseAnimations()
    {
        foreach (var animator in animators)
            animator.speed = 0;
    }
    
    public void ResumeAnimationsAndShader()
    {
        foreach (var animator in animators)
            animator.speed = 1;
        
        PlayShader();
    }
}

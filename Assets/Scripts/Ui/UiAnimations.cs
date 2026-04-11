using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class UiAnimations : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField] private float m_shaderSpeed;
    
    [Title("Set In Inspector")]
    [SerializeField] private List<Animator> m_animators;
    [SerializeField] private List<int> m_animationsOn; // 0 = off, 1 = on
    [SerializeField] private Image m_topShaderImage;
    [SerializeField] private Image m_downShaderImage;
    
    private float m_angle;
    private float m_angleFactor;
    private float m_shaderAlpha;

    [SerializeField] private float value;
    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    void Start()
    {
        m_animationsOn = new List<int>();
        m_animationsOn.Add(0);
        m_animationsOn.Add(0);
        m_animationsOn.Add(1);
        m_animationsOn.Add(1);
        
        m_animators[2].SetTrigger("trigger");
        m_animators[3].SetTrigger("trigger");
        
        Initialize();
    }

    public void Initialize()
    {
        m_topShaderImage.material.SetFloat("_vitesse", 0);
        m_downShaderImage.material.SetFloat("_vitesse", 0);
        m_angle = 0;
        m_angleFactor = 0;
        
        // Shader alpha
        m_shaderAlpha = 0;
        Color color = m_topShaderImage.color;
        color.a = m_shaderAlpha;
        m_topShaderImage.color = color;
        m_downShaderImage.color = color;
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------
    
    private void Update()
    {
        m_angle += Time.deltaTime * m_angleFactor * m_shaderSpeed;
        m_topShaderImage.material.SetFloat("_vitesse", m_angle);
        m_downShaderImage.material.SetFloat("_vitesse", m_angle);
    }

    public void SetAnimation(int index)
    {
        m_animationsOn[index] = 0;
        
        // Choose a random number between 0 and 3
        int randomIndex = Random.Range(0, m_animators.Count);
        while (m_animationsOn[randomIndex] == 1)
            randomIndex = Random.Range(0, m_animators.Count);

        m_animators[randomIndex].SetTrigger("trigger");
        m_animationsOn[randomIndex] = 1;
    }
    
    public void StopShader(float stopSpeed = 3f)
    {
        // Speed
        DOTween.To(() => m_angleFactor, x => {
            m_angleFactor = x;
        }, 0f, stopSpeed);
        
        // Alpha
        DOTween.To(() => m_shaderAlpha, x => {
            m_shaderAlpha = x;
            Color color = m_topShaderImage.color;
            color.a = m_shaderAlpha;
            m_topShaderImage.color = color;
            m_downShaderImage.color = color;
        }, 0, stopSpeed);
    }
    
    public void PlayShader()
    {
        // Speed
        DOTween.To(() => m_angleFactor, x => {
            m_angleFactor = x;
        }, 1, 3f);
        
        // Alpha
        DOTween.To(() => m_shaderAlpha, x => {
            m_shaderAlpha = x;
            Color color = m_topShaderImage.color;
            color.a = m_shaderAlpha;
            m_topShaderImage.color = color;
            m_downShaderImage.color = color;
        }, 1, 3f);
    }

    public void PauseAnimations()
    {
        foreach (var animator in m_animators)
            animator.speed = 0;
    }
    
    public void ResumeAnimationsAndShader()
    {
        foreach (var animator in m_animators)
            animator.speed = 1;
        
        PlayShader();
    }
}

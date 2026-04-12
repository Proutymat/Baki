using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    [Title("Parameters")]
    [SerializeField] private float m_progression;
    [SerializeField] private float m_increaseValue;
    [SerializeField] private float m_decreaseValue;
    [SerializeField] private float m_fillingTweenSpeed;
    [SerializeField] private float m_updateShaderDecreaseInSec;
    
    [Title("Set In Inspector")]
    [SerializeField] private Image m_shaderImage;
    
    [Title("Debug"), SerializeField] private bool m_debug;
    [SerializeField, ShowIf("m_debug")] private bool m_isPaused;
    [SerializeField, ShowIf("m_debug")] private float m_decreaseTimer;
    [SerializeField, ShowIf("m_debug")] private float m_shaderProgression;

    public bool IsPaused { set => m_isPaused = value; }
    
    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    public void Intitialize()
    {
        m_progression = 0;
        m_shaderImage.material.SetFloat("_Progress", 0);
        m_isPaused = false;
        m_decreaseTimer = 0;
        m_shaderProgression = 0;
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    private void Update()
    {
        if (m_isPaused)
        {
            return;
        }
        
        m_progression -= m_decreaseValue * Time.deltaTime;
        m_progression = m_progression < 0 ? 0 : m_progression; // Clamp value
        
        m_decreaseTimer += Time.deltaTime;
        
        // Update shader
        if (m_decreaseTimer >= m_updateShaderDecreaseInSec)
        {
            m_decreaseTimer = 0;
            UpdateShader();
        }
    }

    private void UpdateShader()
    {
        DOTween.To(() => m_shaderProgression, x => {
            m_shaderProgression = x;
            m_shaderImage.material.SetFloat("_Progress", m_shaderProgression);
        }, m_progression / 100, m_fillingTweenSpeed);
    }
    
    public void IncreaseProgressBar()
    {
        m_progression += m_increaseValue;
        m_progression = m_progression > 100 ? 100 : m_progression; // Clamp value
        UpdateShader();
        
        // Bar is full
        if (m_progression >= 100)
        {
            m_progression = 0;
            m_decreaseTimer = 0;
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionBarFull");
            GameManager.Instance.BarIsFulfilled();

            Sequence seq = DOTween.Sequence();
            
            // Fill up to 100%
            seq.Append(DOTween.To(() => m_shaderProgression, x => {
                m_shaderProgression = x;
                m_shaderImage.material.SetFloat("_Progress", m_shaderProgression);
            }, 1, m_fillingTweenSpeed));
            
            // Then empty it
            seq.Append(DOTween.To(() => m_shaderProgression, x => {
                m_shaderProgression = x;
                m_shaderImage.material.SetFloat("_Progress", m_shaderProgression);
            }, 0, m_fillingTweenSpeed));
        }
    }
}

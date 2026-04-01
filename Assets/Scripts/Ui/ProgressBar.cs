using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    [Title("Parameters")]
    [SerializeField] private float m_minimum = 0;
    [SerializeField] private float m_maximum;
    [SerializeField] private float m_current;
    [SerializeField] private float m_increaseValue;
    [SerializeField] private float m_decreaseValue;
    [SerializeField] private float m_fillingSpeed;
    
    [Title("Set In Inspector")]
    [SerializeField] private Image m_shaderImage;
    
    [Title("Debug"), SerializeField] private bool m_debug;
    [SerializeField, ShowIf("m_debug")] private bool isPaused;
    
    public bool IsPaused { set { isPaused = value; } }
    
    private float m_timer;
    private bool m_barIsEmpty;
    private float m_progress;
    

    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Start()
    {
        // Set the initial value of the progress bar
        m_current = m_minimum;
        m_shaderImage.material.SetFloat("_Progress", 0);
        m_progress = 0;

        m_barIsEmpty = true;
        isPaused = true;
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    void FixedUpdate()
    {
        // Decrease progress bar every millisecond
        m_timer -= Time.deltaTime;
        if (m_timer <= 0)
        {
            m_timer = 0.001F;
            if (!isPaused) m_current -= m_decreaseValue / 100;
        }

        m_current = m_current < m_minimum ? m_minimum : m_current > m_maximum ? m_maximum : m_current; // Clamp 'current' values to min and max
        
        if (m_current < 0 && !m_barIsEmpty)
        {
            m_barIsEmpty = true;
            m_current = 0;
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionBarEmpty");
        }
        
        if (UpdateFillAmount() == 1)
        {
            m_current = 0;
            m_barIsEmpty = true;
        }

    }

    float UpdateFillAmount()
    {
        float currentOffset = m_current - m_minimum;
        float maximumOffset = m_maximum - m_minimum;
        float fillAmount = (float)m_current / maximumOffset;
        DOTween.To(() => m_progress, x => {
            m_progress = x;
            m_shaderImage.material.SetFloat("_Progress", m_progress);
        }, fillAmount, m_fillingSpeed);
        
        return fillAmount;
    }
    
    public bool IncreaseProgressBar()
    {
        m_current += m_increaseValue;
        
        // Bar is full
        if (m_current >= m_maximum)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionBarFull");
            m_barIsEmpty = true;
            return true;
        }
        // Bar is filling up
        else
        {
            m_barIsEmpty = false;
            return false;
        }
    }
}

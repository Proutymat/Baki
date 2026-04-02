using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class BeatingArrows : MonoBehaviour
{
    [Title ("Parameters")]
    [SerializeField] private float m_beatingValue;
    
    [Title("Debug"), SerializeField] private bool m_debug;
    [SerializeField, ShowIf("m_debug")] private bool m_isBeating;
    [SerializeField, ShowIf("m_debug")] private float m_beatingArrowTimer;
    
    public bool IsBeating { set => m_isBeating = value; }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------
    
    void Update()
    {
        if (!m_isBeating)
        {
            DOTween.To(() => m_beatingValue, x => {
                m_beatingValue = x;
                PanelManager.Instance.DirectionalArrows.transform.localScale = new Vector3(m_beatingValue, m_beatingValue, 1f);
            }, 1f, 0.5f);
            
        }
        else
        {
            m_beatingArrowTimer += Time.deltaTime;
            if (m_beatingArrowTimer >= 0.5f)
            {
                if (m_beatingValue >= 1)
                {
                    DOTween.To(() => m_beatingValue, x => {
                        m_beatingValue = x;
                        PanelManager.Instance.DirectionalArrows.transform.localScale = new Vector3(m_beatingValue, m_beatingValue, 1f);
                    }, 0.9f, 1f);
                }
                else
                {
                    DOTween.To(() => m_beatingValue, x => {
                        m_beatingValue = x;
                        PanelManager.Instance.DirectionalArrows.transform.localScale = new Vector3(m_beatingValue, m_beatingValue, 1f);
                    }, 1.02f, 1f);
                }
                m_beatingArrowTimer = 0f;
            }
        }
    }
}

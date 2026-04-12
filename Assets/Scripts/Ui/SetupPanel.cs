using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetupPanel : MonoBehaviour
{
    [Title("Set In Inspector")] 
    [SerializeField] private ToggleGroup m_languageToggle;
    [SerializeField] private Toggle m_toggleFrench;
    [SerializeField] private Image m_toggleFrenchImage;
    [SerializeField] private Toggle m_toggleEnglish;
    [SerializeField] private Image m_toggleEnglishImage;
    [SerializeField] private TMP_InputField m_gameDurationInputField;
    [SerializeField] private Toggle m_skipIntroToggle;

    public void UpdateTogglesColor()
    {
        // French
        if (m_toggleFrench.isOn)
        {
            m_toggleFrenchImage.color = Color.yellow;
            m_toggleEnglishImage.color = Color.white;
        }
        // English
        else if (m_toggleEnglish.isOn)
        {
            m_toggleFrenchImage.color = Color.white;
            m_toggleEnglishImage.color = Color.yellow;
        }
    }
    
    
    public void StartExperience()
    {
        int gameDuration = int.Parse(m_gameDurationInputField.text);
        Enum.TryParse(m_languageToggle.GetFirstActiveToggle().gameObject.name, out GameManager.GameLanguage language);
        GameManager.Instance.InitializeGame(gameDuration * 60, language, m_skipIntroToggle.isOn);
    }
}

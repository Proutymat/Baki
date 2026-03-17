using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PanelManager : MonoBehaviour
{ 
    public enum PanelState
    {
        Setup,
        Intro,
        Standard,
        Landmark,
        End
    }
    
    [Title("Panels")]
    [SerializeField] private GameObject setupPanel;
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject standardPanel;
    [SerializeField] private GameObject landmarkPanel;
    [SerializeField] private GameObject endPanel;
    
    [Title("Arrow Buttons")]
    [SerializeField] private Sprite arrowUp;
    [SerializeField] private Sprite arrowUpHovered;
    [SerializeField] private Sprite arrowDown;
    [SerializeField] private Sprite arrowDownHovered;
    [SerializeField] private Sprite arrowLeft;
    [SerializeField] private Sprite arrowLeftHovered;
    [SerializeField] private Sprite arrowRight;
    [SerializeField] private Sprite arrowRightHovered;
    [SerializeField] private Image buttonUp;
    [SerializeField] private Image buttonDown;
    [SerializeField] private Image buttonLeft;
    [SerializeField] private Image buttonRight;
    
    [Title("Intro elements")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Camera canvasCamera;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Button startButton;
    
    [Title("Standard elements")]
    [SerializeField] private UiAnimations uiAnimations;
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private GameObject directionalArrowsObject;
    [SerializeField] private Button arrowButtonForeward;
    [SerializeField] private Button arrowButtonLeft;
    [SerializeField] private Button arrowButtonRight;
    [SerializeField] private Button arrowButtonBackward;
    [SerializeField] private GameObject questionsArea;
    [SerializeField] private GameObject progressBarObject;
    [SerializeField] private GameObject bottomArrowIndication;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI answer1Text;
    [SerializeField] private TextMeshProUGUI answer2Text;
    
    public UiAnimations UIAnimations { get => uiAnimations; set => uiAnimations = value; }
    public ProgressBar ProgressBar { get => progressBar; set => progressBar = value; }
    public GameObject DirectionalArrows { get => directionalArrowsObject; set => directionalArrowsObject = value; }
    public string QuestionText { get => questionText.text; set => questionText.text = value; }
    public string Answer1Text { get => answer1Text.text; set => answer1Text.text = value; }
    public string Answer2Text { get => answer2Text.text; set => answer2Text.text = value; }
    
    [Title("Landmark elements")]
    [SerializeField] private List<Image> bubblePages;
    [SerializeField] private Sprite emptyBubbleSprite;
    [SerializeField] private Sprite filledBubbleSprite;
    [SerializeField] private TextMeshProUGUI landmarkText;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject landmarkAnswer1Button;
    [SerializeField] private GameObject landmarkAnswer2Button;
    
    public string LandmarkText { get => landmarkText.text; set => landmarkText.text = value; }
    public string LandmarkAnswer1Text { get => landmarkAnswer1Button.GetComponentInChildren<TextMeshProUGUI>().text; set => landmarkAnswer1Button.GetComponentInChildren<TextMeshProUGUI>().text = value; }
    public string LandmarkAnswer2Text { get => landmarkAnswer2Button.GetComponentInChildren<TextMeshProUGUI>().text; set => landmarkAnswer2Button.GetComponentInChildren<TextMeshProUGUI>().text = value; }
    
    private static PanelManager m_instance;
    private PanelState m_currentPanelState;
    
    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple PanelManager instances in scene!");
            Destroy(gameObject);
        }
        else
        {
            m_instance = this;
        }
    }
    public static PanelManager Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindFirstObjectByType<PanelManager>();
            return m_instance;
        }
    }

    public void SetPanel(PanelState panel)
    {
        //setupPanel.SetActive(panel == PanelState.Setup);
        introPanel.SetActive(panel == PanelState.Intro);
        standardPanel.SetActive(panel == PanelState.Standard);
        landmarkPanel.SetActive(panel == PanelState.Landmark);
        endPanel.SetActive(panel == PanelState.End);

        if (panel == PanelState.Intro)
        {
            startButton.GetComponent<StartButton>().Initialize();
        }
        
        m_currentPanelState = panel;
    }
    public void ShowDirectionalArrows(bool show)
    {
        directionalArrowsObject.SetActive(show);
    }
    
    public void ShowQuestionArea(bool show)
    {
        questionsArea.SetActive(show);
        progressBarObject.SetActive(show);

    }

    public void UpdateDirectionalArrow(string direction)
    {
        if (direction == "foreward")
        {
            // Update sprites
            buttonUp.sprite = arrowUpHovered;
            buttonDown.sprite = arrowDown;
            buttonLeft.sprite = arrowLeft;
            buttonRight.sprite = arrowRight;

            // Update button states
            arrowButtonForeward.enabled = false;
            arrowButtonLeft.enabled = true;
            arrowButtonRight.enabled = true;
            arrowButtonBackward.enabled = true;
            bottomArrowIndication.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == "backward")
        {
            buttonUp.sprite = arrowUp;
            buttonDown.sprite = arrowDownHovered;
            buttonLeft.sprite = arrowLeft;
            buttonRight.sprite = arrowRight;
            
            arrowButtonForeward.enabled = true;
            arrowButtonLeft.enabled = true;
            arrowButtonRight.enabled = true;
            arrowButtonBackward.enabled = false;
            bottomArrowIndication.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (direction == "left")
        {
            buttonUp.sprite = arrowUp;
            buttonDown.sprite = arrowDown;
            buttonLeft.sprite = arrowLeftHovered;
            buttonRight.sprite = arrowRight;
            
            arrowButtonForeward.enabled = true;
            arrowButtonLeft.enabled = false;
            arrowButtonRight.enabled = true;
            arrowButtonBackward.enabled = true;
            bottomArrowIndication.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direction == "right")
        {
            buttonUp.sprite = arrowUp;
            buttonDown.sprite = arrowDown;
            buttonLeft.sprite = arrowLeft;
            buttonRight.sprite = arrowRightHovered;
            
            arrowButtonForeward.enabled = true;
            arrowButtonLeft.enabled = true;
            arrowButtonRight.enabled = false;
            arrowButtonBackward.enabled = true;
            
            bottomArrowIndication.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            buttonUp.sprite = arrowUp;
            buttonDown.sprite = arrowDown;
            buttonLeft.sprite = arrowLeft;
            buttonRight.sprite = arrowRight;
            
            arrowButtonForeward.enabled = true;
            arrowButtonLeft.enabled = true;
            arrowButtonRight.enabled = true;
            arrowButtonBackward.enabled = true;
        }
    }

    public void NextBubblePage(int pageIndex)
    {
        bubblePages[pageIndex + 1].sprite = emptyBubbleSprite;
        bubblePages[pageIndex].sprite = filledBubbleSprite;
    }
    
    public void PreviousBubblePage(int pageIndex)
    {
        bubblePages[pageIndex - 1].sprite = emptyBubbleSprite;
        bubblePages[pageIndex].sprite = filledBubbleSprite;
    }

    public void LastLandmarkPage()
    {
        nextButton.SetActive(false);
        backButton.SetActive(false);
        landmarkAnswer1Button.SetActive(true);
        landmarkAnswer2Button.SetActive(true);
        landmarkAnswer1Button.GetComponentInChildren<TextMeshProUGUI>().text = QuestionManager.Instance.CurrentLandmarkQuestion.answer1;
        landmarkAnswer2Button.GetComponentInChildren<TextMeshProUGUI>().text = QuestionManager.Instance.CurrentLandmarkQuestion.answer2;
    }
    
    public void EnterLandmark()
    {
        LandmarkQuestion currentLandmarkQuestion = QuestionManager.Instance.CurrentLandmarkQuestion;
        
        SetPanel(PanelState.Landmark);
        landmarkText.text = currentLandmarkQuestion.text1; // Display text
        
        // Set ui bubble pages
        for (int i = 0; i < bubblePages.Count; i++)
        {
            bubblePages[i].gameObject.SetActive(i <= currentLandmarkQuestion.nbTexts);
            bubblePages[i].sprite = emptyBubbleSprite;
        }
        bubblePages[0].sprite = filledBubbleSprite;
    }
    
    public void ExitLandmark()
    {
        nextButton.SetActive(true);
        backButton.SetActive(true);
        landmarkAnswer1Button.SetActive(false);
        landmarkAnswer2Button.SetActive(false);
    }
}

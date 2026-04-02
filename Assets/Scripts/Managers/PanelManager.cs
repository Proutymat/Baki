using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

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
    
    private static PanelManager m_instance;
    
    [Title("Parameters")]
    [SerializeField] private float m_answersAnimDuration;
    [SerializeField] private float m_questionsAnimDuration;

    [SerializeField] private bool m_setInInspector;
    
    [Title("Panels")]
    [SerializeField, ShowIf("m_setInInspector")] private GameObject setupPanel;
    [SerializeField, ShowIf("m_setInInspector")] private GameObject introPanel;
    [SerializeField, ShowIf("m_setInInspector")] private GameObject standardPanel;
    [SerializeField, ShowIf("m_setInInspector")] private GameObject landmarkPanel;
    [SerializeField, ShowIf("m_setInInspector")] private GameObject endPanel;
    
    [Title("Arrow Buttons")]
    [SerializeField, ShowIf("m_setInInspector")] private Sprite arrowUp;
    [SerializeField, ShowIf("m_setInInspector")] private Sprite arrowUpHovered;
    [SerializeField, ShowIf("m_setInInspector")] private Sprite arrowDown;
    [SerializeField, ShowIf("m_setInInspector")] private Sprite arrowDownHovered;
    [SerializeField, ShowIf("m_setInInspector")] private Sprite arrowLeft;
    [SerializeField, ShowIf("m_setInInspector")] private Sprite arrowLeftHovered;
    [SerializeField, ShowIf("m_setInInspector")] private Sprite arrowRight;
    [SerializeField, ShowIf("m_setInInspector")] private Sprite arrowRightHovered;
    [SerializeField, ShowIf("m_setInInspector")] private Image buttonUp;
    [SerializeField, ShowIf("m_setInInspector")] private Image buttonDown;
    [SerializeField, ShowIf("m_setInInspector")] private Image buttonLeft;
    [SerializeField, ShowIf("m_setInInspector")] private Image buttonRight;
    
    [Title("Intro elements")]
    [SerializeField, ShowIf("m_setInInspector")] private Canvas mainCanvas;
    [SerializeField, ShowIf("m_setInInspector")] private Camera canvasCamera;
    [SerializeField, ShowIf("m_setInInspector")] private VideoPlayer videoPlayer;
    [SerializeField, ShowIf("m_setInInspector")] private Button startButton;
    
    [Title("Standard elements")]
    [SerializeField, ShowIf("m_setInInspector")] private UiAnimations uiAnimations;
    [SerializeField, ShowIf("m_setInInspector")] private ProgressBar progressBar;
    [SerializeField, ShowIf("m_setInInspector")] private GameObject directionalArrowsObject;
    [SerializeField, ShowIf("m_setInInspector")] private BeatingArrows directionalArrowsBeating;
    [SerializeField, ShowIf("m_setInInspector")] private Button arrowButtonForeward;
    [SerializeField, ShowIf("m_setInInspector")] private Button arrowButtonLeft;
    [SerializeField, ShowIf("m_setInInspector")] private Button arrowButtonRight;
    [SerializeField, ShowIf("m_setInInspector")] private Button arrowButtonBackward;
    [SerializeField, ShowIf("m_setInInspector")] private GameObject progressBarObject;
    [SerializeField, ShowIf("m_setInInspector")] private GameObject bottomArrowIndication;
    [SerializeField, ShowIf("m_setInInspector")] private GameObject questionsArea;
    [Header("Question zone 1")]
    [SerializeField, ShowIf("m_setInInspector")] private GameObject m_QZ1;
    [SerializeField, ShowIf("m_setInInspector")] private TextMeshProUGUI m_QZ1questionText;
    [SerializeField, ShowIf("m_setInInspector")] private RectTransform m_QZ1AnswersTransform;
    [SerializeField, ShowIf("m_setInInspector")] private TextMeshProUGUI m_QZ1answer1Text;
    [SerializeField, ShowIf("m_setInInspector")] private TextMeshProUGUI m_QZ1answer2Text;
    [SerializeField, ShowIf("m_setInInspector")] private Button m_QZ1answer1Button;
    [SerializeField, ShowIf("m_setInInspector")] private Button m_QZ1answer2Button;
    [Header("Question zone 2")]
    [SerializeField, ShowIf("m_setInInspector")] private GameObject m_QZ2;
    [SerializeField, ShowIf("m_setInInspector")] private TextMeshProUGUI m_QZ2questionText;
    [SerializeField, ShowIf("m_setInInspector")] private RectTransform m_QZ2AnswersTransform;
    [SerializeField, ShowIf("m_setInInspector")] private TextMeshProUGUI m_QZ2answer1Text;
    [SerializeField, ShowIf("m_setInInspector")] private TextMeshProUGUI m_QZ2answer2Text;
    [SerializeField, ShowIf("m_setInInspector")] private Button m_QZ2answer1Button;
    [SerializeField, ShowIf("m_setInInspector")] private Button m_QZ2answer2Button;
    private int m_currentQuestionZone;
    
    
    [Title("Landmark elements")]
    [SerializeField, ShowIf("m_setInInspector")] private List<Image> bubblePages;
    [SerializeField, ShowIf("m_setInInspector")] private Sprite emptyBubbleSprite;
    [SerializeField, ShowIf("m_setInInspector")] private Sprite filledBubbleSprite;
    [SerializeField, ShowIf("m_setInInspector")] private TextMeshProUGUI landmarkText;
    [SerializeField, ShowIf("m_setInInspector")] private GameObject nextButton;
    [SerializeField, ShowIf("m_setInInspector")] private GameObject backButton;
    [SerializeField, ShowIf("m_setInInspector")] private GameObject landmarkAnswer1Button;
    [SerializeField, ShowIf("m_setInInspector")] private GameObject landmarkAnswer2Button;
    
    private PanelState m_currentPanelState;
    
    public UiAnimations UIAnimations { get => uiAnimations; set => uiAnimations = value; }
    public ProgressBar ProgressBar { get => progressBar; set => progressBar = value; }
    public BeatingArrows BeatingArrows { get => directionalArrowsBeating; set => directionalArrowsBeating = value; }
    public GameObject DirectionalArrows { get => directionalArrowsObject; set => directionalArrowsObject = value; }
    public int CurrentQuestionZone { get => m_currentQuestionZone; }

    public void SetQuestionText(string value)
    {
        if (m_currentQuestionZone == 1)
        {
            m_QZ1questionText.text = value;
        }
        else
        {
            m_QZ2questionText.text = value;
        }
    }
    
    public void SetAnswer1Text(string value)
    {
        if (m_currentQuestionZone == 1)
        {
            m_QZ1answer1Text.text = value;
        }
        else
        {
            m_QZ2answer1Text.text = value;
        }
    }
    
    public void SetAnswer2Text(string value)
    {
        if (m_currentQuestionZone == 1)
        {
            m_QZ1answer2Text.text = value;
        }
        else
        {
            m_QZ2answer2Text.text = value;
        }
    }
    
    public string LandmarkText {set => landmarkText.text = value; }

    public void SwitchQuestionZone()
    {
        m_currentQuestionZone = m_currentQuestionZone == 1 ? 2 : 1;
    }

    
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

    public void Initialize()
    {
        uiAnimations.Initialize();
        progressBar.Intitialize();
        directionalArrowsBeating.IsBeating = true;
        m_currentQuestionZone = 1;
    }
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

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
        ProgressBar.IsPaused = !show;
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

    private void EnableAnswersButtons(bool value)
    {
        m_QZ1answer1Button.interactable = value;
        m_QZ1answer2Button.interactable = value;
        m_QZ2answer1Button.interactable = value;
        m_QZ2answer2Button.interactable = value;
    }

    public void QuestionsTransitionAnimation(int answerIndex)
    {
        EnableAnswersButtons(false);

        Sequence anim = DOTween.Sequence();

        
        // Assign question zone
        
        Transform currentZone, nextZone;
        Transform answer1, answer2;
        
        if (m_currentQuestionZone == 1)
        {
            currentZone = m_QZ1.transform;
            nextZone = m_QZ2.transform;
            answer1 = m_QZ1answer1Button.transform;
            answer2 = m_QZ1answer2Button.transform;
            
        }
        else
        {
            currentZone = m_QZ2.transform;
            nextZone = m_QZ1.transform;
            answer1 = m_QZ2answer1Button.transform;
            answer2 = m_QZ2answer2Button.transform;
        }

        
        // Assign selected answer
        
        Transform selected, other;

        if (answerIndex == 1)
        {
            selected = answer1;
            other = answer2;
        }
        else
        {
            selected = answer2;
            other = answer1;
        }

        
        // ANIMATION SEQUENCE
        
        anim.Append(selected.DOLocalMoveX(0, m_answersAnimDuration).SetEase(Ease.InOutQuad)); // Center selected answer
        anim.Join(other.DOLocalMoveY(-200, m_answersAnimDuration).SetEase(Ease.InOutQuad)); // Exit other answer
        anim.Append(currentZone.DOLocalMoveX(750, m_questionsAnimDuration).SetEase(Ease.InOutQuad)); // Exit current zone
        anim.Join(nextZone.DOLocalMoveX(0, m_questionsAnimDuration).SetEase(Ease.InOutQuad)); // Enter next zone
        anim.Append(currentZone.DOLocalMoveX(-750, 0)); // Reset position
        
        anim.OnComplete(() =>
        {
            // Recalculate button positions
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_QZ1AnswersTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_QZ2AnswersTransform);

            EnableAnswersButtons(true); // Reactivate buttons
        });
    }
}

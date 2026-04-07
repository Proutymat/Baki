using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class QuestionManager : SerializedMonoBehaviour
{
    private static QuestionManager m_instance;
    
    [Title("Parameters")]
    [SerializeField] private int m_intervalBetweenTutorials = 3;
    
    [Title("STATIC LISTS (not used in runtime)")]
    [SerializeField] public List<Question> m_tutorials;
    [SerializeField] public List<Question> m_questions;
    [SerializeField] public List<LandmarkQuestion> m_landmarksTypeA;
    [SerializeField] public List<LandmarkQuestion> m_landmarksTypeB;
    [SerializeField] public List<LandmarkQuestion> m_landmarksTypeC;
    [SerializeField] public List<LandmarkQuestion> m_landmarksTypeD;
    
    [Title("Debug"), SerializeField] private bool m_debug;
    [SerializeField, ShowIf("m_debug")] private float m_timer;
    [SerializeField, ShowIf("m_debug")] private bool m_isTutorialQuestion;
    [SerializeField, ShowIf("m_debug")] private int m_landmarkPage;
    [SerializeField, ShowIf("m_debug")] private Question m_currentQuestion;
    [SerializeField, ShowIf("m_debug")] private LandmarkQuestion m_currentLandmarkQuestion;
    [SerializeField, ShowIf("m_debug")] private List<Question> m_runtimeTutorials;
    [SerializeField, ShowIf("m_debug")] private List<Question> m_runtimeQuestions;
    [SerializeField, ShowIf("m_debug")] private List<LandmarkQuestion> m_runtimeLandmarksTypeA;
    [SerializeField, ShowIf("m_debug")] private List<LandmarkQuestion> m_runtimeLandmarksTypeB;
    [SerializeField, ShowIf("m_debug")] private List<LandmarkQuestion> m_runtimeLandmarksTypeC;
    [SerializeField, ShowIf("m_debug")] private List<LandmarkQuestion> m_runtimeLandmarksTypeD;
    [SerializeField, ShowIf("m_debug")] private bool m_tutorialChangeDirectionCheck;
    [SerializeField, ShowIf("m_debug")] private bool m_tutorialStartAfterCheck;
    [SerializeField, ShowIf("m_debug")] private bool m_tutorialChangeDirectionAnytimeCheck;
    
    public float Timer { get => m_timer; set => m_timer = value; }
    public Question CurrentQuestion { get => m_currentQuestion; set => m_currentQuestion = value; }
    public LandmarkQuestion CurrentLandmarkQuestion { get => m_currentLandmarkQuestion; set => m_currentLandmarkQuestion = value; }

    public bool TutorialChangeDirectionCheck { get => m_tutorialChangeDirectionCheck ; set => m_tutorialChangeDirectionCheck = value; }
    public bool TutorialStartAfterCheck { get => m_tutorialStartAfterCheck ; set => m_tutorialStartAfterCheck = value; }
    public bool TutorialChangeDirectionAnytimeCheck { get => m_tutorialChangeDirectionAnytimeCheck ; set => m_tutorialChangeDirectionAnytimeCheck = value; }
    


    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple QuestionManager instances in scene!");
            Destroy(gameObject);
        }
        else
        {
            m_instance = this;
        }
    }
    
    public static QuestionManager Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindFirstObjectByType<QuestionManager>();
            return m_instance;
        }
    }

    public void Initialize()
    {
        m_timer = 0;
        m_landmarkPage = 0;
        m_isTutorialQuestion = false;
        m_runtimeTutorials = new List<Question>(m_tutorials);
        m_runtimeQuestions = new List<Question>(m_questions);
        m_runtimeLandmarksTypeA = new List<LandmarkQuestion>(m_landmarksTypeA);
        m_runtimeLandmarksTypeB = new List<LandmarkQuestion>(m_landmarksTypeB);
        m_runtimeLandmarksTypeC = new List<LandmarkQuestion>(m_landmarksTypeC);
        m_runtimeLandmarksTypeD = new List<LandmarkQuestion>(m_landmarksTypeD);
        m_tutorialChangeDirectionCheck = false;
        m_tutorialStartAfterCheck = false;
        m_tutorialChangeDirectionAnytimeCheck = false;  
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    public void CheckTutorialsQuestion()
    {
        if (m_currentQuestion.flag != TutorialsFlags.None)
        {
            // Add tutorial back if the player has not done it yet
            if (m_currentQuestion.flag == TutorialsFlags.ChangeDirection && !m_tutorialChangeDirectionCheck
                || m_currentQuestion.flag == TutorialsFlags.StartAfter && !m_tutorialStartAfterCheck
                || m_currentQuestion.flag == TutorialsFlags.ChangeDirectionAnytime && !m_tutorialChangeDirectionAnytimeCheck)
            {
                m_runtimeTutorials.Add(m_currentQuestion);
            }
        }
    }
    
    public void NextQuestion()
    {
        if (StatsManager.Instance.NbQuestionsAnswered != 0)
        {
            PanelManager.Instance.SwitchQuestionZone();
        }
        
        // If there are no more questions left
        if (m_runtimeQuestions.Count < 1)
        {
            Debug.Log("No more questions available.");
            PanelManager.Instance.SetQuestionText("No more questions available.");
            PanelManager.Instance.SetAnswer1Text("BAKI");
            PanelManager.Instance.SetAnswer2Text("BAKI");
            return;
        }
        
        // Tutorial questions
        if (m_runtimeTutorials.Count > 0 && StatsManager.Instance.NbQuestionsAnswered > 8 && StatsManager.Instance.NbQuestionsAnswered % m_intervalBetweenTutorials == 0)
        {
            m_currentQuestion = m_runtimeTutorials[0];
            m_runtimeTutorials.RemoveAt(0);
            m_isTutorialQuestion = true;
        }
        // Normal questions
        else
        {
            // Choose a random number between 0 and the number of values
            int questionIndex = Random.Range(0, m_runtimeQuestions.Count);

            // Change and delete the question if both law values are fully checked
            if (m_runtimeQuestions[questionIndex].answer1Type1 >= 0 && CharteManager.Instance.LawCursors[m_runtimeQuestions[questionIndex].answer1Type1].LawsFullyChecked
                                                                    && m_runtimeQuestions[questionIndex].answer2Type1 >=0 && CharteManager.Instance.LawCursors[m_runtimeQuestions[questionIndex].answer2Type1].LawsFullyChecked)
            {
                Debug.Log("Question skipped : " + m_runtimeQuestions[questionIndex].answer1Type1 + "and " + m_runtimeQuestions[questionIndex].answer2Type1 + " are fully checked.");
                m_runtimeQuestions.RemoveAt(questionIndex);
                NextQuestion();
                return;
            }
        
            m_currentQuestion = m_runtimeQuestions[questionIndex];
            m_runtimeQuestions.RemoveAt(questionIndex);
            m_isTutorialQuestion = false;
        }
        
        // Display the question and answers
         PanelManager.Instance.SetQuestionText(m_currentQuestion.question);
         PanelManager.Instance.SetAnswer1Text(m_currentQuestion.answer1);
         PanelManager.Instance.SetAnswer2Text(m_currentQuestion.answer2);
    }
    
    public void NextLandmarkQuestion()
    {
        m_landmarkPage += 1;
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");

        PanelManager.Instance.PreviousBubblePage(m_landmarkPage);
        

        // Last text
        if (m_landmarkPage == m_currentLandmarkQuestion.nbTexts)
        {
            PanelManager.Instance.LastLandmarkPage();
            FMODUnity.RuntimeManager.PlayOneShot("event:/MX/MX_Trig/MX_Trig_Question");
        }

        // Update text
        if (m_landmarkPage == 1)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text2;
        else if (m_landmarkPage == 2)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text3;
        else if (m_landmarkPage == 3)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text4;
        else if (m_landmarkPage == 4)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text5;
        else if (m_landmarkPage == 5)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text6;
    }
    
    public void PreviousLandmarkQuestion()
    {
        // Cannot go back if it's the first question
        if (m_landmarkPage <= 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");
            return;
        }
        
        m_landmarkPage -= 1;
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");
        
        PanelManager.Instance.NextBubblePage(m_landmarkPage);
        
        // Update text
        if (m_landmarkPage == 0)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text1;
        else if (m_landmarkPage == 1)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text2;
        else if (m_landmarkPage == 2)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text3;
        else if (m_landmarkPage == 3)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text4;
        else if (m_landmarkPage == 4)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text5;
    }

    public void EnterLandmark(int landmarkTypeIndex)
    {
        if (landmarkTypeIndex == 0)
        {
            int index = Random.Range(0, m_runtimeLandmarksTypeA.Count);
            m_currentLandmarkQuestion = m_runtimeLandmarksTypeA[index];
            m_runtimeLandmarksTypeA.RemoveAt(index);
        }
        else if (landmarkTypeIndex == 1)
        {
            int index = Random.Range(0, m_runtimeLandmarksTypeB.Count);
            m_currentLandmarkQuestion = m_runtimeLandmarksTypeB[index];
            m_runtimeLandmarksTypeB.RemoveAt(index);
        }
        else if (landmarkTypeIndex == 2)
        {
            int index = Random.Range(0, m_runtimeLandmarksTypeC.Count);
            m_currentLandmarkQuestion = m_runtimeLandmarksTypeC[index];
            m_runtimeLandmarksTypeC.RemoveAt(index);
        }
        else if (landmarkTypeIndex == 3)
        {
            int index = Random.Range(0, m_runtimeLandmarksTypeD.Count);
            m_currentLandmarkQuestion = m_runtimeLandmarksTypeD[index];
            m_runtimeLandmarksTypeD.RemoveAt(index);
        }
        
        Debug.Log("Entering landmark of type: " + landmarkTypeIndex);
    }
    
    public void ExitLandmark()
    {
        m_timer = 0;
        m_landmarkPage = 0;
    }
    
    
#if UNITY_EDITOR
    
    // --------------------------------------------
    //                  IMPORTERS
    // --------------------------------------------

    public void ClearStaticLists()
    {
        m_questions.Clear();
        m_landmarksTypeA.Clear();
        m_landmarksTypeB.Clear();
        m_landmarksTypeC.Clear();
        m_landmarksTypeD.Clear();
    }
    
    [Button, DisableInPlayMode]
    public void LoadTutorials()
    {
        m_tutorials.Clear();
        m_tutorials = FileImporterManager.Instance.LoadTutorialsCSV();
        Debug.Log("Tutorials loaded: " + m_tutorials.Count);
    }
    
    [Button, DisableInPlayMode]
    public void LoadQuestions()
    {
        m_questions.Clear();
        m_questions = FileImporterManager.Instance.LoadQuestionsCSV();
        Debug.Log("Questions loaded: " + m_questions.Count);
    }
    
    [Button, DisableInPlayMode]
    private void LoadLandmarkQuestions()
    {
        m_landmarksTypeA.Clear();
        m_landmarksTypeB.Clear();
        m_landmarksTypeC.Clear();
        m_landmarksTypeD.Clear();
        
        List<List<LandmarkQuestion>> landmarkQuestions = FileImporterManager.Instance.LoadLandmarksCSV();
        m_landmarksTypeA = landmarkQuestions[0];
        m_landmarksTypeB = landmarkQuestions[1];
        m_landmarksTypeC = landmarkQuestions[2];
        m_landmarksTypeD = landmarkQuestions[3];
        
        Debug.Log("Landmark questions loaded: " + (m_landmarksTypeA.Count + m_landmarksTypeB.Count + m_landmarksTypeC.Count + m_landmarksTypeD.Count));
    }
    
#endif
}

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class QuestionManager : SerializedMonoBehaviour
{
    private static QuestionManager m_instance;
    
    [Title("STATIC LISTS (not used in runtime)")]
    [SerializeField] public List<Question> m_tutorials;
    [SerializeField] public List<Question> m_questions;
    [SerializeField] public List<LandmarkQuestion> m_landmarksTypeA;
    [SerializeField] public List<LandmarkQuestion> m_landmarksTypeB;
    [SerializeField] public List<LandmarkQuestion> m_landmarksTypeC;
    [SerializeField] public List<LandmarkQuestion> m_landmarksTypeD;
    
    public List<Question> Tutorials { get => m_tutorials; set => m_tutorials = value; }
    public List<Question> Questions { get => m_questions; set => m_questions = value; }
    public List<LandmarkQuestion> LandmarksTypeA { get => m_landmarksTypeA; set => m_landmarksTypeA = value; }
    public List<LandmarkQuestion> LandmarksTypeB { get => m_landmarksTypeB; set => m_landmarksTypeB = value; }
    public List<LandmarkQuestion> LandmarksTypeC { get => m_landmarksTypeC; set => m_landmarksTypeC = value; }
    public List<LandmarkQuestion> LandmarksTypeD { get => m_landmarksTypeD; set => m_landmarksTypeD = value; }
    
    [Title("Settings")]
    [SerializeField] private int m_intervalBetweenTutorials = 3;
    
    [Title("Debug"), SerializeField] private bool debug;
    [SerializeField, ShowIf("debug")] private Question m_currentQuestion;
    [SerializeField, ShowIf("debug")] private LandmarkQuestion m_currentLandmarkQuestion;
    [SerializeField, ShowIf("debug")] private List<Question> m_runtimeTutorials;
    [SerializeField, ShowIf("debug")] private List<Question> m_runtimeQuestions;
    [SerializeField, ShowIf("debug")] private List<LandmarkQuestion> m_runtimeLandmarksTypeA;
    [SerializeField, ShowIf("debug")] private List<LandmarkQuestion> m_runtimeLandmarksTypeB;
    [SerializeField, ShowIf("debug")] private List<LandmarkQuestion> m_runtimeLandmarksTypeC;
    [SerializeField, ShowIf("debug")] private List<LandmarkQuestion> m_runtimeLandmarksTypeD;
    private float m_timer;
    private bool m_isTutorialQuestion;
    private int m_nbLandmarkQuestions;
    
    public float Timer { get => m_timer; set => m_timer = value; }
    public Question CurrentQuestion { get => m_currentQuestion; set => m_currentQuestion = value; }
    public LandmarkQuestion CurrentLandmarkQuestion { get => m_currentLandmarkQuestion; set => m_currentLandmarkQuestion = value; }
    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    public static QuestionManager Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindFirstObjectByType<QuestionManager>();
            return m_instance;
        }
    }
    
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

    public void Initialize()
    {
        m_runtimeTutorials = new List<Question>(m_tutorials);
        m_runtimeQuestions = new List<Question>(m_questions);
        m_runtimeLandmarksTypeA = new List<LandmarkQuestion>(m_landmarksTypeA);
        m_runtimeLandmarksTypeB = new List<LandmarkQuestion>(m_landmarksTypeB);
        m_runtimeLandmarksTypeC = new List<LandmarkQuestion>(m_landmarksTypeC);
        m_runtimeLandmarksTypeD = new List<LandmarkQuestion>(m_landmarksTypeD);
        m_timer = 0;
        m_nbLandmarkQuestions = 0;
        m_isTutorialQuestion = false;
    }
    
    public void NextQuestion()
    {
        // If there are no more questions left
        if (m_runtimeQuestions.Count < 1)
        {
            Debug.Log("No more questions available.");
            PanelManager.Instance.QuestionText = "No more questions available.";
            PanelManager.Instance.Answer1Text = "BAKI";
            PanelManager.Instance.Answer2Text = "BAKI";
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
            int questionIndex = UnityEngine.Random.Range(0, m_runtimeQuestions.Count);

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
         PanelManager.Instance.QuestionText = m_currentQuestion.question;
         PanelManager.Instance.Answer1Text = m_currentQuestion.answer1;
         PanelManager.Instance.Answer2Text = m_currentQuestion.answer2;
    }
    
    public void NextLandmarkQuestion()
    {
        m_nbLandmarkQuestions += 1;
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");

        PanelManager.Instance.PreviousBubblePage(m_nbLandmarkQuestions);
        

        // Last text
        if (m_nbLandmarkQuestions == m_currentLandmarkQuestion.nbTexts)
        {
            PanelManager.Instance.LastLandmarkPage();
            FMODUnity.RuntimeManager.PlayOneShot("event:/MX/MX_Trig/MX_Trig_Question");
        }

        // Update text
        if (m_nbLandmarkQuestions == 1)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text2;
        else if (m_nbLandmarkQuestions == 2)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text3;
        else if (m_nbLandmarkQuestions == 3)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text4;
        else if (m_nbLandmarkQuestions == 4)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text5;
        else if (m_nbLandmarkQuestions == 5)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text6;
    }
    
    public void PreviousLandmarkQuestion()
    {
        // Cannot go back if it's the first question
        if (m_nbLandmarkQuestions <= 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");
            return;
        }
        
        m_nbLandmarkQuestions -= 1;
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");
        
        PanelManager.Instance.NextBubblePage(m_nbLandmarkQuestions);
        
        // Update text
        if (m_nbLandmarkQuestions == 0)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text1;
        else if (m_nbLandmarkQuestions == 1)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text2;
        else if (m_nbLandmarkQuestions == 2)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text3;
        else if (m_nbLandmarkQuestions == 3)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text4;
        else if (m_nbLandmarkQuestions == 4)
            PanelManager.Instance.LandmarkText = m_currentLandmarkQuestion.text5;
    }

    public void EnterLandmark(int landmarkTypeIndex)
    {
        if (landmarkTypeIndex == 0)
        {
            int index = UnityEngine.Random.Range(0, m_runtimeLandmarksTypeA.Count);
            m_currentLandmarkQuestion = m_runtimeLandmarksTypeA[index];
            m_runtimeLandmarksTypeA.RemoveAt(index);
        }
        else if (landmarkTypeIndex == 1)
        {
            int index = UnityEngine.Random.Range(0, m_runtimeLandmarksTypeB.Count);
            m_currentLandmarkQuestion = m_runtimeLandmarksTypeB[index];
            m_runtimeLandmarksTypeB.RemoveAt(index);
        }
        else if (landmarkTypeIndex == 2)
        {
            int index = UnityEngine.Random.Range(0, m_runtimeLandmarksTypeC.Count);
            m_currentLandmarkQuestion = m_runtimeLandmarksTypeC[index];
            m_runtimeLandmarksTypeC.RemoveAt(index);
        }
        else if (landmarkTypeIndex == 3)
        {
            int index = UnityEngine.Random.Range(0, m_runtimeLandmarksTypeD.Count);
            m_currentLandmarkQuestion = m_runtimeLandmarksTypeD[index];
            m_runtimeLandmarksTypeD.RemoveAt(index);
        }
        
        Debug.Log("Entering landmark of type: " + landmarkTypeIndex);
    }
    
    public void ExitLandmark()
    {
        m_timer = 0;
        m_nbLandmarkQuestions = 0;
    }
    
    
#if UNITY_EDITOR
    
    [Button, DisableInPlayMode]
    public void LoadTutorials()
    {
        m_tutorials = FileImporterManager.Instance.LoadTutorialsCSV();
        Debug.Log("Tutorials loaded: " + m_tutorials.Count);
    }
    
    [Button, DisableInPlayMode]
    public void LoadQuestions()
    {
        m_questions = FileImporterManager.Instance.LoadQuestionsCSV();
        Debug.Log("Questions loaded: " + m_questions.Count);
    }
    
    [Button, DisableInPlayMode]
    private void LoadLandmarkQuestions()
    {
        LandmarksTypeA.Clear();
        LandmarksTypeB.Clear();
        LandmarksTypeC.Clear();
        LandmarksTypeD.Clear();
        
        List<List<LandmarkQuestion>> landmarkQuestions = FileImporterManager.Instance.LoadLandmarksCSV();
        m_landmarksTypeA = landmarkQuestions[0];
        m_landmarksTypeB = landmarkQuestions[1];
        m_landmarksTypeC = landmarkQuestions[2];
        m_landmarksTypeD = landmarkQuestions[3];
        
        Debug.Log("Landmark questions loaded: " + (m_landmarksTypeA.Count + m_landmarksTypeB.Count + m_landmarksTypeC.Count + m_landmarksTypeD.Count));
    }
    
#endif
}

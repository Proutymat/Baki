using UnityEngine;
using Sirenix.OdinInspector;


public class GameManager : SerializedMonoBehaviour
{
    private static GameManager m_instance;
    
    [Title("Parameters")]
    [SerializeField] private float m_gameDuration = 600;
    [SerializeField] private bool m_skipIntro;
    [SerializeField] private bool m_enablePrinters;
    
    [Title("Set in inspector")]
    [SerializeField] private Player player;
    [SerializeField] private GameObject landmarksArrows;
    [SerializeField] private Camera playerCamera;
    
    [Title("Debug"), SerializeField] private bool m_debug;
    [SerializeField, ShowIf("m_debug")]private bool m_onboardingStep1checked;
    [SerializeField, ShowIf("m_debug")]private bool m_onboardingStep2checked;
    [SerializeField, ShowIf("m_debug")] private float m_gameTimer;
    [SerializeField, ShowIf("m_debug")]private bool m_isGameOver;
    [SerializeField, ShowIf("m_debug")]private bool m_inLandmark;
    [SerializeField, ShowIf("m_debug")]private Landmark m_currentLandmark;
    [SerializeField, ShowIf("m_debug")]private string m_currentGameLogFolder;
    [SerializeField, ShowIf("m_debug")]private string m_answersLogFilePath;

    public string CurrentGameLogFolder { get => m_currentGameLogFolder; }
    public bool EnablePrinters { get => m_enablePrinters; }
    public float GameTimer { get => m_gameTimer; }


    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple GameManager instances in scene!");
            Destroy(gameObject);
        }
        else
        {
            m_instance = this;
        }
    }
    
    public static GameManager Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindFirstObjectByType<GameManager>();
            return m_instance;
        }
    }

    private void InitializeGame()
    {
        // Initialize variables
        m_onboardingStep1checked = false;
        m_onboardingStep2checked = false;
        m_gameTimer = m_gameDuration;
        m_isGameOver = false;
        m_inLandmark = false;
        
        // Create log folder
        m_currentGameLogFolder = Application.dataPath + "/Logs";
        if (!System.IO.Directory.Exists(m_currentGameLogFolder))
        {
            System.IO.Directory.CreateDirectory(m_currentGameLogFolder);
        }
        Debug.Log("log folder = " + m_currentGameLogFolder);
        m_currentGameLogFolder += "/" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        System.IO.Directory.CreateDirectory(m_currentGameLogFolder);
        m_answersLogFilePath = $"{m_currentGameLogFolder}/answers.txt";
        
        // Initialize managers
        QuestionManager.Instance.Initialize();
        CharteManager.Instance.Initialize();
        StatsManager.Instance.Initialize();
        PanelManager.Instance.Initialize();
        PrinterManager.Instance.Initialize();
        
        // Set up onboarding
        PanelManager.Instance.ProgressBar.IsPaused = true;
        PanelManager.Instance.ShowDirectionalArrows(false);
        QuestionManager.Instance.NextQuestion();
        
        playerCamera.transform.position = new Vector3(player.transform.position.x, playerCamera.transform.position.y, player.transform.position.z);
        player.Initialize();

        if (m_skipIntro)
        {
            PanelManager.Instance.SetPanel(PanelManager.PanelState.Standard);
        }
        else
        {
            PanelManager.Instance.SetPanel(PanelManager.PanelState.Intro);
        }
    }
    
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        InitializeGame();
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    public void PrintAreaPlayer(bool showAllLandmarkArrows = false)
    {
        playerCamera.transform.position = new Vector3(player.transform.position.x, playerCamera.transform.position.y, player.transform.position.z);
        landmarksArrows.GetComponent<LandmarkDetection>().UpdateLandmarkArrows(showAllLandmarkArrows);
        if (m_enablePrinters) StartCoroutine(PrinterManager.Instance.PNGPrinter.PrintMapTicket());
	}

    public void EnterLandmark(Landmark landmark)
    {
        m_currentLandmark = landmark;
        QuestionManager.Instance.EnterLandmark(m_currentLandmark.Type);
        PanelManager.Instance.EnterLandmark();
        m_inLandmark = true;
        if (m_enablePrinters) PrinterManager.Instance.PNGPrinter.PrintLandmarkTicket(Mathf.FloorToInt(100 * (1 - (m_gameTimer / m_gameDuration))));
    }

    public void ExitLandmark(int buttonIndex)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");
        
        CharteManager.Instance.ExitLandmark(m_currentLandmark.Type);
        
        m_inLandmark = false;
        PanelManager.Instance.SetPanel(PanelManager.PanelState.Standard);
        PanelManager.Instance.ShowQuestionArea(false);
        PanelManager.Instance.UIAnimations.StopShader(0);
        
        // Save answers to file
        if (string.IsNullOrEmpty(m_answersLogFilePath))
        {
            Debug.LogError("Log file path not initialized.");
            return;
        }
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(m_answersLogFilePath, true))
        {
            string answer = "";
            if (buttonIndex == 1)
                answer = QuestionManager.Instance.CurrentLandmarkQuestion.answer1;
            else if (buttonIndex == 2)
                answer = QuestionManager.Instance.CurrentLandmarkQuestion.answer2;
            
            writer.WriteLine("------------------------");
            writer.WriteLine(QuestionManager.Instance.CurrentLandmarkQuestion.text1 + " : " + answer);
            writer.WriteLine("------------------------");
        }
        
        m_currentLandmark.Exit();
        PrintAreaPlayer(true);
    }

    public void UpdateArrowButtonsSprite(string direction)
    {
        PanelManager.Instance.UpdateDirectionalArrow(direction);

        if (!m_onboardingStep2checked)
        {
            m_onboardingStep2checked = true;
            UnboardingStep2();
        }

        PanelManager.Instance.ShowQuestionArea(true);
    }

    private void UnboardingStep1()
    {
        PanelManager.Instance.ShowDirectionalArrows(true);
        PanelManager.Instance.ShowQuestionArea(false);
    }
    
    private void UnboardingStep2()
    {
        PanelManager.Instance.ProgressBar.IsPaused = false;
    }

    private void UpdateDirectionalArrowBeating()
    {
        // Update arrow beating movement
        
    }

    private void EndGame()
    {
        m_isGameOver = true;
        player.SetIsMoving(false);
        StatsManager.Instance.WriteFinalStatsToFile();
        PanelManager.Instance.SetPanel(PanelManager.PanelState.End);
        FMODUnity.RuntimeManager.PlayOneShot("event:/StopAll");
        FMODUnity.RuntimeManager.PlayOneShot("event:/END");
        CharteManager.Instance.EndGame();
    }

    private void Update()
    {
        UpdateDirectionalArrowBeating();
        
        if (m_isGameOver || !m_onboardingStep2checked || m_inLandmark) return;
        
        // Update game time
        m_gameTimer -= Time.deltaTime;
        
        // End game logic
        if (m_gameTimer <= 0)
        {
            EndGame();
        }
        
        // Update charte
        int percentElapsed = Mathf.FloorToInt(100 * (1 - (m_gameTimer / m_gameDuration)));
        CharteManager.Instance.UpdatePercentage(percentElapsed);
        
        // Update stats
        if (player.IsMoving)
            StatsManager.Instance.TimeSpentMoving += Time.deltaTime;
        QuestionManager.Instance.Timer += Time.deltaTime;
    }

    public void AnsweringQuestion(int answerIndex)
    {
        if (answerIndex == 1) FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");
        if (answerIndex == 2) FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick_NAN");
        
        // Save answers to file
        if (string.IsNullOrEmpty(m_answersLogFilePath))
        {
            Debug.LogError("Log file path not initialized.");
            return;
        }
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(m_answersLogFilePath, true))
        {
            //Debug.Log("Writing to file : " + answersLogFilePath);
            writer.WriteLine(QuestionManager.Instance.CurrentQuestion.question + " : " + (answerIndex == 1 ? QuestionManager.Instance.CurrentQuestion.answer1 : QuestionManager.Instance.CurrentQuestion.answer2));
        }

        StatsManager statsManager = StatsManager.Instance;
        
        // UPDATE STATS
        statsManager.NbQuestionsAnswered++;
        if (answerIndex == 1)
            statsManager.NbLeftAnswers++;
        else
            statsManager.NbRightAnswers++;

        // Update time between questions (if more than 5 questions answered)
        if (statsManager.NbQuestionsAnswered > 5)
        {
            if (QuestionManager.Instance.Timer < statsManager.ShortestTimeBetweenQuestions)
                statsManager.ShortestTimeBetweenQuestions = QuestionManager.Instance.Timer;
            if (QuestionManager.Instance.Timer > statsManager.LongestTimeBetweenQuestions)
                statsManager.LongestTimeBetweenQuestions = QuestionManager.Instance.Timer;
        }
        statsManager.TimeBetweenQuestions += QuestionManager.Instance.Timer;
        QuestionManager.Instance.Timer = 0;
        
        
        // Progress bar is full
        if (PanelManager.Instance.ProgressBar.IncreaseProgressBar())
        {
            statsManager.NbProgressBarFull++;
            player.SetIsMoving(false);
            PanelManager.Instance.ShowQuestionArea(false);
            PanelManager.Instance.UIAnimations.StopShader(1.5f);

            
            // Active all children of the landmark arrows
            foreach (Transform child in landmarksArrows.transform)
            {
                child.gameObject.SetActive(true);
            }
            
            if (!m_onboardingStep1checked)
            {
                m_onboardingStep1checked = true;
                UnboardingStep1();
            }

		PrintAreaPlayer();
        }

        // The two laws to update with their increments
        int law1Type, law2Type;
        int lawIncrement1, lawIncrement2;
        
        // Left button
        if (answerIndex == 1)
        {
            law1Type = QuestionManager.Instance.CurrentQuestion.answer1Type1;
            law2Type = QuestionManager.Instance.CurrentQuestion.answer1Type2;
            lawIncrement1 = QuestionManager.Instance.CurrentQuestion.answer1Type1ADD;
            lawIncrement2 = QuestionManager.Instance.CurrentQuestion.answer1Type2ADD;
            if (QuestionManager.Instance.CurrentQuestion.answer1Illustration != "")
            {
                CharteManager.Instance.AddIllustrationToQueue(QuestionManager.Instance.CurrentQuestion.answer1Illustration, QuestionManager.Instance.CurrentQuestion.answer1IllustrationPriority);
            }
        }
        // Right button
        else
        {
            law1Type = QuestionManager.Instance.CurrentQuestion.answer2Type1;
            law2Type = QuestionManager.Instance.CurrentQuestion.answer2Type2;
            lawIncrement1 = QuestionManager.Instance.CurrentQuestion.answer2Type1ADD;
            lawIncrement2 = QuestionManager.Instance.CurrentQuestion.answer2Type2ADD;
            if (QuestionManager.Instance.CurrentQuestion.answer2Illustration != "")
            {
                CharteManager.Instance.AddIllustrationToQueue(QuestionManager.Instance.CurrentQuestion.answer2Illustration, QuestionManager.Instance.CurrentQuestion.answer2IllustrationPriority);
            }
        }
        
        // Update laws if it has a type (!= sansCategorie)
        if (law1Type >= 0)
        {
            CharteManager.Instance.IncrementLawCursor(law1Type, lawIncrement1);
        }
        if (law2Type >= 0)
        {
            CharteManager.Instance.IncrementLawCursor(law2Type, lawIncrement2);
        }

        QuestionManager.Instance.NextQuestion();
    }  
}
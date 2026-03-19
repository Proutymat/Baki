using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;


public class GameManager : SerializedMonoBehaviour
{
    // ------------------------------------
    //             ATTRIBUTES
    // ------------------------------------
    
    [SerializeField] private bool setObjectsInInspector = false;
    
    private float beatingArrowTimer;
    private float beatingValue = 1;
    
    [Header("----- MAP PRINTER -----"), ShowIf("setObjectsInInspector")]
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject landmarksArrows;
    [SerializeField, ShowIf("setObjectsInInspector")] private Camera playerCamera;
    
    [Header("------- INSTANCES -------")]
    private static GameManager _instance;
    [SerializeField, ShowIf("setObjectsInInspector")] private Player player;
    
    
    [Title("Game Settings")]
    [SerializeField] private float gameDuration = 600;
    
    

    private bool unboardingStep1 = false;
    private bool unboardingStep2 = false;
    
    [Header("DEBUGS")]
    [SerializeField] private bool debug = false;
    [SerializeField] private bool skipIntro = false;
    [SerializeField] private bool enablePrinters = true;
    
    [Header("WORKING VALUES (used in runtime)")]
    [SerializeField, ShowIf("debug")] private float gameTimer;
    
    private string currentGameLogFolder;
    
    private string answersLogFilePath;
    private bool isGameOver = false;
    private bool inLandmark;
    private Landmark currentLandmark;

    public string CurrentGameLogFolder { get { return currentGameLogFolder; } }
    public bool EnablePrinters { get => enablePrinters; }
    public float GameTimer { get => gameTimer; }


    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<GameManager>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple GameManager instances in scene!");
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void InitializeGame()
    {
        // Initialize managers
        StatsManager.Instance.Initialize();
        QuestionManager.Instance.Initialize();
        
        CharteManager.Instance.Initialize();

        beatingValue = 1;
        beatingArrowTimer = 0;
        
        // Unboarding
        PanelManager.Instance.ProgressBar.IsPaused = true;
        PanelManager.Instance.ShowDirectionalArrows(false);
        
        // Initialize game settings
        gameTimer = gameDuration;
        isGameOver = false;
        
        
        
        
        
        QuestionManager.Instance.NextQuestion();
        playerCamera.transform.position = new Vector3(player.transform.position.x, playerCamera.transform.position.y, player.transform.position.z);

        
        // DEBUG : Create log folder
        currentGameLogFolder = Application.dataPath + "/Logs";
        if (!System.IO.Directory.Exists(currentGameLogFolder))
        {
            System.IO.Directory.CreateDirectory(currentGameLogFolder);
        }
        Debug.Log("log folder = " + currentGameLogFolder);
        currentGameLogFolder += "/" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        System.IO.Directory.CreateDirectory(currentGameLogFolder);
        
        answersLogFilePath = $"{currentGameLogFolder}/answers.txt";
        
        // Initialize instances 
        player.Initialize();
        PanelManager.Instance.UIAnimations.Initialize();

        if (skipIntro)
        {
            PanelManager.Instance.SetPanel(PanelManager.PanelState.Standard);
        }
        else
        {
            PanelManager.Instance.SetPanel(PanelManager.PanelState.Intro);
        }
        
        PrinterManager.Instance.Initialize();
    }
    
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        InitializeGame();
    }
    
    
    // --------------------------------------------
    //                  QUESTIONS
    // --------------------------------------------

    public void PrintAreaPlayer(bool showAllLandmarkArrows = false)
    {
        playerCamera.transform.position = new Vector3(player.transform.position.x, playerCamera.transform.position.y, player.transform.position.z);
        landmarksArrows.GetComponent<LandmarkDetection>().UpdateLandmarkArrows(showAllLandmarkArrows);
        if (enablePrinters) StartCoroutine(PrinterManager.Instance.PNGPrinter.PrintMapTicket());
	}

    public void EnterLandmark(Landmark landmark)
    {
        currentLandmark = landmark;
        QuestionManager.Instance.EnterLandmark(currentLandmark.Type);
        PanelManager.Instance.EnterLandmark();
        inLandmark = true;
        if (enablePrinters) PrinterManager.Instance.PNGPrinter.PrintLandmarkTicket(Mathf.FloorToInt(100 * (1 - (gameTimer / gameDuration))));
    }

    public void ExitLandmark(int buttonIndex)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");
        
        CharteManager.Instance.ExitLandmark(currentLandmark.Type);
        
        inLandmark = false;
        PanelManager.Instance.SetPanel(PanelManager.PanelState.Standard);
        PanelManager.Instance.ShowQuestionArea(false);
        PanelManager.Instance.UIAnimations.StopShader(0);
        
        // Save answers to file
        if (string.IsNullOrEmpty(answersLogFilePath))
        {
            Debug.LogError("Log file path not initialized.");
            return;
        }
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(answersLogFilePath, true))
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
        
        currentLandmark.Exit();
        PrintAreaPlayer(true);
    }

    public void UpdateArrowButtonsSprite(string direction)
    {
        PanelManager.Instance.UpdateDirectionalArrow(direction);

        if (!unboardingStep2)
        {
            unboardingStep2 = true;
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
        if (player.IsMoving)
        {
            DOTween.To(() => beatingValue, x => {
                beatingValue = x;
                PanelManager.Instance.DirectionalArrows.transform.localScale = new Vector3(beatingValue, beatingValue, 1f);
            }, 1f, 0.5f);
            
        }
        else
        {
            beatingArrowTimer += Time.deltaTime;
            if (beatingArrowTimer >= 0.5f)
            {
                if (beatingValue >= 1)
                {
                    DOTween.To(() => beatingValue, x => {
                        beatingValue = x;
                        PanelManager.Instance.DirectionalArrows.transform.localScale = new Vector3(beatingValue, beatingValue, 1f);
                    }, 0.9f, 1f);
                }
                else
                {
                    DOTween.To(() => beatingValue, x => {
                        beatingValue = x;
                        PanelManager.Instance.DirectionalArrows.transform.localScale = new Vector3(beatingValue, beatingValue, 1f);
                    }, 1.02f, 1f);
                }
                beatingArrowTimer = 0f;
            }
        }
    }

    private void EndGame()
    {
        isGameOver = true;
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
        
        if (isGameOver || !unboardingStep2 || inLandmark) return;
        
        // Update game time
        gameTimer -= Time.deltaTime;
        
        // End game logic
        if (gameTimer <= 0)
        {
            EndGame();
        }
        
        // Update charte
        int percentElapsed = Mathf.FloorToInt(100 * (1 - (gameTimer / gameDuration)));
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
        if (string.IsNullOrEmpty(answersLogFilePath))
        {
            Debug.LogError("Log file path not initialized.");
            return;
        }
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(answersLogFilePath, true))
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
            
            if (!unboardingStep1)
            {
                unboardingStep1 = true;
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
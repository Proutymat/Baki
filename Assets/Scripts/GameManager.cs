using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using UnityEngine.Video;


public class GameManager : SerializedMonoBehaviour
{
    // ------------------------------------
    //             ATTRIBUTES
    // ------------------------------------
    
    [SerializeField] private bool setObjectsInInspector = false;
    [Header("------- MATERIALS -------"), ShowIf("setObjectsInInspector")] 
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialGround;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialWall;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialStart;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialLandmarksA;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialLandmarksB;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialLandmarksC;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialLandmarksD;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialLandmarksE;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialSpecialZoneIn;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialSpecialZoneOut;

    [Header("----- UI ARROW BUTTONS ----- "), ShowIf("setObjectsInInspector")]
    [SerializeField, ShowIf("setObjectsInInspector")] private Sprite arrowUp;
    [SerializeField, ShowIf("setObjectsInInspector")] private Sprite arrowUpHovered;
    [SerializeField, ShowIf("setObjectsInInspector")] private Sprite arrowDown;
    [SerializeField, ShowIf("setObjectsInInspector")] private Sprite arrowDownHovered;
    [SerializeField, ShowIf("setObjectsInInspector")] private Sprite arrowLeft;
    [SerializeField, ShowIf("setObjectsInInspector")] private Sprite arrowLeftHovered;
    [SerializeField, ShowIf("setObjectsInInspector")] private Sprite arrowRight;
    [SerializeField, ShowIf("setObjectsInInspector")] private Sprite arrowRightHovered;
    [SerializeField, ShowIf("setObjectsInInspector")] private Image buttonUp;
    [SerializeField, ShowIf("setObjectsInInspector")] private Image buttonDown;
    [SerializeField, ShowIf("setObjectsInInspector")] private Image buttonLeft;
    [SerializeField, ShowIf("setObjectsInInspector")] private Image buttonRight;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject buttonsArrowsObject;

    [Header("----- UI INTERFACE -----"), ShowIf("setObjectsInInspector")]
    [SerializeField, ShowIf("setObjectsInInspector")] private Canvas mainCanvas;
    [SerializeField, ShowIf("setObjectsInInspector")] private Camera canvasCamera;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject endingCanvas;
    [SerializeField, ShowIf("setObjectsInInspector")] private VideoPlayer videoPlayer;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject introInterface;
    
    
    [Header("--- UI INTERFACE QUESTION ---"), ShowIf("setObjectsInInspector")]
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject questionsInterface;
    [SerializeField, ShowIf("setObjectsInInspector")] private Button arrowButtonForeward;
    [SerializeField, ShowIf("setObjectsInInspector")] private Button arrowButtonLeft;
    [SerializeField, ShowIf("setObjectsInInspector")] private Button arrowButtonRight;
    [SerializeField, ShowIf("setObjectsInInspector")] private Button arrowButtonBackward;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject questionsArea;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject progressBarObject;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject bottomArrowIndication;
    [SerializeField, ShowIf("setObjectsInInspector")] private TextMeshProUGUI questionText;
    [SerializeField, ShowIf("setObjectsInInspector")] private TextMeshProUGUI answer1Text;
    [SerializeField, ShowIf("setObjectsInInspector")] private TextMeshProUGUI answer2Text;
    private float beatingArrowTimer;
    private float beatingValue = 1;
    
    [Header("--- UI INTERFACE LANDMARK ---"), ShowIf("setObjectsInInspector")]
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject landmarksInterface;
    [SerializeField, ShowIf("setObjectsInInspector")] private List<Image> bubblePages;
    [SerializeField, ShowIf("setObjectsInInspector")] private Sprite emptyBubbleSprite;
    [SerializeField, ShowIf("setObjectsInInspector")] private Sprite filledBubbleSprite;
    [SerializeField, ShowIf("setObjectsInInspector")] private TextMeshProUGUI landmarkText;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject nextButton;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject landmarkAnswer1Button;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject landmarkAnswer2Button;
    
    [Header("----- MAP PRINTER -----"), ShowIf("setObjectsInInspector")]
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject landmarksArrows;
    [SerializeField, ShowIf("setObjectsInInspector")] private Camera playerCamera;
    
    [Header("------- INSTANCES -------")]
    private static GameManager _instance;
    [SerializeField, ShowIf("setObjectsInInspector")] private Player player;
    [SerializeField, ShowIf("setObjectsInInspector")] private ProgressBar progressBar;
    [SerializeField, ShowIf("setObjectsInInspector")] private PDFPrinter pdfPrinter;
    [SerializeField, ShowIf("setObjectsInInspector")] private UiAnimations uiAnimations;
    
    [Header("GAME SETTINGS")]
    [SerializeField] private float gameDuration = 600;
    [SerializeField] private int printIntervalsInPercent;
    [SerializeField] private int maxLawsInterval = 5;
    [SerializeField] private int intervalBetweenTutorials = 3;

    private bool unboardingStep1 = false;
    private bool unboardingStep2 = false;
    
    [Header("DEBUGS")]
    [SerializeField] private bool debug = false;
    [SerializeField] private bool skipIntro = false;
    
    [Header("STATIC LISTS (not used in runtime)")]
    [SerializeField, ShowIf("debug")] private List<Question> tutorials;
    [SerializeField, ShowIf("debug")] private List<Question> questions;
    [SerializeField, ShowIf("debug")] private List<LandmarkQuestion> landmarksTypeA;
    [SerializeField, ShowIf("debug")] private List<LandmarkQuestion> landmarksTypeB;
    [SerializeField, ShowIf("debug")] private List<LandmarkQuestion> landmarksTypeC;
    [SerializeField, ShowIf("debug")] private List<LandmarkQuestion> landmarksTypeD;
    [SerializeField, ShowIf("debug")] private List<Value> laws;
    
    [Header("WORKING VALUES (used in runtime)")]
    [SerializeField, ShowIf("debug")] private List<Question> runtimeTutorials;
    [SerializeField, ShowIf("debug")] private List<Question> runtimeQuestions;
    [SerializeField, ShowIf("debug")] private List<LandmarkQuestion> runtimeLandmarksTypeA;
    [SerializeField, ShowIf("debug")] private List<LandmarkQuestion> runtimeLandmarksTypeB;
    [SerializeField, ShowIf("debug")] private List<LandmarkQuestion> runtimeLandmarksTypeC;
    [SerializeField, ShowIf("debug")] private List<LandmarkQuestion> runtimeLandmarksTypeD;
    [SerializeField, ShowIf("debug")] private Question currentQuestion;
    [SerializeField, ShowIf("debug")] private LandmarkQuestion currentLandmarkQuestion;
    [SerializeField, ShowIf("debug")] private List<LawCursor> lawCursors;
    [SerializeField, ShowIf("debug")] private List<string> lawsQueue;
    [SerializeField, ShowIf("debug")] private List<int> lawsQueuePriority;
    [SerializeField, ShowIf("debug")] private float gameTimer;
    [SerializeField, ShowIf("debug")] private int lastPrintedPercent = 0;
    private string logFolderPath;
    private string charteLogFilePath;
    private string answersLogFilePath;
    private bool isGameOver = false;
    private bool inLandmark;
    private bool isTutorialQuestion;
    private int nbLandmarkQuestions;
    private Landmark currentLandmark;
    
    
    // ----------- GAME STATS -------------
    
    // Stats
    private int nbUnitTraveled;
    private int nbLandmarksReached;
    private int nbWallsHit;
    private int nbDirectionChanges;
    private int nbButtonsPressed;
    private float timeSpentMoving;
    private int nbQuestionsAnswered;
    private int nbLeftAnswers;
    private int nbRightAnswers;
    private float timeBetweenQuestions;
    private float shortestTimeBetweenQuestions;
    private float longestTimeBetweenQuestions;
    private int nbProgressBarFull;
    
    // Working values
    private float questionTimer;

    public int DistanceTraveled { get { return nbUnitTraveled; } set { nbUnitTraveled = value; } }
    public int LandmarksReached { get { return nbLandmarksReached; } set { nbLandmarksReached = value; } }
    public int WallsHit { get { return nbWallsHit; } set { nbWallsHit = value; } }
    public int DirectionChanges { get { return nbDirectionChanges; } set { nbDirectionChanges = value; } }
    public int ButtonsPressed { get { return nbButtonsPressed; } set { nbButtonsPressed = value; } }
    public string LogFolderPath { get { return logFolderPath; } }


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
        // Initialize game stats
        nbUnitTraveled = 0;
        nbLandmarksReached = 0;
        nbWallsHit = 0;
        nbDirectionChanges = 0;
        nbButtonsPressed = 0;
        timeSpentMoving = 0;
        nbQuestionsAnswered = 0;
        nbLeftAnswers = 0;
        nbRightAnswers = 0;
        timeBetweenQuestions = 0;
        nbProgressBarFull = 0;
        shortestTimeBetweenQuestions = float.MaxValue;
        longestTimeBetweenQuestions = float.MinValue;
        nbLandmarkQuestions = 0;

        beatingValue = 1;
        beatingArrowTimer = 0;
        questionTimer = 0;
        
        // Unboarding
        progressBar.IsPaused = true;
        buttonsArrowsObject.SetActive(false);
        
        // Initialize game settings
        gameTimer = gameDuration;
        lawsQueue = new List<string>();
        lastPrintedPercent = 0;
        isGameOver = false;
        
        // Load working lists
        runtimeTutorials = new List<Question>(tutorials);
        runtimeQuestions = new List<Question>(questions);
        runtimeLandmarksTypeA = new List<LandmarkQuestion>(landmarksTypeA);
        runtimeLandmarksTypeB = new List<LandmarkQuestion>(landmarksTypeB);
        runtimeLandmarksTypeC = new List<LandmarkQuestion>(landmarksTypeC);
        runtimeLandmarksTypeD = new List<LandmarkQuestion>(landmarksTypeD);

        isTutorialQuestion = false;
        
        // Create law cursors
        lawCursors.Clear();
        lawCursors = new List<LawCursor>();
        for (int i = 0; i < laws.Count; i++)
        {
            LawCursor lawCursor = new LawCursor(laws[i]);
            lawCursors.Add(lawCursor);
        }
        
        Debug.Log("Law cursors created : " + lawCursors.Count);
        
        NextQuestion();
        playerCamera.transform.position = new Vector3(player.transform.position.x, playerCamera.transform.position.y, player.transform.position.z);

        
        // DEBUG : Create log folder
        logFolderPath = Application.dataPath + "/Logs";
        if (!System.IO.Directory.Exists(logFolderPath))
        {
            System.IO.Directory.CreateDirectory(logFolderPath);
        }

        logFolderPath += "/" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        System.IO.Directory.CreateDirectory(logFolderPath);
        charteLogFilePath = $"{logFolderPath}/charte.txt";
        answersLogFilePath = $"{logFolderPath}/answers.txt";
        
        // Initialize instances 
        player.Initialize();
        pdfPrinter.Initialize();
        uiAnimations.Initialize();

        if (skipIntro)
        {
            introInterface.SetActive(false);
            questionsInterface.SetActive(true);
            landmarksInterface.SetActive(false); 
        }
        else
        {
            introInterface.SetActive(true);
            questionsInterface.SetActive(false);
            landmarksInterface.SetActive(false);
        }
    }
    
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        runtimeQuestions = new List<Question>();
        lawCursors = new List<LawCursor>();
        InitializeGame();
    }
    
    private void OnVideoPrepared(VideoPlayer vp)
    {
        videoPlayer.Play();
        Invoke(nameof(DisableIntroInterface), 0.2f);    
    }
    
    private void OnVideoEnd(VideoPlayer vp)
    {
        videoPlayer.gameObject.SetActive(false);
        questionsInterface.SetActive(true);
    }
    
    void DisableIntroInterface()
    {
        introInterface.SetActive(false);
    }
    
    public void LaunchGame()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();
    }
    
    
    // --------------------------------------------
    //                  QUESTIONS
    // --------------------------------------------

    public void PrintAreaPlayer()
    {
        playerCamera.transform.position = new Vector3(player.transform.position.x, playerCamera.transform.position.y, player.transform.position.z);
    	StartCoroutine(pdfPrinter.Print());
	}

    public void NextLandmarkQuestion()
    {
        nbLandmarkQuestions += 1;
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");
        
        // Update bubble pages sprite
        bubblePages[nbLandmarkQuestions - 1].sprite = emptyBubbleSprite;
        bubblePages[nbLandmarkQuestions].sprite = filledBubbleSprite;

        // Last text
        if (nbLandmarkQuestions == currentLandmarkQuestion.nbTexts)
        {
            nextButton.SetActive(false);
            landmarkAnswer1Button.SetActive(true);
            landmarkAnswer2Button.SetActive(true);
            landmarkAnswer1Button.GetComponentInChildren<TextMeshProUGUI>().text = currentLandmarkQuestion.answer1;
            landmarkAnswer2Button.GetComponentInChildren<TextMeshProUGUI>().text = currentLandmarkQuestion.answer2;
        }

        if (nbLandmarkQuestions == 1)
            landmarkText.text = currentLandmarkQuestion.text2;
        else if (nbLandmarkQuestions == 2)
            landmarkText.text = currentLandmarkQuestion.text3;
        else if (nbLandmarkQuestions == 3)
            landmarkText.text = currentLandmarkQuestion.text4;
        else if (nbLandmarkQuestions == 4)
            landmarkText.text = currentLandmarkQuestion.text5;
        else if (nbLandmarkQuestions == 5)
                landmarkText.text = currentLandmarkQuestion.text6;
    }

    public void EnterLandmark(Landmark landmark)
    {
        currentLandmark = landmark;
        
        int landmarkIndex = currentLandmark.Type;
        Debug.Log("Entering landmark of type: " + landmarkIndex);
        if (landmarkIndex == 0)
        {
            int index = UnityEngine.Random.Range(0, runtimeLandmarksTypeA.Count);
            currentLandmarkQuestion = runtimeLandmarksTypeA[index];
            runtimeLandmarksTypeA.RemoveAt(index);
        }
        else if (landmarkIndex == 1)
        {
            int index = UnityEngine.Random.Range(0, runtimeLandmarksTypeB.Count);
            currentLandmarkQuestion = runtimeLandmarksTypeB[index];
            runtimeLandmarksTypeB.RemoveAt(index);
        }
        else if (landmarkIndex == 2)
        {
            int index = UnityEngine.Random.Range(0, runtimeLandmarksTypeC.Count);
            currentLandmarkQuestion = runtimeLandmarksTypeC[index];
            runtimeLandmarksTypeC.RemoveAt(index);
        }
        else if (landmarkIndex == 3)
        {
            int index = UnityEngine.Random.Range(0, runtimeLandmarksTypeD.Count);
            currentLandmarkQuestion = runtimeLandmarksTypeD[index];
            runtimeLandmarksTypeD.RemoveAt(index);
        }
        
        inLandmark = true;
        
        // Change interface
        questionsInterface.SetActive(false);
        landmarksInterface.SetActive(true);
        
        landmarkText.text = currentLandmarkQuestion.text1; // Display text

        // Set ui bubble pages
        for (int i = 0; i < bubblePages.Count; i++)
        {
            bubblePages[i].gameObject.SetActive(i <= currentLandmarkQuestion.nbTexts);
            bubblePages[i].sprite = emptyBubbleSprite;
        }
        bubblePages[0].sprite = filledBubbleSprite;
        
        pdfPrinter.PrintLandmarkPDF(Mathf.FloorToInt(100 * (1 - (gameTimer / gameDuration))));
    }

    public void ExitLandmark(int buttonIndex)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");
        
        nextButton.SetActive(true);
        landmarkAnswer1Button.SetActive(false);
        landmarkAnswer2Button.SetActive(false);
        
        inLandmark = false;
        questionTimer = 0;
        nbLandmarkQuestions = 0;
        questionsInterface.SetActive(true);
        questionsArea.SetActive(false);
        landmarksInterface.SetActive(false);
        uiAnimations.StopShader(0);
        
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
                answer = currentLandmarkQuestion.answer1;
            else if (buttonIndex == 2)
                answer = currentLandmarkQuestion.answer2;
            
            writer.WriteLine("------------------------");
            writer.WriteLine(currentLandmarkQuestion.text1 + " : " + answer);
            writer.WriteLine("------------------------");
        }
        
        PrintAreaPlayer();
        currentLandmark.Exit();
    }
    
    private void WriteFinalStatsToFile()
    {
        if (string.IsNullOrEmpty(charteLogFilePath))
        {
            Debug.LogError("Log file path not initialized.");
            return;
        }

        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(charteLogFilePath, true))
        {
            writer.WriteLine("======== FIN DE PARTIE ========");
            writer.WriteLine($"Temps total : {gameDuration:F2} secondes");
            writer.WriteLine($"Temps passé en mouvement : {timeSpentMoving:F2} secondes");
            writer.WriteLine($"Unités parcourues : {nbUnitTraveled}");
            writer.WriteLine($"Points de repère atteints : {nbLandmarksReached}");
            writer.WriteLine($"Murs percutés : {nbWallsHit}");
            writer.WriteLine($"Changements de direction : {nbDirectionChanges}");
            writer.WriteLine($"Boutons pressés : {nbButtonsPressed}");
            writer.WriteLine($"Questions répondues : {nbQuestionsAnswered}");
            writer.WriteLine($"Réponses gauche : {nbLeftAnswers}");
            writer.WriteLine($"Réponses droite : {nbRightAnswers}");
            writer.WriteLine($"Temps moyen entre les questions : {timeBetweenQuestions/nbQuestionsAnswered:F2} secondes");
            writer.WriteLine($"Temps le plus court entre deux questions : {shortestTimeBetweenQuestions:F2} secondes");
            writer.WriteLine($"Temps le plus long entre deux questions : {longestTimeBetweenQuestions:F2} secondes");
            writer.WriteLine($"Barre de progression pleine : {nbProgressBarFull}");
            writer.WriteLine("================================");
            writer.WriteLine();
        }
    }

    public void UpdateArrowButtonsSprite(string direction)
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

        if (!unboardingStep2)
        {
            unboardingStep2 = true;
            UnboardingStep2();
        }
        
        ShowHideQuestionArea(true);
    }
    
    private void ShowHideQuestionArea(bool show)
    {
        questionsArea.SetActive(show);
        progressBarObject.SetActive(show);

    }

    private void UnboardingStep1()
    {
        buttonsArrowsObject.SetActive(true);
        ShowHideQuestionArea(false);
    }
    
    private void UnboardingStep2()
    {
        progressBar.IsPaused = false;
    }
    
    private void Update()
    {
        if (isGameOver || !unboardingStep2 || inLandmark) return;
        
        // Update game time
        gameTimer -= Time.deltaTime;
        
        // Update fresque
        int percentElapsed = Mathf.FloorToInt(100 * (1 - (gameTimer / gameDuration)));
        if (percentElapsed >= lastPrintedPercent + printIntervalsInPercent)
        {
            lastPrintedPercent += printIntervalsInPercent;
            PrintLawsQueue();
            
            Debug.Log("" + lastPrintedPercent + "% of the game elapsed");
        }
        
        // Update arrow beating movement
        if (player.IsMoving)
        {
            DOTween.To(() => beatingValue, x => {
                beatingValue = x;
                buttonsArrowsObject.transform.localScale = new Vector3(beatingValue, beatingValue, 1f);
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
                        buttonsArrowsObject.transform.localScale = new Vector3(beatingValue, beatingValue, 1f);
                    }, 0.9f, 1f);
                }
                else
                {
                    DOTween.To(() => beatingValue, x => {
                        beatingValue = x;
                        buttonsArrowsObject.transform.localScale = new Vector3(beatingValue, beatingValue, 1f);
                    }, 1.02f, 1f);
                }
                beatingArrowTimer = 0f;
            }
        }
        
        // End game logic
        if (gameTimer <= 0)
        {
            player.SetIsMoving(false);
            WriteFinalStatsToFile();
            isGameOver = true;
            endingCanvas.SetActive(true);
            mainCanvas.gameObject.SetActive(false);
        }
        
        // Update stats
        if (player.IsMoving)
            timeSpentMoving += Time.deltaTime;
        questionTimer += Time.deltaTime;
    }
    
    

    private void PrintLawsQueue()
    {
        if (string.IsNullOrEmpty(charteLogFilePath))
        {
            Debug.LogError("Log file path not initialized.");
            return;
        }


        // Append to file (true = append mode)
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(charteLogFilePath, true))
        {
            writer.WriteLine("------------------------");
            writer.WriteLine($"   SALVE DE LOIS " + lastPrintedPercent + "%");
            writer.WriteLine("------------------------");
            
            int lawPrinted = 0;

            if (lawsQueuePriority.Any())
            {
                int priorityIndex = lawsQueuePriority.Max();
            
                while (lawsQueue.Count > 0 && lawPrinted < maxLawsInterval && priorityIndex > 0)
                {
                    // Get the index of the first law with the highest priority
                    int index = lawsQueuePriority.IndexOf(priorityIndex);
                    writer.WriteLine(lawsQueue[index]);
                    lawsQueue.RemoveAt(index);
                    lawsQueuePriority.RemoveAt(index);
                    lawPrinted++;
                
                    // Update the priority index
                    if (lawsQueuePriority.Count > 0)
                        priorityIndex = lawsQueuePriority.Max();
                }
            
                writer.WriteLine();
            }
        }
    }

    public void AnsweringQuestion(int answerIndex)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");
        
        // Save answers to file
        if (string.IsNullOrEmpty(answersLogFilePath))
        {
            Debug.LogError("Log file path not initialized.");
            return;
        }
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(answersLogFilePath, true))
        {
            //Debug.Log("Writing to file : " + answersLogFilePath);
            writer.WriteLine(currentQuestion.question + " : " + (answerIndex == 1 ? currentQuestion.answer1 : currentQuestion.answer2));
        }
        
        // UPDATE STATS
        nbQuestionsAnswered++;
        if (answerIndex == 1)
            nbLeftAnswers++;
        else
            nbRightAnswers++;

        // Update time between questions (if more than 5 questions answered)
        if (nbQuestionsAnswered > 5)
        {
            if (questionTimer < shortestTimeBetweenQuestions)
                shortestTimeBetweenQuestions = questionTimer;
            if (questionTimer > longestTimeBetweenQuestions)
                longestTimeBetweenQuestions = questionTimer;
        }
        timeBetweenQuestions += questionTimer;
        questionTimer = 0;
        
        
        // Progress bar is full
        if (progressBar.IncreaseProgressBar())
        {
            nbProgressBarFull++;
            player.SetIsMoving(false);
            ShowHideQuestionArea(false);
            uiAnimations.StopShader(1.5f);

            
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
            law1Type = currentQuestion.answer1Type1;
            law2Type = currentQuestion.answer1Type2;
            lawIncrement1 = currentQuestion.answer1Type1ADD;
            lawIncrement2 = currentQuestion.answer1Type2ADD;
        }
        // Right button
        else
        {
            law1Type = currentQuestion.answer2Type1;
            law2Type = currentQuestion.answer2Type2;
            lawIncrement1 = currentQuestion.answer2Type1ADD;
            lawIncrement2 = currentQuestion.answer2Type2ADD;
        }
        
        // Update laws if it has a type (!= sansCategorie)
        if (law1Type >= 0)
        {
            (string, int) resultLaw1 = lawCursors[law1Type].IncrementLawCursorValue(lawIncrement1);
            if (resultLaw1.Item1 != "")
            {
                lawsQueue.Add(resultLaw1.Item1);
                lawsQueuePriority.Add(resultLaw1.Item2);
                Debug.Log("New law : " + resultLaw1);
            }
        }
        if (law2Type >= 0)
        {
            (string, int) resultLaw2 = lawCursors[law2Type].IncrementLawCursorValue(lawIncrement2);
            if (resultLaw2.Item1 != "")
            {
                lawsQueue.Add(resultLaw2.Item1);
                lawsQueuePriority.Add(resultLaw2.Item2);
                Debug.Log("New law : " + resultLaw2);
            }
        }

        NextQuestion();
    }

    private void NextQuestion()
    {
        // If there are no more questions left
        if (runtimeQuestions.Count < 1)
        {
            Debug.Log("No more questions available.");
            questionText.text = "No more questions available.";
            answer1Text.text = "BAKI";
            answer2Text.text = "BAKI";
            return;
        }
        
        // Tutorial questions
        if (runtimeTutorials.Count > 0 && nbQuestionsAnswered > 8 && nbQuestionsAnswered % intervalBetweenTutorials == 0)
        {
            currentQuestion = runtimeTutorials[0];
            runtimeTutorials.RemoveAt(0);
            isTutorialQuestion = true;
        }
        // Normal questions
        else
        {
            // Choose a random number between 0 and the number of values
            int questionIndex = UnityEngine.Random.Range(0, runtimeQuestions.Count);

            // Change and delete the question if both law values are fully checked
            if (runtimeQuestions[questionIndex].answer1Type1 >= 0 && lawCursors[runtimeQuestions[questionIndex].answer1Type1].LawsFullyChecked
                                                                  && runtimeQuestions[questionIndex].answer2Type1 >=0 && lawCursors[runtimeQuestions[questionIndex].answer2Type1].LawsFullyChecked)
            {
                Debug.Log("Question skipped : " + runtimeQuestions[questionIndex].answer1Type1 + "and " + runtimeQuestions[questionIndex].answer2Type1 + " are fully checked.");
                runtimeQuestions.RemoveAt(questionIndex);
                NextQuestion();
                return;
            }
        
            currentQuestion = runtimeQuestions[questionIndex];
            runtimeQuestions.RemoveAt(questionIndex);
            isTutorialQuestion = false;
        }
        
        // Display the question and answers
        questionText.text = currentQuestion.question;
        answer1Text.text = currentQuestion.answer1;
        answer2Text.text = currentQuestion.answer2;
    }

#if UNITY_EDITOR
    
    [Header("CSV INFOS")]
    [SerializeField] private string tutorialsFileName;
    [SerializeField] private string questionsFileName;
    [SerializeField] private string landmarkQuestionsFileName;
    [SerializeField] private string lawsFileName;
    
    [Button, DisableInPlayMode]
    private void LoadTutorials()
    {
        tutorials.Clear();
        tutorials = LoadCSV.LoadTutorialsCSV(tutorialsFileName);
        Debug.Log("Tutorials loaded : " + tutorials.Count);
    }
    
    [Button, DisableInPlayMode]
    private void LoadLandmarkQuestions()
    {
        landmarksTypeA.Clear();
        landmarksTypeB.Clear();
        landmarksTypeC.Clear();
        landmarksTypeD.Clear();
        
        List<List<LandmarkQuestion>> landmarkQuestions = LoadCSV.LoadLandmarksCSV(landmarkQuestionsFileName);
        
        landmarksTypeA = landmarkQuestions[0];
        landmarksTypeB = landmarkQuestions[1];
        landmarksTypeC = landmarkQuestions[2];
        landmarksTypeD = landmarkQuestions[3];
        
        Debug.Log("Landmark questions loaded : " + landmarkQuestions.Count);
    }
    
    [Button, DisableInPlayMode]
    private void LoadLaws()
    {
        laws.Clear();
        laws = LoadCSV.LoadLawsCSV(lawsFileName);
        Debug.Log("Laws loaded : " + laws.Count);
    }
    
    [Button, DisableInPlayMode]
    private void LoadQuestions()
    {
        questions.Clear();
        questions = LoadCSV.LoadQuestionsCSV(questionsFileName, laws);
    }
    
    [Button, DisableInPlayMode]
    private void ClearAllScriptables()
    {
        landmarksTypeA.Clear();
        landmarksTypeB.Clear();
        landmarksTypeC.Clear();
        landmarksTypeD.Clear();
        questions.Clear();
        laws.Clear();
        LoadCSV.ClearScriptables();
        Debug.Log("All scriptables cleared.");
    }
#endif    
    
}
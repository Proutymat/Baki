using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using TMPro;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;


public class GameManager : SerializedMonoBehaviour
{
    [SerializeField] private bool setObjectsInInspector = false;
    [Header("Materials"), ShowIf("setObjectsInInspector")] 
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialGround;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialWall;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialStart;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialLandmarksA;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialLandmarksB;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialLandmarksC;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialLandmarksD;
    [SerializeField, ShowIf("setObjectsInInspector")] public Material materialLandmarksE;
    [Header("UI text"), ShowIf("setObjectsInInspector")]
    [SerializeField, ShowIf("setObjectsInInspector")] private TextMeshProUGUI questionText;
    [SerializeField, ShowIf("setObjectsInInspector")] private TextMeshProUGUI answer1Text;
    [SerializeField, ShowIf("setObjectsInInspector")] private TextMeshProUGUI answer2Text;
    [SerializeField, ShowIf("setObjectsInInspector")] private TextMeshProUGUI dilemmeText;
    [SerializeField, ShowIf("setObjectsInInspector")] private TextMeshProUGUI answer1DilemmeText;
    [SerializeField, ShowIf("setObjectsInInspector")] private TextMeshProUGUI answer2DilemmeText;
    [SerializeField, ShowIf("setObjectsInInspector")] private TextMeshProUGUI answer3DilemmeText;
    [SerializeField, ShowIf("setObjectsInInspector")] private TextMeshProUGUI answer4DilemmeText;
    [Header("UI arrows"), ShowIf("setObjectsInInspector")]
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
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject bottomArrowIndication;

    [Header("Others"), ShowIf("setObjectsInInspector")] 
    [SerializeField, ShowIf("setObjectsInInspector")] private Canvas mainCanvas;
    [SerializeField, ShowIf("setObjectsInInspector")] private Camera canvasCamera;
    [SerializeField, ShowIf("setObjectsInInspector")] private Camera playerCamera;
    [SerializeField, ShowIf("setObjectsInInspector")] private Canvas blackScreenCanvas;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject arrows;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject questionsArea;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject progressBarObject;
    [SerializeField, ShowIf("setObjectsInInspector")] private Image uiBackground;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject endingCanvas;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject directionnalArrows;
    [SerializeField, ShowIf("setObjectsInInspector")] private UiAnimations uiAnimations;

    
    [Header("PROTOTYPE THINGS")]
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject normalBackground;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject dilemmeBackground;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject deco;

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 600;
    [SerializeField] private int printIntervalsInPercent;
    [SerializeField] private int maxLawsInterval = 5;
    [SerializeField] private string questionsFileName;
    [SerializeField] private string lawsFileName;
    [SerializeField] private string dilemmeFileName;
    
    
    private static GameManager _instance;
    [SerializeField] private Player player;
    private ProgressBar progressBar;

    private bool unboardingStep1 = false;
    private bool unboardingStep2 = false;
    

    [SerializeField] private bool debug = false;
    
    [Header("Static lists (not used in game)")]
    [SerializeField, ShowIf("debug")] private List<Question> questions;
    [SerializeField, ShowIf("debug")] private List<Value> laws;
    [SerializeField, ShowIf("debug")] private List<Dilemme> dilemmes;
    
    [Header("Working values")]
    [SerializeField, ShowIf("debug")] private List<Question> runtimeQuestions;
    [SerializeField, ShowIf("debug")] private Question currentQuestion;
    [SerializeField, ShowIf("debug")] private List<LawCursor> lawCursors;
    [SerializeField, ShowIf("debug")] private List<string> lawsQueue;
    [SerializeField, ShowIf("debug")] private List<int> lawsQueuePriority;
    [SerializeField, ShowIf("debug")] private float gameTimer;
    [SerializeField, ShowIf("debug")] private int lastPrintedPercent = 0;
    private string fresqueLogFilePath; // DEBUG FRESQUE : Path to the log folder
    private string answersLogFilePath; // DEBUG QUESTIONS : Path to the log folder
    private bool isGameOver = false;
    private bool inLandmark;
    private Dilemme currentDilemme;
    
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

    private float questionTimer;

    public int DistanceTraveled
    {
        get { return nbUnitTraveled; }
        set { nbUnitTraveled = value; }
    }

    public int LandmarksReached
    {
        get { return nbLandmarksReached; }
        set { nbLandmarksReached = value; }
    }

    public int WallsHit
    {
        get { return nbWallsHit; }
        set { nbWallsHit = value; }
    }

    public int DirectionChanges
    {
        get { return nbDirectionChanges; }
        set { nbDirectionChanges = value; }
    }

    public int ButtonsPressed
    {
        get { return nbButtonsPressed; }
        set { nbButtonsPressed = value; }
    }


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
        
        // Activate the second display if available
        if(Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
            Debug.Log("Display 2 activated");
        }
        else
        {
            Debug.Log("Display 2 not found");
        }
    }

    private void InitializeGame()
    {
        player.Initialize();
        
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
 
        questionTimer = 0;
        
        // Unboarding
        progressBar.IsPaused = true;
        arrows.SetActive(false);
        
        // Initialize game settings
        gameTimer = gameDuration;
        lawsQueue = new List<string>();
        lastPrintedPercent = 0;
        isGameOver = false;
        
        // Load questions in working list
        runtimeQuestions = new List<Question>(questions);
        
        lawCursors.Clear();
        
        // Create law cursors
        lawCursors = new List<LawCursor>();
        for (int i = 0; i < laws.Count; i++)
        {
            LawCursor lawCursor = new LawCursor(laws[i]);
            lawCursors.Add(lawCursor);
        }
        
        Debug.Log("Law cursors created : " + lawCursors.Count);
        
        NextQuestion();
        PrintAreaPlayer();
        
        // DEBUG FRESQUE : Create log file
        string folderPath = Application.dataPath + "/Logs";
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        folderPath += "/" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        System.IO.Directory.CreateDirectory(folderPath);
        fresqueLogFilePath = $"{folderPath}/fresque.txt";
        answersLogFilePath = $"{folderPath}/answers.txt";
    }
    
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        progressBar = FindFirstObjectByType<ProgressBar>();
        runtimeQuestions = new List<Question>();
        lawCursors = new List<LawCursor>();
        InitializeGame();
    }

    [Button("Update Cam")]
    public void PrintAreaPlayer()
    {
        playerCamera.transform.position = new Vector3(player.transform.position.x, playerCamera.transform.position.y, player.transform.position.z);
    }

    public void EnterLandmark()
    {
        // FMOD : stop ambient music
        FMODUnity.RuntimeManager.PlayOneShot("event:/MX/MX_Interest_Point1");
        
        if (dilemmes.Count < 1)
        {
            Debug.Log("No more landmark questions available.");
            dilemmeText.text = "No more landmark questions available.";
            answer1DilemmeText.text = "BAKI";
            answer2DilemmeText.text = "BAKI";
            answer3DilemmeText.text = "BAKI";
            answer4DilemmeText.text = "BAKI";
            return;
        }
        
        // Chose a random dilemme
        int index = UnityEngine.Random.Range(0, dilemmes.Count);
        currentDilemme = dilemmes[index];
        dilemmes.RemoveAt(index);
        
        inLandmark = true;
        progressBar.gameObject.SetActive(false);
        questionsArea.SetActive(false);
        arrows.SetActive(false);
        deco.SetActive(false);
        normalBackground.SetActive(false);
        dilemmeBackground.SetActive(true);
        dilemmeText.transform.parent.transform.parent.gameObject.SetActive(true);
        
        // Display the question and answers
        dilemmeText.text = currentDilemme.question;
        answer1DilemmeText.text = currentDilemme.answer1;
        answer2DilemmeText.text = currentDilemme.answer2;
        answer3DilemmeText.text = currentDilemme.answer3;
        answer4DilemmeText.text = currentDilemme.answer4;
    }

    public void ExitLandmark(int answerIndex)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");
        // FMOD : stop landmark music
        
        inLandmark = false;
        progressBar.gameObject.SetActive(true);
        questionsArea.SetActive(true);
        arrows.SetActive(true);
        normalBackground.SetActive(true);
        deco.SetActive(true);
        questionTimer = 0;
        
        dilemmeBackground.SetActive(false);
        dilemmeText.transform.parent.transform.parent.gameObject.SetActive(false);
        
        // Save answers to file
        if (string.IsNullOrEmpty(answersLogFilePath))
        {
            Debug.LogError("Log file path not initialized.");
            return;
        }
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(answersLogFilePath, true))
        {
            string answer = "";
            if (answerIndex == 1)
                answer = currentDilemme.answer1;
            else if (answerIndex == 2)
                answer = currentDilemme.answer2;
            else if (answerIndex == 3)
                answer = currentDilemme.answer3;
            else if (answerIndex == 4)
                answer = currentDilemme.answer4;
            
            writer.WriteLine("------------------------");
            writer.WriteLine(currentDilemme.question + " : " + answer);
            writer.WriteLine("------------------------");
        }
    }
    
    private void WriteFinalStatsToFile()
    {
        if (string.IsNullOrEmpty(fresqueLogFilePath))
        {
            Debug.LogError("Log file path not initialized.");
            return;
        }

        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fresqueLogFilePath, true))
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
            buttonUp.sprite = arrowUpHovered;
            buttonDown.sprite = arrowDown;
            buttonLeft.sprite = arrowLeft;
            buttonRight.sprite = arrowRight;
            
            bottomArrowIndication.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == "backward")
        {
            buttonUp.sprite = arrowUp;
            buttonDown.sprite = arrowDownHovered;
            buttonLeft.sprite = arrowLeft;
            buttonRight.sprite = arrowRight;
            
            bottomArrowIndication.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (direction == "left")
        {
            buttonUp.sprite = arrowUp;
            buttonDown.sprite = arrowDown;
            buttonLeft.sprite = arrowLeftHovered;
            buttonRight.sprite = arrowRight;
            
            bottomArrowIndication.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direction == "right")
        {
            buttonUp.sprite = arrowUp;
            buttonDown.sprite = arrowDown;
            buttonLeft.sprite = arrowLeft;
            buttonRight.sprite = arrowRightHovered;
            
            bottomArrowIndication.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            buttonUp.sprite = arrowUp;
            buttonDown.sprite = arrowDown;
            buttonLeft.sprite = arrowLeft;
            buttonRight.sprite = arrowRight;
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
        if (show)
        {
            questionsArea.SetActive(true);
            progressBarObject.SetActive(true);
        }
        else
        {
            questionsArea.SetActive(false);
            progressBarObject.SetActive(false);
        }
    }

    private void UnboardingStep1()
    {
        arrows.SetActive(true);
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
        if (string.IsNullOrEmpty(fresqueLogFilePath))
        {
            Debug.LogError("Log file path not initialized.");
            return;
        }


        // Append to file (true = append mode)
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fresqueLogFilePath, true))
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
            PrintAreaPlayer();
            player.EnableMeshRenderer(true);
            ShowHideQuestionArea(false);
            uiAnimations.StopShader(1.5f);

            
            // Active all children of the directional arrows
            foreach (Transform child in directionnalArrows.transform)
            {
                child.gameObject.SetActive(true);
            }
            
            if (!unboardingStep1)
            {
                unboardingStep1 = true;
                UnboardingStep1();
            }
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
        
        // Display the question and answers
        questionText.text = currentQuestion.question;
        answer1Text.text = currentQuestion.answer1;
        answer2Text.text = currentQuestion.answer2;
        
        // Remove the question from the list
        runtimeQuestions.RemoveAt(questionIndex);
    }

#if UNITY_EDITOR
    [Button, DisableInPlayMode]
    private void LoadDilemme()
    {
        dilemmes.Clear();
        dilemmes = LoadCSV.LoadDilemmeCSV(dilemmeFileName);
        Debug.Log("Dilemmes loaded : " + dilemmes.Count);
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
        dilemmes.Clear();
        questions.Clear();
        laws.Clear();
        LoadCSV.ClearScriptables();
        Debug.Log("All scriptables cleared.");
    }
#endif    
    
}
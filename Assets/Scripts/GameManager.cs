using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Sirenix.Serialization;
using TMPro;
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

    [Header("Others"), ShowIf("setObjectsInInspector")] 
    [SerializeField, ShowIf("setObjectsInInspector")] private Camera canvasCamera;
    [SerializeField, ShowIf("setObjectsInInspector")] private Camera playerCamera;
    [SerializeField, ShowIf("setObjectsInInspector")] private Canvas blackScreenCanvas;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject arrows;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject questionsArea;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject progressBarObject;


    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 600;
    [SerializeField] private int printIntervalsInPercent;
    [SerializeField] private string questionsFileName;
    [SerializeField] private string valuesFileName;
    
    
    private static GameManager _instance;
    private Player player;
    private ProgressBar progressBar;

    private bool unboardingStep1 = false;
    private bool unboardingStep2 = false;
    

    [SerializeField] private bool debug = false;
    
    [Header("Static lists (not used in game)")]
    [SerializeField, ShowIf("debug")] private List<Question> _questionsNoCategory;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsMort;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsProgres;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsSpriritualite;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsTravail;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsNature;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsMorale;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsJustice;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsArt;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsTraditions;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsEgo;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsBonheur;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsMerite;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsEducation;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsChancesVsResultats;
    [SerializeField, ShowIf("debug")] private List<Value> _values;
    
    [Header("Working values")]
    [SerializeField, ShowIf("debug")] private List<List<Question>> _questions;
    [SerializeField, ShowIf("debug")] private Question currentQuestion;
    [SerializeField, ShowIf("debug")] private List<LawCursor> _lawCursors;
    [SerializeField, ShowIf("debug")] private List<string> lawsQueue;
    [SerializeField, ShowIf("debug")] private float gameTimer;
    [SerializeField, ShowIf("debug")] private int lastPrintedPercent = 0;
    private string logFilePath; // DEBUG FRESQUE : Path to the log folder
    private bool isGameOver = false;
    
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
        
        // Load questions in working lists
        _questions = new List<List<Question>>();
        _questions.Add(new List<Question>(_questionsNoCategory));
        _questions.Add(new List<Question>(_questionsMort));
        _questions.Add(new List<Question>(_questionsProgres));
        _questions.Add(new List<Question>(_questionsSpriritualite));
        _questions.Add(new List<Question>(_questionsTravail));
        _questions.Add(new List<Question>(_questionsNature));
        _questions.Add(new List<Question>(_questionsMorale));
        _questions.Add(new List<Question>(_questionsJustice));
        _questions.Add(new List<Question>(_questionsArt));
        _questions.Add(new List<Question>(_questionsTraditions));
        _questions.Add(new List<Question>(_questionsEgo));
        _questions.Add(new List<Question>(_questionsBonheur));
        _questions.Add(new List<Question>(_questionsMerite));
        _questions.Add(new List<Question>(_questionsEducation));
        _questions.Add(new List<Question>(_questionsChancesVsResultats));
        
        _lawCursors.Clear();
        
        // Create law cursors
        _lawCursors = new List<LawCursor>();
        for (int i = 0; i < _values.Count; i++)
        {
            LawCursor lawCursor = new LawCursor(_values[i]);
            _lawCursors.Add(lawCursor);
        }
        
        NextQuestion();
        PrintAreaPlayer();
        
        // DEBUG FRESQUE : Create log file
        string folderPath = Application.dataPath + "/FresquesDebug";
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        logFilePath = $"{folderPath}/fresque{timestamp}.txt";
    }
    
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        progressBar = FindFirstObjectByType<ProgressBar>();
        _questions = new List<List<Question>>();
        _lawCursors = new List<LawCursor>();
        InitializeGame();
    }

    public void PrintAreaPlayer()
    {
        playerCamera.transform.position = new Vector3(player.transform.position.x, playerCamera.transform.position.y, player.transform.position.z);
    }
    
    private void WriteFinalStatsToFile()
    {
        if (string.IsNullOrEmpty(logFilePath))
        {
            Debug.LogError("Log file path not initialized.");
            return;
        }

        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFilePath, true))
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
        }
        else if (direction == "backward")
        {
            buttonUp.sprite = arrowUp;
            buttonDown.sprite = arrowDownHovered;
            buttonLeft.sprite = arrowLeft;
            buttonRight.sprite = arrowRight;
        }
        else if (direction == "left")
        {
            buttonUp.sprite = arrowUp;
            buttonDown.sprite = arrowDown;
            buttonLeft.sprite = arrowLeftHovered;
            buttonRight.sprite = arrowRight;
        }
        else if (direction == "right")
        {
            buttonUp.sprite = arrowUp;
            buttonDown.sprite = arrowDown;
            buttonLeft.sprite = arrowLeft;
            buttonRight.sprite = arrowRightHovered;
        }
        
        if (!unboardingStep2)
        {
            unboardingStep2 = true;
            UnboardingStep2();
        }
    }

    private void UnboardingStep1()
    {
        arrows.SetActive(true);
        questionsArea.SetActive(false);
        blackScreenCanvas.gameObject.SetActive(false);
    }
    
    private void UnboardingStep2()
    {
        questionsArea.SetActive(true);
        progressBarObject.SetActive(true);
        progressBar.IsPaused = false;
    }

    private void Update()
    {
        // Update game time
        gameTimer -= Time.deltaTime;
        if (gameTimer <= 0)
        {
            // End game logic
            if (!isGameOver)
            {
                WriteFinalStatsToFile();
                isGameOver = true;
                Debug.Log("Game Over");
            }
            
        }
        
        // Update fresque
        int percentElapsed = Mathf.FloorToInt(100 * (1 - (gameTimer / gameDuration)));
        if (percentElapsed >= lastPrintedPercent + printIntervalsInPercent)
        {
            lastPrintedPercent += printIntervalsInPercent;
            PrintLawsQueue();
            
            Debug.Log("" + lastPrintedPercent + "% of the game elapsed");
        }
        
        
        // Update stats
        if (player.IsMoving)
            timeSpentMoving += Time.deltaTime;
        questionTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R) && Input.GetKeyDown(KeyCode.E) && Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.T)) 
        {
            InitializeGame();
        }
    }

    private void PrintLawsQueue()
    {
        if (string.IsNullOrEmpty(logFilePath))
        {
            Debug.LogError("Log file path not initialized.");
            return;
        }


        // Append to file (true = append mode)
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFilePath, true))
        {
            writer.WriteLine("------------------------");
            writer.WriteLine($"   SALVE DE LOIS " + lastPrintedPercent);
            writer.WriteLine("------------------------");
            foreach (var law in lawsQueue)
            {
                writer.WriteLine(law);
            }
            writer.WriteLine();
        }
        
        lawsQueue.Clear();
    }

    public void AnsweringQuestion(int answerIndex)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");
        
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
            
            if (!unboardingStep1)
            {
                unboardingStep1 = true;
                UnboardingStep1();
            }
        }
        
        
        // If the question has no category, skip
        if (currentQuestion.type != 0)
        {
            int lawIncrement = answerIndex == 1 ? currentQuestion.answer1Increment : currentQuestion.answer2Increment;
            string result = _lawCursors[currentQuestion.type - 1].IncrementLawCursorValue(lawIncrement);
            if (result != "")
            {
                lawsQueue.Add(result);
                Debug.Log("New law : " + result);
            }
        }

        NextQuestion();
    }

    private void NextQuestion()
    {
        if (_questions.Count < 1)
        {
            Debug.Log("No more questions available.");
            questionText.text = "No more questions available.";
            answer1Text.text = "BAKI";
            answer2Text.text = "BAKI";
            return;
        }
        
        
        // Choose a random number between 0 and the number of values
        int valueIndex = UnityEngine.Random.Range(0, _questions.Count);
        int questionIndex = UnityEngine.Random.Range(0, _questions[valueIndex].Count);

        currentQuestion = _questions[valueIndex][questionIndex];
        
        // Display the question and answers
        questionText.text = currentQuestion.question;
        answer1Text.text = currentQuestion.answer1;
        answer2Text.text = currentQuestion.answer2;
        
        // Remove the question from the list
        _questions[valueIndex].RemoveAt(questionIndex);
        if (_questions[valueIndex].Count == 0)
            _questions.RemoveAt(valueIndex);
    }
    
    
    
    // ---------------------------------
    //      DEBUGGING AND TESTING
    // ---------------------------------
    
#if UNITY_EDITOR

    private void ClearLawsFolder()
    {
        _values.Clear();
        
        string folderPath = "Assets/Resources/Scriptables/Laws";
        // If the folder doesn't exist, create it
        if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Scriptables", "Laws");
        }
        // Delete all existing Laws assets in the folder
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Laws", new[] { folderPath });
        foreach (string guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            UnityEditor.AssetDatabase.DeleteAsset(assetPath);
        }
    }
    
    private void LoadValuesCSV()
    {
        ClearLawsFolder();
        
        // Check if the file exists
        TextAsset csvFile = Resources.Load<TextAsset>(valuesFileName);
        if (csvFile == null)
        {
            Debug.LogError($"CSV file '{valuesFileName}.csv' not found in Resources folder.");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);


        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] fields = line.Split(';');

            if (fields.Length < 8)
            {
                Debug.LogWarning("Malformed line skipped: " + line);
                continue;
            }

            Value scriptable = ScriptableObject.CreateInstance<Value>();
            scriptable.valueName = fields[0];
            scriptable.law1 = fields[1];
            scriptable.law2 = fields[2];
            scriptable.law3 = fields[3];
            scriptable.law4 = fields[4];
            scriptable.law5 = fields[5];
            scriptable.law6 = fields[6];
            scriptable.law7 = fields[7];


            // Sauvegarde du ScriptableObject dans le projet (dans un dossier "Assets/Resources/Questions")
            string assetPath = $"Assets/Resources/Scriptables/Laws/law_" + i + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(scriptable, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();

            _values.Add(scriptable);
            
            Debug.Log("Values imported successfully!");
        }
    }
    
    private void ClearQuestionsFolder()
    {
        _questionsMort.Clear();
        _questionsProgres.Clear();
        _questionsSpriritualite.Clear();
        _questionsTravail.Clear();
        _questionsNature.Clear();
        _questionsMorale.Clear();
        _questionsJustice.Clear();
        _questionsArt.Clear();
        _questionsTraditions.Clear();
        _questionsEgo.Clear();
        _questionsBonheur.Clear();
        _questionsMerite.Clear();
        _questionsEducation.Clear();
        _questionsChancesVsResultats.Clear();
        _questionsNoCategory.Clear();
        
        string folderPath = "Assets/Resources/Scriptables/Questions";
        // If the folder doesn't exist, create it
        if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Scriptables", "Questions");
        }
        // Delete all existing Question assets in the folder
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Question", new[] { folderPath });
        foreach (string guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            UnityEditor.AssetDatabase.DeleteAsset(assetPath);
        }
    }
    
    private void LoadQuestionsCSV()
    {
        ClearQuestionsFolder();
        
        // Check if the file exists
        TextAsset csvFile = Resources.Load<TextAsset>(questionsFileName);
        if (csvFile == null)
        {
            Debug.LogError($"CSV file '{questionsFileName}.csv' not found in Resources folder.");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] fields = line.Split(';');

            if (fields.Length < 6)
            {
                Debug.LogWarning("Malformed line skipped: " + line);
                continue;
            }

            Question scriptable = ScriptableObject.CreateInstance<Question>();
            scriptable.question = fields[0];
            scriptable.answer1 = fields[2];
            scriptable.answer1Increment = int.Parse(fields[3]);
            scriptable.answer2 = fields[4];
            scriptable.answer2Increment = int.Parse(fields[5]);
            

            // Sauvegarde du ScriptableObject dans le projet (dans un dossier "Assets/Resources/Questions")
            string assetPath = $"Assets/Resources/Scriptables/Questions/question_" + i + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(scriptable, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            
            // Assigning to value type
            if (fields[1] == "sansCategorie")
            {
                scriptable.type = 0;
                _questionsNoCategory.Add(scriptable);
            }
            else if (fields[1] == "mort")
            {
                scriptable.type = 1;
                _questionsMort.Add(scriptable);
            }
            else if (fields[1] == "progres")
            {
                scriptable.type = 2;
                _questionsProgres.Add(scriptable);
            }
            else if (fields[1] == "spiritualite")
            {
                scriptable.type = 3;
                _questionsSpriritualite.Add(scriptable);
            }
            else if (fields[1] == "travail")
            {
                scriptable.type = 4;
                _questionsTravail.Add(scriptable);
            }
            else if (fields[1] == "nature")
            {
                scriptable.type = 5;
                _questionsNature.Add(scriptable);
            }
            else if (fields[1] == "morale")
            {
                scriptable.type = 6;
                _questionsMorale.Add(scriptable);
            }
            else if (fields[1] == "justice")
            {
                scriptable.type = 7;
                _questionsJustice.Add(scriptable);
            }
            else if (fields[1] == "art")
            {
                scriptable.type = 8;
                _questionsJustice.Add(scriptable);
            }
            else if (fields[1] == "traditions")
            {
                scriptable.type = 9;
                _questionsTraditions.Add(scriptable);
            }
            else if (fields[1] == "ego")
            {
                scriptable.type = 10;
                _questionsEgo.Add(scriptable);
            }
            else if (fields[1] == "bonheur")
            {
                scriptable.type = 11;
                _questionsBonheur.Add(scriptable);
            }
            else if (fields[1] == "merite")
            {
                scriptable.type = 12;
                _questionsMerite.Add(scriptable);
            }
            else if (fields[1] == "education")
            {
                scriptable.type = 13;
                _questionsEducation.Add(scriptable);
            }
            else if (fields[1] == "chancesVsResultats")
            {
                scriptable.type = 14;
                _questionsChancesVsResultats.Add(scriptable);
            }
            else
            {
                Debug.LogWarning("Unknown question type: " + fields[1]);
            }
        }

        Debug.Log("Questions imported successfully!");
    }

    [DisableInPlayMode]
    [Button]
    private void ClearScriptables()
    {
        ClearQuestionsFolder();
        ClearLawsFolder();
    }

    [DisableInPlayMode][Button]
    private void LoadCSV()
    {
        LoadValuesCSV();
        LoadQuestionsCSV();
    }
#endif
}

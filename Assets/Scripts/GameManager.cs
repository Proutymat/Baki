using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Sirenix.Serialization;
using TMPro;


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


    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 600;
    [SerializeField] private int printIntervalsInPercent;
    [SerializeField] private string questionsFileName;
    [SerializeField] private string valuesFileName;
    
    
    private static GameManager _instance;
    private Player player;
    private ProgressBar progressBar;

    [SerializeField] private bool debug = false;
    
    [Header("Static lists (not used in game)")]
    [SerializeField, ShowIf("debug")] private List<Question> _questionsNoCategory;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsValue1;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsValue2;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsValue3;
    [SerializeField, ShowIf("debug")] private List<Question> _questionsValue4;
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
        
        // Initialize game settings
        gameTimer = gameDuration;
        lawsQueue = new List<string>();
        lastPrintedPercent = 0;
        isGameOver = false;
        
        // Load questions in working lists
        _questions = new List<List<Question>>();
        _questions.Add(new List<Question>(_questionsNoCategory));
        _questions.Add(new List<Question>(_questionsValue1));
        _questions.Add(new List<Question>(_questionsValue2));
        _questions.Add(new List<Question>(_questionsValue3));
        _questions.Add(new List<Question>(_questionsValue4));
        
        _lawCursors.Clear();
        
        // Create law cursors
        _lawCursors = new List<LawCursor>();
        for (int i = 0; i < _values.Count; i++)
        {
            LawCursor lawCursor = new LawCursor(_values[i]);
            _lawCursors.Add(lawCursor);
        }
        
        NextQuestion();
        
        
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

    private void Play()
    {
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Play();
        }

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

    private void ClearValuesFolder()
    {
        _values.Clear();
        
        string folderPath = "Assets/Resources/Scriptables/Values";
        // If the folder doesn't exist, create it
        if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Scriptables", "Values");
        }
        // Delete all existing Values assets in the folder
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Values", new[] { folderPath });
        foreach (string guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            UnityEditor.AssetDatabase.DeleteAsset(assetPath);
        }
    }
    
    private void LoadValuesCSV()
    {
        ClearValuesFolder();
        
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
            string assetPath = $"Assets/Resources/Scriptables/Values/value_" + i + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(scriptable, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();

            _values.Add(scriptable);
            
            Debug.Log("Values imported successfully!");
        }
    }
    
    private void ClearQuestionsFolder()
    {
        _questionsValue1.Clear();
        _questionsValue2.Clear();
        _questionsValue3.Clear();
        _questionsValue4.Clear();
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
            else if (fields[1] == "valeur1")
            {
                scriptable.type = 1;
                _questionsValue1.Add(scriptable);
            }
            else if (fields[1] == "valeur2")
            {
                scriptable.type = 2;
                _questionsValue2.Add(scriptable);
            }
            else if (fields[1] == "valeur3")
            {
                scriptable.type = 3;
                _questionsValue3.Add(scriptable);
            }
            else if (fields[1] == "valeur4")
            {
                scriptable.type = 4;
                _questionsValue4.Add(scriptable);
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
        ClearValuesFolder();
    }

    [DisableInPlayMode][Button]
    private void LoadCSV()
    {
        LoadValuesCSV();
        LoadQuestionsCSV();
    }
#endif
}

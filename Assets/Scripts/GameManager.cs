using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    [Header("Materials")] public Material materialGround;
    public Material materialWall;
    public Material materialStart;
    public Material materialLandmarksA;
    public Material materialLandmarksB;
    public Material materialLandmarksC;
    public Material materialLandmarksD;
    public Material materialLandmarksE;

    [Header("Game Settings")] public float gameTime = 600;
    [SerializeField] private string questionsFileName;


    private static GameManager _instance;
    private Player _player;
    
    private List<Question> _questions;

    // STATS
    private int nbUnitTraveled;
    private int nbLandmarksReached;
    private int nbWallsHit;
    private int nbDirectionChanges;
    private int nbButtonsPressed;
    private float timeSpentMoving;

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

    private void Start()
    {
        _player = FindFirstObjectByType<Player>();

        // Initialize game settings
        nbUnitTraveled = 0;
        nbLandmarksReached = 0;
        nbWallsHit = 0;
        nbDirectionChanges = 0;
        
        // Load questions
        _questions = new List<Question>();
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Question", new[] { "Assets/Scriptables/Questions" });
        foreach (string guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            Question question = UnityEditor.AssetDatabase.LoadAssetAtPath<Question>(assetPath);
            if (question != null)
            {
                _questions.Add(question);
            }
        }
        Debug.Log("Questions loaded: " + _questions.Count);
    }

    private void Update()
    {
        // Update game time
        gameTime -= Time.deltaTime;
        if (gameTime <= 0)
        {
            // End game logic
            Debug.Log("Game Over");
        }

        // Update stats
        if (_player.IsMoving)
            timeSpentMoving += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _questions[0].nbQuestionAsked++;
        }
    }

#if UNITY_EDITOR
    
    [DisableInPlayMode][Button]
    private void ClearQuestionsFolder()
    {
        _questions.Clear();
        
        string folderPath = "Assets/Scriptables/Questions";
        // If the folder doesn't exist, create it
        if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets/Scriptables", "Questions");
        }
        // Delete all existing Question assets in the folder
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Question", new[] { folderPath });
        foreach (string guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            UnityEditor.AssetDatabase.DeleteAsset(assetPath);
        }
    }
    
    [DisableInPlayMode][Button]
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

            if (fields.Length < 4)
            {
                Debug.LogWarning("Malformed line skipped: " + line);
                continue;
            }

            Question scriptable = ScriptableObject.CreateInstance<Question>();
            scriptable.question = fields[0];
            scriptable.answers = new List<string> { fields[1], fields[2] };
            scriptable.type = int.Parse(fields[3]);
            

            // Sauvegarde du ScriptableObject dans le projet (dans un dossier "Assets/Resources/Questions")
            string assetPath = $"Assets/Scriptables/Questions/question_" + i + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(scriptable, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            
            _questions.Add(scriptable);
        }

        Debug.Log("Questions imported successfully!");
    }
#endif
}

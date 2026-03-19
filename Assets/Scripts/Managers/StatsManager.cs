using Sirenix.OdinInspector;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    private static StatsManager m_instance;
    
    [Title ("Game Statistics")]
    [SerializeField] private int m_nbUnitTraveled;
    [SerializeField] private int m_nbLandmarksReached;
    [SerializeField] private int m_nbWallsHit;
    [SerializeField] private int m_nbDirectionChanges;
    [SerializeField] private int m_nbButtonsPressed;
    [SerializeField] private float m_timeSpentMoving;
    [SerializeField] private int m_nbQuestionsAnswered;
    [SerializeField] private int m_nbLeftAnswers;
    [SerializeField] private int m_nbRightAnswers;
    [SerializeField] private float m_timeBetweenQuestions;
    [SerializeField] private float m_shortestTimeBetweenQuestions;
    [SerializeField] private float m_longestTimeBetweenQuestions;
    [SerializeField] private int m_nbProgressBarFull;
    
    // Getters and setters for each variable
    public int NbUnitTraveled { get => m_nbUnitTraveled; set => m_nbUnitTraveled = value; }
    public int NbLandmarksReached { get => m_nbLandmarksReached; set => m_nbLandmarksReached = value; }
    public int NbWallsHit { get => m_nbWallsHit; set => m_nbWallsHit = value; }
    public int NbDirectionChanges { get => m_nbDirectionChanges; set => m_nbDirectionChanges = value; }
    public int NbButtonsPressed { get => m_nbButtonsPressed; set => m_nbButtonsPressed = value; }
    public float TimeSpentMoving { get => m_timeSpentMoving; set => m_timeSpentMoving = value; }
    public int NbQuestionsAnswered { get => m_nbQuestionsAnswered; set => m_nbQuestionsAnswered = value; }
    public int NbLeftAnswers { get => m_nbLeftAnswers; set => m_nbLeftAnswers = value; }
    public int NbRightAnswers { get => m_nbRightAnswers; set => m_nbRightAnswers = value; }
    public float TimeBetweenQuestions { get => m_timeBetweenQuestions; set => m_timeBetweenQuestions = value; }
    public float ShortestTimeBetweenQuestions { get => m_shortestTimeBetweenQuestions; set => m_shortestTimeBetweenQuestions = value; }
    public float LongestTimeBetweenQuestions { get => m_longestTimeBetweenQuestions; set => m_longestTimeBetweenQuestions = value; }
    public int NbProgressBarFull { get => m_nbProgressBarFull; set => m_nbProgressBarFull = value; }
    
    public static StatsManager Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindFirstObjectByType<StatsManager>();
            return m_instance;
        }
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple StatsManager instances in scene!");
            Destroy(gameObject);
        }
        else
        {
            m_instance = this;
        }
    }

    public void Initialize()
    {
        m_nbUnitTraveled = 0;
        m_nbLandmarksReached = 0;
        m_nbWallsHit = 0;
        m_nbDirectionChanges = 0;
        m_nbButtonsPressed = 0;
        m_timeSpentMoving = 0f;
        m_nbQuestionsAnswered = 0;
        m_nbLeftAnswers = 0;
        m_nbRightAnswers = 0;
        m_timeBetweenQuestions = 0f;
        m_shortestTimeBetweenQuestions = float.MaxValue;
        m_longestTimeBetweenQuestions = float.MinValue;
        m_nbProgressBarFull = 0;
    }
    
    public void WriteFinalStatsToFile()
    {
        if (string.IsNullOrEmpty(CharteManager.Instance.LogFilePath))
        {
            Debug.LogError("Log file path not initialized.");
            return;
        }

        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(CharteManager.Instance.LogFilePath, true))
        {
            writer.WriteLine("======== FIN DE PARTIE ========");
            writer.WriteLine($"Temps total : {GameManager.Instance.GameTimer:F2} secondes");
            writer.WriteLine($"Temps passé en mouvement : {m_timeSpentMoving:F2} secondes");
            writer.WriteLine($"Unités parcourues : {m_nbUnitTraveled}");
            writer.WriteLine($"Points de repère atteints : {m_nbLandmarksReached}");
            writer.WriteLine($"Murs percutés : {m_nbWallsHit}");
            writer.WriteLine($"Changements de direction : {m_nbDirectionChanges}");
            writer.WriteLine($"Boutons pressés : {m_nbButtonsPressed}");
            writer.WriteLine($"Questions répondues : {m_nbQuestionsAnswered}");
            writer.WriteLine($"Réponses gauche : {m_nbLeftAnswers}");
            writer.WriteLine($"Réponses droite : {m_nbRightAnswers}");
            writer.WriteLine($"Temps moyen entre les questions : {m_timeBetweenQuestions/m_nbQuestionsAnswered:F2} secondes");
            writer.WriteLine($"Temps le plus court entre deux questions : {m_shortestTimeBetweenQuestions:F2} secondes");
            writer.WriteLine($"Temps le plus long entre deux questions : {m_longestTimeBetweenQuestions:F2} secondes");
            writer.WriteLine($"Barre de progression pleine : {m_nbProgressBarFull}");
            writer.WriteLine("================================");
            writer.WriteLine();
        }
    }
}

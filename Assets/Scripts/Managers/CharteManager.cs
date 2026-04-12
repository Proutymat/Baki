using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class CharteManager : MonoBehaviour
{
    private static CharteManager m_instance;
    
    [Title("Parameters")]
    [SerializeField] private int m_maxLawsInterval = 5;
    [SerializeField] private int m_printIntervalsInPercent;
    
    [Title("Static list")]
    [SerializeField] private List<Value> m_laws;
    
    // Temporary solution
    private List<string> m_illusLandmarkA;
    private List<string> m_illusLandmarkB;
    private List<string> m_illusLandmarkC;
    private List<string> m_illusLandmarkD;
    private List<string> m_illustrations;
    
    [Title("Debug"), SerializeField] private bool m_debug;
    [SerializeField, ShowIf("m_debug")] private int m_lastPrintedPercent;
    [SerializeField, ShowIf("m_debug")] private string m_logFilePath;
    [SerializeField, ShowIf("m_debug")] private List<LawCursor> m_lawCursors;
    [SerializeField, ShowIf("m_debug")] private List<string> m_lawsQueue;
    [SerializeField, ShowIf("m_debug")] private List<int> m_lawsQueuePriority;
    [SerializeField, ShowIf("m_debug")] private List<string> m_illusQueue;
    [SerializeField, ShowIf("m_debug")] private List<int> m_illusQueuePriority;
    
    public List<Value> Laws { get => m_laws; }
    public List<LawCursor> LawCursors { get => m_lawCursors; set => m_lawCursors = value; }
    public string LogFilePath { get => m_logFilePath; }
    
    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple CharteManager instances in scene!");
            Destroy(gameObject);
        }
        else
        {
            m_instance = this;
        }
    }
    
    public static CharteManager Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindFirstObjectByType<CharteManager>();
            return m_instance;
        }
    }

    // Temporary solution, because we still not have chosen which illus goes with each anwsers
    private void TemporaryFillingIllustrations()
    {
        m_illustrations = new List<string>();
        m_illusLandmarkA = new List<string>();
        m_illusLandmarkB = new List<string>();
        m_illusLandmarkC = new List<string>();
        m_illusLandmarkD = new List<string>();
        
        m_illusLandmarkA.Add("PAPILLON");
        m_illusLandmarkA.Add("LEZARD");
        m_illusLandmarkA.Add("BATEAU");
        m_illusLandmarkA.Add("COLONNE");
        
        m_illusLandmarkB.Add("VASE");
        m_illusLandmarkB.Add("VAJRA");
        m_illusLandmarkB.Add("PEIGNE");
        m_illusLandmarkB.Add("EVENTAIL");
        
        m_illusLandmarkC.Add("CORAIL");
        m_illusLandmarkC.Add("CHAMPIGNONS");
        m_illusLandmarkC.Add("CRAPAUD");
        m_illusLandmarkC.Add("PIEUVRE");
        
        m_illusLandmarkD.Add("CHATAIGNE");
        m_illusLandmarkD.Add("DENTS");
        m_illusLandmarkD.Add("CHAINE");
        m_illusLandmarkD.Add("DAGUE");
        
        m_illustrations.Add("LYRE");
        m_illustrations.Add("CHAUVESOURIS");
        m_illustrations.Add("CHRYSALIDE");
        m_illustrations.Add("BALANCE");
        m_illustrations.Add("CALISSE");
        m_illustrations.Add("CORDEAU");
        m_illustrations.Add("RUCHE");
        m_illustrations.Add("SERPENT");
        m_illustrations.Add("GANTS");
        m_illustrations.Add("SCARABE");
        m_illustrations.Add("CORNEABONDANCE");
        m_illustrations.Add("POISSON");
    }

    public void Initialize()
    {
        TemporaryFillingIllustrations();
        
        m_lastPrintedPercent = 0;
        m_logFilePath = $"{GameManager.Instance.CurrentGameLogFolder}/charte.txt";
        
        // Create law cursors
        m_lawCursors.Clear();
        m_lawCursors = new List<LawCursor>();
        for (int i = 0; i < m_laws.Count; i++)
        {
            LawCursor lawCursor = new LawCursor(m_laws[i]);
            m_lawCursors.Add(lawCursor);
        }
        
        m_lawsQueue = new List<string>();
        m_lawsQueuePriority = new List<int>();
        m_illusQueue = new List<string>();
        m_illusQueuePriority = new List<int>();
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    public void IncrementLawCursor(int lawType, int lawIncrement)
    {
        (string, int) resultLaw = m_lawCursors[lawType].IncrementLawCursorValue(lawIncrement);
        if (resultLaw.Item1 != "")
        {
            m_lawsQueue.Add(resultLaw.Item1);
            m_lawsQueuePriority.Add(resultLaw.Item2);
            Debug.Log("New law : " + resultLaw);
        }
    }

    public void AddIllustrationToQueue(string illustration, int priority)
    {
        m_illusQueue.Add(illustration);
        m_illusQueuePriority.Add(priority);
    }

    public void ExitLandmark(int landmarkType)
    {
        if (landmarkType == 0 && m_illusLandmarkA.Count > 0)
        {
            int index = Random.Range(0, m_illusLandmarkA.Count);
            m_illusQueue.Add(m_illusLandmarkA[index]);
            m_illusQueuePriority.Add(1);
            m_illusLandmarkA.RemoveAt(index);
        }
        else if (landmarkType == 1 && m_illusLandmarkB.Count > 0)
        {
            int index = Random.Range(0, m_illusLandmarkB.Count);
            m_illusQueue.Add(m_illusLandmarkB[index]);
            m_illusQueuePriority.Add(1);
            m_illusLandmarkB.RemoveAt(index);
        }
        else if (landmarkType == 2 && m_illusLandmarkC.Count > 0)
        {
            int index = Random.Range(0, m_illusLandmarkC.Count);
            m_illusQueue.Add(m_illusLandmarkC[index]);
            m_illusQueuePriority.Add(1);
            m_illusLandmarkC.RemoveAt(index);
        }
        else if (landmarkType == 3 && m_illusLandmarkD.Count > 0)
        {
            int index = Random.Range(0, m_illusLandmarkD.Count);
            m_illusQueue.Add(m_illusLandmarkD[index]);
            m_illusQueuePriority.Add(1);
            m_illusLandmarkD.RemoveAt(index);
        }
    }
    
    private void PrintLawsQueue()
    {
        //if (enablePrinters) pngPrinter.PrintCharteTicket(lawsQueue, lawsQueue);
        
        if (string.IsNullOrEmpty(m_logFilePath))
        {
            Debug.LogError("Log file path not initialized.");
            return;
        }


        // Append to file (true = append mode)
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(m_logFilePath, true))
        {
            writer.WriteLine("------------------------");
            writer.WriteLine($"   SALVE DE LOIS " + m_lastPrintedPercent + "%");
            writer.WriteLine("------------------------");
            
            int lawPrinted = 0;

            if (m_lawsQueuePriority.Any())
            {
                int priorityIndex = m_lawsQueuePriority.Max();
            
                while (m_lawsQueue.Count > 0 && lawPrinted < m_maxLawsInterval && priorityIndex > 0)
                {
                    // Get the index of the first law with the highest priority
                    int index = m_lawsQueuePriority.IndexOf(priorityIndex);
                    writer.WriteLine(m_lawsQueue[index]);
                    m_lawsQueue.RemoveAt(index);
                    m_lawsQueuePriority.RemoveAt(index);
                    lawPrinted++;
                
                    // Update the priority index
                    if (m_lawsQueuePriority.Count > 0)
                        priorityIndex = m_lawsQueuePriority.Max();
                }
            
                writer.WriteLine();
            }
        }
    }

    public void UpdatePercentage(int percentElapsed)
    {
        if (percentElapsed >= m_lastPrintedPercent + m_printIntervalsInPercent)
        {
            m_lastPrintedPercent += m_printIntervalsInPercent;
            PrintLawsQueue();
            
            Debug.Log("" + m_lastPrintedPercent + "% of the game elapsed");
        }
    }

    private void FillMissingIllustrations()
    {
        while (Mathf.RoundToInt(m_lawsQueue.Count / 5f) > Mathf.RoundToInt(m_illusQueue.Count / 3f) && m_illustrations.Count > 0)
        {
            int index = Random.Range(0, m_illustrations.Count);
            string illu = m_illustrations[index];
            m_illusQueue.Add(illu);
            m_illustrations.RemoveAt(index);
        }
    }
    
    private void UnlockStatsIllustrations()
    {
        StatsManager statsManager = StatsManager.Instance;
        
        // Number of landmarks reached
        if (statsManager.NbLandmarksReached >= 4)
        {
            m_illusQueue.Add("BATONSOURCIER");
        }
        else
        {
            m_illusQueue.Add("PHARE");
        }
        
        // Longest time between questions
        if (statsManager.LongestTimeBetweenQuestions > 30)
        {
            m_illusQueue.Add("YOYO");
        }
        
        // Unit traveled
        if (statsManager.NbUnitTraveled <= 95)
        {
            m_illusQueue.Add("TONGS");
        }
        else if (statsManager.NbUnitTraveled <= 155)
        {
            m_illusQueue.Add("CHAUSSURES");
        }
        else
        {
            m_illusQueue.Add("RANDO");
        }
        
        // Nb of questions answered
        if (statsManager.NbQuestionsAnswered > 185)
        {
            m_illusQueue.Add("SABLIER");
        }
        else
        {
            m_illusQueue.Add("CHANDELIER");
        }
        
        // Nb walls hit
        if (statsManager.NbWallsHit > 21)
        {
            m_illusQueue.Add("VAUTOUR");
        }
        else
        {
            m_illusQueue.Add("ROUE");
        }
        
        // Nb of direction changes
        if (statsManager.NbDirectionChanges > 30)
        {
            m_illusQueue.Add("COMPAS");
        }
            
        // Nb of left answers
        if (statsManager.NbLeftAnswers == statsManager.NbRightAnswers)
        {
            m_illusQueue.Add("DICE");
        }
        else if (statsManager.NbLeftAnswers > statsManager.NbRightAnswers)
        {
            m_illusQueue.Add("FIGUE");
        }
        else
        {
            m_illusQueue.Add("POMME");
        }
    }

    public void EndGame()
    {
        FillMissingIllustrations();
        UnlockStatsIllustrations();
        if (GameManager.Instance.EnablePrinters) StartCoroutine(PrinterManager.Instance.PNGPrinter.PrintCharteTicket(m_lawsQueue, m_illusQueue));
    }
    
    
#if UNITY_EDITOR
    
    [Button, DisableInPlayMode]
    private void LoadLaws()
    {
        m_laws.Clear();
        m_laws = FileImporterManager.Instance.LoadLawsCSV();
        Debug.Log("Laws loaded : " + m_laws.Count);
    }
    
#endif
}

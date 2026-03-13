using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;


public class GameManager : SerializedMonoBehaviour
{
    // ------------------------------------
    //             ATTRIBUTES
    // ------------------------------------
    
    [SerializeField] private bool setObjectsInInspector = false;

    
    
    private float beatingArrowTimer;
    private float beatingValue = 1;
    
    [Header("----- UI INTERFACE ENDING -----"), ShowIf("setObjectsInInspector")]
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject endingInterface;
    
    [Header("----- MAP PRINTER -----"), ShowIf("setObjectsInInspector")]
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject landmarksArrows;
    [SerializeField, ShowIf("setObjectsInInspector")] private Camera playerCamera;
    
    [Header("------- INSTANCES -------")]
    private static GameManager _instance;
    [SerializeField, ShowIf("setObjectsInInspector")] private Player player;
    [SerializeField, ShowIf("setObjectsInInspector")] private ProgressBar progressBar;
    [SerializeField, ShowIf("setObjectsInInspector")] private PNGPrinter pngPrinter;
    [SerializeField, ShowIf("setObjectsInInspector")] private UIAnimations uiAnimations;
    
    [Title("Game Settings")]
    [SerializeField] private float gameDuration = 600;
    [SerializeField] private int printIntervalsInPercent;
    [SerializeField] private int maxLawsInterval = 5;

    private bool unboardingStep1 = false;
    private bool unboardingStep2 = false;
    
    [Header("DEBUGS")]
    [SerializeField] private bool debug = false;
    [SerializeField] private bool skipIntro = false;
    [SerializeField] private bool enablePrinters = true;
    
    
    [SerializeField, ShowIf("debug")] public List<Value> laws; // Should be private in chartemanager or something
    
    [Header("WORKING VALUES (used in runtime)")]
    [SerializeField, ShowIf("debug")] public List<LawCursor> lawCursors;
    [SerializeField, ShowIf("debug")] private List<string> lawsQueue;
    [SerializeField, ShowIf("debug")] private List<int> lawsQueuePriority;
    [SerializeField, ShowIf("debug")] private List<string> illusQueue;
    [SerializeField, ShowIf("debug")] private List<int> illusQueuePriority;
    [SerializeField, ShowIf("debug")] private float gameTimer;
    [SerializeField, ShowIf("debug")] private int lastPrintedPercent = 0;
    private string currentGameLogFolder;
    private string charteLogFilePath;
    private string answersLogFilePath;
    private bool isGameOver = false;
    private bool inLandmark;
    private Landmark currentLandmark;

    private List<string> illusLandmarkA;
    private List<string> illusLandmarkB;
    private List<string> illusLandmarkC;
    private List<string> illusLandmarkD;
    private List<string> illustrations;
    
    // Managers
    public StatsManager m_statsManager;
    public PanelManager m_panelManager;
    
    public StatsManager Statistics { get { return m_statsManager; } }

    public string CurrentGameLogFolder { get { return currentGameLogFolder; } }


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
        illustrations = new List<string>();
        illusLandmarkA = new List<string>();
        illusLandmarkB = new List<string>();
        illusLandmarkC = new List<string>();
        illusLandmarkD = new List<string>();
        
        illusLandmarkA.Add("PAPILLON");
        illusLandmarkA.Add("LEZARD");
        illusLandmarkA.Add("BATEAU");
        illusLandmarkA.Add("COLONNE");
        
        illusLandmarkB.Add("VASE");
        illusLandmarkB.Add("VAJRA");
        illusLandmarkB.Add("PEIGNE");
        illusLandmarkB.Add("EVENTAIL");
        
        illusLandmarkC.Add("CORAIL");
        illusLandmarkC.Add("CHAMPIGNONS");
        illusLandmarkC.Add("CRAPAUD");
        illusLandmarkC.Add("PIEUVRE");
        
        illusLandmarkD.Add("CHATAIGNE");
        illusLandmarkD.Add("DENTS");
        illusLandmarkD.Add("CHAINE");
        illusLandmarkD.Add("DAGUE");
        
        illustrations.Add("LYRE");
        illustrations.Add("CHAUVESOURIS");
        illustrations.Add("CHRYSALIDE");
        illustrations.Add("BALANCE");
        illustrations.Add("CALISSE");
        illustrations.Add("CORDEAU");
        illustrations.Add("RUCHE");
        illustrations.Add("SERPENT");
        illustrations.Add("GANTS");
        illustrations.Add("SCARABE");
        illustrations.Add("CORNEABONDANCE");
        illustrations.Add("POISSON");
        
        // Initialize managers
        m_statsManager = new StatsManager();

        beatingValue = 1;
        beatingArrowTimer = 0;
        
        // Unboarding
        progressBar.IsPaused = true;
        PanelManager.Instance.ShowDirectionalArrows(false);
        
        // Initialize game settings
        gameTimer = gameDuration;
        lawsQueue = new List<string>();
        lastPrintedPercent = 0;
        isGameOver = false;
        
        // Create law cursors
        lawCursors.Clear();
        lawCursors = new List<LawCursor>();
        for (int i = 0; i < laws.Count; i++)
        {
            LawCursor lawCursor = new LawCursor(laws[i]);
            lawCursors.Add(lawCursor);
        }
        
        Debug.Log("Law cursors created : " + lawCursors.Count);
        
        QuestionManager.Instance.NextQuestion();
        playerCamera.transform.position = new Vector3(player.transform.position.x, playerCamera.transform.position.y, player.transform.position.z);

        
        // DEBUG : Create log folder
        currentGameLogFolder = Application.dataPath + "/Logs";
        if (!System.IO.Directory.Exists(currentGameLogFolder))
        {
            System.IO.Directory.CreateDirectory(currentGameLogFolder);
        }

        currentGameLogFolder += "/" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        System.IO.Directory.CreateDirectory(currentGameLogFolder);
        charteLogFilePath = $"{currentGameLogFolder}/charte.txt";
        answersLogFilePath = $"{currentGameLogFolder}/answers.txt";
        
        // Initialize instances 
        player.Initialize();
        pngPrinter.Initialize();
        uiAnimations.Initialize();

        if (skipIntro)
        {
            m_panelManager.SetPanel(PanelManager.PanelState.Standard);
        }
        else
        {
            m_panelManager.SetPanel(PanelManager.PanelState.Intro);
        }
    }
    
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        lawCursors = new List<LawCursor>();
        InitializeGame();
    }
    
    
    // --------------------------------------------
    //                  QUESTIONS
    // --------------------------------------------

    public void PrintAreaPlayer(bool showAllLandmarkArrows = false)
    {
        playerCamera.transform.position = new Vector3(player.transform.position.x, playerCamera.transform.position.y, player.transform.position.z);
        landmarksArrows.GetComponent<LandmarkDetection>().UpdateLandmarkArrows(showAllLandmarkArrows);
        if (enablePrinters) StartCoroutine(pngPrinter.PrintMapTicket());
	}

    public void EnterLandmark(Landmark landmark)
    {
        QuestionManager.Instance.EnterLandmark(currentLandmark.Type);
        PanelManager.Instance.EnterLandmark();
        currentLandmark = landmark;
        inLandmark = true;
        if (enablePrinters) pngPrinter.PrintLandmarkTicket(Mathf.FloorToInt(100 * (1 - (gameTimer / gameDuration))));
    }

    public void ExitLandmark(int buttonIndex)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionRespondClick");

        if (currentLandmark.Type == 0 && illusLandmarkA.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, illusLandmarkA.Count);
            illusQueue.Add(illusLandmarkA[index]);
            illusQueuePriority.Add(1);
            illusLandmarkA.RemoveAt(index);
        }
        else if (currentLandmark.Type == 1 && illusLandmarkB.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, illusLandmarkB.Count);
            illusQueue.Add(illusLandmarkB[index]);
            illusQueuePriority.Add(1);
            illusLandmarkB.RemoveAt(index);
        }
        else if (currentLandmark.Type == 2 && illusLandmarkC.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, illusLandmarkC.Count);
            illusQueue.Add(illusLandmarkC[index]);
            illusQueuePriority.Add(1);
            illusLandmarkC.RemoveAt(index);
        }
        else if (currentLandmark.Type == 3 && illusLandmarkD.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, illusLandmarkD.Count);
            illusQueue.Add(illusLandmarkD[index]);
            illusQueuePriority.Add(1);
            illusLandmarkD.RemoveAt(index);
        }
        
        inLandmark = false;
        m_panelManager.SetPanel(PanelManager.PanelState.Standard);
        m_panelManager.ShowQuestionArea(false);
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
            writer.WriteLine($"Temps passé en mouvement : {m_statsManager.TimeSpentMoving:F2} secondes");
            writer.WriteLine($"Unités parcourues : {m_statsManager.NbUnitTraveled}");
            writer.WriteLine($"Points de repère atteints : {m_statsManager.NbLandmarksReached}");
            writer.WriteLine($"Murs percutés : {m_statsManager.NbWallsHit}");
            writer.WriteLine($"Changements de direction : {m_statsManager.NbDirectionChanges}");
            writer.WriteLine($"Boutons pressés : {m_statsManager.NbButtonsPressed}");
            writer.WriteLine($"Questions répondues : {m_statsManager.NbQuestionsAnswered}");
            writer.WriteLine($"Réponses gauche : {m_statsManager.NbLeftAnswers}");
            writer.WriteLine($"Réponses droite : {m_statsManager.NbRightAnswers}");
            writer.WriteLine($"Temps moyen entre les questions : {m_statsManager.TimeBetweenQuestions/m_statsManager.NbQuestionsAnswered:F2} secondes");
            writer.WriteLine($"Temps le plus court entre deux questions : {m_statsManager.ShortestTimeBetweenQuestions:F2} secondes");
            writer.WriteLine($"Temps le plus long entre deux questions : {m_statsManager.LongestTimeBetweenQuestions:F2} secondes");
            writer.WriteLine($"Barre de progression pleine : {m_statsManager.NbProgressBarFull}");
            writer.WriteLine("================================");
            writer.WriteLine();
        }
    }

    public void UpdateArrowButtonsSprite(string direction)
    {
        PanelManager.Instance.UpdateDirectionalArrow(direction);

        if (!unboardingStep2)
        {
            unboardingStep2 = true;
            UnboardingStep2();
        }

        m_panelManager.ShowQuestionArea(true);
    }

    private void UnboardingStep1()
    {
        PanelManager.Instance.ShowDirectionalArrows(true);
        m_panelManager.ShowQuestionArea(false);
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
        
        // End game logic
        if (gameTimer <= 0)
        {
            player.SetIsMoving(false);
            WriteFinalStatsToFile();
            isGameOver = true;
            m_panelManager.SetPanel(PanelManager.PanelState.End);
            endingInterface.SetActive(true);
            FMODUnity.RuntimeManager.PlayOneShot("event:/StopAll");
            FMODUnity.RuntimeManager.PlayOneShot("event:/END");
            UnlockIllustrations();
            
            // Add illustrations if not enough
            while (Mathf.RoundToInt(lawsQueue.Count / 5f) > Mathf.RoundToInt(illusQueue.Count / 3f) && illustrations.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, illustrations.Count);
                string illu = illustrations[index];
                illusQueue.Add(illu);
                illustrations.RemoveAt(index);
            }

            if (enablePrinters) StartCoroutine(pngPrinter.PrintCharteTicket(lawsQueue, illusQueue));
        }
        
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
        
        // Update stats
        if (player.IsMoving)
            m_statsManager.TimeSpentMoving += Time.deltaTime;
        QuestionManager.Instance.Timer += Time.deltaTime;
    }

    private void UnlockIllustrations()
    {
        // Number of landmarks reached
        if (m_statsManager.NbLandmarksReached >= 4)
        {
            illusQueue.Add("BATONSOURCIER");
        }
        else
        {
            illusQueue.Add("PHARE");
        }
        
        // Longest time between questions
        if (m_statsManager.LongestTimeBetweenQuestions > 30)
        {
            illusQueue.Add("YOYO");
        }
        
        // Unit traveled
        if (m_statsManager.NbUnitTraveled <= 95)
        {
            illusQueue.Add("TONGS");
        }
        else if (m_statsManager.NbUnitTraveled <= 155)
        {
            illusQueue.Add("CHAUSSURES");
        }
        else
        {
            illusQueue.Add("RANDO");
        }
        
        // Nb of questions answered
        if (m_statsManager.NbQuestionsAnswered > 185)
        {
            illusQueue.Add("SABLIER");
        }
        else
        {
            illusQueue.Add("CHANDELIER");
        }
        
        // Nb walls hit
        if (m_statsManager.NbWallsHit > 21)
        {
            illusQueue.Add("VAUTOUR");
        }
        else
        {
            illusQueue.Add("ROUE");
        }
        
        // Nb of direction changes
        if (m_statsManager.NbDirectionChanges > 30)
        {
            illusQueue.Add("COMPAS");
        }
        
            
        // Nb of left answers
        if (m_statsManager.NbLeftAnswers == m_statsManager.NbRightAnswers)
        {
            illusQueue.Add("DICE");
        }
        else if (m_statsManager.NbLeftAnswers > m_statsManager.NbRightAnswers)
        {
            illusQueue.Add("FIGUE");
        }
        else
        {
            illusQueue.Add("POMME");
        }
        
    }

    private void PrintLawsQueue()
    {
        //if (enablePrinters) pngPrinter.PrintCharteTicket(lawsQueue, lawsQueue);
        
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
        
        // UPDATE STATS
        m_statsManager.NbQuestionsAnswered++;
        if (answerIndex == 1)
            m_statsManager.NbLeftAnswers++;
        else
            m_statsManager.NbRightAnswers++;

        // Update time between questions (if more than 5 questions answered)
        if (m_statsManager.NbQuestionsAnswered > 5)
        {
            if (QuestionManager.Instance.Timer < m_statsManager.ShortestTimeBetweenQuestions)
                m_statsManager.ShortestTimeBetweenQuestions = QuestionManager.Instance.Timer;
            if (QuestionManager.Instance.Timer > m_statsManager.LongestTimeBetweenQuestions)
                m_statsManager.LongestTimeBetweenQuestions = QuestionManager.Instance.Timer;
        }
        m_statsManager.TimeBetweenQuestions += QuestionManager.Instance.Timer;
        QuestionManager.Instance.Timer = 0;
        
        
        // Progress bar is full
        if (progressBar.IncreaseProgressBar())
        {
            m_statsManager.NbProgressBarFull++;
            player.SetIsMoving(false);
            PanelManager.Instance.ShowQuestionArea(false);
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
            law1Type = QuestionManager.Instance.CurrentQuestion.answer1Type1;
            law2Type = QuestionManager.Instance.CurrentQuestion.answer1Type2;
            lawIncrement1 = QuestionManager.Instance.CurrentQuestion.answer1Type1ADD;
            lawIncrement2 = QuestionManager.Instance.CurrentQuestion.answer1Type2ADD;
            if (QuestionManager.Instance.CurrentQuestion.answer1Illustration != "")
            {
                illusQueue.Add(QuestionManager.Instance.CurrentQuestion.answer1Illustration);
                illusQueuePriority.Add(QuestionManager.Instance.CurrentQuestion.answer1IllustrationPriority);
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
                illusQueue.Add(QuestionManager.Instance.CurrentQuestion.answer2Illustration);
                illusQueuePriority.Add(QuestionManager.Instance.CurrentQuestion.answer2IllustrationPriority);
            }
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

        QuestionManager.Instance.NextQuestion();
    }  
}
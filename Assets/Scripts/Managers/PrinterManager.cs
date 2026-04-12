using Sirenix.OdinInspector;
using UnityEngine;

public class PrinterManager : MonoBehaviour
{
    private static PrinterManager m_instance;
    
    [Title("Set in inspector")]
    [SerializeField] private PNGPrinter pngPrinter;
    [SerializeField] private GameObject m_percentageFrenchText;
    [SerializeField] private GameObject m_percentageEnglishText;
    
    public PNGPrinter PNGPrinter { get => pngPrinter; }
    
    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple PrinterManager instances in scene!");
            Destroy(gameObject);
        }
        else
        {
            m_instance = this;
        }
    }
    
    public static PrinterManager Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindFirstObjectByType<PrinterManager>();
            return m_instance;
        }
    }

    public void Initialize(GameManager.GameLanguage language)
    {
        pngPrinter.Initialize(language);
        
        // Setup language for percentage image
        if (language == GameManager.GameLanguage.French)
        {
            m_percentageFrenchText.SetActive(true);
            m_percentageEnglishText.SetActive(false);
        }
        else if (language == GameManager.GameLanguage.English)
        {
            m_percentageFrenchText.SetActive(false);
            m_percentageEnglishText.SetActive(true);
        }
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------
}

using UnityEngine;

public class PrinterManager : MonoBehaviour
{
    private static PrinterManager m_instance;
    
    [SerializeField] private PNGPrinter pngPrinter;
    
    public PNGPrinter PNGPrinter { get => pngPrinter; }
    
    public static PrinterManager Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindFirstObjectByType<PrinterManager>();
            return m_instance;
        }
    }
    
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

    public void Initialize()
    {
        pngPrinter.Initialize();
    }
}

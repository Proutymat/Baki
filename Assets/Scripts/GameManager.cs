using UnityEngine;



public class GameManager : MonoBehaviour
{
    [Header("Materials")]
    public Material materialGround;
    public Material materialWall;
    public Material materialStart;
    public Material materialLandmarksA;
    public Material materialLandmarksB;
    public Material materialLandmarksC;
    public Material materialLandmarksD;
    public Material materialLandmarksE;
    
    private static GameManager _instance;
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
    
}

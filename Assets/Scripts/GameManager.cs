using System;
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

    [Header("Game Settings")]
    public float gameTime = 600;
    
    private static GameManager _instance;
    private Player _player;
    
    // STATS
    private int nbUnitTraveled;
    private int nbLandmarksReached;
    private int nbWallsHit;
    private int nbDirectionChanges;
    private int nbButtonsPressed;
    private float timeSpentMoving;
    public int DistanceTraveled { get { return nbUnitTraveled; } set { nbUnitTraveled = value; } }
    public int LandmarksReached { get { return nbLandmarksReached; } set { nbLandmarksReached = value; } }
    public int WallsHit { get { return nbWallsHit; } set { nbWallsHit = value; } }
    public int DirectionChanges { get { return nbDirectionChanges; } set { nbDirectionChanges = value; } }
    public int ButtonsPressed { get { return nbButtonsPressed; } set { nbButtonsPressed = value; } }
    
    
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
    }
}

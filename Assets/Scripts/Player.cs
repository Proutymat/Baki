using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private bool isMoving;
    [SerializeField] private float secondPerUnit = 1f;
    [SerializeField] private Vector3 currentDirection;

    private float timer;
    private float gridCellSize;
    private GameManager gameManager;
    
    public bool IsMoving { get { return isMoving; } set { isMoving = value; } }

    private void Start()
    {
        timer = 0f;
        isMoving = false;
        gridCellSize = FindFirstObjectByType<CustomGrid>().transform.GetChild(1).localScale.x;
        currentDirection = transform.forward * gridCellSize;

        Vector3 startCellPosition = GameObject.FindGameObjectWithTag("PlayerStart").transform.position;
        transform.position = new Vector3(startCellPosition.x, startCellPosition.y + gridCellSize / 2, startCellPosition.z);
        
        gameManager = GameManager.Instance;
    }

    public void SetIsMoving(bool isMoving)
    {
        this.isMoving = isMoving;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player hit wall
        if (other.tag == "Wall")
        {
            Debug.Log("Wall hit");
            this.transform.position -= currentDirection;
            gameManager.WallsHit++;
        }
        // Player hit landmark   
        else if (other.tag == "Landmark")
        {
            Debug.Log("Landmark reached");
            Destroy(other.gameObject);
            gameManager.LandmarksReached++;
        }
        SetIsMoving(false);
    }

    public void ChangeSpeed(float secondPerUnit)
    {
        this.secondPerUnit = secondPerUnit;
    }
    
    public void ChangeDirection(string direction)
    {
        Vector3 previousDirection = currentDirection;
        
        SetIsMoving(true);
        
        // Direction are inverted because I fucked up the axis in map generation
        if (direction == "foreward")
            currentDirection = Vector3.left * gridCellSize;
        else if (direction == "backward")
            currentDirection = Vector3.right * gridCellSize;
        else if (direction == "right")
            currentDirection = Vector3.forward * gridCellSize;
        else if (direction == "left")
            currentDirection = Vector3.back * gridCellSize;

        // Game stats
        if (previousDirection != currentDirection)
            gameManager.DirectionChanges++;
        gameManager.ButtonsPressed++;
    }

    void Update() {
        
        timer += Time.deltaTime;
        
        if (isMoving && timer >= secondPerUnit)
        {
            timer = 0f;
            this.transform.position += currentDirection;
            gameManager.DistanceTraveled++;
        }

    }
}
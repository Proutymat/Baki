using System;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private bool isMoving;
    [SerializeField] private float secondPerUnit = 1f;
    [SerializeField] private Vector3 currentDirection;

    private float unitTimer;
    private float gridCellSize;
    private GameManager gameManager;
    private string previousDirection;
    private Vector3 startCellPosition;
    private bool midSoundPlayed;
    private bool hasMovedOnce;
    
    public bool IsMoving { get { return isMoving; } set { isMoving = value; } }

    private void Start()
    {
        gridCellSize = FindFirstObjectByType<CustomGrid>().transform.GetChild(1).localScale.x;
        currentDirection = transform.forward * gridCellSize;
        startCellPosition = GameObject.FindGameObjectWithTag("PlayerStart").transform.position;
        gameManager = GameManager.Instance;
        
        Initialize();
    }

    public void Initialize()
    {
        unitTimer = 0f;
        isMoving = false;
        currentDirection = transform.forward * gridCellSize;
        previousDirection = "none";
        transform.position = new Vector3(startCellPosition.x, startCellPosition.y + gridCellSize / 2, startCellPosition.z);
        midSoundPlayed = false;
        hasMovedOnce = false;
    }

    public void SetIsMoving(bool newMovingValue)
    {
        // Player restart moving
        if (newMovingValue && !isMoving)
        {
            unitTimer = 0;
            midSoundPlayed = true; // Set to true to avoid playing mid sound with starting sound
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_BordReStart");
            if (!hasMovedOnce)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/AMB/AMB_InGame/AMB_IG_SystemStart");
                hasMovedOnce = true;
            }
        }
        // Player stop moving
        else if (!newMovingValue && isMoving)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/AMB/AMB_InGame/AMB_IG_SystemStop");
            //FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_BordStop");
        }

        isMoving = newMovingValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player hit wall
        if (other.tag == "Wall")
        {
            Debug.Log("Wall hit");
            this.transform.position -= currentDirection;
            gameManager.WallsHit++;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_BoardImpact");
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
        gameManager.ButtonsPressed++;
        SetIsMoving(true);
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_DirectionClick");
        
        // If the direction is the same as the previous one, skip
        if (isMoving && previousDirection == direction)
        {
            return;
        }
        
        previousDirection = direction;
        gameManager.DirectionChanges++;
        
        // Direction are inverted because I fucked up the axis in map generation
        if (direction == "foreward")
        {
            currentDirection = Vector3.left * gridCellSize;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalRotate_SPAT/SFX_IG_DirectionalRotate_C");
            Debug.Log("Moving foreward");
        }
        else if (direction == "backward")
        {
            currentDirection = Vector3.right * gridCellSize;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalRotate_SPAT/SFX_IG_DirectionalRotate_B");
            Debug.Log("Moving backward");
        }
        else if (direction == "right")
        {
            currentDirection = Vector3.forward * gridCellSize;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalRotate_SPAT/SFX_IG_DirectionalRotate_R");
            Debug.Log("Moving right");
        }
        else if (direction == "left")
        {
            currentDirection = Vector3.back * gridCellSize;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalRotate_SPAT/SFX_IG_DirectionalRotate_L");
            Debug.Log("Moving left");
        }
        
        
        
    }

    void Update() 
    {
        if (!isMoving) return;
        
        
        unitTimer += Time.deltaTime;
        
        // Direction are still fucked up ! FUCKKK YEAHHHHH
        
        // Mid sound
        if (!midSoundPlayed && unitTimer >= secondPerUnit / 2)
        {
            // Forward sound
            if (currentDirection == Vector3.left * gridCellSize)
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalStep_SPAT/SFX_IG_DirectionalStep_C");
            // Backward sound
            else if (currentDirection == Vector3.right * gridCellSize)
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalStep_SPAT/SFX_IG_DirectionalStep_B");
            // Left sound
            else if (currentDirection == Vector3.back * gridCellSize)
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalStep_SPAT/SFX_IG_DirectionalStep_L");
            // Right sound
            else if (currentDirection == Vector3.forward * gridCellSize)
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalStep_SPAT/SFX_IG_DirectionalStep_R");
            
            midSoundPlayed = true;
            Debug.Log("Mid sound played");
        }
        
        // Move the player to next unit
        if (unitTimer >= secondPerUnit)
        {
            unitTimer = 0f;
            this.transform.position += currentDirection;
            gameManager.DistanceTraveled++;
            midSoundPlayed = false;
            
            // Forward sound
            if (currentDirection == Vector3.left * gridCellSize)
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalInfo_SPAT/SFX_IG_DirectionalInfo_C");
            // Backward sound
            else if (currentDirection == Vector3.right * gridCellSize)
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalInfo_SPAT/SFX_IG_DirectionalInfo_B");
            // Left sound
            else if (currentDirection == Vector3.back * gridCellSize)
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalInfo_SPAT/SFX_IG_DirectionalInfo_L");
            // Right sound
            else if (currentDirection == Vector3.forward * gridCellSize)
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalInfo_SPAT/SFX_IG_DirectionalInfo_R");
        }

    }
}
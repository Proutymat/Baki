using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;
using FMOD.Studio;

public class Player : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private bool isMoving;
    [SerializeField] private bool inSpecialZone;
    [SerializeField] private float secondPerUnit = 3f;
    [SerializeField] private float secondPerUnitSpecialZone = 2f;
    [SerializeField] private Vector3 currentDirection;
    [SerializeField] private Vector3 previousDirectionVector;
    [SerializeField] private GameObject avatarArrow;
    
    [Header("UI Animations")]
    [SerializeField] private Animator moveAnimation;
    [SerializeField] private UiAnimations uiAnimations;
    
    [Header("MAP STUFF")]
    [SerializeField] private GameObject dottedLinePrefab;
    [SerializeField] private GameObject dottedLineCurvedPrefab;
    [SerializeField] private GameObject PlayerPathDottedLineParent;

    private float unitTimer;
    private float gridCellSize;
    private GameManager gameManager;
    private string previousDirection;
    private Vector3 startCellPosition;
    private bool midSoundPlayed;
    private bool hasMovedOnce;
    private float speed;
    
    private PlayerSpecialZone playerColliders;
    
    public bool IsMoving { get { return isMoving; }}
    
    private void Start()
    {
        gridCellSize = FindFirstObjectByType<CustomGrid>().transform.GetChild(1).GetChild(0).localScale.x;
        currentDirection = transform.forward * gridCellSize;
        startCellPosition = GameObject.FindGameObjectWithTag("PlayerStart").transform.position;
        gameManager = GameManager.Instance;
        playerColliders = GetComponent<PlayerSpecialZone>();
        
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
        speed = secondPerUnit;
        previousDirectionVector = currentDirection;
        currentDirection = transform.forward * gridCellSize;
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
                FMODUnity.RuntimeManager.PlayOneShot("event:/AMB/AMB_InGame/AMB_IG_Start");
                hasMovedOnce = true;
            }
            
            uiAnimations.ResumeAnimationsAndShader();
        }
        // Player stop moving
        else if (!newMovingValue && isMoving)
        {
            gameManager.UpdateArrowButtonsSprite("stop");
            if (inSpecialZone) playerColliders.StopSpecialZoneSound();
            //FMODUnity.RuntimeManager.PlayOneShot("event:/AMB/AMB_InGame/AMB_IG_SystemStop");
            //FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_BordStop");
        }
        
        isMoving = newMovingValue;
    }

    public void OnColliderTriggered(Collider collider)
    {
        // Player hit wall
        if (collider.tag == "Wall")
        {
            Debug.Log("Wall hit");
            this.transform.position -= currentDirection;
            gameManager.WallsHit++;
            uiAnimations.PauseAnimations();
            uiAnimations.StopShader(3);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_BoardImpact");
            SetIsMoving(false);
        }
        // Player hit landmark   
        else if (collider.tag == "Landmark")
        {
            Debug.Log("Landmark reached");
            gameManager.LandmarksReached++;
            gameManager.EnterLandmark(collider.GetComponent<Landmark>());
            SetIsMoving(false);
        }
        /*
        // Player hit special zone in   
        else if (collider.tag == "SpecialZoneIn" && !inSpecialZone)
        {
            Debug.Log("Entering special zone");
            inSpecialZone = true;
            playerColliders.UpdateSpecialZoneDetection();
            speed = secondPerUnitSpecialZone;
            FMODUnity.RuntimeManager.PlayOneShot("event:/AMB/AMB_SpecialZone/AMB_SZ_OutZone/AMB_SZ_OutZone_TrigEnter");
        }
        else if (collider.tag == "SpecialZoneOut" && inSpecialZone)
        {
            Debug.Log("Exiting special zone");
            inSpecialZone = false;
            playerColliders.StopSpecialZoneSound();
            speed = secondPerUnit;
            //FMODUnity.RuntimeManager.PlayOneShot("event:/AMB/AMB_InGame/AMB_IG_SystemMove");
            FMODUnity.RuntimeManager.PlayOneShot("event:/AMB/AMB_SpecialZone/AMB_SZ_OutZone/AMB_SZ_OutZone_TrigExit");
        }*/
    }
    
    public void ChangeDirection(string direction)
    {
        gameManager.ButtonsPressed++;
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
            avatarArrow.transform.rotation = Quaternion.Euler(90, -90, 0);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalRotate_SPAT/SFX_IG_DirectionalRotate_C");
            Debug.Log("Moving foreward");
        }
        else if (direction == "backward")
        {
            currentDirection = Vector3.right * gridCellSize;
            avatarArrow.transform.rotation = Quaternion.Euler(90, 90, 0);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalRotate_SPAT/SFX_IG_DirectionalRotate_B");
            Debug.Log("Moving backward");
        }
        else if (direction == "right")
        {
            currentDirection = Vector3.forward * gridCellSize;
            avatarArrow.transform.rotation = Quaternion.Euler(90, 0, 0);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalRotate_SPAT/SFX_IG_DirectionalRotate_R");
            Debug.Log("Moving right");
        }
        else if (direction == "left")
        {
            currentDirection = Vector3.back * gridCellSize;
            avatarArrow.transform.rotation = Quaternion.Euler(90, 180, 0);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_DirectionalRotate_SPAT/SFX_IG_DirectionalRotate_L");
            Debug.Log("Moving left");
        }

        // Move the player
        if (!isMoving)
        {
            Move();
        }
        
        SetIsMoving(true);
    }

    public void SetInSpecialZone(bool newInSpecialZone)
    {
        // Enter special zone
        if (newInSpecialZone && !isMoving)
        {
            speed = secondPerUnitSpecialZone;
            playerColliders.UpdateSpecialZoneDetection();
            uiAnimations.SetShaderSpeed(1);
        }
        // Exit special zone
        else if (!newInSpecialZone && isMoving)
        {
            speed = secondPerUnit;
            playerColliders.StopSpecialZoneSound();
            uiAnimations.SetShaderSpeed(0.8f);
        }
        
        inSpecialZone = newInSpecialZone;
    }

    bool IsHorizontal(Vector3 direction)
    {
        return direction == Vector3.left * gridCellSize || direction == Vector3.right * gridCellSize;
    }

    bool IsVertical(Vector3 direction)
    {
        return direction == Vector3.forward * gridCellSize || direction == Vector3.back * gridCellSize;
    }
    
    void InstantiateDottedLine(GameObject prefab, Quaternion rotation)
    {
        GameObject line = Instantiate(prefab, transform.position, rotation);
        line.transform.position = new Vector3(transform.position.x, 2, transform.position.z);
        line.transform.parent = PlayerPathDottedLineParent.transform;
    }

    private void Move()
    {
        bool sameAxis = (IsHorizontal(previousDirectionVector) && IsHorizontal(currentDirection)) ||
                        (IsVertical(previousDirectionVector) && IsVertical(currentDirection));

        // First move
        if (!hasMovedOnce)
        {
            InstantiateDottedLine(dottedLinePrefab, Quaternion.Euler(90, 0, 0));
        }
        // Straight dotted line
        else if (sameAxis)
        {
            Quaternion rotation = IsHorizontal(previousDirectionVector)
                ? Quaternion.Euler(90, 0, 0)
                : Quaternion.Euler(90, 90, 0);
            InstantiateDottedLine(dottedLinePrefab, rotation);
        }
        // Curved dotted line
        else
        {
            Quaternion rotation;

            if (previousDirectionVector == Vector3.back * gridCellSize)
                rotation = currentDirection == Vector3.left * gridCellSize ? Quaternion.Euler(90, 180, 0) : Quaternion.Euler(90, -90, 0);
            else if (previousDirectionVector == Vector3.forward * gridCellSize) 
                rotation = currentDirection == Vector3.left * gridCellSize ? Quaternion.Euler(90, 90, 0) : Quaternion.Euler(90, 0, 0);
            else if (previousDirectionVector == Vector3.left * gridCellSize)
                rotation = currentDirection == Vector3.forward * gridCellSize ? Quaternion.Euler(90, -90, 0) : Quaternion.Euler(90, 0, 0);
            else
                rotation = currentDirection == Vector3.forward * gridCellSize ? Quaternion.Euler(90, 180, 0) : Quaternion.Euler(90, 90, 0);
            
            InstantiateDottedLine(dottedLineCurvedPrefab, rotation);
        }
        
        moveAnimation.SetTrigger("trigger");
        unitTimer = 0f;
        this.transform.position += currentDirection;
        gameManager.DistanceTraveled++;
        midSoundPlayed = false;
        if (inSpecialZone)
            playerColliders.UpdateSpecialZoneDetection();
            
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
        
        previousDirectionVector = currentDirection;
    }

    void Update() 
    {
        if (!isMoving) return;
        
        
        unitTimer += Time.deltaTime;
        
        // Direction are still fucked up ! FUCKKK YEAHHHHH
        
        // Mid sound
        if (!midSoundPlayed && unitTimer >= speed / 2)
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
        }
        
        // Move the player to next unit
        if (unitTimer >= speed)
        {
            Move();
        }
    }
}
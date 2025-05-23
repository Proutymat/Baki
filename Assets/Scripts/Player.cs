using System;
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
    [SerializeField] private GameObject specialZonesBoxes;

    private float unitTimer;
    private float gridCellSize;
    private GameManager gameManager;
    private string previousDirection;
    private Vector3 startCellPosition;
    private bool midSoundPlayed;
    private bool hasMovedOnce;
    private float speed;
    
    private MeshRenderer meshRenderer;
    
    // Special zone
    private BoxCollider szBox_FF; // Forward forward
    private BoxCollider szBox_FL; // Forward left
    private BoxCollider szBox_F; // Forward
    private BoxCollider szBox_FR; // Forward right
    private BoxCollider szBox_LL; // Left left
    private BoxCollider szBox_L; // Left
    private BoxCollider szBox_R; // Right
    private BoxCollider szBox_RR; // Right right
    private BoxCollider szBox_BL; // Backward left
    private BoxCollider szBox_B; // Backward
    private BoxCollider szBox_BR; // Backward right
    private BoxCollider szBox_BB; // Backward backward
    
    //Instance Fmod event

    [SerializeField] private FMODUnity.EventReference BA_C;
    [SerializeField] private FMODUnity.EventReference BA_L;
    [SerializeField] private FMODUnity.EventReference BA_LS;
    [SerializeField] private FMODUnity.EventReference BA_LR;
    [SerializeField] private FMODUnity.EventReference BA_R;
    [SerializeField] private FMODUnity.EventReference BA_RS;
    [SerializeField] private FMODUnity.EventReference BA_RR;
    [SerializeField] private FMODUnity.EventReference BA_B;
    FMOD.Studio.EventInstance BA_C_Instance;
    FMOD.Studio.EventInstance BA_L_Instance;
    FMOD.Studio.EventInstance BA_LS_Instance;
    FMOD.Studio.EventInstance BA_LR_Instance;
    FMOD.Studio.EventInstance BA_R_Instance;
    FMOD.Studio.EventInstance BA_RS_Instance;
    FMOD.Studio.EventInstance BA_RR_Instance;
    FMOD.Studio.EventInstance BA_B_Instance;
    
    public bool IsMoving { get { return isMoving; } set { isMoving = value; } }
    
    private void Start()
    {
        gridCellSize = FindFirstObjectByType<CustomGrid>().transform.GetChild(1).localScale.x;
        currentDirection = transform.forward * gridCellSize;
        startCellPosition = GameObject.FindGameObjectWithTag("PlayerStart").transform.position;
        gameManager = GameManager.Instance;
        meshRenderer = GetComponent<MeshRenderer>();
        
        //FMOD Instance start
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_SpecialZone/SFX_SZ_OutZone/SFX_SZ_OZ_BoardAppraoch/SFX_SZ_OZ_BA_Start");
        
        BA_C_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_C);
        BA_L_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_L);
        BA_LS_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_LS);
        BA_LR_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_LR);
        BA_R_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_R);
        BA_RS_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_RS);
        BA_RR_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_RR);
        BA_B_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_B);
        
        BA_C_Instance.start();
        BA_L_Instance.start();
        BA_LS_Instance.start();
        BA_LR_Instance.start();
        BA_R_Instance.start();
        BA_RS_Instance.start();
        BA_RR_Instance.start();
        BA_B_Instance.start();
        
        BA_C_Instance.release();
        BA_L_Instance.release();
        BA_LS_Instance.release();
        BA_LR_Instance.release();
        BA_R_Instance.release();
        BA_RS_Instance.release();
        BA_RR_Instance.release();
        BA_B_Instance.release();
        
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
    }

    public void EnableMeshRenderer(bool enable)
    {
        meshRenderer.enabled = enable;
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
            
            EnableMeshRenderer(false);
        }
        // Player stop moving
        else if (!newMovingValue && isMoving)
        {
            gameManager.UpdateArrowButtonsSprite("stop");
            //FMODUnity.RuntimeManager.PlayOneShot("event:/AMB/AMB_InGame/AMB_IG_SystemStop");
            //FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_BordStop");
        }

        isMoving = newMovingValue;
    }

    public void SetInSpecialZone(bool newInSpecialZone)
    {
        // Enter special zone
        if (newInSpecialZone && !isMoving)
        {
            speed = secondPerUnitSpecialZone;
        }
        // Exit special zone
        else if (!newInSpecialZone && isMoving)
        {
            speed = secondPerUnit;
        }
        
        inSpecialZone = newInSpecialZone;
    }

    public void SZDetectorEnterTriggered(string zoneName)
    {
        switch(zoneName)
        {
            case "FF":
                BA_C_Instance.setParameterByName("BordDistance",2);
                break;
            case "FL":
                // FMOD EVENT
                break;
            case "F":
                // FMOD EVENT
                BA_C_Instance.setParameterByName("BordDistance",1);
                break;
            case "FR":
                // FMOD EVENT
                break;
            case "LL":
                BA_L_Instance.setParameterByName("BordDistance",2);
                break;
            case "L":
                BA_L_Instance.setParameterByName("BordDistance",1);
                break;
            case "R":
                BA_R_Instance.setParameterByName("BordDistance",1);
                break;
            case "RR":
                BA_R_Instance.setParameterByName("BordDistance",2);
                break;
            case "BL":
                // FMOD EVENT
                break;
            case "B":
                BA_B_Instance.setParameterByName("BordDistance",1);
                break;
            case "BR":
                // FMOD EVENT
                break;
            case "BB":
                BA_B_Instance.setParameterByName("BordDistance",2);
                break;
        }
    }
    
    public void SZDetectorExitTriggered(string zoneName)
    {
        switch(zoneName)
        {
            case "FF":
                BA_C_Instance.setParameterByName("BordDistance",4);
                break;
            case "FL":
                // STOP FMOD EVENT
                break;
            case "F":
                BA_C_Instance.setParameterByName("BordDistance",4);
                break;
            case "FR":
                // STOP FMOD EVENT
                break;
            case "LL":
                BA_L_Instance.setParameterByName("BordDistance",4);
                break;
            case "L":
                BA_L_Instance.setParameterByName("BordDistance",4);
                break;
            case "R":
                BA_R_Instance.setParameterByName("BordDistance",4);
                break;
            case "RR":
                BA_R_Instance.setParameterByName("BordDistance",4);
                break;
            case "BL":
                // STOP FMOD EVENT
                break;
            case "B":
                BA_B_Instance.setParameterByName("BordDistance",4);
                break;
            case "BR":
                // STOP FMOD EVENT
                break;
            case "BB":
                BA_B_Instance.setParameterByName("BordDistance",4);
                break;
        }
    }

    public void OnColliderTriggered(string collisionName)
    {
        // Player hit wall
        if (collisionName == "Wall")
        {
            Debug.Log("Wall hit");
            this.transform.position -= currentDirection;
            gameManager.WallsHit++;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_InGame/SFX_IG_BoardImpact");
        }
        // Player hit landmark   
        else if (collisionName == "Landmark")
        {
            Debug.Log("Landmark reached");
            //Destroy(other.gameObject);
            gameManager.LandmarksReached++;
            gameManager.PrintAreaPlayer();
            gameManager.EnterLandmark();
            EnableMeshRenderer(true);
        }
        SetIsMoving(false);
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
            Debug.Log("Mid sound played");
        }
        
        // Move the player to next unit
        if (unitTimer >= speed)
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
    
#if UNITY_EDITOR
    
    [Button, DisableInPlayMode]
    private void CreateSpecialZoneCollider()
    {
        // Destroy all child objects
        Transform[] children = new Transform[specialZonesBoxes.transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = specialZonesBoxes.transform.GetChild(i);
        }

        foreach (Transform child in children)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
        
        
        gridCellSize = FindFirstObjectByType<CustomGrid>().transform.GetChild(1).localScale.x;
        
        // Forward forward
        GameObject szFF = new GameObject("szBoxFF");
        szFF.AddComponent<SpecialZoneDetector>().ZoneName = "FF";   
        szBox_FF = szFF.AddComponent<BoxCollider>();
        szBox_FF.isTrigger = true;
        szFF.transform.localScale = new Vector3(gridCellSize * 1, gridCellSize, gridCellSize);
        szFF.transform.parent = specialZonesBoxes.transform;
        szFF.transform.position = new Vector3(transform.position.x - 2 * gridCellSize, transform.position.y, transform.position.z);
        
        // Forward left
        GameObject szFL = new GameObject("szBoxFL");
        szFL.AddComponent<SpecialZoneDetector>().ZoneName = "FL";
        szBox_FL = szFL.AddComponent<BoxCollider>();
        szBox_FL.isTrigger = true;
        szFL.transform.localScale = new Vector3(gridCellSize * 1, gridCellSize, gridCellSize);
        szFL.transform.parent = specialZonesBoxes.transform;
        szFL.transform.position = new Vector3(transform.position.x - 1 * gridCellSize, transform.position.y, transform.position.z - 1 * gridCellSize);
        
        // Forward
        GameObject szF = new GameObject("szBoxF");
        szF.AddComponent<SpecialZoneDetector>().ZoneName = "F";
        szBox_F = szF.AddComponent<BoxCollider>();
        szBox_F.isTrigger = true;
        szF.transform.localScale = new Vector3(gridCellSize * 1, gridCellSize, gridCellSize);
        szF.transform.parent = specialZonesBoxes.transform;
        szF.transform.position = new Vector3(transform.position.x - 1 * gridCellSize, transform.position.y, transform.position.z);
        
        // Forward right
        GameObject szFR = new GameObject("szBoxFR");
        szFR.AddComponent<SpecialZoneDetector>().ZoneName = "FR";
        szBox_FR = szFR.AddComponent<BoxCollider>();
        szBox_FR.isTrigger = true;
        szFR.transform.localScale = new Vector3(gridCellSize * 1, gridCellSize, gridCellSize);
        szFR.transform.parent = specialZonesBoxes.transform;
        szFR.transform.position = new Vector3(transform.position.x - 1 * gridCellSize, transform.position.y, transform.position.z + 1 * gridCellSize);
        
        
        // Left left
        GameObject szLL = new GameObject("szBoxLL");
        szLL.AddComponent<SpecialZoneDetector>().ZoneName = "LL";
        szBox_LL = szLL.AddComponent<BoxCollider>();
        szBox_LL.isTrigger = true;
        szLL.transform.localScale = new Vector3(gridCellSize * 1, gridCellSize, gridCellSize);
        szLL.transform.parent = specialZonesBoxes.transform;
        szLL.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 2 * gridCellSize);
        
        // Left
        GameObject szL = new GameObject("szBoxL");
        szL.AddComponent<SpecialZoneDetector>().ZoneName = "L";
        szBox_L = szL.AddComponent<BoxCollider>();
        szBox_L.isTrigger = true;
        szL.transform.localScale = new Vector3(gridCellSize * 1, gridCellSize, gridCellSize);
        szL.transform.parent = specialZonesBoxes.transform;
        szL.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1 * gridCellSize);
        
        // Right
        GameObject szR = new GameObject("szBoxR");
        szR.AddComponent<SpecialZoneDetector>().ZoneName = "R";
        szBox_R = szR.AddComponent<BoxCollider>();
        szBox_R.isTrigger = true;
        szR.transform.localScale = new Vector3(gridCellSize * 1, gridCellSize, gridCellSize);
        szR.transform.parent = specialZonesBoxes.transform;
        szR.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1 * gridCellSize);
        
        // Right right
        GameObject szRR = new GameObject("szBoxRR");
        szRR.AddComponent<SpecialZoneDetector>().ZoneName = "RR";
        szBox_RR = szRR.AddComponent<BoxCollider>();
        szBox_RR.isTrigger = true;
        szRR.transform.localScale = new Vector3(gridCellSize * 1, gridCellSize, gridCellSize);
        szRR.transform.parent = specialZonesBoxes.transform;
        szRR.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2 * gridCellSize);
        
        // Backward left
        GameObject szBL = new GameObject("szBoxBL");
        szBL.AddComponent<SpecialZoneDetector>().ZoneName = "BL";
        szBox_BL = szBL.AddComponent<BoxCollider>();
        szBox_BL.isTrigger = true;
        szBL.transform.localScale = new Vector3(gridCellSize * 1, gridCellSize, gridCellSize);
        szBL.transform.parent = specialZonesBoxes.transform;
        szBL.transform.position = new Vector3(transform.position.x + 1 * gridCellSize, transform.position.y, transform.position.z - 1 * gridCellSize);
        
        // Backward
        GameObject szB = new GameObject("szBoxB");
        szB.AddComponent<SpecialZoneDetector>().ZoneName = "B";
        szBox_B = szB.AddComponent<BoxCollider>();
        szBox_B.isTrigger = true;
        szB.transform.localScale = new Vector3(gridCellSize * 1, gridCellSize, gridCellSize);
        szB.transform.parent = specialZonesBoxes.transform;
        szB.transform.position = new Vector3(transform.position.x + 1 * gridCellSize, transform.position.y, transform.position.z);
        
        // Backward right
        GameObject szBR = new GameObject("szBoxBR");
        szBR.AddComponent<SpecialZoneDetector>().ZoneName = "BR";
        szBox_BR = szBR.AddComponent<BoxCollider>();
        szBox_BR.isTrigger = true;
        szBR.transform.localScale = new Vector3(gridCellSize * 1, gridCellSize, gridCellSize);
        szBR.transform.parent = specialZonesBoxes.transform;
        szBR.transform.position = new Vector3(transform.position.x + 1 * gridCellSize, transform.position.y, transform.position.z + 1 * gridCellSize);
        
        // Backward backward
        GameObject szBB = new GameObject("szBoxBB");
        szBB.AddComponent<SpecialZoneDetector>().ZoneName = "BB";
        szBox_BB = szBB.AddComponent<BoxCollider>();
        szBox_BB.isTrigger = true;
        szBB.transform.localScale = new Vector3(gridCellSize * 1, gridCellSize, gridCellSize);
        szBB.transform.parent = specialZonesBoxes.transform;
        szBB.transform.position = new Vector3(transform.position.x + 2 * gridCellSize, transform.position.y, transform.position.z);
    }
#endif
    
}
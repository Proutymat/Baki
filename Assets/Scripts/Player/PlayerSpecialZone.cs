using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class PlayerSpecialZone : MonoBehaviour
{
    
    private Player player;
    
    [SerializeField] private List<SpecialZoneDetector> specialZoneDetectors;
    [SerializeField] private GameObject specialZonesBoxes;
    
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

    [Header("Fmod Events")]
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
    
    private void Start()
    {
        player = GetComponentInParent<Player>();
        
        BA_C_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_C);
        BA_L_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_L);
        BA_LS_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_LS);
        BA_LR_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_LR);
        BA_R_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_R);
        BA_RS_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_RS);
        BA_RR_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_RR);
        BA_B_Instance = FMODUnity.RuntimeManager.CreateInstance(BA_B);
        
        BA_C_Instance.start(); BA_C_Instance.release();
        BA_L_Instance.start(); BA_L_Instance.release();
        BA_LS_Instance.start(); BA_LS_Instance.release();
        BA_LR_Instance.start(); BA_LR_Instance.release();
        BA_R_Instance.start(); BA_R_Instance.release();
        BA_RS_Instance.start(); BA_RS_Instance.release();
        BA_RR_Instance.start(); BA_RR_Instance.release();
        BA_B_Instance.start(); BA_B_Instance.release();
    }
    
    public void StopSpecialZoneSound()
    {
        BA_C_Instance.setParameterByName("BordDistance", 4);
        BA_L_Instance.setParameterByName("BordDistance", 4);
        BA_R_Instance.setParameterByName("BordDistance", 4);
        BA_LS_Instance.setParameterByName("BordDistance", 4);
        BA_RS_Instance.setParameterByName("BordDistance", 4);
        BA_LR_Instance.setParameterByName("BordDistance", 4);
        BA_RR_Instance.setParameterByName("BordDistance", 4);
        BA_B_Instance.setParameterByName("BordDistance", 4);
    }
    
    public void UpdateSpecialZoneDetection()
    {
        foreach (SpecialZoneDetector detector in specialZoneDetectors)
            detector.CheckCollision(); 
        
        // Check FF + F
        if (specialZoneDetectors[2].IsOn)
            BA_C_Instance.setParameterByName("BordDistance", 1);
        else if (specialZoneDetectors[0].IsOn)
            BA_C_Instance.setParameterByName("BordDistance", 3);
        else
            BA_C_Instance.setParameterByName("BordDistance", 4);
        // Check FL
        if (specialZoneDetectors[1].IsOn)
            BA_L_Instance.setParameterByName("BordDistance", 2);
        else
            BA_L_Instance.setParameterByName("BordDistance", 4);
        // Check FR
        if (specialZoneDetectors[3].IsOn)
            BA_R_Instance.setParameterByName("BordDistance", 2);
        else
            BA_R_Instance.setParameterByName("BordDistance", 4);
        // Check LL + L
        if (specialZoneDetectors[5].IsOn)
            BA_LS_Instance.setParameterByName("BordDistance", 1);
        else if (specialZoneDetectors[4].IsOn)
            BA_LS_Instance.setParameterByName("BordDistance", 3);
        else
            BA_LS_Instance.setParameterByName("BordDistance", 4);
        // Check R + RR
        if (specialZoneDetectors[6].IsOn)
            BA_RS_Instance.setParameterByName("BordDistance", 1);
        else if (specialZoneDetectors[7].IsOn)
            BA_RS_Instance.setParameterByName("BordDistance", 3);
        else
            BA_RS_Instance.setParameterByName("BordDistance", 4);
        // Check BL
        if (specialZoneDetectors[8].IsOn)
            BA_LR_Instance.setParameterByName("BordDistance", 2);
        else
            BA_LR_Instance.setParameterByName("BordDistance", 4);
        // Check BR
        if (specialZoneDetectors[10].IsOn)
            BA_RR_Instance.setParameterByName("BordDistance", 2);
        else
            BA_RR_Instance.setParameterByName("BordDistance", 4);
        // Check B + BB
        if (specialZoneDetectors[9].IsOn)
            BA_B_Instance.setParameterByName("BordDistance", 1);
        else if (specialZoneDetectors[11].IsOn)
            BA_B_Instance.setParameterByName("BordDistance", 3);
        else
            BA_B_Instance.setParameterByName("BordDistance", 4);
    }
    
#if UNITY_EDITOR
    
    [Button, DisableInPlayMode]
    private void CreateSpecialZoneCollider()
    {
        specialZoneDetectors.Clear();
        
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
        
        
        float gridCellSize = FindFirstObjectByType<CustomGrid>().transform.GetChild(1).GetChild(0).localScale.x;
        
        // Forward forward
        GameObject szFF = new GameObject("szBoxFF");
        specialZoneDetectors.Add(szFF.AddComponent<SpecialZoneDetector>());
        szFF.GetComponent<SpecialZoneDetector>().ZoneName = "FF";
        szBox_FF = szFF.AddComponent<BoxCollider>();
        szBox_FF.isTrigger = true;
        szFF.transform.localScale = new Vector3(gridCellSize * 0.5f, gridCellSize * 0.5f, gridCellSize * 0.5f);
        szFF.transform.parent = specialZonesBoxes.transform;
        szFF.transform.position = new Vector3(transform.position.x - 2 * gridCellSize, transform.position.y, transform.position.z);
        
        // Forward left
        GameObject szFL = new GameObject("szBoxFL");
        specialZoneDetectors.Add(szFL.AddComponent<SpecialZoneDetector>());
        szFL.GetComponent<SpecialZoneDetector>().ZoneName = "FL";
        szBox_FL = szFL.AddComponent<BoxCollider>();
        szBox_FL.isTrigger = true;
        szFL.transform.localScale = new Vector3(gridCellSize * 0.5f, gridCellSize * 0.5f, gridCellSize * 0.5f);
        szFL.transform.parent = specialZonesBoxes.transform;
        szFL.transform.position = new Vector3(transform.position.x - 1 * gridCellSize, transform.position.y, transform.position.z - 1 * gridCellSize);
        
        // Forward
        GameObject szF = new GameObject("szBoxF");
        specialZoneDetectors.Add(szF.AddComponent<SpecialZoneDetector>());
        szF.GetComponent<SpecialZoneDetector>().ZoneName = "F";
        szBox_F = szF.AddComponent<BoxCollider>();
        szBox_F.isTrigger = true;
        szF.transform.localScale = new Vector3(gridCellSize * 0.5f, gridCellSize * 0.5f, gridCellSize * 0.5f);
        szF.transform.parent = specialZonesBoxes.transform;
        szF.transform.position = new Vector3(transform.position.x - 1 * gridCellSize, transform.position.y, transform.position.z);
        
        // Forward right
        GameObject szFR = new GameObject("szBoxFR");
        specialZoneDetectors.Add(szFR.AddComponent<SpecialZoneDetector>());
        szFR.GetComponent<SpecialZoneDetector>().ZoneName = "FR";
        szBox_FR = szFR.AddComponent<BoxCollider>();
        szBox_FR.isTrigger = true;
        szFR.transform.localScale = new Vector3(gridCellSize * 0.5f, gridCellSize * 0.5f, gridCellSize * 0.5f);
        szFR.transform.parent = specialZonesBoxes.transform;
        szFR.transform.position = new Vector3(transform.position.x - 1 * gridCellSize, transform.position.y, transform.position.z + 1 * gridCellSize);
        
        
        // Left left
        GameObject szLL = new GameObject("szBoxLL");
        specialZoneDetectors.Add(szLL.AddComponent<SpecialZoneDetector>());
        szLL.GetComponent<SpecialZoneDetector>().ZoneName = "LL";
        szBox_LL = szLL.AddComponent<BoxCollider>();
        szBox_LL.isTrigger = true;
        szLL.transform.localScale = new Vector3(gridCellSize * 0.5f, gridCellSize * 0.5f, gridCellSize * 0.5f);
        szLL.transform.parent = specialZonesBoxes.transform;
        szLL.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 2 * gridCellSize);
        
        // Left
        GameObject szL = new GameObject("szBoxL");
        specialZoneDetectors.Add(szL.AddComponent<SpecialZoneDetector>());
        szL.GetComponent<SpecialZoneDetector>().ZoneName = "L";
        szBox_L = szL.AddComponent<BoxCollider>();
        szBox_L.isTrigger = true;
        szL.transform.localScale = new Vector3(gridCellSize * 0.5f, gridCellSize * 0.5f, gridCellSize * 0.5f);
        szL.transform.parent = specialZonesBoxes.transform;
        szL.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1 * gridCellSize);
        
        // Right
        GameObject szR = new GameObject("szBoxR");
        specialZoneDetectors.Add(szR.AddComponent<SpecialZoneDetector>());
        szR.GetComponent<SpecialZoneDetector>().ZoneName = "R";
        szBox_R = szR.AddComponent<BoxCollider>();
        szBox_R.isTrigger = true;
        szR.transform.localScale = new Vector3(gridCellSize * 0.5f, gridCellSize * 0.5f, gridCellSize * 0.5f);
        szR.transform.parent = specialZonesBoxes.transform;
        szR.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1 * gridCellSize);
        
        // Right right
        GameObject szRR = new GameObject("szBoxRR");
        specialZoneDetectors.Add(szRR.AddComponent<SpecialZoneDetector>());
        szRR.GetComponent<SpecialZoneDetector>().ZoneName = "RR";
        szBox_RR = szRR.AddComponent<BoxCollider>();
        szBox_RR.isTrigger = true;
        szRR.transform.localScale = new Vector3(gridCellSize * 0.5f, gridCellSize * 0.5f, gridCellSize * 0.5f);
        szRR.transform.parent = specialZonesBoxes.transform;
        szRR.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2 * gridCellSize);
        
        // Backward left
        GameObject szBL = new GameObject("szBoxBL");
        specialZoneDetectors.Add(szBL.AddComponent<SpecialZoneDetector>());
        szBL.GetComponent<SpecialZoneDetector>().ZoneName = "BL";
        szBox_BL = szBL.AddComponent<BoxCollider>();
        szBox_BL.isTrigger = true;
        szBL.transform.localScale = new Vector3(gridCellSize * 0.5f, gridCellSize * 0.5f, gridCellSize * 0.5f);
        szBL.transform.parent = specialZonesBoxes.transform;
        szBL.transform.position = new Vector3(transform.position.x + 1 * gridCellSize, transform.position.y, transform.position.z - 1 * gridCellSize);
        
        // Backward
        GameObject szB = new GameObject("szBoxB");
        specialZoneDetectors.Add(szB.AddComponent<SpecialZoneDetector>());
        szB.GetComponent<SpecialZoneDetector>().ZoneName = "B";
        szBox_B = szB.AddComponent<BoxCollider>();
        szBox_B.isTrigger = true;
        szB.transform.localScale = new Vector3(gridCellSize * 0.5f, gridCellSize * 0.5f, gridCellSize * 0.5f);
        szB.transform.parent = specialZonesBoxes.transform;
        szB.transform.position = new Vector3(transform.position.x + 1 * gridCellSize, transform.position.y, transform.position.z);
        
        // Backward right
        GameObject szBR = new GameObject("szBoxBR");
        specialZoneDetectors.Add(szBR.AddComponent<SpecialZoneDetector>());
        szBR.GetComponent<SpecialZoneDetector>().ZoneName = "BR";
        szBox_BR = szBR.AddComponent<BoxCollider>();
        szBox_BR.isTrigger = true;
        szBR.transform.localScale = new Vector3(gridCellSize * 0.5f, gridCellSize * 0.5f, gridCellSize * 0.5f);
        szBR.transform.parent = specialZonesBoxes.transform;
        szBR.transform.position = new Vector3(transform.position.x + 1 * gridCellSize, transform.position.y, transform.position.z + 1 * gridCellSize);
        
        // Backward backward
        GameObject szBB = new GameObject("szBoxBB");
        specialZoneDetectors.Add(szBB.AddComponent<SpecialZoneDetector>());
        szBB.GetComponent<SpecialZoneDetector>().ZoneName = "BB";
        szBox_BB = szBB.AddComponent<BoxCollider>();
        szBox_BB.isTrigger = true;
        szBB.transform.localScale = new Vector3(gridCellSize * 0.5f, gridCellSize * 0.5f, gridCellSize * 0.5f);
        szBB.transform.parent = specialZonesBoxes.transform;
        szBB.transform.position = new Vector3(transform.position.x + 2 * gridCellSize, transform.position.y, transform.position.z);
    }
#endif
    
}

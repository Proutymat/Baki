using UnityEngine;

public class Landmark : MonoBehaviour
{
    [SerializeField] private int type; public int Type { get { return type; } }
    [SerializeField] private GameObject landmarkCheckedPrefab;

    void Start()
    {
        landmarkCheckedPrefab = Resources.Load<GameObject>("Prefabs/LandmarkChecked");
        if (landmarkCheckedPrefab == null)
        {
            Debug.LogError("Landmark checked prefab not found!");
        }
    }
    
    public void Init(int type)
    {
        this.type = type;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Player player = collider.GetComponent<Player>();
            Enter();
        }
    }

    public void Enter()
    {
        if (type == 0)
            FMODUnity.RuntimeManager.PlayOneShot("event:/MX/MX_Trig/MX_Trig_Z1G/MX_Trig_Z1G_PI_Start");
        else if (type == 1)
            FMODUnity.RuntimeManager.PlayOneShot("event:/MX/MX_Trig/MX_Trig_Z3B/MX_Trig_Z3B_PI_Start");
        else if (type == 2)
            FMODUnity.RuntimeManager.PlayOneShot("event:/MX/MX_Trig/MX_Trig_Z2P/MX_Trig_Z2P_PI_Start");
        else if (type == 3)
            FMODUnity.RuntimeManager.PlayOneShot("event:/MX/MX_Trig/MX_Trig_Z4H/MX_Trig_Z4H_PI_Start");
    }
    
    public void Exit()
    {
        if (type == 0)
            FMODUnity.RuntimeManager.PlayOneShot("event:/MX/MX_Trig/MX_Trig_Z1G/MX_Trig_Z1G_PI_Stop");
        if (type == 1)
            FMODUnity.RuntimeManager.PlayOneShot("event:/MX/MX_Trig/MX_Trig_Z3B/MX_Trig_Z3B_PI_Stop");
        if (type == 2)
            FMODUnity.RuntimeManager.PlayOneShot("event:/MX/MX_Trig/MX_Trig_Z2P/MX_Trig_Z2P_PI_Stop");
        if (type == 3)
            FMODUnity.RuntimeManager.PlayOneShot("event:/MX/MX_Trig/MX_Trig_Z4H/MX_Trig_Z4H_PI_Stop");
        
        // Instantiate the checked landmark prefab
        GameObject checkedLandmark = Instantiate(landmarkCheckedPrefab, transform.position, Quaternion.identity);
        checkedLandmark.transform.localScale = transform.localScale;
        checkedLandmark.transform.rotation = transform.rotation;
        checkedLandmark.transform.parent = transform.parent;
        
        Destroy(this.gameObject);
    }
}

using UnityEngine;

public class SpecialZoneDetector : MonoBehaviour
{
    [SerializeField] private string zoneName;
    private Player player;
    
    public string ZoneName {set { zoneName = value; } }


    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Zone touchée : " + zoneName);
        player.SZDetectorEnterTriggered(zoneName);
    }
    
    private void OnTriggerExit(Collider other)
    {
        player.SZDetectorExitTriggered(zoneName);
    }
}
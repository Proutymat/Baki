using System;
using UnityEngine;

public class SpecialZoneDetector : MonoBehaviour
{
    [SerializeField] private string zoneName;
    private bool isOn;
    private Player player;
    
    public string ZoneName {set { zoneName = value; } get { return zoneName; } }
    public bool IsOn { get { return isOn; }}


    private void Start()
    {
        player = GetComponentInParent<Player>();
        isOn = false;
    }

    public bool CheckCollision()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);
        
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                Debug.Log("Collision with wall detected in " + zoneName);
                return isOn = true;
            }
        }

        return isOn = false;
    }
}
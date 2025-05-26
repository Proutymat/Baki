using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    private Player player;
    
    private void Start()
    {
        player = GetComponentInParent<Player>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        player.OnColliderTriggered(other);
    }
}
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private bool isMoving;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 currentDirection;

    private void Start()
    {
        isMoving = false;
        currentDirection = transform.forward;
    }

    public void SetIsMoving(bool isMoving)
    {
        this.isMoving = isMoving;
    }
    
    public void ChangeSpeed(int speed)
    {
        this.speed = speed;
    }
    
    public void ChangeDirection(string direction)
    {
        if (direction == "left")
            currentDirection = Vector3.left;
        else if (direction == "right")
            currentDirection = Vector3.right;
        else if (direction == "foreward")
            currentDirection = Vector3.forward;
        else if (direction == "backward")
            currentDirection = Vector3.back;
    }

    void FixedUpdate()
    {
        if (isMoving)
            this.transform.Translate(currentDirection * this.speed);
    }
}
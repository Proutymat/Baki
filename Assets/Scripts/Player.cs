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

    private void Start()
    {
        isMoving = false;
        timer = 0f;
        gridCellSize = FindFirstObjectByType<CustomGrid>().transform.GetChild(0).localScale.x;
        currentDirection = transform.forward * gridCellSize;

        Vector3 startPos = GameObject.FindGameObjectWithTag("PlayerStart").transform.position;
        transform.position = new Vector3(startPos.x, startPos.y + gridCellSize / 2, startPos.z);
    }

    public void SetIsMoving(bool isMoving)
    {
        this.isMoving = isMoving;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Wall hit");
        this.transform.position -= currentDirection;
        SetIsMoving(false);
    }

    public void ChangeSpeed(float secondPerUnit)
    {
        this.secondPerUnit = secondPerUnit;
    }
    
    public void ChangeDirection(string direction)
    {
        SetIsMoving(true);
        
        if (direction == "left")
            currentDirection = Vector3.left * gridCellSize;
        else if (direction == "right")
            currentDirection = Vector3.right * gridCellSize;
        else if (direction == "foreward")
            currentDirection = Vector3.forward * gridCellSize;
        else if (direction == "backward")
            currentDirection = Vector3.back * gridCellSize;
    }

    void Update() {
        
        timer += Time.deltaTime;
        
        if (isMoving && timer >= secondPerUnit)
        {
            timer = 0f;
            this.transform.position += currentDirection;
        }

    }
}
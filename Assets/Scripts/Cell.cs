using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{   
    private int _type; // 1 = player start, 2 = ground, 3 = wall, 4 = landmark A, 5 = landmark B, 6 = landmark C, 7 = landmark D, 8 = landmark E
    
    public void Init(float x, float y, float z, float cellSize, Mesh mesh, int type)
    {
        _type = type;
        transform.position = new Vector3(x * cellSize, y - cellSize / 2, z * cellSize);
        
        // Create the mesh
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        
        // Create the renderer
        var GM = GameManager.Instance;
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = _type switch
        {
            1 => GM.materialStart,
            2 => GM.materialGround,
            3 => GM.materialWall,
            4 => GM.materialLandmarksA,
            5 => GM.materialLandmarksB,
            6 => GM.materialLandmarksC,
            7 => GM.materialLandmarksD,
            8 => GM.materialLandmarksE,
            _ => null
        };

        // Create the collider if the cell is a wall or landmark
        if (type == 3)
        {
            gameObject.AddComponent<BoxCollider>();
            gameObject.tag = "Wall";
        }
        else if (type >= 4)
        {
            gameObject.AddComponent<BoxCollider>();
            gameObject.tag = "Landmark";
        }
        else if (type == 1)
            gameObject.tag = "PlayerStart";
    }
}
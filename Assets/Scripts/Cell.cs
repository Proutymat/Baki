using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private GameObject _placedObject;
    
    public void Init(float x, float y, float z, float cellSize, Material material, Mesh mesh)
    {
        transform.position = new Vector3(x * cellSize, y - cellSize / 2, z * cellSize);
        
        // Create the mesh
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        
        // Create the renderer
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        if (material != null)
        {
            meshRenderer.material = material;
        }

        // Create the collider if the cell is a wall
        if (y != 0)
            gameObject.AddComponent<BoxCollider>();
    }
}
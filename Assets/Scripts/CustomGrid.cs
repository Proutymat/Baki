using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class CustomGrid : MonoBehaviour
{
    [SerializeField] private float _cellSize = 1f;
    [SerializeField] private int _xSize = 10;
    [SerializeField] private int _ySize = 10;
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _materialGround;
    [SerializeField] private Material _materialWall;
    [SerializeField] private bool _drawGizmos = true;
    
    private List<Cell> _grounds = new List<Cell>();
    private List<Cell> _walls = new List<Cell>();

    private List<int> _groundsIndex;
    
    public float CellSize { get { return _cellSize; } }
    public int XSize { get { return _xSize; } }
    public int YSize { get { return _ySize; } }


    private void ClearGrid()
    {
        // Clear grounds
        foreach (Cell cell in _grounds)
        {
            if (Application.isPlaying)
                Destroy(cell.gameObject);
            else
                DestroyImmediate(cell.gameObject);
        }
        
        _grounds.Clear();
        
        // Clear walls
        foreach (Cell cell in _walls)
        {
            if (Application.isPlaying)
                Destroy(cell.gameObject);
            else
                DestroyImmediate(cell.gameObject);
        }
        
        _walls.Clear();
    }
    
    [Button]
    void GenerateGrid()
    {
         ClearGrid();
         
         _groundsIndex = new List<int>(_xSize * _ySize);
    
         for (int i = 0; i < _xSize * _ySize; i++)
         {
             _groundsIndex.Add(i % 4);
         }
         
         Debug.Log("COUNT " + _groundsIndex.Count);
         
         int size = _groundsIndex.Count;
         
         Debug.Log("SIZE " + size);
        
        // Generate grid
        for (int x = 0; x < _xSize; x++)
        {
            for (int z = 0; z < _ySize; z++)
            {
                // GROUNDS
                GameObject cellObject = new GameObject("Cell(" + x + "," + z + ")");
                Cell cell = cellObject.AddComponent<Cell>();
                cellObject.transform.parent = this.transform;
                cellObject.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
                cell.Init(x, 0, z, _cellSize, _materialGround, _mesh);
                _grounds.Add(cell);
                
                // WALLS
                if (_groundsIndex[x * _xSize + z] == 1)
                {
                    Debug.Log("zzz");
                    GameObject wallObject = new GameObject("Wall(" + x + "," + z + ")");
                    Cell wallCell = wallObject.AddComponent<Cell>();
                    wallObject.transform.parent = this.transform;
                    wallObject.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
                    wallCell.Init(x, _cellSize, z, _cellSize, _materialWall, _mesh);
                    _walls.Add(wallCell);
                }
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (!_drawGizmos) return;
        
        Gizmos.color = Color.white;
        for (int x = 0; x < _xSize; x++)
        {
            for (int y = 0; y < _ySize; y++)
            {
                Vector3 cellPos = new Vector3(x * _cellSize, 0, y * _cellSize);
                Gizmos.DrawWireCube(cellPos, new Vector3(_cellSize, 0, _cellSize));
            }
        }
    }
}
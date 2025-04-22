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
    [SerializeField] private Material _materialStart;
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField] private string _mapFileName;
    
    private List<Cell> _grounds = new List<Cell>();
    private List<Cell> _walls = new List<Cell>();
    private List<int> _wallsIndex;

    private int _xStart;
    private int _zStart;
    
    public float CellSize { get { return _cellSize; } }
    public int XSize { get { return _xSize; } }
    public int YSize { get { return _ySize; } }

    [Button]
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
         LoadCSV();
         ClearGrid();
         
         _xSize = Mathf.Clamp(_xSize, 1, 150);
         _ySize = Mathf.Clamp(_ySize, 1, 150);
         
        
         int size = _wallsIndex.Count;
         
        // Generate grid
        for (int x = 0; x < _ySize; x++)
        {
            for (int z = 0; z < _xSize; z++)
            {
                // GROUNDS
                GameObject cellObject = new GameObject("Cell(" + x + "," + z + ")");
                Cell cell = cellObject.AddComponent<Cell>();
                cellObject.transform.parent = this.transform;
                cellObject.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
                cell.Init(x, 0, z, _cellSize, _materialGround, _mesh, 0);
                _grounds.Add(cell);
                
                // WALLS
                if (_wallsIndex[x * _xSize + z] == 1)
                {
                    GameObject wallObject = new GameObject("Wall(" + x + "," + z + ")");
                    Cell wallCell = wallObject.AddComponent<Cell>();
                    wallObject.transform.parent = this.transform;
                    wallObject.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
                    wallCell.Init(x, _cellSize, z, _cellSize, _materialWall, _mesh, 1);
                    _walls.Add(wallCell);
                }
                else if (_wallsIndex[x * _xSize + z] == 3)
                {
                    GameObject startObject = new GameObject("PlayerStart(" + x + "," + z + ")");
                    Cell startCell = startObject.AddComponent<Cell>();
                    startObject.transform.parent = this.transform;
                    startObject.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
                    startCell.Init(x, _cellSize, z, _cellSize, _materialStart, _mesh, 3);
                }
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (!_drawGizmos) return;
        
        Gizmos.color = Color.white;
        for (int x = 0; x < _ySize; x++)
        {
            for (int y = 0; y < _xSize; y++)
            {
                Vector3 cellPos = new Vector3(x * _cellSize, 0, y * _cellSize);
                Gizmos.DrawWireCube(cellPos, new Vector3(_cellSize, 0, _cellSize));
            }
        }
    }

    
    void Awake()
    {
        // If the grid is empty, generate it
        if (gameObject.transform.childCount == 0)
        {
            GenerateGrid();
        }
    }

   
    
    private void LoadCSV()
    {
        
        Debug.Log("Loading CSV file: " + _mapFileName);
        TextAsset csvFile = Resources.Load<TextAsset>(_mapFileName);
        if (csvFile == null)
        {
            Debug.LogError($"CSV file '{_mapFileName}.csv' not found in Resources folder.");
            return;
        }

        _wallsIndex = new List<int>();

        string[] lines = csvFile.text.Split('\n');
        _ySize = lines.Length - 1; 
        _xSize = 0;

        for (int y = 0; y < _ySize; y++)
        {
            string line = lines[y].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(','); // Utilisation de tabulation comme séparateur ici
            if (_xSize == 0)
                _xSize = values.Length;

            foreach (string symbol in values)
            {
                int mappedValue = symbol.Trim() switch
                {
                    "X" => 1,
                    "." => 0,
                    "O" => 2,
                    "S" => 3,
                    _ => 0 // Par défaut
                };
                _wallsIndex.Add(mappedValue);
            }
        }
    }
}
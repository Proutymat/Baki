using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class CustomGrid : MonoBehaviour
{
    [Header("Assigned Objects")]
    [SerializeField] private Mesh _mesh;
    
    [Header("Grid Settings")]
    [SerializeField] private float _cellSize = 1f;
    [SerializeField] private int _landmarkSize = 3;
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField] private string _mapFileName;
    
    private List<int> _wallsIndex;

    private int _xSize = 10;
    private int _ySize = 10;
    private int _xStart;
    private int _zStart;
        
    public float CellSize { get { return _cellSize; } }
    public int XSize { get { return _xSize; } }
    public int YSize { get { return _ySize; } }

    
    [Button]
    private void ClearGrid()
    {
        // Destroy all child objects
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = transform.GetChild(i);
        }

        foreach (Transform child in children)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
        
        Debug.Log("Grid cleared.");
    }
    
    [Button]
    void GenerateGrid()
    {
         if (LoadCSV() == 0)
         {
             Debug.LogError("Grid generation failed. Check the CSV file.");
             return;
         }
         ClearGrid();
         
         _xSize = Mathf.Clamp(_xSize, 1, 150);
         _ySize = Mathf.Clamp(_ySize, 1, 150);
         
        // Ground
         GameObject ground = new GameObject("Ground");
         Cell groundCell = ground.AddComponent<Cell>();
         ground.transform.parent = this.transform;
         ground.transform.localScale = new Vector3(_cellSize * _ySize, _cellSize, _cellSize * _xSize);
         groundCell.Init(0 + _ySize / 2f, 0, 0 + _xSize / 2f, _cellSize, _mesh, 2);
         
        // Generate grid
        for (int x = 0; x < _ySize; x++)
        {
            for (int z = 0; z < _xSize; z++)
            {
                if (_wallsIndex[x * _xSize + z] == 2) continue; // Skip ground cells
                
                GameObject cellObject = new GameObject("Cell(" + x + "," + z + ")");
                Cell cell = cellObject.AddComponent<Cell>();
                cellObject.transform.parent = this.transform;
                
                // Landmarks
                if (_wallsIndex[x * _xSize + z] >= 4)
                {
                    cellObject.transform.localScale = new Vector3(_cellSize * _landmarkSize, _cellSize, _cellSize * _landmarkSize);
                }
                // Walls and start
                else
                {
                    cellObject.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
                }
                
                cell.Init(x, _cellSize, z, _cellSize, _mesh, _wallsIndex[x * _xSize + z]);
            }
        }
        
        Debug.Log("Grid successfully generated !");
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

   
    
    private int LoadCSV()
    {
        bool hasStartCell = false;
        
        TextAsset csvFile = Resources.Load<TextAsset>(_mapFileName);
        if (csvFile == null)
        {
            Debug.LogError($"CSV file '{_mapFileName}.csv' not found in Resources folder.");
            return 0;
        }

        _wallsIndex = new List<int>();

        string[] lines = csvFile.text.Split('\n');
        _ySize = lines.Length - 1; 
        _xSize = 0;

        for (int y = 0; y < _ySize; y++)
        {
            string line = lines[y].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            string[] values = line.Split(';'); // Utilisation de tabulation comme séparateur ici
            Debug.Log("Y : " + y + " VALUES : " + string.Join(", ", values));
            if (_xSize == 0)
                _xSize = values.Length;

            foreach (string symbol in values)
            {
                int mappedValue = symbol.Trim() switch
                {
                    "S" => 1,
                    "" => 2,
                    "X" => 3,
                    "A" => 4,
                    "B" => 5,
                    "C" => 6,
                    "D" => 7,
                    "E" => 8,
                    _ => 0 // Par défaut
                };
                Debug.Log("Mapped value: " + symbol);
                _wallsIndex.Add(mappedValue);
                
                if (mappedValue == 1)
                    hasStartCell = true;
            }
        }
        
        if (!hasStartCell)
        {
            Debug.LogError("No start cell found in the CSV file.");
            return 0;
        }
        
        Debug.Log(_mapFileName + ".csv correctly loaded.");
        return 1;
    }
}
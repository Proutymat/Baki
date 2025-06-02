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
         
        GameObject walls = new GameObject("WALLS");
        walls.transform.parent = this.transform;
        GameObject landmarks = new GameObject("LANDMARKS");
        landmarks.transform.parent = this.transform;
        GameObject specialZonesIn = new GameObject("SPECIAL_ZONES_IN");
        specialZonesIn.transform.parent = this.transform;
        GameObject specialZonesOut = new GameObject("SPECIAL_ZONES_OUT");
        specialZonesOut.transform.parent = this.transform;
         
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
                if (_wallsIndex[x * _xSize + z] >= 400)
                {
                    int value = _wallsIndex[x * _xSize + z];
                    int width = (value % 100) / 10;
                    int height = value % 10;
                    cellObject.transform.localScale = new Vector3(_cellSize * height, _cellSize, _cellSize * width);
                }
                // Walls, start and special zones in/out
                else
                {
                    cellObject.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
                }
                
                cell.Init(x, _cellSize, z, _cellSize, _mesh, _wallsIndex[x * _xSize + z]);

                if (_wallsIndex[x * _xSize + z] == 3)
                {
                    cellObject.transform.parent = walls.transform;
                }
                else if (_wallsIndex[x * _xSize + z] == 9)
                {
                    cellObject.transform.parent = specialZonesIn.transform;
                }
                else if (_wallsIndex[x * _xSize + z] == 10)
                {
                    cellObject.transform.parent = specialZonesOut.transform;
                }
                else if (_wallsIndex[x * _xSize + z] >= 400)
                {
                    cellObject.transform.parent = landmarks.transform;
                }
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
                string trimmed = symbol.Trim();
                int mappedValue = 0;

                if (string.IsNullOrEmpty(trimmed))
                {
                    mappedValue = 2; // Cellule vide (terrain)
                }
                else if (trimmed == "S")
                {
                    mappedValue = 1; // Start
                    hasStartCell = true;
                }
                else if (trimmed.StartsWith("X"))
                {
                    mappedValue = 3;
                }
                else if (trimmed.StartsWith("I"))
                {
                    mappedValue = 9; // In special zone
                }
                else if (trimmed.StartsWith("O"))
                {
                    mappedValue = 10; // Out special zone
                }
                else
                {
                    char prefix = trimmed[0];
                    string numberPart = trimmed.Substring(1);

                    if (int.TryParse(numberPart, out int number))
                    {
                        // First number is the landmark type, second is the width, third is the height
                        // A45 -> 445, B23 -> 523, etc. 
                        switch (prefix)
                        {
                            case 'A': mappedValue = 400 + number; break;
                            case 'B': mappedValue = 500 + number; break;
                            case 'C': mappedValue = 600 + number; break;
                            case 'D': mappedValue = 700 + number; break;
                            case 'E': mappedValue = 800 + number; break;
                            default: mappedValue = 0; break;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Unrecognized symbol format: {trimmed}");
                        mappedValue = 0;
                    }
                }

                Debug.Log("Mapped value: " + symbol + " -> " + mappedValue);
                _wallsIndex.Add(mappedValue);
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
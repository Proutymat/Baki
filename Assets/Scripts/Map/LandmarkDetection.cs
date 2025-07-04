using System;
using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;
using Sirenix.OdinInspector;

public class LandmarkDetection : MonoBehaviour
{
    [SerializeField, ReadOnly] List<GameObject> _landmarks = new List<GameObject>();
    [SerializeField] private GameObject _arrowTemplate;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private int _rangeLandDetection = 1;
    [SerializeField] private List<GameObject> _currentArrows = new List<GameObject>();
    [SerializeField] private float _distanceArrow = 5f;
    
    [Header("Flèche dynamique")]
    [SerializeField] private float _minArrowScale = 0.3f;
    [SerializeField] private float _maxArrowScale = 2.0f;
    [SerializeField] private float _maxDetectionDistance = 100f;

    [Header("SET IN INSPECTOR")]
    [SerializeField] private Camera playerCamera;


    private void Awake()
    { 
        CalculateLandmarks();
    }
    
    // TO CHANGE :
    public void CalculateLandmarks()
    {
        _landmarks.Clear();
        _landmarks.AddRange(GameObject.FindGameObjectsWithTag("Landmark"));
        _landmarks.RemoveAll(l => l == null);
    }

    public void UpdateLandmarkArrows(bool showAllArrows)
    {
        CalculateLandmarks();
        
        _landmarks.Sort((GameObject a, GameObject b) =>
        {
            float distanceA = Vector3.Distance(_playerTransform.position, a.transform.position);
            float distanceB = Vector3.Distance(_playerTransform.position, b.transform.position);
            if (distanceA < distanceB)
                return -1;
            else if (distanceA > distanceB)
                return 1;
            else
                return 0;
        });
        
        for(int i = 0; i < _rangeLandDetection; i++)
        {
            GameObject landmark = _landmarks[i];
            float distance = Vector3.Distance(_playerTransform.position, landmark.transform.position);
            
            if (_currentArrows.Count <= i)
            {
                GameObject obj = GameObject.Instantiate(_arrowTemplate, _playerTransform.position, _arrowTemplate.transform.rotation, transform);
                _currentArrows.Add(obj);
                Debug.Log("Arrow created: " + obj.name);
            }
            GameObject arrow = _currentArrows[i];
            
            Vector3 dir = Vector3.ProjectOnPlane((landmark.transform.position - _playerTransform.position), Vector3.up).normalized;
            arrow.transform.position = _playerTransform.position + dir * _distanceArrow + Vector3.up * 3;
            arrow.transform.rotation = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(-90, 0, 90); 
            
            // Change the scale of the arrow based on the distance
            float t = Mathf.Clamp01(distance / _maxDetectionDistance);
            float scale = Mathf.Lerp(_maxArrowScale, _minArrowScale, t);
            arrow.transform.localScale = new Vector3(_arrowTemplate.transform.localScale.x * scale, _arrowTemplate.transform.localScale.y * scale, _arrowTemplate.transform.localScale.z * scale);
            
            // Check if the landmark is in view of the camera
            Vector3 viewportPos = playerCamera.WorldToViewportPoint(landmark.transform.position);
            bool isInView = viewportPos.z > 0 &&
                            viewportPos.x > 0 && viewportPos.x < 1 &&
                            viewportPos.y > 0 && viewportPos.y < 1;

            if (!showAllArrows && isInView)
                _currentArrows[i].SetActive(false);
            else
                _currentArrows[i].SetActive(true);
        }
    }
}

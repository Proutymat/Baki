using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class LandmarkDetection : MonoBehaviour
{
    
    [Title("Parameters")]
    [SerializeField] private int m_nbArrowToDisplay;
    [SerializeField] private float m_distanceFromPlayer = 5f;
    [SerializeField] private float m_minArrowScale = 0.3f;
    [SerializeField] private float m_maxArrowScale = 2.0f;
    [SerializeField] private float m_maxDetectionDistance = 100f;

    [Title("Set in inspector")]
    [SerializeField] private Camera m_playerCamera;
    [SerializeField] private GameObject m_arrowTemplate;
    [SerializeField] private Transform m_playerTransform;
    
    [Title("Debug"), SerializeField] private bool m_debug;
    [SerializeField, ShowIf("m_debug"), ReadOnly] private List<GameObject> m_landmarks;
    [SerializeField, ShowIf("m_debug")] private List<GameObject> _currentArrows;
    [SerializeField, ShowIf("m_debug")] private float m_navMeshSampleRadius = 5f;
    

    // Rayon de recherche pour "snapper" une position au NavMesh
    

    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Awake()
    {
        CalculateLandmarks();
    }
    
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    public void CalculateLandmarks()
    {
        m_landmarks.Clear();
        m_landmarks.AddRange(GameObject.FindGameObjectsWithTag("Landmark"));
        m_landmarks.RemoveAll(l => l == null);
    }
    
    private Vector3? GetPathfindingDirection(Vector3 from, Vector3 to)
    {
        // Snap positions to the NavMesh
        NavMeshHit hitFrom, hitTo;

        if (!NavMesh.SamplePosition(from, out hitFrom, m_navMeshSampleRadius, NavMesh.AllAreas))
            return null;

        if (!NavMesh.SamplePosition(to, out hitTo, m_navMeshSampleRadius, NavMesh.AllAreas))
            return null;

        NavMeshPath path = new NavMeshPath();
        bool found = NavMesh.CalculatePath(hitFrom.position, hitTo.position, NavMesh.AllAreas, path);

        if (!found || path.status == NavMeshPathStatus.PathInvalid || path.corners.Length < 2)
            return null;

        // path.corners[0] = start position, path.corners[1] = first waypoint
        Vector3 nextWaypoint = path.corners[1];
        Vector3 dir = Vector3.ProjectOnPlane(nextWaypoint - from, Vector3.up).normalized;

        return dir == Vector3.zero ? null : (Vector3?)dir;
    }

    public void UpdateLandmarkArrows(bool showAllArrows)
    {
        CalculateLandmarks();

        // Sort landmarks by distance
        m_landmarks.Sort((a, b) =>
        {
            float dA = Vector3.Distance(m_playerTransform.position, a.transform.position);
            float dB = Vector3.Distance(m_playerTransform.position, b.transform.position);
            return dA.CompareTo(dB);
        });

        // Update every arrow (m_nbArrowToDisplay)
        for (int i = 0; i < m_nbArrowToDisplay && i < m_landmarks.Count; i++)
        {
            GameObject landmark = m_landmarks[i];
            float distance = Vector3.Distance(m_playerTransform.position, landmark.transform.position);

            // Create arrow if dont exists
            if (_currentArrows.Count <= i)
            {
                GameObject obj = Instantiate(m_arrowTemplate, m_playerTransform.position,
                    m_arrowTemplate.transform.rotation, transform);
                _currentArrows.Add(obj);
            }

            GameObject arrow = _currentArrows[i];

            // --- PATHFINDING ---
            Vector3? pathDir = GetPathfindingDirection(m_playerTransform.position, landmark.transform.position);

            // Fallback : direct direction if pathfinding fails
            Vector3 dir = pathDir ?? Vector3.ProjectOnPlane(
                landmark.transform.position - m_playerTransform.position, Vector3.up).normalized;
            // -------------------

            // Position and rotation
            arrow.transform.position = m_playerTransform.position + dir * m_distanceFromPlayer + Vector3.up * 3;
            arrow.transform.rotation = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(-90, 0, 90);

            // Scale based on distance
            float t = Mathf.Clamp01(distance / m_maxDetectionDistance);
            float scale = Mathf.Lerp(m_maxArrowScale, m_minArrowScale, t);
            Vector3 baseScale = m_arrowTemplate.transform.localScale;
            arrow.transform.localScale = baseScale * scale;

            // Hide if landmark is on camera
            Vector3 viewportPos = m_playerCamera.WorldToViewportPoint(landmark.transform.position);
            bool isInView = viewportPos.z > 0 &&
                            viewportPos.x > 0 && viewportPos.x < 1 &&
                            viewportPos.y > 0 && viewportPos.y < 1;

            arrow.SetActive(showAllArrows || !isInView);
        }
    }
}
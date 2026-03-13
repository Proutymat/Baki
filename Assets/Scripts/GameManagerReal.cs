using Sirenix.OdinInspector;
using UnityEngine;

public class GameManagerReal : MonoBehaviour
{
    [Title("Game Settings")]
    [SerializeField] private float gameDuration = 600;
    [SerializeField] private int printIntervalsInPercent;
    [SerializeField] private int maxLawsInterval = 5;
    [SerializeField] private int intervalBetweenTutorials = 3;
}

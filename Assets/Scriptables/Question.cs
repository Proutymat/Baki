using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Question", menuName = "Scriptable Objects/Question")]
public class Question : ScriptableObject
{
    public string question;
    public string answer1;
    public string answer2;
    public int answer1Increment;
    public int answer2Increment;
    public int type; // 0 = No category, 1 = Value 1, 2 = Value 2, 3 = Value 3, 4 = Value 4
    
    // STATS
    public int nbQuestionAsked;
    public int nbAnswer1;
    public int nbAnswer2;
    public float timeSpentAnswering;

}

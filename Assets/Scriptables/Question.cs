using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Question", menuName = "Scriptable Objects/Question")]
public class Question : ScriptableObject
{
    public string question; 
    public string answer1;
    public string answer2;
    public int answer1Type1;
    public int answer1Type1ADD;
    public int answer2Type1;
    public int answer2Type1ADD;
    public int answer1Type2;
    public int answer1Type2ADD;
    public int answer2Type2;
    public int answer2Type2ADD;
    public string answer1Illustration;
    public int answer1IllustrationPriority;
    public string answer2Illustration;
    public int answer2IllustrationPriority;
}

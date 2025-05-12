using UnityEngine;

[CreateAssetMenu(fileName = "Dilemme", menuName = "Scriptable Objects/Dilemme")]
public class Dilemme : ScriptableObject
{
    public string question;
    public string answer1;
    public string answer2;
    public string answer3;
    public string answer4;
    
    public int nbAnswer1;
    public int nbAnswer2;
    public int nbAnswer3;
    public int nbAnswer4;
}

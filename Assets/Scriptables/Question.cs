using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Question", menuName = "Scriptable Objects/Question")]
public class Question : ScriptableObject
{
    public string question;
    public List<string> answers;
    public int type; // 0 = none, 1 = Tu préfères, 2 = Dilemme, 3 = Oui ou Non
    public int landmarkType; // 0 = none, 1 = A, 2 = B, 3 = C, 4 = D, 5 = E
}

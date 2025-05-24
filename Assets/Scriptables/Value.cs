using UnityEngine;

[CreateAssetMenu(fileName = "Value", menuName = "Scriptable Objects/Value")]
public class Value : ScriptableObject
{
    public string valueName;
    public string law1; // Law -3
    public int law1Priority;
    public string law2; // Law -2
    public int law2Priority;
    public string law3; // Law -1
    public int law3Priority;
    public string law4; // Law 1
    public int law4Priority;
    public string law5; // Law 2
    public int law5Priority;
    public string law6; // Law 3
    public int law6Priority;
}

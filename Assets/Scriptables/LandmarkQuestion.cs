using UnityEngine;

[CreateAssetMenu(fileName = "Landmark", menuName = "Scriptable Objects/Landmark")]
public class LandmarkQuestion : ScriptableObject
{
    public string text1;
    public string text2;
    public string text3;
    public string text4;
    public string text5;
    public string text6;
    
    public string answer1;
    public string answer2;
    
    public int type;
    public int nbTexts;
}

using UnityEngine;

[CreateAssetMenu(fileName = "Landmark", menuName = "Scriptable Objects/Landmark")]
public class LandmarkQuestion : ScriptableObject
{
    // French
    public string text1;
    public string text2;
    public string text3;
    public string text4;
    public string text5;
    public string text6;
    public string answer1;
    public string answer2;
    
    // English
    public string text1_EN;
    public string text2_EN;
    public string text3_EN;
    public string text4_EN;
    public string text5_EN;
    public string text6_EN;
    public string answer1_EN;
    public string answer2_EN;
    
    public int type;
    public int nbTexts;
}

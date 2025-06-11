using System;
using UnityEngine;

[Serializable]
public class LawCursor
{
    [SerializeField] private string valueName;
    
    private string law1; // Law -3
    private string law2; // Law -2
    private string law3; // Law -1
    private string law4; // Law 1
    private string law5; // Law 2
    private string law6; // Law 3
    
    private int law1Priority;
    private int law2Priority;
    private int law3Priority;
    private int law4Priority;
    private int law5Priority;
    private int law6Priority;

    private bool law1Checked;
    private bool law2Checked;
    private bool law3Checked;
    private bool law4Checked;
    private bool law5Checked;
    private bool law6Checked;
    
    private bool lawsFullyChecked;

    private static int law1Value = -70;
    private static int law2Value = -50;
    private static int law3Value = -30;
    private static int law4Value = 30;
    private static int law5Value = 50;
    private static int law6Value = 70;
    
    [SerializeField] private int lawCursorValue;

    public bool LawsFullyChecked { get { return lawsFullyChecked; } }
    
    public (string, int) IncrementLawCursorValue(int value)
    {
        if (lawsFullyChecked)
            return ("", -1);
            
        lawCursorValue += value;

        // Law -3
        if (lawCursorValue <= law1Value && law1Checked == false)
        {
            law1Checked = true;
            lawsFullyChecked = true;
            return (law1, law1Priority);
        }

        // Law -2
        if (lawCursorValue <= law2Value && law2Checked == false)
        {
            law2Checked = true;
            return (law2, law2Priority);
        }

        // Law -1
        if (lawCursorValue <= law3Value && law3Checked == false)
        {
            law3Checked = true;
            return (law3, law3Priority);
        }

        // Law 1
        if (lawCursorValue >= law4Value && law4Checked == false)
        {
            law4Checked = true;
            return (law4, law4Priority);
        }

        // Law 2
        if (lawCursorValue >= law5Value && law5Checked == false)
        {
            law5Checked = true;
            return (law5, law5Priority);
        }

        // Law 3
        if (lawCursorValue >= law6Value && law6Checked == false)
        {
            law6Checked = true;
            lawsFullyChecked = true;
            return (law6, law6Priority);
        }
        
        return ("" , -1);
    }

public LawCursor(Value value)
    {
        valueName = value.valueName;
        Debug.Log("LawCursor created with value: " + valueName);
        law1 = value.law1;
        law2 = value.law2;
        law3 = value.law3;
        law4 = value.law4;
        law5 = value.law5;
        law6 = value.law6;
        
        law1Priority = value.law1Priority;
        law2Priority = value.law2Priority;
        law3Priority = value.law3Priority;
        law4Priority = value.law4Priority;
        law5Priority = value.law5Priority;
        law6Priority = value.law6Priority;
        
        law1Checked = false;
        law2Checked = false;
        law3Checked = false;
        law4Checked = false;
        law5Checked = false;
        law6Checked = false;
        
        lawCursorValue = 0;
        
        lawsFullyChecked = false;
    }
}

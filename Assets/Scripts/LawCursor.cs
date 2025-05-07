using System;
using UnityEngine;

[Serializable]
public class LawCursor
{
    [SerializeField] private string valueName;
    private string law1; // Law -3
    private string law2; // Law -2
    private string law3; // Law -1
    private string law4; // Law 0
    private string law5; // Law 1
    private string law6; // Law 2
    private string law7; // Law 3

    private bool law1Checked;
    private bool law2Checked;
    private bool law3Checked;
    private bool law4Checked;
    private bool law5Checked;
    private bool law6Checked;
    private bool law7Checked;

    [SerializeField] private int lawCursorValue;
    
    public void IncrementLawCursorValue(int value)
    {
        lawCursorValue += value;
        Debug.Log("LAW CURSOR VALUE: " + lawCursorValue);
    }

    public LawCursor(Value value)
    {
        valueName = value.name;
        law1 = value.law1;
        law2 = value.law2;
        law3 = value.law3;
        law4 = value.law4;
        law5 = value.law5;
        law6 = value.law6;
        law7 = value.law7;
        
        law1Checked = false;
        law2Checked = false;
        law3Checked = false;
        law4Checked = false;
        law5Checked = false;
        law6Checked = false;
        law7Checked = false;
        
        lawCursorValue = 0;
    }
}

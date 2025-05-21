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

    private static int law1Value = -100;
    private static int law2Value = -66;
    private static int law3Value = -33;
    private static int law4Value = 0;
    private static int law5Value = 33;
    private static int law6Value = 66;
    private static int law7Value = 100;

    [SerializeField] private int lawCursorValue;

    public string IncrementLawCursorValue(int value)
    {
        lawCursorValue += value;

        // Law -3
        if (lawCursorValue <= law1Value && law1Checked == false)
        {
            law1Checked = true;
            return law1;
        }

        // Law -2
        if (lawCursorValue <= law2Value && law2Checked == false)
        {
            law2Checked = true;
            return law2;
        }

        // Law -1
        if (lawCursorValue <= law3Value && law3Checked == false)
        {
            law3Checked = true;
            return law3;
        }

        // Law 1
        if (lawCursorValue >= law5Value && law5Checked == false)
        {
            law5Checked = true;
            return law5;
        }

        // Law 2
        if (lawCursorValue >= law6Value && law6Checked == false)
        {
            law6Checked = true;
            return law6;
        }

        // Law 3
        if (lawCursorValue >= law7Value && law7Checked == false)
        {
            law6Checked = true;
            return law6;
        }
        return "";
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

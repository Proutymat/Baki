#if UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;

public static class LoadCSV
{
    static void ClearFolder(string name)
    {
        string folderPath = "Assets/Resources/Scriptables/" + name;
        // If the folder doesn't exist, create it
        if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Scriptables", name);
        }

        // Delete all existing Dilemme assets in the folder
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + name, new[] { folderPath });
        foreach (string guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            UnityEditor.AssetDatabase.DeleteAsset(assetPath);
        }
        
        Debug.Log(name + "folder cleared.");
    }

    public static List<Question> LoadTutorialsCSV(string tutorialsFileName)
    {
        ClearFolder("Tutorials");
        List<Question> tutorials = new List<Question>();
        
        // Check if the file exists
        TextAsset csvFile = Resources.Load<TextAsset>(tutorialsFileName);
        if (csvFile == null)
        {
            Debug.LogError($"CSV file '{tutorialsFileName}.csv' not found in Resources folder.");
            return tutorials;
        }
        
        string[] lines = csvFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] fields = line.Split(';');

            if (fields.Length < 2)
            {
                Debug.LogWarning("Malformed line skipped: " + line);
                continue;
            }

            Question scriptable = ScriptableObject.CreateInstance<Question>();
            scriptable.question = fields[0];
            scriptable.answer1 = fields[1];
            scriptable.answer2 = fields[2];


            // Sauvegarde du ScriptableObject dans le projet (dans un dossier "Assets/Resources/Questions")
            string assetPath = $"Assets/Resources/Scriptables/Tutorials/tutorial_" + i + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(scriptable, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();

            tutorials.Add(scriptable);

            
        }
        Debug.Log("Tutorials imported successfully : " + tutorials.Count);
        return tutorials;
    }

    public static List<List<LandmarkQuestion>> LoadLandmarksCSV(string landmarksFileName)
    {
        ClearFolder("Landmarks");
        List<List<LandmarkQuestion>> landmarks = new List<List<LandmarkQuestion>>();
        List<LandmarkQuestion> landmarksTypeA = new List<LandmarkQuestion>();
        List<LandmarkQuestion> landmarksTypeB = new List<LandmarkQuestion>();
        List<LandmarkQuestion> landmarksTypeC = new List<LandmarkQuestion>();
        List<LandmarkQuestion> landmarksTypeD = new List<LandmarkQuestion>();

        // Check if the file exists
        TextAsset csvFile = Resources.Load<TextAsset>(landmarksFileName);
        if (csvFile == null)
        {
            Debug.LogError($"CSV file '{landmarksFileName}.csv' not found in Resources folder.");
            return landmarks;
        }

        string[] lines = csvFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] fields = line.Split(';');

            if (fields.Length < 5)
            {
                Debug.LogWarning("Malformed line skipped: " + line);
                continue;
            }

            LandmarkQuestion scriptable = ScriptableObject.CreateInstance<LandmarkQuestion>();
            scriptable.text1 = fields[0];
            scriptable.text2 = fields[1];
            scriptable.text3 = fields[2];
            scriptable.text4 = fields[3];
            scriptable.text5 = fields[4];
            scriptable.text6 = fields[5];
            scriptable.answer1 = fields[6];
            scriptable.answer2 = fields[7];
            scriptable.type = int.Parse(fields[8]);
            
            int nbTexts = 0;
            for (int k = 1; k < 6; k++)
            {
                if (fields[k] != "")
                    nbTexts++;
            }
            scriptable.nbTexts = nbTexts;


            // Sauvegarde du ScriptableObject dans le projet (dans un dossier "Assets/Resources/Landmarks")
            string assetPath = $"Assets/Resources/Scriptables/Landmarks/landmark_" + i + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(scriptable, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();

            if (scriptable.type == 0)
            {
                landmarksTypeA.Add(scriptable);
            }
            else if (scriptable.type == 1)
                landmarksTypeB.Add(scriptable);
            else if (scriptable.type == 2)
                landmarksTypeC.Add(scriptable);
            else if (scriptable.type == 3)
                landmarksTypeD.Add(scriptable);
            else
                Debug.LogWarning("Unknown landmark type: " + scriptable.type);

        }

        landmarks.Add(landmarksTypeA);
        landmarks.Add(landmarksTypeB);
        landmarks.Add(landmarksTypeC);
        landmarks.Add(landmarksTypeD);
        return landmarks;
    }

    public static List<Value> LoadLawsCSV(string valuesFileName)
    {
        ClearFolder("Laws");
        List<Value> laws = new List<Value>();

        // Check if the file exists
        TextAsset csvFile = Resources.Load<TextAsset>(valuesFileName);
        if (csvFile == null)
        {
            Debug.LogError($"CSV file '{valuesFileName}.csv' not found in Resources folder.");
            return laws;
        }

        string[] lines = csvFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);


        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] fields = line.Split(';');

            if (fields.Length < 8)
            {
                Debug.LogWarning("Malformed line skipped: " + line);
                continue;
            }

            Value scriptable = ScriptableObject.CreateInstance<Value>();
            scriptable.valueName = fields[0];
            scriptable.law1 = fields[1];
            scriptable.law1Priority = int.Parse(fields[2]);
            scriptable.law2 = fields[3];
            scriptable.law2Priority = int.Parse(fields[4]);
            scriptable.law3 = fields[5];
            scriptable.law3Priority = int.Parse(fields[6]);
            scriptable.law4 = fields[7];
            scriptable.law4Priority = int.Parse(fields[8]);
            scriptable.law5 = fields[9];
            scriptable.law5Priority = int.Parse(fields[10]);
            scriptable.law6 = fields[11];
            scriptable.law6Priority = int.Parse(fields[12]);



            // Sauvegarde du ScriptableObject dans le projet (dans un dossier "Assets/Resources/Questions")
            string assetPath = $"Assets/Resources/Scriptables/Laws/law_" + i + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(scriptable, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();

            laws.Add(scriptable);

            Debug.Log("Values imported successfully!");
        }

        return laws;
    }

    static int LawsStringToInt(string lawsName, List<Value> laws)
    {
        int i = 0;
        foreach (Value law in laws)
        {
            if (law.valueName == lawsName)
                return i;
            
            i++;
        }

        return -1;
    }

    static int ParseSafe(string s)
    {
        int value;
        return int.TryParse(s.Trim(), out value) ? value : 0;
    }
    
    public  static List<Question> LoadQuestionsCSV(string questionsFileName, List<Value> laws)
    {
        ClearFolder("Questions");
        List<Question> questions = new List<Question>();

        // Check if the file exists
        TextAsset csvFile = Resources.Load<TextAsset>(questionsFileName);
        if (csvFile == null)
        {
            Debug.LogError($"CSV file '{questionsFileName}.csv' not found in Resources folder.");
            return questions;
        }

        string[] lines = csvFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        int i;
        for (i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] fields = line.Split(';');

            if (fields.Length < 6)
            {
                Debug.LogWarning("Malformed line skipped: " + line);
                continue;
            }

            Question scriptable = ScriptableObject.CreateInstance<Question>();
            scriptable.question = fields[0];
            scriptable.answer1 = fields[1];
            scriptable.answer2 = fields[2];
            scriptable.answer1Type1 = LawsStringToInt(fields[3], laws);
            scriptable.answer1Type1ADD = int.Parse(fields[4]);
            scriptable.answer2Type1 = LawsStringToInt(fields[5], laws);
            scriptable.answer2Type1ADD = int.Parse(fields[6]);
            scriptable.answer1Type2 = LawsStringToInt(fields[7], laws);
            scriptable.answer1Type2ADD = int.Parse(fields[8]);
            scriptable.answer2Type2 = LawsStringToInt(fields[9], laws);
            scriptable.answer2Type2ADD = int.Parse(fields[10]);
            scriptable.answer1Illustration = fields[11];
            scriptable.answer1IllustrationPriority = ParseSafe(fields[12]);
            scriptable.answer2Illustration = fields[13];
            scriptable.answer2IllustrationPriority = ParseSafe(fields[14]);


            // Sauvegarde du ScriptableObject dans le projet (dans un dossier "Assets/Resources/Questions")
            string assetPath = $"Assets/Resources/Scriptables/Questions/question_" + i + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(scriptable, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();

            questions.Add(scriptable);
        }

        Debug.Log(questions.Count + "/" + (i - 1) + " questions imported successfully!");

        return questions;
    }

    public static void ClearScriptables()
    {
        ClearFolder("Tutorials");
        ClearFolder("Landmarks");
        ClearFolder("Laws");
        ClearFolder("Questions");
    }
}
#endif
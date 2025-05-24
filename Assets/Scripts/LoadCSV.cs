#if UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;

public static class LoadCSV
{
    static void ClearDilemmeFolder()
    {
        string folderPath = "Assets/Resources/Scriptables/Dilemme";
        // If the folder doesn't exist, create it
        if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Scriptables", "Dilemme");
        }

        // Delete all existing Dilemme assets in the folder
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Dilemme", new[] { folderPath });
        foreach (string guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            UnityEditor.AssetDatabase.DeleteAsset(assetPath);
        }
        
        Debug.Log("Dilemme folder cleared.");
    }

    static void ClearLawsFolder()
    {
        string folderPath = "Assets/Resources/Scriptables/Laws";
        // If the folder doesn't exist, create it
        if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Scriptables", "Laws");
        }
        
        // Delete all existing Law assets in the folder
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Value", new[] { folderPath });
        foreach (string guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            UnityEditor.AssetDatabase.DeleteAsset(assetPath);
        }
        
        Debug.Log("Laws folder cleared.");
    }

    static void ClearQuestionsFolder()
    {
        string folderPath = "Assets/Resources/Scriptables/Questions";
        // If the folder doesn't exist, create it
        if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Scriptables", "Questions");
        }

        // Delete all existing Question assets in the folder
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Question", new[] { folderPath });
        foreach (string guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            UnityEditor.AssetDatabase.DeleteAsset(assetPath);
        }
        
        Debug.Log("Questions folder cleared.");
    }

    public static List<Dilemme> LoadDilemmeCSV(string dilemmeFileName)
    {
        ClearDilemmeFolder();
        List<Dilemme> dilemmes = new List<Dilemme>();

        // Check if the file exists
        TextAsset csvFile = Resources.Load<TextAsset>(dilemmeFileName);
        if (csvFile == null)
        {
            Debug.LogError($"CSV file '{dilemmeFileName}.csv' not found in Resources folder.");
            return dilemmes;
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

            Dilemme scriptable = ScriptableObject.CreateInstance<Dilemme>();
            scriptable.question = fields[0];
            scriptable.answer1 = fields[1];
            scriptable.answer2 = fields[2];
            scriptable.answer3 = fields[3];
            scriptable.answer4 = fields[4];


            // Sauvegarde du ScriptableObject dans le projet (dans un dossier "Assets/Resources/Questions")
            string assetPath = $"Assets/Resources/Scriptables/Dilemme/dilemme_" + i + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(scriptable, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();

            dilemmes.Add(scriptable);

            Debug.Log("Dilemmes imported successfully : " + dilemmes.Count);
        }

        return dilemmes;
    }

    public static List<Value> LoadLawsCSV(string valuesFileName)
    {
        ClearLawsFolder();
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

    public  static List<Question> LoadQuestionsCSV(string questionsFileName, List<Value> laws)
    {
        ClearQuestionsFolder();
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
            scriptable.answer1Type1ADD = int.Parse(fields[8]);
            scriptable.answer2Type2 = LawsStringToInt(fields[9], laws);
            scriptable.answer2Type1ADD = int.Parse(fields[10]);


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
        ClearQuestionsFolder();
        ClearLawsFolder();
        ClearDilemmeFolder();
    }
}
#endif
using UnityEditor;
using UnityEngine;

public class DeveloperMenu
{
    /// <summary>
    /// Deletes all data from PlayerPrefs.
    /// </summary>
    [MenuItem("Developer/ClearPlayerPrefs")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs cleared");
    }

    /// <summary>
    /// Prints a funny message.
    /// </summary>
    [MenuItem("Developer/PrintFunnyMessage")]
    public static void PrintFunnyMessage()
    {
        Debug.Log("\"Run\" said Gandalf the White,\nand got blown up by a dynamite.");
    }
}


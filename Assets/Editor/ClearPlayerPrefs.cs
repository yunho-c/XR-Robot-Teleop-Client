using UnityEngine;
using UnityEditor;

public class ClearPlayerPrefs : Editor
{
    [MenuItem("Jobs/Clear PlayerPrefs")]
    private static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs have been cleared.");
    }
}

using System;
using UnityEngine;

//a save exists only once a checkpoint has been reached: SetCheckpoint and MarkCompleted
//are the things that set the exists flag.
public static class SaveService
{
    private const string KeyExists = "save.exists";
    private const string KeyCheckpoint = "save.checkpoint";
    private const string KeyCompleted = "save.completed";
    
    public const int IntroCheckpoint = 0;

    public static bool HasSave => PlayerPrefs.GetInt(KeyExists, 0) != 0;

    public static int Checkpoint => Mathf.Clamp(PlayerPrefs.GetInt(KeyCheckpoint, IntroCheckpoint), 0, 8);

    public static bool Completed => PlayerPrefs.GetInt(KeyCompleted, 0) != 0;
    
    public static void DeleteSave()
    {
        Write(() =>
        {
            PlayerPrefs.DeleteKey(KeyExists);
            PlayerPrefs.DeleteKey(KeyCheckpoint);
            PlayerPrefs.DeleteKey(KeyCompleted);
        });
    }

    public static void SetCheckpoint(int globalCheckpoint)
    {
        Write(() =>
        {
            PlayerPrefs.SetInt(KeyExists, 1);
            PlayerPrefs.SetInt(KeyCheckpoint, Mathf.Clamp(globalCheckpoint, 0, 8));
        });
    }

    public static void MarkCompleted()
    {
        Write(() =>
        {
            PlayerPrefs.SetInt(KeyExists, 1);
            PlayerPrefs.SetInt(KeyCompleted, 1);
        });
    }
    
    private static void Write(Action mutate)
    {
        try
        {
            mutate();
            PlayerPrefs.Save();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SaveService] could not persist save data: {e.Message}");
        }
    }
}

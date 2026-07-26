using System;
using UnityEngine;


// See scriptable object in editor to add more checkpoints
[CreateAssetMenu(fileName = "CheckpointTableSO", menuName = "Scriptable Objects/CheckpointTableSO")]
public class CheckpointTableSO : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        [Tooltip("Global Checkpoint number")]
        public int globalCheckpoint;

        [Tooltip("Scene to load. Must be in Build Settings.")]
        public string sceneName;

        [Tooltip("Matches the globalCheckpoint id, or just start at beginning if no match is found. ")]
        public int markerId;

        [Tooltip("Shown to the player, e.g. \"Village - Act 1\".")]
        public string displayName;
    }

    [Header("Intro / New Game Scene")]
    [SerializeField] private string _introSceneName = "Village";

    [Header("Global Checkpoints")]
    [SerializeField] private Entry[] _entries;

    [Header("Ending")]
    [SerializeField] private string _endingSceneName = "";

    public string IntroSceneName => _introSceneName;
    public string EndingSceneName => _endingSceneName;
    public Entry[] Entries => _entries;

    public Entry Find(int globalCheckpoint)
    {
        if (_entries == null)
            return null;

        foreach (var entry in _entries)
        {
            if (entry != null && entry.globalCheckpoint == globalCheckpoint)
                return entry;
        }

        return null;
    }
}

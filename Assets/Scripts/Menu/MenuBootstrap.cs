using UnityEngine;

public static class MenuBootstrap
{
    private const string ResourcePath = "MenuRoot";

    private static GameObject _instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        _instance = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (_instance != null)
            return;

        if (GameManager.Instance != null)
        {
            _instance = GameManager.Instance.gameObject;
            return;
        }

        var prefab = Resources.Load<GameObject>(ResourcePath);
        if (prefab == null)
        {
            Debug.LogError($"[Bootstrap] Assets/Prefabs/{ResourcePath}.prefab is missing.");
            return;
        }

        _instance = Object.Instantiate(prefab);
        _instance.name = prefab.name;
        Object.DontDestroyOnLoad(_instance);
    }
}

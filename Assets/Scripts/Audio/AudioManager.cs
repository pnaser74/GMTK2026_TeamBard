using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioDataSO audioData;
    FMOD.Studio.EventInstance menuMusicEvent;
    FMOD.Studio.EventInstance overworldMusicEvent;
    FMOD.Studio.EventInstance villageAmbienceEvent;
    void Awake()
    {
         Object[] objs = Object.FindObjectsByType<AudioManager>(0);

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        StartMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateAmbienceEvents()
    {
        villageAmbienceEvent = FMODUnity.RuntimeManager.CreateInstance(audioData.villageAmbienceEvent);
    }
    
    private void CreateMusicEvents()
    {
        menuMusicEvent = FMODUnity.RuntimeManager.CreateInstance(audioData.titleMusicEvent);
        overworldMusicEvent = FMODUnity.RuntimeManager.CreateInstance(audioData.gameMusicEvent);
    }

    public void StartMusic()
    {
        CreateMusicEvents();
        overworldMusicEvent.start();

    }
}

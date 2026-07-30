using System.Collections;
using FMODUnityResonance;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioDataSO audioData;
    public FMOD.Studio.EventInstance menuMusicInstance;
    public FMOD.Studio.EventInstance overworldMusicInstance;
    public FMOD.Studio.EventInstance villageAmbienceInstance;
    public delegate void Change();
    void Awake()
    {
         Object[] objs = Object.FindObjectsByType<AudioManager>(0);

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        SceneManager.activeSceneChanged += ChangedActiveScene;
        setUpGameAudio();
        menuMusicInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChangedActiveScene(Scene current, Scene next) 
    {
        Debug.Log("Scene is changing! current scene is: " + current.name + " next scene is: "+ next.name);
        switch(next.name)
        {
            case "MainMenu":
                StartCoroutine(ChangeMusic(overworldMusicInstance, menuMusicInstance, 1f));
                break;
            case "Village":
                StartCoroutine(ChangeMusic(menuMusicInstance, overworldMusicInstance, 1f));
                break;
            case "Credits":
                StartCoroutine(ChangeMusic(overworldMusicInstance, menuMusicInstance, 1f));
                break;
            case "GameOver":
                StartCoroutine(ChangeMusic(overworldMusicInstance, menuMusicInstance, 1f));
                break;
            default:
                break;
        }
    }

    private void CreateAmbienceInstances()
    {
        villageAmbienceInstance = FMODUnity.RuntimeManager.CreateInstance(audioData.villageAmbienceEvent);
    }
    
    private void CreateMusicInstances()
    {
        menuMusicInstance = FMODUnity.RuntimeManager.CreateInstance(audioData.titleMusicEvent);
        overworldMusicInstance = FMODUnity.RuntimeManager.CreateInstance(audioData.gameMusicEvent);
    }
    public void setUpGameAudio()
    {
        CreateAmbienceInstances();
        CreateMusicInstances();   
    }

    public IEnumerator ChangeMusic(FMOD.Studio.EventInstance currentMusic, FMOD.Studio.EventInstance nextMusic, float waitTime)
    {
        // Takes a user provided "wait time" argument and waits that long before playing the next music, after fading out current music
        currentMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        yield return new WaitForSeconds(waitTime);
        nextMusic.start();
        yield return null;
    }
}

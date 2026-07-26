using FMODUnity;
using UnityEngine;

public class UISfx : MonoBehaviour
{
    public static UISfx Instance { get; private set; }

    [Header("Data")]
    [SerializeField] private AudioDataSO _audioData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public static void PlayHover()
    {
        var data = Data();
        if (data != null)
            Play(data.buttonHoverEvent);
    }

    public static void PlayClick()
    {
        var data = Data();
        if (data != null)
            Play(data.buttonClickEvent);
    }

    public static void PlayBack()
    {
        var data = Data();
        if (data != null)
            Play(data.buttonBackEvent);
    }

    public static void PlaySlider()
    {
        var data = Data();
        if (data != null)
            Play(data.sliderMoveEvent);
    }
    
    public static void PlayInvalid()
    {
        var data = Data();
        if (data == null)
            return;

        Play(data.buttonBackEvent);
    }

    private static AudioDataSO Data()
    {
        return Instance != null ? Instance._audioData : null;
    }

    private static void Play(EventReference reference)
    {
        if (reference.IsNull)
            return;
        
        try
        {
            RuntimeManager.PlayOneShot(reference);
        }
        catch (EventNotFoundException)
        {
        }
        catch (SystemNotInitializedException)
        {
        }
    }
}

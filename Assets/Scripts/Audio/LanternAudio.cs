using Unity.VisualScripting;
using UnityEngine;

public class LanternAudio : MonoBehaviour
{
    public AudioDataSO audioData;
    FMOD.Studio.EventInstance lanternInstance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FMODUnity.EventReference lanternAudio = audioData.burningObjectEvent;
        lanternInstance = FMODUnity.RuntimeManager.CreateInstance(lanternAudio);
        lanternInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        lanternInstance.start();
    }

    private void OnDestroy()
    {
        lanternInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        lanternInstance.release();
    }
}

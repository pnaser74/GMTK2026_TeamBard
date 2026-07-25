using UnityEngine;

public class EnemyNPCAudio : MonoBehaviour
{
    public AudioDataSO audioData;
    FMOD.Studio.EventInstance npcInstance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npcInstance = FMODUnity.RuntimeManager.CreateInstance(audioData.villagerDialogueEvent);
        npcInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        npcInstance.start();
    }

    void OnDestroy()
    {
        npcInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        npcInstance.release();
    }
}

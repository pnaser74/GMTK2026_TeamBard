using UnityEngine;

public class AudioVolumeSettings : MonoBehaviour
{
    public AudioDataSO audioData;
    public FMOD.Studio.VCA masterVolVCA;
    public FMOD.Studio.VCA musicVCA;
    public FMOD.Studio.VCA sfxVCA;
    public FMOD.Studio.VCA voxVCA;
    public FMOD.Studio.VCA uiVCA;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        masterVolVCA = FMODUnity.RuntimeManager.GetVCA(audioData.masterVolVCAPath);
        musicVCA = FMODUnity.RuntimeManager.GetVCA(audioData.musicVCAPath);
        sfxVCA = FMODUnity.RuntimeManager.GetVCA(audioData.sfxVCAPath);
        voxVCA = FMODUnity.RuntimeManager.GetVCA(audioData.voxVCAPath);
        uiVCA = FMODUnity.RuntimeManager.GetVCA(audioData.uiVCAPath);
    }
    void updateMasterVolume(float vol)
    {
        masterVolVCA.setVolume(vol);
    }
    
    void updateMusicVolume(float vol)
    {
        musicVCA.setVolume(vol);
    }
    
    void updateSFXVolume(float vol)
    {
        sfxVCA.setVolume(vol);
    }
    
    void updateUIVolume(float vol)
    {
        uiVCA.setVolume(vol);
    }
    
    void updateVoxVolume(float vol)
    {
        voxVCA.setVolume(vol);
    }
}
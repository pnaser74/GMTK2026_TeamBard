using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "AudioDataSO", menuName = "Scriptable Objects/AudioDataSO")]
public class AudioDataSO : ScriptableObject
{
    [Header("FMOD Global Parameter References")]
    public FMOD.Studio.PARAMETER_ID footstepSurfaceParameter;

    [Header ("Music Event References")]
    public FMODUnity.EventReference titleMusicEvent;
    public FMODUnity.EventReference gameMusicEvent;

    [Header ("SFX Event References")]
    public FMODUnity.EventReference footstepsEvent;
    public FMODUnity.EventReference jumpEvent;

     [Header("FMOD UI Event References")]
    public FMODUnity.EventReference buttonClickEvent;
    public FMODUnity.EventReference buttonBackEvent;
    public FMODUnity.EventReference buttonHoverEvent;
    public FMODUnity.EventReference sliderMoveEvent;


    [Header ("VCA References")]
    public string masterVolVCAPath;
    public string musicVCAPath;
    public string sfxVCAPath;
    public string voxVCAPath;
    public string uiVCAPath;


}

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

    [Header ("Player SFX Event References")]
    public FMODUnity.EventReference footstepsEvent;
    public FMODUnity.EventReference jumpEvent;
    public FMODUnity.EventReference takeDamageEvent;//in essence this is just losing the Count
    
    [Header ("Count SFX Event References")]
    public FMODUnity.EventReference wingsFlappingEvent;
    // public FMODUnity.EventReference jumpEvent;
    
    [Header ("Enemy SFX Event References")]
    public FMODUnity.EventReference torchAttackEvent;
    public FMODUnity.EventReference pitchforkAttackEvent;
    
    [Header ("Environmental SFX Event References")]
    public FMODUnity.EventReference platformCollapseEvent;
    public FMODUnity.EventReference burningObjectEvent;

    [Header ("Voice Lines Event References")]
    public FMODUnity.EventReference countDialogueEvent;
    public FMODUnity.EventReference villagerDialogueEvent;

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

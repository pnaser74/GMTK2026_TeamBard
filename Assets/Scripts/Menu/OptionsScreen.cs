using UnityEngine;

public class OptionsScreen : MenuScreen
{
    [Header("Volume")]
    [SerializeField] private MenuStepper _masterVolume;
    [SerializeField] private MenuStepper _musicVolume;
    [SerializeField] private MenuStepper _sfxVolume;
    [SerializeField] private MenuStepper _dialogueVolume;

    [Header("Gameplay")]
    [SerializeField] private MenuToggle _quipToggle;

    [Header("Navigation")]
    [SerializeField] private MenuButton _returnItem;

    protected override void Awake()
    {
        base.Awake();
        _masterVolume.OnStepChanged.AddListener(OnMasterVolumeChanged);
        _musicVolume.OnStepChanged.AddListener(OnMusicVolumeChanged);
        _sfxVolume.OnStepChanged.AddListener(OnSfxVolumeChanged);
        _dialogueVolume.OnStepChanged.AddListener(OnDialogueVolumeChanged);
        _quipToggle.OnValueChanged.AddListener(OnQuipToggleChanged);
        _returnItem.OnActivate.AddListener(OnReturnToMenuPressed);
    }

    public override void OnOpen()
    {
        //push current values in without raising change events.
        _masterVolume.SetStepSilent(SettingsService.GetStep(VolumeChannel.Master));
        _musicVolume.SetStepSilent(SettingsService.GetStep(VolumeChannel.Music));
        _sfxVolume.SetStepSilent(SettingsService.GetStep(VolumeChannel.Sfx));
        _dialogueVolume.SetStepSilent(SettingsService.GetStep(VolumeChannel.Dialogue));
        _quipToggle.SetValueSilent(SettingsService.QuipsEnabled);
    }

    // ---- item callbacks ----

    public void OnMasterVolumeChanged(int step)
    {
        SettingsService.SetStep(VolumeChannel.Master, step);
    }

    public void OnMusicVolumeChanged(int step)
    {
        SettingsService.SetStep(VolumeChannel.Music, step);
    }

    public void OnSfxVolumeChanged(int step)
    {
        SettingsService.SetStep(VolumeChannel.Sfx, step);
    }

    public void OnDialogueVolumeChanged(int step)
    {
        SettingsService.SetStep(VolumeChannel.Dialogue, step);
    }

    public void OnQuipToggleChanged(bool value)
    {
        SettingsService.SetQuipsEnabled(value);
    }

    public void OnReturnToMenuPressed()
    {
        ScreenRouter.Instance.Pop(false);
    }
}

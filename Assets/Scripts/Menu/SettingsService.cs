using System;
using UnityEngine;

public enum VolumeChannel
{
    Master,
    Music,
    Sfx,
    Dialogue
}

// player-facing settings, backed by PlayerPrefs.
// volumes are stored as a step index 0..10
public static class SettingsService
{
    public const int MaxStep = 10;
    public const int DefaultStep = 8; //80%

    private const int SettingsVersion = 1;

    private const string KeyVersion = "opt.version";
    private const string KeyMaster = "opt.vol.master";
    private const string KeyMusic = "opt.vol.music";
    private const string KeySfx = "opt.vol.sfx";
    private const string KeyDialogue = "opt.vol.dialogue";
    private const string KeyQuips = "opt.quips";

    private static readonly int[] _steps = new int[4];
    private static bool _quipsEnabled = true;
    private static bool _loaded;

    //raised after any setting changes so the FMOD mixer can re-apply.
    public static event Action Changed;

    public static bool QuipsEnabled
    {
        get
        {
            EnsureLoaded();
            return _quipsEnabled;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        _loaded = false;
        Changed = null;
    }

    public static void Load()
    {
        _steps[(int)VolumeChannel.Master] = ReadStep(KeyMaster);
        _steps[(int)VolumeChannel.Music] = ReadStep(KeyMusic);
        _steps[(int)VolumeChannel.Sfx] = ReadStep(KeySfx);
        _steps[(int)VolumeChannel.Dialogue] = ReadStep(KeyDialogue);
        _quipsEnabled = PlayerPrefs.GetInt(KeyQuips, 1) != 0;
        _loaded = true;
    }

    public static int GetStep(VolumeChannel channel)
    {
        EnsureLoaded();
        return _steps[(int)channel];
    }

    public static int GetPercent(VolumeChannel channel)
    {
        return GetStep(channel) * 10;
    }

    public static void SetStep(VolumeChannel channel, int step)
    {
        EnsureLoaded();
        step = Mathf.Clamp(step, 0, MaxStep);
        if (_steps[(int)channel] == step)
            return;

        _steps[(int)channel] = step;
        Write(KeyFor(channel), step);
        Changed?.Invoke();
    }

    public static void SetQuipsEnabled(bool value)
    {
        EnsureLoaded();
        if (_quipsEnabled == value)
            return;

        _quipsEnabled = value;
        Write(KeyQuips, value ? 1 : 0);
        Changed?.Invoke();
    }

    //0 -> silence, 100 -> unity gain. VCA.setVolume takes a linear gain multiplier,
    //not dB - a plain linear mapping reads as "the slider does nothing until the top
    //quarter", so square it. swap this one function to change the whole curve.
    public static float PercentToLinear(int percent)
    {
        if (percent <= 0)
            return 0f;

        var normalised = Mathf.Clamp01(percent / 100f);
        return normalised * normalised;
    }

    public static float GetLinearVolume(VolumeChannel channel)
    {
        return PercentToLinear(GetPercent(channel));
    }

    private static void EnsureLoaded()
    {
        if (!_loaded)
            Load();
    }

    private static int ReadStep(string key)
    {
        return Mathf.Clamp(PlayerPrefs.GetInt(key, DefaultStep), 0, MaxStep);
    }

    private static string KeyFor(VolumeChannel channel)
    {
        switch (channel)
        {
            case VolumeChannel.Master: return KeyMaster;
            case VolumeChannel.Music: return KeyMusic;
            case VolumeChannel.Sfx: return KeySfx;
            default: return KeyDialogue;
        }
    }
    
    private static void Write(string key, int value)
    {
        try
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.SetInt(KeyVersion, SettingsVersion);
            PlayerPrefs.Save();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SettingsService] could not persist '{key}': {e.Message}");
        }
    }
}

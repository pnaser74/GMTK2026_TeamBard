using UnityEngine;


[CreateAssetMenu(fileName = "MenuThemeSO", menuName = "Scriptable Objects/MenuThemeSO")]
public class MenuThemeSO : ScriptableObject
{
    [Header("Label Colours")]
    [SerializeField] private Color _dimColor = new Color(0.66f, 0.63f, 0.60f, 1f);
    [SerializeField] private Color _highlightColor = new Color(1f, 0.96f, 0.90f, 1f);
    [SerializeField] private Color _disabledColor = new Color(0.34f, 0.32f, 0.31f, 1f);

    [Tooltip("How much of the highlight colour a disabled item picks up when focused. " +
             "The flow chart calls this \"grayed out with slight highlight\".")]
    [SerializeField, Range(0f, 1f)] private float _disabledHighlightBlend = 0.35f;

    [Header("Focus")]
    [SerializeField] private float _highlightScale = 1.06f;
    [SerializeField] private float _focusTweenTime = 0.07f;

    [Header("Rejection Shake")]
    [SerializeField] private float _shakeAmplitude = 12f;
    [SerializeField] private float _shakeDuration = 0.22f;
    [SerializeField] private float _shakeFrequency = 26f;

    [Header("Screen Fade")]
    [SerializeField] private float _screenFadeTime = 0.1f;

    [Header("Popup Backdrop")]
    [SerializeField] private Color _backdropColor = new Color(0f, 0f, 0f, 0.72f);

    public float HighlightScale => _highlightScale;
    public float FocusTweenTime => _focusTweenTime;
    public float ShakeAmplitude => _shakeAmplitude;
    public float ShakeDuration => _shakeDuration;
    public float ShakeFrequency => _shakeFrequency;
    public float ScreenFadeTime => _screenFadeTime;
    public Color BackdropColor => _backdropColor;

    public Color ResolveLabelColor(bool focused, bool itemEnabled)
    {
        if (itemEnabled)
            return focused ? _highlightColor : _dimColor;

        return focused
            ? Color.Lerp(_disabledColor, _highlightColor, _disabledHighlightBlend)
            : _disabledColor;
    }
}

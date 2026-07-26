using System.Collections;
using UnityEngine;

// Base class for menu screens handling show/hide,
// default focus and cancel behavior.
[RequireComponent(typeof(CanvasGroup))]
public class MenuScreen : MonoBehaviour
{
    [Header("Screen")]
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private MenuThemeSO _theme;

    [Tooltip("Default focused item when this screen opens. Popups may resolve it at Show() time.")]
    [SerializeField] private MenuWidget _defaultFocus;

    [Tooltip("True for popups - the screen underneath stays rendered but non-interactive.")]
    [SerializeField] private bool _keepUnderlyingVisible;

    [Tooltip("False for screens that must not be dismissed with the cancel button.")]
    [SerializeField] private bool _cancelPops = true;

    private Coroutine _fade;

    public bool KeepUnderlyingVisible => _keepUnderlyingVisible;

    protected MenuThemeSO Theme => _theme;

    protected virtual void Awake()
    {
        if (_group == null)
            _group = GetComponent<CanvasGroup>();
    }

    //called by ScreenRouter after the panel is activated.
    public virtual void OnOpen()
    {
    }

    //called by ScreenRouter before the panel is deactivated.
    public virtual void OnClose()
    {
    }

    public virtual MenuWidget ResolveDefaultFocus()
    {
        return _defaultFocus;
    }

    //return true if the cancel press was consumed.
    public virtual bool OnCancel()
    {
        if (!_cancelPops)
            return false;

        ScreenRouter.Instance.Pop();
        return true;
    }

    public void SetInteractable(bool value)
    {
        if (_group == null)
            return;

        _group.interactable = value;
        _group.blocksRaycasts = value;
    }

    public void SetShown(bool value)
    {
        gameObject.SetActive(value);

        if (!value || _group == null)
            return;

        //unscaled time - the pause menu opens at Time.timeScale = 0.
        if (_fade != null)
            StopCoroutine(_fade);

        var duration = _theme != null ? _theme.ScreenFadeTime : 0f;
        if (duration <= 0f)
        {
            _group.alpha = 1f;
            return;
        }

        _fade = StartCoroutine(FadeIn(duration));
    }

    private IEnumerator FadeIn(float duration)
    {
        var elapsed = 0f;
        _group.alpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _group.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        _group.alpha = 1f;
        _fade = null;
    }
}

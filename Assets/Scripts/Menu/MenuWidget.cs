using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//base for the menu widgets
[RequireComponent(typeof(Selectable))]
public abstract class MenuWidget : MonoBehaviour,
    ISelectHandler,
    IDeselectHandler,
    IPointerEnterHandler,
    ISubmitHandler,
    IPointerClickHandler
{
    [Header("Wiring")]
    [SerializeField] private TMP_Text _label;
    [SerializeField] private MenuThemeSO _theme;

    [Header("State")]
    [SerializeField] private bool _startsEnabled = true;

    private Selectable _selectable;
    private RectTransform _rect;
    private Coroutine _shake;
    private Vector2 _shakeRestPosition;

    
    private static int _suppressHoverFrame = -1;

    public bool ItemEnabled { get; private set; }
    public bool Focused { get; private set; }
    public Selectable Selectable => _selectable;

    protected MenuThemeSO Theme => _theme;
    protected TMP_Text Label => _label;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        _suppressHoverFrame = -1;
    }

    public static void SuppressHoverSfxThisFrame()
    {
        _suppressHoverFrame = Time.frameCount;
    }

    protected virtual void Awake()
    {
        _rect = (RectTransform)transform;
        _selectable = GetComponent<Selectable>();
        _selectable.interactable = true;
        _selectable.transition = Selectable.Transition.None;

        ItemEnabled = _startsEnabled;
        Repaint();
    }

    public void SetItemEnabled(bool value)
    {
        if (ItemEnabled == value)
            return;

        ItemEnabled = value;
        Repaint();
    }

    public void Focus()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    protected abstract void OnActivated();

    protected virtual void OnRepaint()
    {
    }

    protected void Repaint()
    {
        if (_label != null && _theme != null)
            _label.color = _theme.ResolveLabelColor(Focused, ItemEnabled);

        if (_theme != null && _shake == null)
        {
            var scale = Focused && ItemEnabled ? _theme.HighlightScale : 1f;
            _rect.localScale = new Vector3(scale, scale, 1f);
        }

        OnRepaint();
    }

    protected void TryActivate()
    {
        if (ItemEnabled)
        {
            UISfx.PlayClick();
            OnActivated();
            return;
        }

        //"On Selection: Animation - Rejection Shake, SFX - Invalid Action"
        UISfx.PlayInvalid();
        RejectionShake();
    }

    public void RejectionShake()
    {
        if (_theme == null || _shake != null)
            return;

        _shakeRestPosition = _rect.anchoredPosition;
        _shake = StartCoroutine(ShakeRoutine());
    }

    //unscaled time is mandatory: the pause menu runs at Time.timeScale = 0, where
    //the codebase's usual Time.deltaTime tweens silently freeze.
    private IEnumerator ShakeRoutine()
    {
        var elapsed = 0f;

        while (elapsed < _theme.ShakeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            var decay = 1f - Mathf.Clamp01(elapsed / _theme.ShakeDuration);
            var offset = Mathf.Sin(elapsed * _theme.ShakeFrequency) * _theme.ShakeAmplitude * decay;
            _rect.anchoredPosition = _shakeRestPosition + new Vector2(offset, 0f);
            yield return null;
        }

        _rect.anchoredPosition = _shakeRestPosition;
        _shake = null;
        Repaint();
    }

    //a screen closing mid-shake should not leave items displaced.
    protected virtual void OnDisable()
    {
        if (_shake == null)
            return;

        StopCoroutine(_shake);
        _shake = null;
        _rect.anchoredPosition = _shakeRestPosition;
        Repaint();
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        Focused = true;
        Repaint();

        if (_suppressHoverFrame != Time.frameCount)
            UISfx.PlayHover();
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        Focused = false;
        Repaint();
    }

    
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == gameObject)
            return;

        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public virtual void OnSubmit(BaseEventData eventData)
    {
        TryActivate();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        TryActivate();
    }
}

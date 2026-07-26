using System;
using TMPro;
using UnityEngine;

//"Are You Sure?" popup
public class ConfirmPopupScreen : MenuScreen
{
    public enum DefaultOption
    {
        Confirm,
        Cancel,
        None
    }

    [Header("Text")]
    [SerializeField] private TMP_Text _titleLabel;
    [SerializeField] private TMP_Text _bodyLabel;

    [Header("Items")]
    [SerializeField] private MenuButton _confirmItem;
    [SerializeField] private MenuButton _cancelItem;

    private Action _onConfirm;
    private DefaultOption _defaultOption = DefaultOption.Cancel;

    protected override void Awake()
    {
        base.Awake();
        _confirmItem.OnActivate.AddListener(OnConfirmPressed);
        _cancelItem.OnActivate.AddListener(OnCancelPressed);
    }

    public void Show(string title, string body, string confirmLabel, string cancelLabel,
        DefaultOption defaultOption, Action onConfirm)
    {
        if (_titleLabel != null)
            _titleLabel.text = title;

        if (_bodyLabel != null)
            _bodyLabel.text = body;

        _confirmItem.SetLabel(confirmLabel);
        _cancelItem.SetLabel(cancelLabel);
        _defaultOption = defaultOption;
        _onConfirm = onConfirm;

        ScreenRouter.Instance.Push(this);
    }
    
    public override MenuWidget ResolveDefaultFocus()
    {
        switch (_defaultOption)
        {
            case DefaultOption.Confirm: return _confirmItem;
            case DefaultOption.Cancel: return _cancelItem;
            default: return null;
        }
    }

    private void OnConfirmPressed()
    {
        var callback = _onConfirm;
        _onConfirm = null;

        ScreenRouter.Instance.Pop(false);
        callback?.Invoke();
    }

    private void OnCancelPressed()
    {
        _onConfirm = null;
        ScreenRouter.Instance.Pop(false);
    }
}

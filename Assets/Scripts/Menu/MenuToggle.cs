using TMPro;
using UnityEngine;
using UnityEngine.Events;

//checkbox widget
public class MenuToggle : MenuWidget
{
    [Header("Value")]
    [SerializeField] private TMP_Text _valueLabel;
    [SerializeField] private string _onText = "ON";
    [SerializeField] private string _offText = "OFF";
    [SerializeField] private bool _isOn = true;

    [Header("Events")]
    [SerializeField] private UnityEvent<bool> _onValueChanged;

    public bool IsOn => _isOn;
    public UnityEvent<bool> OnValueChanged => _onValueChanged;

    //set from code when the options screen opens; does not raise onValueChanged.
    public void SetValueSilent(bool value)
    {
        _isOn = value;
        Repaint();
    }

    protected override void OnActivated()
    {
        _isOn = !_isOn;
        Repaint();
        _onValueChanged?.Invoke(_isOn);
    }

    protected override void OnRepaint()
    {
        if (_valueLabel == null)
            return;

        _valueLabel.text = _isOn ? _onText : _offText;
        if (Theme != null)
            _valueLabel.color = Theme.ResolveLabelColor(Focused, ItemEnabled);
    }
}

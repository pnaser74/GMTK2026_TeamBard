using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Stepper widget, increments of 10
public class MenuStepper : MenuWidget, IMoveHandler
{
    [Header("Value")]
    [SerializeField] private TMP_Text _valueLabel;
    [SerializeField] private int _stepCount = 11;
    [SerializeField] private int _valuePerStep = 10;
    [SerializeField] private string _valueSuffix = "";
    [SerializeField] private int _step;

    [Header("Arrows")]
    [SerializeField] private Graphic _leftArrow;
    [SerializeField] private Graphic _rightArrow;
    [SerializeField] private Color _arrowActiveColor = new Color(1f, 0.96f, 0.90f, 1f);
    [SerializeField] private Color _arrowEndColor = new Color(1f, 0.96f, 0.90f, 0.2f);

    [Header("Events")]
    [SerializeField] private UnityEvent<int> _onStepChanged;

    public int Step => _step;
    public int Value => _step * _valuePerStep;
    public UnityEvent<int> OnStepChanged => _onStepChanged;

    public void SetStepSilent(int step)
    {
        _step = Mathf.Clamp(step, 0, Mathf.Max(0, _stepCount - 1));
        Repaint();
    }
    
    public void OnMove(AxisEventData eventData)
    {
        if (eventData.moveDir == MoveDirection.Left)
        {
            Nudge(-1);
            eventData.Use();
            return;
        }

        if (eventData.moveDir == MoveDirection.Right)
        {
            Nudge(1);
            eventData.Use();
        }
    }

    public void Nudge(int direction)
    {
        if (!ItemEnabled)
        {
            UISfx.PlayInvalid();
            RejectionShake();
            return;
        }

        var next = Mathf.Clamp(_step + direction, 0, Mathf.Max(0, _stepCount - 1));
        if (next == _step)
        {
            return;
        }

        _step = next;
        UISfx.PlaySlider();
        Repaint();
        _onStepChanged?.Invoke(_step);
    }

    protected override void OnActivated()
    {
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        if (ItemEnabled)
            return;

        base.OnSubmit(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (ItemEnabled)
            return;

        base.OnPointerClick(eventData);
    }

    protected override void OnRepaint()
    {
        if (_valueLabel != null)
        {
            _valueLabel.text = $"{Value}{_valueSuffix}";
            if (Theme != null)
                _valueLabel.color = Theme.ResolveLabelColor(Focused, ItemEnabled);
        }

        if (_leftArrow != null)
            _leftArrow.color = _step > 0 ? _arrowActiveColor : _arrowEndColor;

        if (_rightArrow != null)
            _rightArrow.color = _step < _stepCount - 1 ? _arrowActiveColor : _arrowEndColor;
    }
}

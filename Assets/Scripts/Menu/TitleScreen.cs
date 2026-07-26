using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;

public class TitleScreen : MenuScreen
{
    [Header("Wiring")]
    [SerializeField] private MenuScreen _mainMenuScreen;

    [Tooltip("Ignore presses for this long after the screen opens, so the click that " +
             "returned us to the title cannot immediately dismiss it again.")]
    [SerializeField] private float _inputGraceTime = 0.25f;

    private System.IDisposable _anyButtonListener;
    private float _openedAtUnscaledTime;

    public override void OnOpen()
    {
        _openedAtUnscaledTime = Time.unscaledTime;
        _anyButtonListener?.Dispose();
        _anyButtonListener = InputSystem.onAnyButtonPress.Call(OnAnyButtonPressed);
    }

    public override void OnClose()
    {
        _anyButtonListener?.Dispose();
        _anyButtonListener = null;
    }

    private void OnDisable()
    {
        _anyButtonListener?.Dispose();
        _anyButtonListener = null;
    }

    public override bool OnCancel()
    {
        return true;
    }

    private void OnAnyButtonPressed(InputControl control)
    {
        if (Time.unscaledTime - _openedAtUnscaledTime < _inputGraceTime)
            return;
        
        if (!(control is ButtonControl))
            return;

        if (IsCancelControl(control))
            return;

        UISfx.PlayClick();
        ScreenRouter.Instance.Push(_mainMenuScreen);
    }

    private static bool IsCancelControl(InputControl control)
    {
        var cancel = InputSystem.actions != null ? InputSystem.actions.FindAction("UI/Cancel") : null;
        if (cancel == null)
            return false;

        foreach (var candidate in cancel.controls)
        {
            if (candidate == control)
                return true;
        }

        return false;
    }
}

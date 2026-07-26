using UnityEngine;
using UnityEngine.InputSystem;

//single owner of the game's input. every gameplay script shares one InputActions
//instance through here instead of doing "new InputActions()" per component.
//
//the Pause action is declared in code on purpose. InputActions.inputactions has
//generateWrapperCode enabled and its generated InputActions.cs is checked in, so
//editing that asset outside the editor would leave the generated file stale.
//
//pause opening/closing is bound to gamepad start only. escape reaches us through
//the UI map's Cancel action (see ScreenRouter) - binding escape here as well would
//fire both paths on one keypress.
public static class GameInput
{
    private static InputActions _actions;
    private static InputAction _pause;

    //gameplay scripts poll this before reading movement. Time.timeScale = 0 is not
    //enough on its own: coroutines that "yield return null" keep ticking and the
    //input system is unaffected by time scale.
    public static bool GameplayEnabled { get; private set; } = true;

    public static InputActions Actions
    {
        get
        {
            EnsureCreated();
            return _actions;
        }
    }

    public static InputAction PauseAction
    {
        get
        {
            EnsureCreated();
            return _pause;
        }
    }

    //EditorSettings has EnterPlayModeOptions enabled, so a future "disable domain
    //reload" would carry these statics between play sessions.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        if (_pause != null)
        {
            _pause.Disable();
            _pause.Dispose();
            _pause = null;
        }

        if (_actions != null)
        {
            _actions.Dispose();
            _actions = null;
        }

        GameplayEnabled = true;
    }

    private static void EnsureCreated()
    {
        if (_actions != null)
            return;

        _actions = new InputActions();
        _actions.PlayerDefault.Enable();

        _pause = new InputAction("Pause", InputActionType.Button, "<Gamepad>/start");
        _pause.Enable();
    }

    public static void SetGameplayEnabled(bool value)
    {
        EnsureCreated();
        GameplayEnabled = value;

        //disabling the map also stops gamepad south from being read as Jump while a
        //menu is up, since UI/Submit is bound to the same button.
        if (value)
            _actions.PlayerDefault.Enable();
        else
            _actions.PlayerDefault.Disable();
    }
}

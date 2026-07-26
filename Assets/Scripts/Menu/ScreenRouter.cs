using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

// Manages the stack of menu screens
public class ScreenRouter : MonoBehaviour
{
    public static ScreenRouter Instance { get; private set; }

    [Header("Wiring")]
    [SerializeField] private InputSystemUIInputModule _inputModule;

    private struct Frame
    {
        public MenuScreen Screen;
        public GameObject Focus; // remembers which menu item was selected
    }

    private readonly List<Frame> _stack = new List<Frame>();

    public MenuScreen Top => _stack.Count > 0 ? _stack[_stack.Count - 1].Screen : null;
    public int Depth => _stack.Count;
    public bool HasScreens => _stack.Count > 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Push(MenuScreen screen)
    {
        if (screen == null || screen == Top)
            return;

        if (_stack.Count > 0)
        {
            var below = _stack[_stack.Count - 1];
            below.Focus = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
            _stack[_stack.Count - 1] = below;

            below.Screen.SetInteractable(false);
            if (!screen.KeepUnderlyingVisible)
                below.Screen.SetShown(false);
        }

        _stack.Add(new Frame { Screen = screen, Focus = null });

        screen.SetShown(true);
        screen.SetInteractable(true);
        screen.OnOpen();
        FocusDefaultOf(screen);
    }
    
    
    public bool Pop(bool playBackSfx = true)
    {
        if (_stack.Count == 0)
            return false;

        var top = _stack[_stack.Count - 1];
        _stack.RemoveAt(_stack.Count - 1);

        top.Screen.OnClose();
        top.Screen.SetInteractable(false);
        top.Screen.SetShown(false);

        if (playBackSfx)
            UISfx.PlayBack();

        if (_stack.Count == 0)
        {
            ClearFocus();
            return true;
        }

        var below = _stack[_stack.Count - 1];
        below.Screen.SetShown(true);
        below.Screen.SetInteractable(true);

        if (below.Focus != null && below.Focus.activeInHierarchy)
        {
            MenuWidget.SuppressHoverSfxThisFrame();
            EventSystem.current.SetSelectedGameObject(below.Focus);
        }
        else
        {
            FocusDefaultOf(below.Screen);
        }

        return true;
    }
    
    public void ClearAll()
    {
        for (var i = _stack.Count - 1; i >= 0; i--)
        {
            var frame = _stack[i];
            frame.Screen.OnClose();
            frame.Screen.SetInteractable(false);
            frame.Screen.SetShown(false);
        }

        _stack.Clear();
        ClearFocus();
    }

    public void ResetTo(MenuScreen screen)
    {
        ClearAll();
        Push(screen);
    }

    private void FocusDefaultOf(MenuScreen screen)
    {
        var item = screen.ResolveDefaultFocus();
        MenuWidget.SuppressHoverSfxThisFrame();

        if (EventSystem.current == null)
            return;

        EventSystem.current.SetSelectedGameObject(item != null ? item.gameObject : null);
    }
    
    private void ClearFocus()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private void Update()
    {
        HandleCancel();
        KeepFocusInsideTopScreen();
    }
    
    private void HandleCancel()
    {
        var cancel = _inputModule != null && _inputModule.cancel != null ? _inputModule.cancel.action : null;
        if (cancel == null || !cancel.WasPerformedThisFrame())
            return;

        Cancel();
    }
    
    public void Cancel()
    {
        if (_stack.Count == 0)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnCancelOutsideMenus();
            return;
        }

        Top.OnCancel();
    }
    
    private void KeepFocusInsideTopScreen()
    {
        if (_stack.Count == 0 || EventSystem.current == null)
            return;

        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected != null && selected.activeInHierarchy && selected.transform.IsChildOf(Top.transform))
            return;

        var move = _inputModule != null && _inputModule.move != null ? _inputModule.move.action : null;
        if (move == null || !move.WasPerformedThisFrame())
            return;

        FocusDefaultOf(Top);
    }
}

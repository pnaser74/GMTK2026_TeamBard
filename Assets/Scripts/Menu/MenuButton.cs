using UnityEngine;
using UnityEngine.Events;

public class MenuButton : MenuWidget
{
    [Header("Action")]
    [SerializeField] private UnityEvent _onActivate;

    public UnityEvent OnActivate => _onActivate;

    public void SetLabel(string text)
    {
        if (Label != null)
            Label.text = text;
    }

    protected override void OnActivated()
    {
        _onActivate?.Invoke();
    }
}

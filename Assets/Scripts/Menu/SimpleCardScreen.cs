using UnityEngine;

// stub for intro/ending/credits
public class SimpleCardScreen : MenuScreen
{
    public enum DismissAction
    {
        PopBack,
        StartAtIntroCheckpoint,
        ReturnToMainMenu
    }

    [Header("Behaviour")]
    [SerializeField] private DismissAction _onDismiss = DismissAction.PopBack;
    [SerializeField] private MenuButton _dismissItem;

    protected override void Awake()
    {
        base.Awake();
        _dismissItem.OnActivate.AddListener(OnDismissPressed);
    }

    public void OnDismissPressed()
    {
        switch (_onDismiss)
        {
            case DismissAction.StartAtIntroCheckpoint:
                GameManager.Instance.LoadCheckpoint(SaveService.IntroCheckpoint);
                break;

            case DismissAction.ReturnToMainMenu:
                GameManager.Instance.ReturnToMainMenu();
                break;

            default:
                ScreenRouter.Instance.Pop(false);
                break;
        }
    }

    public override bool OnCancel()
    {
        if (_onDismiss != DismissAction.PopBack)
            return true;

        return base.OnCancel();
    }
}

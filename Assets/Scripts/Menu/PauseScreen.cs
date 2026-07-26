using UnityEngine;

//Resume / Options / Return to Main Menu.
public class PauseScreen : MenuScreen
{
    [Header("Items")]
    [SerializeField] private MenuButton _resumeItem;
    [SerializeField] private MenuButton _optionsItem;
    [SerializeField] private MenuButton _returnToMainMenuItem;

    [Header("Screens")]
    [SerializeField] private OptionsScreen _optionsScreen;
    [SerializeField] private ConfirmPopupScreen _confirmPopup;

    protected override void Awake()
    {
        base.Awake();
        _resumeItem.OnActivate.AddListener(OnResumePressed);
        _optionsItem.OnActivate.AddListener(OnOptionsPressed);
        _returnToMainMenuItem.OnActivate.AddListener(OnReturnToMainMenuPressed);
    }

    public override bool OnCancel()
    {
        GameManager.Instance.Resume();
        return true;
    }

    // ---- callbacks ----

    public void OnResumePressed()
    {
        GameManager.Instance.Resume();
    }

    public void OnOptionsPressed()
    {
        ScreenRouter.Instance.Push(_optionsScreen);
    }

    public void OnReturnToMainMenuPressed()
    {
        _confirmPopup.Show(
            "Return to Main Menu?",
            "When you return to the game, you will start at the beginning of the current area.",
            "Continue",
            "Cancel",
            ConfirmPopupScreen.DefaultOption.Cancel,
            () => GameManager.Instance.ReturnToMainMenu());
    }
}

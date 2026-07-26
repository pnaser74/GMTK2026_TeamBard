using UnityEngine;

//Continue / New Game / Options / Quit
public class MainMenuScreen : MenuScreen
{
    [Header("Items")]
    [SerializeField] private MenuButton _continueItem;
    [SerializeField] private MenuButton _newGameItem;
    [SerializeField] private MenuButton _optionsItem;
    [SerializeField] private MenuButton _quitItem;

    [Header("Screens")]
    [SerializeField] private OptionsScreen _optionsScreen;
    [SerializeField] private ConfirmPopupScreen _confirmPopup;

    private MenuWidget[] _rows;
    private Vector2[] _rowSlots;

    protected override void Awake()
    {
        base.Awake();
        _continueItem.OnActivate.AddListener(OnContinuePressed);
        _newGameItem.OnActivate.AddListener(OnNewGamePressed);
        _optionsItem.OnActivate.AddListener(OnOptionsPressed);
        _quitItem.OnActivate.AddListener(OnQuitPressed);
        
        _rows = new MenuWidget[] { _continueItem, _newGameItem, _optionsItem, _quitItem };
        _rowSlots = new Vector2[_rows.Length];
        for (var i = 0; i < _rows.Length; i++)
            _rowSlots[i] = ((RectTransform)_rows[i].transform).anchoredPosition;
    }

    public override void OnOpen()
    {
        _continueItem.gameObject.SetActive(SaveService.HasSave);
        _quitItem.gameObject.SetActive(GameManager.CanQuit);

        PackVisibleRows();
        
        MenuNavigation.WireVertical(_rows);
    }

    private void PackVisibleRows()
    {
        var slot = 0;
        foreach (var row in _rows)
        {
            if (row == null || !row.gameObject.activeSelf)
                continue;

            ((RectTransform)row.transform).anchoredPosition = _rowSlots[slot];
            slot++;
        }
    }

    public override MenuWidget ResolveDefaultFocus()
    {
        return SaveService.HasSave ? _continueItem : (MenuWidget)_newGameItem;
    }

    // ---- item callbacks ----

    public void OnContinuePressed()
    {
        GameManager.Instance.ContinueGame();
    }

    public void OnNewGamePressed()
    {
        if (!SaveService.HasSave)
        {
            GameManager.Instance.StartNewGame();
            return;
        }

        //"Are You Sure? - Delete Existing Save / Cancel", cancel is the default.
        _confirmPopup.Show(
            "Are You Sure?",
            "Starting a new game will delete your existing save.",
            "Delete Existing Save",
            "Cancel",
            ConfirmPopupScreen.DefaultOption.Cancel,
            OnDeleteExistingSaveConfirmed);
    }

    public void OnOptionsPressed()
    {
        ScreenRouter.Instance.Push(_optionsScreen);
    }

    public void OnQuitPressed()
    {
        GameManager.Instance.QuitGame();
    }

    private void OnDeleteExistingSaveConfirmed()
    {
        SaveService.DeleteSave();
        GameManager.Instance.StartNewGame();
    }
}

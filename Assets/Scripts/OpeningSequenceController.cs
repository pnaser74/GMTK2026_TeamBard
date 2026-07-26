using UnityEngine;
using UnityEngine.Playables;

public class OpeningSequenceController : MonoBehaviour
{
    [Header("Cutscene")]
    [SerializeField] private PlayableDirector director;

    [Header("Player")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody2D playerRigidbody;

    [Header("UI")]
    [SerializeField] private CanvasGroup blackOverlay;

    private bool _cutsceneFinished;

    private void Awake()
    {
        playerController.SetInputEnabled(false);

        if (playerRigidbody != null)
            playerRigidbody.linearVelocity = Vector2.zero;
    }

    private void OnEnable()
    {
        director.stopped += OnCutsceneStopped;
    }

    private void Start()
    {
        director.time = 0;
        director.Play();
    }

    private void OnCutsceneStopped(PlayableDirector stoppedDirector)
    {
        if (_cutsceneFinished)
            return;

        _cutsceneFinished = true;

        if (blackOverlay != null)
            blackOverlay.alpha = 0;

        playerController.SetInputEnabled(true);
    }

    private void OnDisable()
    {
        if (director != null)
            director.stopped -= OnCutsceneStopped;
    }
}
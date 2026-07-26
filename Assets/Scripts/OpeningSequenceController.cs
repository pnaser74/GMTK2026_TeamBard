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
    private RigidbodyType2D _originalBodyType;

    private void Awake()
    {
        playerController.SetInputEnabled(false);

        if (playerRigidbody != null)
        {
            _originalBodyType = playerRigidbody.bodyType;
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
            playerRigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void OnEnable()
    {
        if (director != null)
            director.stopped += OnCutsceneStopped;
    }

    private void Start()
    {
        if (director == null)
        {
            Debug.LogError("Opening Sequence Controller has no Playable Director assigned.");
            return;
        }

        director.time = 0;
        director.Play();

        Debug.Log($"Opening cutscene started. Duration: {director.duration}");
    }

    private void OnCutsceneStopped(PlayableDirector stoppedDirector)
    {
        if (_cutsceneFinished)
            return;

        _cutsceneFinished = true;

        if (blackOverlay != null)
            blackOverlay.alpha = 0f;

        if (playerRigidbody != null)
        {
            playerRigidbody.bodyType = _originalBodyType;
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        playerController.SetInputEnabled(true);
    }

    private void OnDisable()
    {
        if (director != null)
            director.stopped -= OnCutsceneStopped;
    }
}
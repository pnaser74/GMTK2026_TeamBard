using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class OpeningSequenceController : MonoBehaviour
{
    [Header("Cutscene")]
    [SerializeField] private PlayableDirector director;

    [Header("Player")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody2D playerRigidbody;

    [Header("UI")]
    [SerializeField] private CanvasGroup blackOverlay;

    private bool cutsceneFinished;

    private void Awake()
    {
        // Renfield exists and can be animated, but the player cannot control him.
        playerInput.enabled = false;

        if (playerRigidbody != null)
            playerRigidbody.linearVelocity = Vector2.zero;
    }

    private void OnEnable()
    {
        director.stopped += OnCutsceneStopped;
    }

    private void OnDisable()
    {
        director.stopped -= OnCutsceneStopped;
    }

    private void Start()
    {
        director.time = 0;
        director.Play();
    }

    private void OnCutsceneStopped(PlayableDirector stoppedDirector)
    {
        if (cutsceneFinished)
            return;

        cutsceneFinished = true;

        if (blackOverlay != null)
            blackOverlay.alpha = 0;

        playerInput.enabled = true;
    }
}
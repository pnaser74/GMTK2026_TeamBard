using UnityEngine;
using FMODUnity;

public class GateSwitch : MonoBehaviour
{
    [Header("Gate")]
    [SerializeField] private RetractingGate _gate;
    [SerializeField] private string _requiredTag = "Player";

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _activatedSprite;

    [Header("FMOD Audio")]
    [SerializeField] private EventReference _activationSound;
    [SerializeField] private EventReference _gateRetractingSound;

    private bool _hasActivated;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasActivated || !other.CompareTag(_requiredTag))
            return;

        Activate();
    }

    private void Activate()
    {
        _hasActivated = true;

        if (_spriteRenderer != null && _activatedSprite != null)
            _spriteRenderer.sprite = _activatedSprite;

        if (!_activationSound.IsNull)
            RuntimeManager.PlayOneShot(_activationSound, transform.position);

        if (_gate != null)
        {
            if (!_gateRetractingSound.IsNull)
            {
                RuntimeManager.PlayOneShot(
                    _gateRetractingSound,
                    _gate.transform.position
                );
            }

            _gate.Open();
        }
    }
}
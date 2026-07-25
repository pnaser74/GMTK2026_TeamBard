using UnityEngine;

public class GateSwitch : MonoBehaviour
{
    [SerializeField] private RetractingGate _gate;
    [SerializeField] private string _requiredTag = "Player";

    private bool _hasActivated;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasActivated)
            return;

        if (!other.CompareTag(_requiredTag))
            return;

        _hasActivated = true;
        _gate.Open();
    }
}
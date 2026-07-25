using System.Collections;
using UnityEngine;

public class RetractingGate : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _retractDistance = 4f;
    [SerializeField] private float _retractSpeed = 3f;

    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private Coroutine _moveRoutine;
    private bool _isOpen;

    private void Awake()
    {
        _closedPosition = transform.position;
        _openPosition = _closedPosition + Vector3.up * _retractDistance;
    }

    public void Open()
    {
        if (_isOpen)
            return;

        _isOpen = true;

        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);

        _moveRoutine = StartCoroutine(MoveGate(_openPosition));
    }

    private IEnumerator MoveGate(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                _retractSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;
        _moveRoutine = null;
    }
}
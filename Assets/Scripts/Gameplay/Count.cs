using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Count : MonoBehaviour
{
    public enum FormState { Contained, Bat }

    [Header("Components")]
    [SerializeField] private PlayerController _player;

    [Header("Game State")]
    private FormState _currForm = FormState.Contained;

    [Header("Bat Form")]
    [SerializeField] private float _batRadiusHorizontal = 6f;
    [SerializeField] private float _batRangeVertical = 4f;
    [SerializeField] private float _batMovementSmoothness = 2f;
    [SerializeField] private float _batSeparationTime = 0.5f;
    [SerializeField] private float _batAvoidantBehaviorAmount = 0.5f;
    private Vector2 _batRangeCenter;
    private float _adherenceToPlayer = 1f;

    public void TurnToBat()
    {
        if (_currForm == FormState.Bat)
            return;

        _currForm = FormState.Bat;
        _batRangeCenter = Camera.main.transform.position;
        StartCoroutine(TransitionPlayerAdherence(1f, -_batAvoidantBehaviorAmount));
        StartCoroutine(MoveAsBat()); 
    }

    private IEnumerator TransitionPlayerAdherence(float start, float end, UnityEvent onFinished = null)
    {
        float startTime = Time.time;

        while (Time.time < startTime + _batSeparationTime)
        {
            float prog = (Time.time - startTime) / _batSeparationTime;
            prog = 1f - Mathf.Pow(1 - prog, 3f);
            _adherenceToPlayer = Mathf.Lerp(start, end, prog);
            yield return null;
        }

        _adherenceToPlayer = end;
        if (onFinished != null)
            onFinished.Invoke();
    }

    private IEnumerator MoveAsBat()
    {
        var horizRef = new Vector2(Random.Range(0, 100000f), Random.Range(0, 100000f));
        var vertRef = new Vector2(Random.Range(0, 100000f), Random.Range(0, 100000f));
        float startTime = Time.time;
        float minX = _batRangeCenter.x - _batRadiusHorizontal;
        float minY = _batRangeCenter.y - _batRangeVertical;

        while (_currForm == FormState.Bat)
        {
            float elapsedTime = Time.time - startTime;
            float perlinX = Mathf.PerlinNoise(horizRef.x + elapsedTime / _batMovementSmoothness, horizRef.y);
            float perlinY = Mathf.PerlinNoise(vertRef.x, vertRef.y + elapsedTime / _batMovementSmoothness);
            float freeX = minX + _batRadiusHorizontal * 2f * perlinX;
            float freeY = minY + _batRangeVertical * 2f * perlinY;
            var freePos = new Vector2(freeX, freeY);
            Vector2 playerPos = _player.transform.position;
            transform.position = freePos * (1f - _adherenceToPlayer) + playerPos * _adherenceToPlayer;
            yield return null;
        }
    }

    private void ReturnToPlayer()
    {
        _currForm = FormState.Contained;
        gameObject.SetActive(false);
        _player.OnCountRecontained();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_currForm == FormState.Bat && 
            collision.gameObject.CompareTag("Player") && 
            _adherenceToPlayer <= 0f) //bat caught by player
        {
            UnityEvent onCaught = new(); 
            onCaught.AddListener(() => ReturnToPlayer());
            StartCoroutine(TransitionPlayerAdherence(_adherenceToPlayer, 1f, onCaught));
        }
    }
}

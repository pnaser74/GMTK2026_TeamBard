using UnityEngine;

public class PitchforkPeasant : Enemy
{
    
    [Header("Pitchfork")]
    [SerializeField] private Transform _pitchforkPivot;
    [SerializeField] private float _jabDistance = 0.35f; // forward thrust length
    [SerializeField] private float _jabSpeed = 3.5f;     // idle jab rate
    [SerializeField] private float _nearJabSpeed = 20f;  // frenzied jabbing when close to player :)
    [SerializeField] private float _jabRange = 4f;       // distance at which jabs speed up
    
    private Vector3 _pivotBasePos; // rest position for the +x (right) facing; mirrored when facing left
    private float _jabPhase;      // offset so pitchforks don't sync
    private float _jabPhaseAccum; // accumulated phase; advanced by the current rate
    private Transform _player;

    protected override void Awake()
    {
        if (_pitchforkPivot != null)
            _pivotBasePos = _pitchforkPivot.localPosition;
        base.Awake();
        _jabPhase = (GetInstanceID() % 50) * 0.13f;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _player = player.transform;
    }

    protected override void Update()
    {
        base.Update();
        if (_pitchforkPivot == null)
            return;

        // Increase frenzied jab rate up as the player gets closer :)
        float rate = _jabSpeed;
        if (_player != null && _jabRange > 0f)
        {
            float toPlayerX = _player.position.x - transform.position.x;
            bool facingPlayer = toPlayerX * _currentMoveDirection >= 0f;
            if (facingPlayer)
            {
                float dist = Vector2.Distance(_player.position, transform.position);
                float proximity = Mathf.Clamp01(1f - dist / _jabRange);
                rate = Mathf.Lerp(_jabSpeed, _nearJabSpeed, proximity);
            }
        }

        // Accumulate the phase so changing the rate speeds up without glitching.
        _jabPhaseAccum += Time.deltaTime * rate;
        float thrust = Mathf.Max(0f, Mathf.Sin(_jabPhaseAccum + _jabPhase));
        Vector3 rest = new Vector3(Mathf.Abs(_pivotBasePos.x) * _currentMoveDirection, _pivotBasePos.y, _pivotBasePos.z);
        _pitchforkPivot.localPosition = rest + new Vector3(_jabDistance * thrust * _currentMoveDirection, 0f, 0f);
    }

    protected override void ApplyFacing()
    {
        base.ApplyFacing();
        if (_pitchforkPivot != null)
        {
            Vector3 scale = _pitchforkPivot.localScale;
            scale.x = Mathf.Abs(scale.x) * _currentMoveDirection;
            _pitchforkPivot.localScale = scale;
            
            Vector3 pos = _pitchforkPivot.localPosition;
            pos.x = Mathf.Abs(_pivotBasePos.x) * _currentMoveDirection;
            _pitchforkPivot.localPosition = pos;
        }
    }

    protected override void OnHitPlayer(Collision2D collision)
    {
       // Audio cue?
    }
}

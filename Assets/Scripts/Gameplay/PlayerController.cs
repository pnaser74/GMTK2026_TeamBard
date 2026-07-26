using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Collider2D _coll;
    [SerializeField] private Count _count;
    [SerializeField] private Animator animator;
    [SerializeField] private int countDownLength;

    private float currentTime;
    private bool alive;
    private InputActions _inputActions;
    private bool _inputEnabled = true;

    [Header("Game Feel")]
    [SerializeField] private float _maxJumpHeight = 3f;
    [SerializeField] private float _maxRunSpeed = 4f;
    [SerializeField] private float _timeToMaxRunSpeed = 0.5f;
    [SerializeField] private float _stopTime = 0.2f;

    [Header("Current Game State")]
    private bool _countContained = true;

    private void Awake()
    {
        InitializeInputActions();
    }

    private void Start()
    {
        StartCoroutine(ReadPlayerInput());
    }

    private void InitializeInputActions()
    {
        if (_inputActions != null)
            return;

        _inputActions = new InputActions();

        if (_inputEnabled)
            _inputActions.Enable();
    }

    public void SetInputEnabled(bool enabled)
    {
        _inputEnabled = enabled;
        InitializeInputActions();

        if (enabled)
        {
            _inputActions.Enable();
        }
        else
        {
            _inputActions.Disable();

            // Prevent Renfield from continuing to slide during the cutscene.
            if (_rb != null)
                _rb.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator ReadPlayerInput()
    {
        while (true)
        {
            if (!_inputEnabled)
            {
                yield return null;
                continue;
            }

            // Check horizontal movement.
            var movement =
                _inputActions.PlayerDefault.Run.ReadValue<Vector2>();

            if (movement.x != 0)
            {
                Move(movement);
            }
            else if (_rb.linearVelocityX != 0 && OnGround())
            {
                StopMoving();
            }

            // Check jumping.
            if (_inputActions.PlayerDefault.Jump.triggered && OnGround())
                StartCoroutine(Jump());

            yield return null;
        }
    }

    private void Move(Vector2 input)
    {
        // Gradually accelerate to max speed, but turn around instantly.
        var delta =
            Time.deltaTime * _maxRunSpeed / _timeToMaxRunSpeed;

        var startVelo = input.x < 0
            ? Mathf.Min(0, _rb.linearVelocityX)
            : Mathf.Max(0, _rb.linearVelocityX);

        var direction = input.x < 0 ? -1 : 1;

        var xVelo = Mathf.Clamp(
            startVelo + delta * direction,
            -_maxRunSpeed,
            _maxRunSpeed
        );

        _rb.linearVelocityX = xVelo;
    }

    private void StopMoving()
    {
        // Gradually slow to a stop.
        var start = _rb.linearVelocityX;

        var delta =
            Time.deltaTime *
            _maxRunSpeed /
            _stopTime *
            (start > 0 ? -1 : 1);

        var xVelo = start > 0
            ? Mathf.Max(start + delta, 0f)
            : Mathf.Min(start + delta, 0f);

        _rb.linearVelocityX = xVelo;
    }

    private bool OnGround()
    {
        // Check for a point of collision below the player's center.
        var points = new List<ContactPoint2D>();
        _coll.GetContacts(points);

        foreach (var point in points)
        {
            if (point.point.y < transform.position.y)
                return true;
        }

        return false;
    }

    private IEnumerator Jump()
    {
        var startTime = Time.time;
        var acceleration = Physics2D.gravity.y * _rb.gravityScale;
        var startingVelocity =
            Mathf.Sqrt(-2f * acceleration * _maxJumpHeight);

        var endTime =
            startTime + (-startingVelocity / acceleration);

        while (
            _inputEnabled &&
            !_inputActions.PlayerDefault.Jump.WasReleasedThisFrame() &&
            Time.time < endTime
        )
        {
            var elapsedTime = Time.time - startTime;

            var verticalVelocity =
                startingVelocity + acceleration * elapsedTime;

            _rb.linearVelocity = new Vector2(
                _rb.linearVelocityX,
                verticalVelocity
            );

            yield return null;
        }

        if (_rb.linearVelocityY > 0)
            _rb.linearVelocityY = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == 6)
            OnTouchedHazard();
    }

    private void OnTouchedHazard()
    {
        if (!_countContained)
            return;

        _countContained = false;
        _count.gameObject.SetActive(true);
        _count.TurnToBat();
        animator.SetBool("countContained", _countContained);
    }

    public void OnCountRecontained()
    {
        _countContained = true;
        animator.SetBool("countContained", _countContained);
    }

    private void OnDestroy()
    {
        if (_inputActions != null)
            _inputActions.Disable();
    }
}
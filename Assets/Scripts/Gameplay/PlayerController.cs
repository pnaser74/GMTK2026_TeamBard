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
    private InputActions _inputActions;

    [Header("Game Feel")]
    [SerializeField] private float _maxJumpHeight = 3f;
    [SerializeField] private float _maxRunSpeed = 4f;
    [SerializeField] private float _timeToMaxRunSpeed = 0.5f;
    [SerializeField] private float _stopTime = 0.2f;

    [Header("Current Game State")]
    private bool _countContained = true;

    private void Start()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();
        StartCoroutine(ReadPlayerInput());
    }

    private IEnumerator ReadPlayerInput()
    {
        while (true)
        {
            //check horizontal movement
            var movement = _inputActions.PlayerDefault.Run.ReadValue<Vector2>();
            if (movement.x != 0)
                Move(movement);
            else if (_rb.linearVelocityX != 0 && OnGround())
                StopMoving();

            //check jumping
            if (_inputActions.PlayerDefault.Jump.triggered && OnGround())
                StartCoroutine(Jump());

            yield return null;
        }
    }

    private void Move(Vector2 input)
    {
        //gradually accelerate to max speed, turn around instantly
        var delta = Time.deltaTime * _maxRunSpeed / _timeToMaxRunSpeed;
        var startVelo = input.x < 0 ? Mathf.Min(0, _rb.linearVelocityX) : Mathf.Max(0, _rb.linearVelocityX);
        var xVelo = Mathf.Clamp(startVelo + delta * (input.x < 0 ? -1 : 1), -_maxRunSpeed, _maxRunSpeed);
        _rb.linearVelocityX = xVelo;
    }

    private void StopMoving()
    {
        //gradually slow to a stop
        var start = _rb.linearVelocityX;
        var delta = Time.deltaTime * _maxRunSpeed / _stopTime * (start > 0 ? -1 : 1);
        var xVelo = start > 0 ? Mathf.Max(start + delta, 0f) : Mathf.Min(start + delta, 0f);
        _rb.linearVelocityX = xVelo;
    }

    private bool OnGround()
    {
        //check for point of collision below center of player
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
        var a = Physics2D.gravity.y * _rb.gravityScale;
        var u = Mathf.Sqrt(-2f * a * _maxJumpHeight);
        var endTime = startTime + -u / a;

        //slow upward velocity to mirror real-world physics, aborting for small jumps
        while (!_inputActions.PlayerDefault.Jump.WasReleasedThisFrame() && Time.time < endTime)
        {
            var t = Time.time - startTime;
            var v = u + a * t;
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, v);
            yield return null;
        }

        if (_rb.linearVelocityY > 0)
            _rb.linearVelocityY = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == 6) //hazard layer
            OnTouchedHazard();
    }

    private void OnTouchedHazard()
    {
        if (!_countContained)
            return;

        _countContained = false;
        _count.gameObject.SetActive(true);
        _count.TurnToBat();
    }

    public void OnCountRecontained()
    {
        _countContained = true;
    }

    private void OnDestroy()
    {
        _inputActions.Disable();
    }
}

using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    [Header("Patrol")]
    [SerializeField] protected float _moveSpeed = 1.5f;
    [SerializeField] protected int _startDirection = 1;  // 1 is right, -1 is left
    
    [Header("Walking")] // Probabaly remove this once we have different art.
    [SerializeField] protected float _bobAmplitude = 0.08f;
    [SerializeField] protected float _bobFrequency = 8f;   // bobs per second

    [Header("Ledge Detection")]
    [SerializeField] protected bool _detectLedges = true;
    [SerializeField] protected float _ledgeCheckDistance = 0.75f;
    [SerializeField] protected Vector2 _ledgeCheckOffset = new Vector2(0.4f, 0f);
    [SerializeField] protected LayerMask _groundMask;

    [Header("Sprite")]
    [SerializeField] protected SpriteRenderer _spriteRenderer; // body sprite
    [SerializeField] protected bool _spriteFacesRight = false;

    protected Rigidbody2D _rigidbody;
    protected Collider2D _collider;
    protected int _currentMoveDirection;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _currentMoveDirection = _startDirection < 0 ? -1 : 1;
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        ApplyFacing();
    }

    protected virtual void FixedUpdate()
    {
        if (_detectLedges && !GroundAhead())
            Flip();

        _rigidbody.linearVelocity = new Vector2(_currentMoveDirection * _moveSpeed, _rigidbody.linearVelocity.y);
    }
    
    protected virtual void Update() { }

    protected bool GroundAhead()
    {
        float bottom = _collider != null ? _collider.bounds.min.y : transform.position.y;
        Vector2 origin = new Vector2(
            transform.position.x + _ledgeCheckOffset.x * _currentMoveDirection,
            bottom + _ledgeCheckOffset.y - 0.02f);
        return Physics2D.Raycast(origin, Vector2.down, _ledgeCheckDistance, _groundMask);
    }
    
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            OnHitPlayer(collision);
            
        // A mostly-horizontal contact normal means we hit some kind of wall: turn around.
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.9f)
            {
                Flip();
                break;
            }
        }
    }
    
    protected virtual void Flip()
    {
        _currentMoveDirection *= -1;
        ApplyFacing();
    }

    protected virtual void ApplyFacing()
    {
        if (_spriteRenderer != null)
            _spriteRenderer.flipX = (_currentMoveDirection > 0) != _spriteFacesRight;
    }

    protected virtual void OnHitPlayer(Collision2D collision) { }

}

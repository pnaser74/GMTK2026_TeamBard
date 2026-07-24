using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.5f;

    private Rigidbody2D rb;
    private int moveDirection = -1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            moveDirection * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("PLAYER HAS BEEN HIT!");
        }

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // A mostly horizontal normal means the enemy hit a wall.
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                moveDirection *= -1;
                break;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public LayerMask platformsLayerMask;

    public float speedMod;

    CapsuleCollider2D capsuleCollider2d;
    Rigidbody2D rb2D;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        capsuleCollider2d = GetComponent<CapsuleCollider2D>();
    }

    private Vector2 movement = Vector3.zero;
    void Update()
    {
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            float jumpVelocity = 25f;
            Debug.Log("jump");
            rb2D.velocity = Vector2.up * jumpVelocity;
        }

    }

    bool IsGrounded()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(capsuleCollider2d.bounds.center, capsuleCollider2d.bounds.size, 0f, Vector2.down, 0.1f, platformsLayerMask);
        return raycastHit2d.collider != null;
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);

        // You have to maintain the current speed in the Y direction or else other Y velocities will be slowed. To do this, set the y-velocity to the value it already is.
        rb2D.velocity = movement.normalized * speedMod + new Vector3(0.0f, rb2D.velocity.y, 0.0f);
    }
}

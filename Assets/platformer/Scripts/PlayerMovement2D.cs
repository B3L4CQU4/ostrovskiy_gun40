using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement2D : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 3f;

    private Rigidbody2D body;
    private InputAction moveAction;
    private InputAction jumpAction;
    private bool isGrounded;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        PlayerInput input = GetComponent<PlayerInput>();
        moveAction = input.actions.FindAction("Move", true);
        jumpAction = input.actions.FindAction("Jump", true);
    }

    private void Update()
    {
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce);
            isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        float direction = moveAction.ReadValue<Vector2>().x;
        body.velocity = new Vector2(direction * speed, body.velocity.y);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Поверхность находится под ногами
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
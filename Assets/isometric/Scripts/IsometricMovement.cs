using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class IsometricMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody2D body;
    private InputAction moveAction;
    private Vector2 input;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        moveAction = GetComponent<PlayerInput>()
            .actions.FindAction("Move", true);
    }

    private void Update()
    {
        input = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector2 isometricDirection = new Vector2(
            input.x - input.y,
            (input.x + input.y) * 0.5f
        ).normalized;

        body.velocity = isometricDirection * speed;
    }
}
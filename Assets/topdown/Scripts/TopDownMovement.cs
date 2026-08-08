using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class TopDownMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody2D body;
    private InputAction moveAction;
    private Vector2 direction;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        moveAction = GetComponent<PlayerInput>()
            .actions.FindAction("Move", true);
    }

    private void Update()
    {
        direction = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        body.velocity = direction.normalized * speed;
    }
}
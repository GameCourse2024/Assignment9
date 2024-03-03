using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] InputAction moveAction;

    private Animator _animator;
    private CharacterController _controller;
    private Vector3 velocity = new Vector3(0, 0, 0);

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    void OnValidate()
    {
        // Provide default bindings for the input actions.
        if (moveAction == null)
            moveAction = new InputAction(type: InputActionType.Button);
        if (moveAction.bindings.Count == 0)
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
    }

    public override void Spawned()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
    }

    public override void FixedUpdateNetwork()
{
    if (!HasStateAuthority)
    {
        return;
    }

    Vector3 movement = moveAction.ReadValue<Vector2>();
    velocity.x = movement.x * speed;
    velocity.z = movement.y * speed;
    _controller.Move(velocity * Runner.DeltaTime);

    // Set walking animation based on movement magnitude
    bool isWalking = movement.magnitude > 0.1f;
    Debug.Log("FixedUpdateNetwork - isWalking: " + isWalking);
    _animator.SetBool("isRunning", isWalking);
}
}

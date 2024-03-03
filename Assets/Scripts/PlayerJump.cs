using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

// Based on Fusion tutorial: https://doc.photonengine.com/fusion/current/tutorials/shared-mode-basics/3-movement-and-camera
public class PlayerJump: NetworkBehaviour {
    [SerializeField] float JumpVelocity    = 5f;
    [SerializeField] float GravityValue = -9.81f;
    [SerializeField] float speed = 2f;

    private Animator _animator;
    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction jumpAction;
    private void OnEnable() { 
        jumpAction.Enable();
        moveAction.Enable();
    }
    private void OnDisable() { 
        jumpAction.Disable();
        moveAction.Disable();
    }
    void OnValidate() {
        // Provide default bindings for the input actions. Based on answer by DMGregory: https://gamedev.stackexchange.com/a/205345/18261
        if (jumpAction == null)
            jumpAction = new InputAction(type: InputActionType.Button);
        if (jumpAction.bindings.Count == 0)
            jumpAction.AddBinding("<Keyboard>/space");

        if (moveAction == null)
            moveAction = new InputAction(type: InputActionType.Button);
        if (moveAction.bindings.Count == 0)
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
    }

    private bool _jumpPressed;
    void Update() {  // We have to read the button status in Update, because FixedNetworkUpdate might miss it.
        if (jumpAction.WasPressedThisFrame()) {
            _jumpPressed = true;
        }
    }

    private CharacterController _controller;
    public override void Spawned() {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
    }

    private Vector3 velocity = new Vector3(0, 0, 0);
    public override void FixedUpdateNetwork() {
        if (!HasStateAuthority) {
            return;
        }
        // Check if the player is jumping based on the vertical velocity
        bool isJumping = velocity.y > 0.1f;
        _animator.SetBool("isJumping", isJumping);
        
        if (_controller.isGrounded) {
            velocity = new Vector3(0, -1, 0);
        } else {
            velocity.y += GravityValue * Runner.DeltaTime;
        }
        if (_jumpPressed && _controller.isGrounded) {
            velocity.y += JumpVelocity;
        }

        Vector3 movement = moveAction.ReadValue<Vector2>();
        velocity.x = movement.x * speed;
        velocity.z = movement.y * speed;

        _controller.Move(velocity*Runner.DeltaTime);
        _jumpPressed = false;
        // Set walking animation based on movement magnitude
        bool isWalking = movement.magnitude > 0.1f;
        Debug.Log(isWalking);
        
        // Add the line below to set the walking animation parameter
        _animator.SetBool("isRunning", isWalking);
        
        // Rotate the player based on movement input
        if (movement.magnitude > 0.1f)
        {
            float targetRotation = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetRotation, 0f);
        }
    }
}



using Fusion;
using UnityEngine;

public enum PlayerButtons {
    Jump = 1 << 0,
    Run = 1 << 1
}

public struct NetworkInputData : INetworkInput {
    public Vector2 move;
    public NetworkButtons buttons;

    public bool RunHeld => buttons.IsSet((int)PlayerButtons.Run);
}

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class PlayerController : NetworkBehaviour {

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Movement Smoothing")]
    [SerializeField] private bool useAcceleration = false;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpUpMultiplier = 1.5f;
    [SerializeField] private float fallMultiplier = 2.5f;

    [Header("Jump Timing")]
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController characterController;
    private Vector3 currentVelocity;
    private Vector3 velocity;
    private Vector2 moveInput;
    private bool isRunning;
    private bool isGrounded;

    private float jumpBufferCounter;
    private float coyoteTimeCounter;

    private NetworkButtons prevButtons;

    public override void Spawned() {

        characterController = GetComponent<CharacterController>();
        characterController.minMoveDistance = 0f;
        velocity = Vector3.zero;
        isGrounded = false;
    }

    public override void FixedUpdateNetwork() {

        if(GetInput(out NetworkInputData input)) {
            moveInput = input.move;
            isRunning = input.RunHeld;

            if(input.buttons.WasPressed(prevButtons, (int)PlayerButtons.Jump)) {
                jumpBufferCounter = jumpBufferTime;
            }

            prevButtons = input.buttons;
        }

        // -------- Horizontal movement --------
        float targetSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 targetMove = transform.right * moveInput.x + transform.forward * moveInput.y;
        targetMove *= targetSpeed;

        // --- Smooth or Instant Movement ---
        if(useAcceleration) {
            float lerpRate = targetMove.magnitude > 0.01f ? acceleration : deceleration;
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetMove, lerpRate * Runner.DeltaTime);
            characterController.Move(currentVelocity * Runner.DeltaTime);

        } else {
            characterController.Move(targetMove * Runner.DeltaTime);
        }

        // -------- Grounding --------
        bool wasGrounded = isGrounded;
        isGrounded = characterController.isGrounded || Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded) {
            coyoteTimeCounter = coyoteTime;

            if(velocity.y < 0f) {
                velocity.y = -2f;
            }

        } else {
            coyoteTimeCounter -= Runner.DeltaTime;
        }

        // -------- Jump --------
        if(jumpBufferCounter > 0 && coyoteTimeCounter > 0) {

            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        // -------- Gravity --------
        if(!isGrounded) {
            if(velocity.y < 0f) {
                velocity.y += gravity * fallMultiplier * Runner.DeltaTime;      // Falling

            } else if(velocity.y > 0f) {
                velocity.y += gravity * jumpUpMultiplier * Runner.DeltaTime;    // Rising

            } else {
                velocity.y += gravity * Runner.DeltaTime;                       // Neutral (edge cases)
            }
        }

        // -------- Apply vertical velocity --------
        characterController.Move(Vector3.up * velocity.y * Runner.DeltaTime);

        // -------- Timers --------
        jumpBufferCounter -= Runner.DeltaTime;
    }

    private void OnDrawGizmos() {

        if(!groundCheck) {
            return;
        }

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}

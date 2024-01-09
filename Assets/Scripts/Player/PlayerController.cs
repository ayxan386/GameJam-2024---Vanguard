using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float rotationSpeed = 10f;
    [FormerlySerializedAs("coyoteTime")] [SerializeField] private float jumpBuffer = 0.1f;
    [SerializeField] private Animator animator;

    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector3 jumpVector;

    private bool isJumping;
    private bool groundedPlayer;
    private float jumpTime;

    public int PlayerIndex => playerInput.playerIndex;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        MainGameManager.Instance.PlayerJoined(this);
    }

    private void Update()
    {
        if (!MainGameManager.Instance.CanMove) return;

        HandleMovement();
    }

    public void MoveTo(Transform location)
    {
        characterController.enabled = false;

        transform.position = location.position;
        transform.rotation = location.rotation;

        characterController.enabled = true;
    }

    private void HandleMovement()
    {
        groundedPlayer = characterController.isGrounded;
        if (groundedPlayer && jumpVector.y < 0)
        {
            jumpVector.y = 0f;
        }

        // Calculate movement direction
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        // moveDirection = transform.TransformDirection(moveDirection);
        moveDirection.Normalize(); // Ensure the diagonal movement isn't faster

        // Rotate towards the movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            animator.SetBool("running", true);
        }
        else
        {
            animator.SetBool("running", false);
        }

        moveDirection *= moveSpeed;

        // Move the character controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Handle jumping
        if (isJumping && groundedPlayer)
        {
            jumpVector.y += jumpForce;
            animator.SetTrigger("jump");
        }

        // Apply gravity
        if (!groundedPlayer)
        {
            jumpVector.y -= gravity * Time.deltaTime;
        }

        if (jumpTime + jumpBuffer < Time.time)
        {
            isJumping = false;
        }

        characterController.Move(jumpVector * Time.deltaTime);
    }

    private void OnMove(InputValue val)
    {
        moveInput = val.Get<Vector2>();
    }

    private void OnJump()
    {
        isJumping = true;
        jumpTime = Time.time;
    }
}
using System;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

    [Header("cameras")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private Camera playerCamera;

    [Header("Beacon detection")]
    [SerializeField] private LayerMask beaconLayer;

    [SerializeField] private float detectionRadius;
    [Header("UI elements")]
    [SerializeField] private Image selectedColorImage;
    [SerializeField] private GameObject eliminationPanel;
    [SerializeField] private TextMeshProUGUI reachCountText;

    [Header("Power ups")]
    [SerializeField] private LayerMask powerUpLayers;

    [SerializeField] private float powerUpDetectionRadius;

    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector3 jumpVector;

    private bool isJumping;
    private bool groundedPlayer;
    private float jumpTime;
    private Color selectedColor;

    public int PlayerIndex => playerInput.playerIndex;
    public bool IsEliminated { get; set; }
    public bool Reached { get; set; }

    public static event Action<PlayerController> OnReachedBeacon; 

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        MainGameManager.Instance.PlayerJoined(this);

        var layerMask = MainGameManager.Instance.PlayerLayers[PlayerIndex];
        int layerToAdd = (int)Mathf.Log(layerMask.value, 2);
        //set the layer
        virtualCamera.gameObject.layer = layerToAdd;
        //add the layer
        for (var index = 0; index < MainGameManager.Instance.PlayerLayers.Length; index++)
        {
            if(index == PlayerIndex) continue;
            var layer = MainGameManager.Instance.PlayerLayers[index];
            int layerToRemove = (int)Mathf.Log(layer.value, 2);
            playerCamera.cullingMask &= ~(1 << layerToRemove);
        }

        GameController.OnNextStageStarted += OnNextStageStarted;
        GameController.OnReachCountUpdate += OnReachCountUpdate;
    }

    private void OnReachCountUpdate(int current, int total)
    {
        reachCountText.text = $"{current}/{total}";
    }

    private void OnNextStageStarted(Color color)
    {
        if(IsEliminated) return;
        selectedColor = color;
        color.a = 1f;
        selectedColorImage.color = color;
        Reached = false;
    }

    private void Update()
    {
        if (!MainGameManager.Instance.CanMove
            || IsEliminated) return;

        HandleMovement();
        CheckForBeacon();
        CheckForPowerUp();
    }

    private void CheckForPowerUp()
    {
        var basePos = transform.position;
        
        var allPowerUps = Physics.CapsuleCastAll(basePos - Vector3.up, basePos + Vector3.up, 
            powerUpDetectionRadius,
            Vector3.up, powerUpDetectionRadius, powerUpLayers);

        foreach (var powerUpHit in allPowerUps)
        {
            if (powerUpHit.transform.TryGetComponent(out PowerUp powerUp))
            {
                powerUp.Use(this);
            }
        }
    }

    private void CheckForBeacon()
    {
        if(Reached) return;
        var allBeacons = Physics.SphereCastAll(transform.position, detectionRadius, Vector3.forward, detectionRadius, beaconLayer);
        foreach (var beacon in allBeacons)
        {
            if (beacon.transform.TryGetComponent(out Renderer rend))
            {
                if (rend.material.color == selectedColor)
                {
                    Reached = true;
                    OnReachedBeacon?.Invoke(this);
                }
            }
        }
    }

    public void TemporaryBoostForSpeed(float mult)
    {
        moveSpeed *= mult;
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
        Vector3 moveDirection = new Vector3(0, 0f, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection.Normalize(); 

        
        transform.Rotate(Vector3.up, moveInput.x * rotationSpeed * Time.deltaTime);
        // Rotate towards the movement direction
        if (moveDirection != Vector3.zero)
        {
            // if (moveInput.y >= 0)
            // {
            //     Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            //     transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            // }
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

    public void SetEliminated()
    {
        IsEliminated = true;
        selectedColorImage.gameObject.SetActive(false);
        eliminationPanel.SetActive(true);
        animator.SetBool("running", false);
    }
}
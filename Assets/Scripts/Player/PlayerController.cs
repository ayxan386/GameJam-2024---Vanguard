using System;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float rotationSpeed = 10f;
    [FormerlySerializedAs("coyoteTime")] [SerializeField] private float jumpBuffer = 0.1f;

    [Header("Sprinting")]
    [SerializeField] private float sprintBoost = 8.5f;

    [SerializeField] private float sprintDuration;
    [SerializeField] private Image sprintImage;
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
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Button backToMainMenu;
    [SerializeField] private TextMeshProUGUI reachCountText;

    [Header("Power ups")]
    [SerializeField] private LayerMask powerUpLayers;

    [SerializeField] private float powerUpDetectionRadius;

    [Header("Player indicator")]
    [SerializeField] private Renderer colorIndicator;

    [SerializeField] private Transform rigs;

    [field: SerializeField] public ThrowableObject Throwable { get; set; }
    [field: SerializeField] public HandAnimator handAnimator { get; private set; }
    public int Score { get; private set; }

    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector3 jumpVector;

    private bool isJumping;
    private bool groundedPlayer;
    private float jumpTime;
    private float sprintTime;
    private Color selectedColor;
    private bool isSprinting;
    private float speedMult = 1;

    public int PlayerIndex => playerInput.playerIndex;
    public bool IsEliminated { get; set; }
    public bool Reached { get; set; }
    public bool IsFrozen { get; set; }
    public Color PlayerColor { get; set; }
    public Animator Animator => animator;

    public static event Action<PlayerController> OnReachedBeacon;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        MainGameManager.Instance.PlayerJoined(this);

        CameraLayerSettings();
        colorIndicator.material.color = PlayerColor;

        GameController.OnNextStageStarted += OnNextStageStarted;
        GameController.OnPlayerVictory += OnPlayerVictory;

        var rig = rigs.GetChild(PlayerIndex);
        rig.gameObject.SetActive(true);
        animator = rig.GetComponent<Animator>();
        handAnimator = rig.GetComponent<HandAnimator>();
    }

    private void OnDestroy()
    {
        GameController.OnNextStageStarted -= OnNextStageStarted;
        GameController.OnPlayerVictory -= OnPlayerVictory;
    }


    public void Restart()
    {
        SceneManager.LoadScene("Map_01");
    }

    private void OnPlayerVictory(int obj)
    {
        if (IsEliminated) return;

        print(victoryPanel);

        victoryPanel.SetActive(true);
        backToMainMenu.Select();
    }

    private void CameraLayerSettings()
    {
        var layerMask = MainGameManager.Instance.PlayerLayers[PlayerIndex];
        int layerToAdd = (int)Mathf.Log(layerMask.value, 2);
        //set the layer
        virtualCamera.gameObject.layer = layerToAdd;
        //add the layer
        for (var index = 0; index < MainGameManager.Instance.PlayerLayers.Length; index++)
        {
            if (index == PlayerIndex) continue;
            var layer = MainGameManager.Instance.PlayerLayers[index];
            int layerToRemove = (int)Mathf.Log(layer.value, 2);
            playerCamera.cullingMask &= ~(1 << layerToRemove);
        }
    }

    public void OnScoreUpdate(int current, int total)
    {
        reachCountText.text = $"{current}/{total}";
    }

    private void OnNextStageStarted(Color color)
    {
        if (IsEliminated || selectedColorImage == null) return;
        selectedColor = color;
        color.a = 1f;
        selectedColorImage.color = color;
        Reached = false;
    }

    private void Update()
    {
        if (!MainGameManager.Instance.CanMove
            || IsEliminated) return;

        ApplySprintTimes();
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
        if (Reached) return;
        var allBeacons = Physics.SphereCastAll(transform.position, detectionRadius, Vector3.forward, detectionRadius,
            beaconLayer);
        foreach (var beacon in allBeacons)
        {
            if (beacon.transform.TryGetComponent(out Renderer rend))
            {
                if (rend.material.color == selectedColor)
                {
                    Reached = true;
                    Score++;
                    OnReachedBeacon?.Invoke(this);
                }
            }
        }
    }

    public void TemporaryBoostForSpeed(float mult)
    {
        speedMult = mult;

        IsFrozen = mult == 0;
    }

    public void MoveTo(Transform location)
    {
        characterController.enabled = false;

        transform.SetPositionAndRotation(location.position, location.rotation);

        characterController.enabled = true;
    }

    private void HandleMovement()
    {
        if (IsFrozen) return;
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

        animator.SetBool("running", moveDirection != Vector3.zero);
        animator.SetBool("walking", moveDirection == Vector3.zero && moveInput.x != 0);


        moveDirection *= (isSprinting ? sprintBoost : moveSpeed) * speedMult;

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

    private void ApplySprintTimes()
    {
        sprintTime += (isSprinting ? -1 : 1) * Time.deltaTime;
        sprintTime = Mathf.Clamp(sprintTime, 0, sprintDuration);

        if (sprintTime <= 0)
        {
            isSprinting = false;
        }

        sprintImage.fillAmount = sprintTime / sprintDuration;
    }

    public void OnThrow(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && Throwable != null)
        {
            handAnimator.StopHolding(Throwable, transform.forward);
            Throwable = null;
        }
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isSprinting = true;
        }
        else if (ctx.canceled)
        {
            isSprinting = false;
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnJump()
    {
        isJumping = true;
        jumpTime = Time.time;
    }

    public void SetEliminated()
    {
        IsEliminated = true;
        selectedColorImage.gameObject.SetActive(false);
        eliminationPanel.SetActive(true);
        characterController.enabled = false;
        animator.SetBool("running", false);
        animator.SetTrigger("dead");
    }
}
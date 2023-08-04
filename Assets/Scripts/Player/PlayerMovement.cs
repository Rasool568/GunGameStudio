using UnityEngine;

public enum PlayerState
{
    Idle,
    Walk,
    Sprint,
    Crouch,
}

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public bool Busy = false;
    public bool lockView = false;
    public bool lockMove = false;
    public bool lockJump = false;
    public bool lockRunning = false;

    [Header("Key Binds")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Variables")]
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float sensitivity = 0.8f;
    public float gravity = 20.0f;
    private float defaultSpeedMultiplier = 1f;
    [Range(1f, 2f)] public float crouchSpeedMultiplier = 1.5f;
    [Range(1f, 2f)] public float sprintSpeedMultiplier = 1.5f;
    [HideInInspector] public float speedMultiplier;

    public Vector3 moveDirection = Vector3.zero;
    public PlayerState currentState;
    public PlayerState newState;

    public CharacterController controller;
    public Transform playerCamera;

    private void OnValidate()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>().transform;
    }

    private void Awake()
    {
        newState = PlayerState.Walk;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!Busy) MyInput();
    }

    //Обработка ввода игрока и просчёт движения и поворота
    private void MyInput()
    {
        HandleMovementStates();
        if (!lockView)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);

            playerCamera.Rotate(-Input.GetAxis("Mouse Y") * sensitivity, 0, 0);
            if (playerCamera.localRotation.eulerAngles.y != 0)
                playerCamera.Rotate(Input.GetAxis("Mouse Y") * sensitivity, 0, 0);
        }
        if (!lockMove)
        {
            //Sprint
            if (Input.GetKeyDown(sprintKey) && !lockRunning)
                newState = PlayerState.Sprint;
            if (Input.GetKeyUp(sprintKey) && !lockRunning)
                newState = PlayerState.Walk;

            //Crouch
            if (Input.GetKeyDown(crouchKey))
                newState = PlayerState.Crouch;
            if (Input.GetKeyUp(crouchKey))
                newState = PlayerState.Walk;

            //Jump
            if (controller.isGrounded)
            {
                if (Input.GetKeyDown(jumpKey) && !lockJump) moveDirection.y = jumpSpeed;
                else moveDirection.y = 0;
            }

            moveDirection = new Vector3(Input.GetAxis("Horizontal") * speed * speedMultiplier, moveDirection.y, Input.GetAxis("Vertical") * speed * speedMultiplier);
            moveDirection = transform.TransformDirection(moveDirection);
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    //Проверяет в каком из возможных состояний находится игрок
    private void HandleMovementStates()
    {
        if (currentState == newState)
            return;

        speedMultiplier = defaultSpeedMultiplier;
        controller.height = 1.7f;
        controller.center = new Vector3(0, 0, 0);

        if (newState == PlayerState.Idle)
        {
            speedMultiplier = 0;
        }
        else if (newState == PlayerState.Walk)
        {
            //Nothing Here
        }
        else if (newState == PlayerState.Sprint)
        {
            speedMultiplier *= sprintSpeedMultiplier;
        }
        else if (newState == PlayerState.Crouch)
        {
            speedMultiplier /= crouchSpeedMultiplier;
            controller.height = 1f;
            controller.center = new Vector3(0, 0.3f, 0);
        }
        currentState = newState;
    }

    //Меняет чувствительность камеры игрока
    public void ChangeSensitivity(float _value)
    {
        sensitivity = _value;
    }
}

using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 8f;
    public float laneDistance = 3f; // khoảng cách giữa các lane
    public float laneChangeSpeed = 10f;
    public float centerX = -12.67f;
    [Header("Jump")]
    public float jumpForce = 7f;
    public float gravity = -20f;

    private CharacterController controller;
    private Vector3 velocity;

    private int currentLane = 1; // 0 = left, 1 = middle, 2 = right
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        HandleLaneChange();
        HandleJump();

        Vector3 move = Vector3.forward * forwardSpeed;

        // Target position theo lane
        float targetX = centerX + (currentLane - 1) * laneDistance;
        float diff = targetX - transform.position.x;
        move.x = diff * laneChangeSpeed;

        controller.Move(move * Time.deltaTime);

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleLaneChange()
    {
        if (Input.GetKeyDown(KeyCode.A) && currentLane > 0)
            currentLane--;

        if (Input.GetKeyDown(KeyCode.D) && currentLane < 2)
            currentLane++;

        // Swipe mobile (basic)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                if (touch.deltaPosition.x > 50 && currentLane < 2)
                    currentLane++;
                else if (touch.deltaPosition.x < -50 && currentLane > 0)
                    currentLane--;
                else if (touch.deltaPosition.y > 50 && isGrounded)
                    velocity.y = jumpForce;
            }
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = jumpForce;
        }
    }
}


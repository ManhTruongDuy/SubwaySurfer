using UnityEngine;
using System.Collections; // BẮT BUỘC PHẢI THÊM DÒNG NÀY ĐỂ DÙNG LỆNH CHỜ (IEnumerator)

public class Character : MonoBehaviour
{
    public Animator animator;

    [Header("Movement")]
    public float startSpeed = 8f;
    public float maxSpeed = 25f;
    public float speedIncreaseAmount = 2f;
    public float speedIncreaseInterval = 5f;

    public float laneDistance = 3f;
    public float laneChangeSpeed = 10f;
    public float centerX = -12.67f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public float gravity = -20f;

    // --- THÊM PHẦN BIẾN LỘN VÒNG (ROLL) ---
    [Header("Roll Settings")]
    public float rollDuration = 0.8f; // Thời gian lộn vòng
    private float originalHeight;
    private Vector3 originalCenter;
    private bool isRolling = false;
    // ---------------------------------

    [Header("Chase & Death")]
    public GameObject guardModel;
    public float chaseDuration = 5f;

    private int stumbleCount = 0;
    private float chaseTimer = 0f;
    private bool isDead = false;

    private float hitCooldown = 0f;

    private CharacterController controller;
    private Vector3 velocity;

    private int currentLane = 1;
    private bool isGrounded;

    private float currentSpeed;
    private float speedTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = startSpeed;

        if (animator != null)
        {
            animator.SetBool("IsGame", true);
        }

        if (guardModel != null) guardModel.SetActive(false);

        // LƯU LẠI CHIỀU CAO GỐC LÚC MỚI VÀO GAME
        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    void Update()
    {
        if (isDead) return;

        if (hitCooldown > 0) hitCooldown -= Time.deltaTime;

        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        HandleLaneChange();
        HandleJump();
        HandleRoll(); // GỌI HÀM LỘN VÒNG

        speedTimer += Time.deltaTime;
        if (speedTimer >= speedIncreaseInterval)
        {
            speedTimer = 0f;
            currentSpeed = Mathf.Min(currentSpeed + speedIncreaseAmount, maxSpeed);
        }

        Vector3 move = Vector3.forward * currentSpeed;

        float targetX = centerX + (currentLane - 1) * laneDistance;
        float diff = targetX - transform.position.x;
        move.x = diff * laneChangeSpeed;

        controller.Move(move * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (stumbleCount == 1)
        {
            chaseTimer -= Time.deltaTime;
            if (chaseTimer <= 0)
            {
                stumbleCount = 0;
                if (guardModel != null) guardModel.SetActive(false);
                Debug.Log("Cắt đuôi thành công! An toàn!");
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("💀 Test Death bằng nút T!");
            Die();
        }
    }

    void HandleLaneChange()
    {
        if (Input.GetKeyDown(KeyCode.A) && currentLane > 0)
            currentLane--;

        if (Input.GetKeyDown(KeyCode.D) && currentLane < 2)
            currentLane++;

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
                // THÊM: VUỐT XUỐNG ĐỂ LỘN VÒNG
                else if (touch.deltaPosition.y < -50 && !isRolling)
                    StartCoroutine(RollRoutine());
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

    // HÀM BẮT PHÍM LỘN VÒNG
    void HandleRoll()
    {
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !isRolling)
        {
            StartCoroutine(RollRoutine());
        }
    }

    // HÀM ÉP LÙN VA CHẠM (COROUTINE)
    private IEnumerator RollRoutine()
    {
        isRolling = true;

        // Gọi Animation lộn vòng
        if (animator != null)
        {
            animator.SetTrigger("Roll");
        }

        // Ép lùn chiều cao xuống 1 nửa
        controller.height = originalHeight / 6f;
        controller.center = new Vector3(originalCenter.x, originalCenter.y / 6f, originalCenter.z);

        // Đợi bằng đúng thời gian cuộn
        yield return new WaitForSeconds(rollDuration);

        // Đứng dậy, trả lại chiều cao như cũ
        controller.height = originalHeight;
        controller.center = originalCenter;

        isRolling = false;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isDead) return;

        if (hit.gameObject.CompareTag("Train"))
        {
            if (hit.normal.z < -0.5f)
            {
                Debug.Log("💥 Đâm mặt trước Tàu hỏa! Chết ngay!");
                Die();
            }
            else
            {
                HitObstacle();
            }
        }
        else if (hit.gameObject.CompareTag("Obstacle"))
        {
            HitObstacle();
            hit.collider.enabled = false;
        }
    }

    void HitObstacle()
    {
        if (hitCooldown > 0) return;

        hitCooldown = 0.5f;

        stumbleCount++;

        if (stumbleCount == 1)
        {
            chaseTimer = chaseDuration;
            if (guardModel != null) guardModel.SetActive(true);
            Debug.Log("⚠️ Vấp sườn tàu/rào chắn! Bảo vệ xuất hiện đuổi theo!");
        }
        else if (stumbleCount >= 2)
        {
            Debug.Log("💀 Vấp lần 2 khi đang bị đuổi! Bảo vệ tóm cổ!");
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        this.enabled = false;

        if (animator != null) animator.enabled = false;
        if (guardModel != null) guardModel.SetActive(false);

        GameUIController ui = UnityEngine.Object.FindFirstObjectByType<GameUIController>();
        if (ui != null)
        {
            int currentScore = ScoreManager.instance != null ? ScoreManager.instance.score : 0;
            int currentCoins = ScoreManager.instance != null ? ScoreManager.instance.coins : 0;
            ui.ShowDeathMenu(currentScore, currentCoins);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy GameUIController trong Scene!");
        }
    }
}
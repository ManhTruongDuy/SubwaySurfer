using UnityEngine;

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

    // --- THÊM PHẦN BIẾN CHO BẢO VỆ ---
    [Header("Chase & Death")]
    public GameObject guardModel;    // Kéo thả ông bảo vệ (Pepsi) vào đây
    public float chaseDuration = 5f; // Thời gian bảo vệ đuổi (5 giây)
    
    private int stumbleCount = 0;    // Số lần vấp ngã
    private float chaseTimer = 0f;   // Bộ đếm thời gian
    private bool isDead = false;     // Trạng thái sống chết

    private float hitCooldown = 0f;
    // ---------------------------------

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
            animator.SetBool("IsGame", true); // Tên "IsGame" phải giống hệt bước 1
        }

        // Đảm bảo lúc mới vào game ông bảo vệ bị ẩn đi
        if (guardModel != null) guardModel.SetActive(false);
    }

    void Update()
    {
        // NẾU CHẾT RỒI THÌ DỪNG MỌI HOẠT ĐỘNG
        if (isDead) return;

        if (hitCooldown > 0) hitCooldown -= Time.deltaTime;

        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        HandleLaneChange();
        HandleJump();

        // ⏱ tăng speed mỗi 5s
        speedTimer += Time.deltaTime;
        if (speedTimer >= speedIncreaseInterval)
        {
            speedTimer = 0f;
            currentSpeed = Mathf.Min(currentSpeed + speedIncreaseAmount, maxSpeed);
        }

        Vector3 move = Vector3.forward * currentSpeed;

        // lane movement
        float targetX = centerX + (currentLane - 1) * laneDistance;
        float diff = targetX - transform.position.x;
        move.x = diff * laneChangeSpeed;

        controller.Move(move * Time.deltaTime);

        // gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- LOGIC BẢO VỆ ĐUỔI ---
        if (stumbleCount == 1)
        {
            chaseTimer -= Time.deltaTime; // Trừ dần 5 giây
            if (chaseTimer <= 0)
            {
                stumbleCount = 0; // Hết giờ -> Thoát nạn
                if (guardModel != null) guardModel.SetActive(false); // Ẩn bảo vệ
                Debug.Log("Cắt đuôi thành công! An toàn!");
            }
        }

        // --- NÚT T ĐỂ TEST CHẾT ---
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("💀 Test Death bằng nút T!");
            Die(); // Đã gom code của Hải xuống hàm Die() ở dưới cho gọn
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

    // --- THÊM HÀM XỬ LÝ VA CHẠM THỰC TẾ ---
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isDead) return;

        if (hit.gameObject.CompareTag("Train"))
        {
            // hit.normal.z < -0.5 tức là bề mặt va chạm đang đẩy ngược lại phía sau 
            // -> Chứng tỏ Hải đang đâm trực diện vào mặt trước của tàu
            if (hit.normal.z < -0.5f)
            {
                Debug.Log("💥 Đâm mặt trước Tàu hỏa! Chết ngay!");
                Die();
            }
            else
            {
                // Nếu không phải mặt trước thì là quẹt mạn sườn -> Bị bảo vệ đuổi
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
        // Nếu vừa mới vấp xong, đang trong thời gian chờ thì không tính thêm nữa
        if (hitCooldown > 0) return;

        hitCooldown = 0.5f; // Khóa va chạm trong 0.5 giây tiếp theo để không bị x2 số lần vấp

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

    // Hàm Die được gom lại từ code cũ của Hải
    void Die()
    {
        isDead = true;
        this.enabled = false;      // Dừng nhân vật
        
        if (animator != null) animator.enabled = false; // Tắt animation
        if (guardModel != null) guardModel.SetActive(false); // Ẩn luôn ông bảo vệ cho gọn

        GameUIController ui = UnityEngine.Object.FindFirstObjectByType<GameUIController>();
        if (ui != null)
        {
            int currentScore = ScoreManager.instance != null ? ScoreManager.instance.score : 0;
            int currentCoins = ScoreManager.instance != null ? ScoreManager.instance.coins : 0;
            ui.ShowDeathMenu(currentScore, currentCoins); // Hiện bảng điểm thật
        }
        else 
        {
            Debug.LogWarning("Không tìm thấy GameUIController trong Scene!");
        }
    }
}
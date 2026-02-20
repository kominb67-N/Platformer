using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public int max_jumps = 2;

    [Header("Juice & Lag Fix")]
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    [Header("Audio & Ground")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;
    public AudioClip jumpSound;
    private AudioSource audioSource;

    private Rigidbody2D rb;
    private Animator anim;
    private int jumpCount;
    private float moveInput;
    private Vector2 platformVelocity;
    private bool mobileLeft, mobileRight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb.freezeRotation = true;
        Application.targetFrameRate = 60;

        // --- แก้บั๊ก Checkpoint: วาร์ปไปจุดล่าสุดทันทีที่เริ่มฉาก ---
        if (CheckpointManager.hasCheckpoint)
        {
            transform.position = CheckpointManager.lastCheckpointPos;
        }
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (mobileLeft) moveInput = -1;
        if (mobileRight) moveInput = 1;

        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (isGrounded && rb.linearVelocity.y <= 0)
        {
            coyoteTimeCounter = coyoteTime;
            jumpCount = 0;
            if (anim != null) anim.SetBool("IsJumping", false);
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            if (anim != null) anim.SetBool("IsJumping", true);
        }

        if (Input.GetKeyDown(KeyCode.Space)) jumpBufferCounter = jumpBufferTime;
        else jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && (coyoteTimeCounter > 0f || jumpCount < max_jumps))
        {
            ExecuteJump();
            jumpBufferCounter = 0f;
        }

        if (anim != null) anim.SetFloat("Speed", Mathf.Abs(moveInput));
        if (moveInput > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2((moveInput * moveSpeed) + platformVelocity.x, rb.linearVelocity.y);
    }

    void ExecuteJump()
    {
        if (audioSource != null && jumpSound != null) audioSource.PlayOneShot(jumpSound);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += Vector2.up * jumpForce;
        jumpCount++;
        coyoteTimeCounter = 0f;
    }

    public void SetPlatformVelocity(Vector2 v) => platformVelocity = v;
    public void PointerDownLeft() => mobileLeft = true;
    public void PointerUpLeft() => mobileLeft = false;
    public void PointerDownRight() => mobileRight = true;
    public void PointerUpRight() => mobileRight = false;
    public void MobileJumpClick() => jumpBufferCounter = jumpBufferTime;
}
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public int maxJumps = 2;
    private int jumpCount = 0;

    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // เช็คพื้น
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // รีเซ็ตการกระโดด
        if (isGrounded && rb.linearVelocity.y <= 0)
        {
            jumpCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded || jumpCount < maxJumps)
            {
                Jump();
            }
        }

        // บรรทัดนี้จะบอกคุณใน Console ว่าสถานะพื้นเป็นอย่างไร
//Debug.Log("Is Grounded: " + isGrounded + " | Jump Count: " + jumpCount);
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += Vector2.up * jumpForce;
        jumpCount++;
    }

    // วาดวงกลมให้เห็นในหน้า Scene
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red; // แตะพื้นจะเป็นสีเขียว ไม่แตะจะเป็นสีแดง
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}
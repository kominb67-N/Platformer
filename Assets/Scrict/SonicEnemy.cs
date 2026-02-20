using UnityEngine;
using System.Collections;

public class SonicEnemy : MonoBehaviour
{
    [Header("1. การเคลื่อนที่")]
    public float patrolSpeed = 2f;
    public float dashSpeed = 12f;
    public float chargeTime = 0.8f;
    public float spinSpeed = 1000f;

    [Header("2. ส่วนประกอบ")]
    public Transform bodySprite;
    public Transform frontCheck;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    private Rigidbody2D rb;
    private int direction = 1;
    private bool isDashing = false;
    private bool isCharging = false;
    private bool isDead = false;
    private float currentSpeed;

    public AudioClip deathSound;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb) rb.freezeRotation = true;
        currentSpeed = patrolSpeed;
    }

    void Update()
    {
        if (isDead) return;
        if (isDashing && bodySprite != null)
            bodySprite.Rotate(0, 0, -spinSpeed * direction * Time.deltaTime);
        else if (!isCharging && bodySprite != null)
            bodySprite.rotation = Quaternion.Lerp(bodySprite.rotation, Quaternion.identity, Time.deltaTime * 10f);
    }

    void FixedUpdate()
    {
        if (isDead || isCharging) { if (isCharging) rb.linearVelocity = Vector2.zero; return; }
        rb.linearVelocity = new Vector2(currentSpeed * direction, rb.linearVelocity.y);

        RaycastHit2D playerHit = Physics2D.Raycast(frontCheck.position, Vector2.right * direction, 5f, whatIsPlayer);
        if (!isDashing && !isCharging && playerHit.collider != null && playerHit.collider.CompareTag("Player"))
            StartCoroutine(ChargeAndDash());

        RaycastHit2D wallHit = Physics2D.Raycast(frontCheck.position, Vector2.right * direction, 0.6f, whatIsGround);
        bool isWall = (wallHit.collider != null && !wallHit.collider.CompareTag("Player"));
        bool isEdge = !Physics2D.Raycast(frontCheck.position, Vector2.down, 1f, whatIsGround);

        if (isWall || isEdge) { if (isDashing) { isDashing = false; currentSpeed = patrolSpeed; } Flip(); }
    }

    IEnumerator ChargeAndDash() { isCharging = true; yield return new WaitForSeconds(chargeTime); isCharging = false; isDashing = true; currentSpeed = dashSpeed; }

    void Flip() { direction *= -1; Vector3 s = transform.localScale; s.x *= -1; transform.localScale = s; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            // *** 1. ถ้าผู้เล่นอมตะ (ร่างสายรุ้ง) -> ตายแบบดาวกระจาย ***
            if (playerHealth != null && playerHealth.isInvincible)
            {
                StartCoroutine(StarDeath());
                return;
            }

            // *** 2. ถ้าไม่ได้อมตะ เช็กการเหยียบหัว หรือการพุ่งชน ***
            if (collision.GetContact(0).normal.y < -0.5f)
            {
                // เหยียบหัว -> ตายแบบตัวแบน
                collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 10f);
                StartCoroutine(SquashAndDie());
            }
            else
            {
                // ชนด้านข้าง
                if (isDashing)
                {
                    playerHealth?.TakeDamage(1);
                    Flip(); isDashing = false; currentSpeed = patrolSpeed;
                }
                else playerHealth?.TakeDamage(1);
            }
        }
    }

    // ท่าตายแบบตัวแบน (Squash)
    IEnumerator SquashAndDie()
    {
        isDead = true;
        if (deathSound != null) AudioSource.PlayClipAtPoint(deathSound, transform.position); // เล่นเสียง ณ จุดตาย
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.2f, 1f);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    // ท่าตายแบบดาวกระจาย (Star)
    IEnumerator StarDeath()
    {
        isDead = true;
        GetComponent<Collider2D>().enabled = false;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 3f;
            rb.linearVelocity = new Vector2(Random.Range(-5f, 5f), 15f);
            rb.angularVelocity = 1000f;
        }
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
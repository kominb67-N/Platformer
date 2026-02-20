using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
    public enum EnemyType { Classic, Smart, Rusher, Jumper }

    [Header("1. ตั้งค่านิสัย")]
    public EnemyType enemyType = EnemyType.Smart;
    public bool randomizeTypeOnStart = true;

    [Header("2. การเคลื่อนที่")]
    public float moveSpeed = 2f;
    public float rushSpeed = 5f;
    public float jumpForce = 5f;
    private float currentSpeed;
    private int direction = 1;
    private bool isDead = false;

    [Header("3. ระบบเซนเซอร์")]
    public Transform frontCheck;
    public float checkDistance = 0.5f;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    private Rigidbody2D rb;
    private bool canJump = true;
    public AudioClip deathSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = moveSpeed;
        if (randomizeTypeOnStart) enemyType = (EnemyType)Random.Range(0, 4);
    }

    void FixedUpdate()
    {
        if (isDead) return;
        rb.linearVelocity = new Vector2(currentSpeed * direction, rb.linearVelocity.y);
        bool wall = Physics2D.Raycast(frontCheck.position, Vector2.right * direction, checkDistance, whatIsGround);
        bool edge = false;
        if (enemyType != EnemyType.Classic) { RaycastHit2D g = Physics2D.Raycast(frontCheck.position, Vector2.down, 1f, whatIsGround); if (g.collider == null) edge = true; }
        if (wall || edge) Flip();
        if (enemyType == EnemyType.Rusher || enemyType == EnemyType.Jumper) CheckPlayer();
    }

    void CheckPlayer()
    {
        RaycastHit2D p = Physics2D.Raycast(frontCheck.position, Vector2.right * direction, 4f, whatIsPlayer);
        if (p.collider != null && p.collider.CompareTag("Player"))
        {
            if (enemyType == EnemyType.Rusher) currentSpeed = rushSpeed;
            else if (enemyType == EnemyType.Jumper && canJump && IsGrounded()) { rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); StartCoroutine(JumpCooldown()); }
        }
        else currentSpeed = moveSpeed;
    }

    bool IsGrounded() { return Physics2D.Raycast(transform.position, Vector2.down, 1.1f, whatIsGround); }
    IEnumerator JumpCooldown() { canJump = false; yield return new WaitForSeconds(1.5f); canJump = true; }
    void Flip() { direction *= -1; Vector3 s = transform.localScale; s.x *= -1; transform.localScale = s; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            // *** 1. ถ้าผู้เล่นอมตะ -> ตายแบบดาวกระจาย ***
            if (playerHealth != null && playerHealth.isInvincible)
            {
                StartCoroutine(StarDeath());
                return;
            }

            // *** 2. ถ้าไม่ได้อมตะ เช็กการเหยียบหัว ***
            if (collision.GetContact(0).normal.y < -0.5f)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 10f);
                StartCoroutine(SquashAndDie());
            }
            else playerHealth?.TakeDamage(1);
        }
    }

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
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform stompCheck;
    [SerializeField] private Transform hitCheck;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask ladderLayerMask;
    [SerializeField] private TrailRenderer trender;
    [SerializeField] private Animator anim;
    [SerializeField] private int health;
    [SerializeField] private int score;
    [SerializeField] private BMOController bmoController;
    protected bool playerActive = false;

    [Header("Platform Variables")]
    [SerializeField] private Collider2D platformCollider;
    [SerializeField] private PlatformEffector2D effector;

    [Header("Sound Variables")]
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private AudioClip jumpSound;

    [Header("Health Variables")]
    private int maxHealth = 5;
    protected float health_remaining = 0.0f;

    [Header("Level Boundary Variables")]
    private float rightBound = 27f;
    private float leftBound = -45f;
    private float adjustRBound = 26.75f;
    private float adjustLBound = -44.75f;

    [Header("Movement Variables")]
    protected RaycastHit2D climbInfo;
    private bool isGrounded = false;
    private bool isJumping = false;
    private bool facingRight = true;
    private float groundCheckRadius = 0.2f;
    private float horizInput;
    private float vertInput;
    private float moveSpeed = 450.0f;

    [Header("Jump Variables")]
    private float gravity;
    private int jumpsMax = 2;
    protected int jumpsAvailable = 0;
    private float jumpHeight = 4.0f;
    private float jumpTime = 0.75f;
    private float initialJumpVelocity;

    [Header("Dashing Variables")]
    private bool isDashing = false;
    private bool dashAvail = false;
    private float dashStrength = 16f;
    private float jumpDashStrength = 10f;
    private float dashTime = 0.5f;
    private float initialGravity;
    private Vector2 dashDirection;

    [Header("Miscellaneous Animation Variables")]
    private bool isRunning = false;
    private bool isAttacking = false;

    [Header("Attack Variables")]
    private float attackRange = 0.5f;
    private float jumpAttackRange = 0.3f;
    protected int attackDamage = 1;
    protected bool swordAttack = false;
    private float stompCheckRadius = 0.5f;

    [Header("Miscellaneous")]
    private float distance = 0f;
    private int winningScore = 15;
    private float shortDelay = 0.25f;
    private float hitDelay = 2.0f;
    private float waterDelay = 1.0f;
    protected bool isHit = false;
    protected bool isWater = false;

    private void Awake()
    {
        Messenger<int>.AddListener(GameEvent.PICKUP_HEALTH,
                                   this.OnPickupHealth);
        Messenger<int>.AddListener(GameEvent.PICKUP_COIN, this.OnPickupCoin);
    }

    private void OnDestroy()
    {
        Messenger<int>.RemoveListener(GameEvent.PICKUP_HEALTH,
                                      this.OnPickupHealth);
        Messenger<int>.RemoveListener(GameEvent.PICKUP_COIN, this.OnPickupCoin);
    }

    private void Start()
    {
        float timeToApex = jumpTime / 2.0f;
        gravity = (-2 * jumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);
        health = maxHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isDashing)
        {
            return;
        }

        // ensures coroutines are only called when player is active
        if (player.activeSelf)
        {
            playerActive = true;
        }
        else
        {
            playerActive = false;
        }

        // movement
        horizInput = Input.GetAxisRaw("Horizontal");

        // running
        isRunning = horizInput > 0.01 || horizInput < -0.01;
        anim.SetBool("isRunning", isRunning);

        isGrounded =
            Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius,
                                    groundLayerMask) &&
            rbody.velocity.y < 0.01;

        if (isGrounded)
        {
            jumpsAvailable = jumpsMax;  // double-jump
            dashAvail = true;
        }
        else if (!isGrounded && !isJumping && !isDashing)
        {
            rbody.gravityScale = gravity / Physics2D.gravity.y;
        }

        // jump
        if (Input.GetButton("Jump") && isGrounded)
        {
            Jump();
            isJumping = false;
        }
        // jump
        else if (Input.GetButton("Jump") && jumpsAvailable > 0)
        {
            Jump();
            jumpsAvailable--;
            isJumping = false;
        }

        // attack
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
        {
            swordAttack = true;
            Attack(hitCheck, swordAttack);
        }

        // resets attack state
        if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E))
        {
            isAttacking = false;
            anim.SetBool("isAttacking", isAttacking);
        }

        // dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashAvail)
        {
            Dash();
        }

        // drop-down
        if (Input.GetKeyDown(KeyCode.S))
        {
            DropDown();
        }

        // resets drop-down
        if (Input.GetKeyUp(KeyCode.S))
        {
            StartCoroutine(ResetEffector());
        }

        // orients player sprite
        if ((!facingRight && horizInput > 0.01f) ||
            (facingRight && horizInput < -0.01f))
        {
            Flip();
        }

        // ensures player movement is constrained to level bounds
        if (transform.position.x >= rightBound)
        {
            transform.position =
                new Vector2(adjustRBound, transform.position.y);
        }
        else if (transform.position.x <= leftBound)
        {
            transform.position =
                new Vector2(adjustLBound, transform.position.y);
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            // determines x velocity
            float xVel = horizInput * moveSpeed * Time.deltaTime;
            rbody.velocity = new Vector2(xVel, rbody.velocity.y);
        }
        // distance between player and BMO character
        distance =
            Vector2.Distance(groundCheck.position, bmoController.BMOLocation);
        CheckForWinCondition(score, distance);
    }

    private void Jump()
    {
        isJumping = true;
        anim.SetTrigger("jump");
        soundManager.PlaySoundClip(jumpSound, transform, 1f);
        rbody.velocity = new Vector2(rbody.velocity.x, initialJumpVelocity);
        rbody.gravityScale = gravity / Physics2D.gravity.y;
        StartCoroutine(ResetJumpTrigger());
    }

    private void Attack(Transform checkType, bool swordAttack)
    {
        Collider2D[] hitEnemy;
        if (swordAttack)
        {
            isAttacking = true;
            anim.SetBool("isAttacking", isAttacking);
            hitEnemy = Physics2D.OverlapCircleAll(checkType.position,
                                                  attackRange, enemyLayerMask);
            EnactHit(hitEnemy);
        }

        // jump attack
        hitEnemy = Physics2D.OverlapCircleAll(checkType.position,
                                              jumpAttackRange, enemyLayerMask);
        EnactHit(hitEnemy);
    }

    private void EnactHit(Collider2D[] nearbyEnemies)
    {
        foreach (Collider2D enemy in nearbyEnemies)
        {
            if (enemy != null)
            {
                EnemyHealth eh = enemy.GetComponent<EnemyHealth>();
                if (eh != null)
                {
                    eh.enemyHit(attackDamage);
                }
            }
        }
    }

    private void Dash()
    {
        isDashing = true;
        dashAvail = false;
        initialGravity = rbody.gravityScale;
        rbody.gravityScale = 0f;
        dashDirection = new Vector2(horizInput, vertInput);

        if (dashDirection == Vector2.zero)
        {
            // dash takes place in direction player is facing
            if (facingRight)
            {
                SetVelocity(transform.localScale.x, dashStrength);
                if (!isGrounded)
                {
                    SetVelocity(transform.localScale.x, jumpDashStrength);
                }
            }
            else
            {
                SetVelocity(-transform.localScale.x, dashStrength);
                if (!isGrounded)
                {
                    SetVelocity(-transform.localScale.x, jumpDashStrength);
                }
            }
        }
        if (trender != null)
        {
            trender.emitting = true;
        }
        StartCoroutine(PauseDashing());
    }

    private void SetVelocity(float xCoordinate, float dashStrength)
    {
        rbody.velocity = new Vector2(xCoordinate * dashStrength, 0f);
    }

    private IEnumerator PauseDashing()
    {
        yield return new WaitForSeconds(dashTime);
        rbody.gravityScale = initialGravity;
        if (trender != null)
        {
            trender.emitting = false;
        }
        isDashing = false;
    }

    private void DropDown() { effector.rotationalOffset = 180f; }

    private IEnumerator ResetEffector()
    {
        yield return new WaitForSeconds(shortDelay);
        effector.rotationalOffset = 0f;
        isGrounded = true;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(Vector3.up * 180);
    }

    public void Hit(bool isWater)
    {
        anim.SetTrigger("hit");
        health -= 1;
        health_remaining = ((float)health) / maxHealth;
        if (health == 0)
        {
            Messenger.Broadcast(GameEvent.PLAYER_DEAD);
        }
        if (!isWater)
        {
            StartCoroutine(HitDelay(hitDelay));
        }
        else
        {
            StartCoroutine(HitDelay(waterDelay));
        }
    }

    public IEnumerator HitDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        isHit = false;
    }

    private IEnumerator ResetJumpTrigger()
    {
        yield return new WaitForSeconds(shortDelay);
        anim.ResetTrigger("jump");
    }

    private void OnDrawGizmos()
    {
        // Draws a sphere at the transform's position
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(stompCheck.position, stompCheckRadius);

        if (hitCheck != null)
        {
            Gizmos.DrawWireSphere(hitCheck.position, attackRange);
        }
    }

    public void OnPickupHealth(int healthAdded)
    {
        health += healthAdded;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        float healthPerecent = ((float)health / maxHealth);
        Messenger<float>.Broadcast(GameEvent.HEALTH_CHANGED, healthPerecent);
    }

    public void OnPickupCoin(int coinToAdd)
    {
        score += coinToAdd;
        Messenger<int>.Broadcast(GameEvent.SCORE_CHANGED, score);
    }

    public void CheckForWinCondition(int score, float distance)
    {
        if (score == winningScore && Mathf.Abs(distance) < 5f)
        {
            Messenger.Broadcast(GameEvent.WIN_GAME);
        }
    }

    public IEnumerator CheckForEffector(Collider2D collision)
    {
        yield return new WaitForSeconds(0.5f);
        if (collision != null)
        {
            if (collision.tag == "Platform")
            {
                isGrounded = true;
            }
        }
    }

    // if jumping or dashing, can attack enemy, otherwise it damages player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        swordAttack = false;
        if (collision.gameObject.tag == "Enemy")
        {
            if (isDashing)
            {
                Attack(hitCheck, swordAttack);
            }
            else if (!isGrounded)
            {
                Attack(stompCheck, swordAttack);
            }
            else
            {
                isHit = true;
                if (isHit)
                {
                    Hit(isWater);
                }
            }
        }

        if (collision.gameObject.tag == "Water")
        {
            isGrounded = false;
            isHit = true;
            if (isHit)
            {
                isWater = true;
                Hit(isWater);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Water")
        {
            isGrounded = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            StartCoroutine(CheckForEffector(collision));
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.tag == "Platform")
            {
                isGrounded = false;
            }
            if ((collision.gameObject.tag == "Enemy" ||
                 collision.gameObject.tag == "Water"))
            {
                if (anim != null)
                {
                    anim.ResetTrigger("hit");
                }

                if (collision.gameObject.tag == "Water")
                {
                    effector.enabled = true;
                }
            }
        }
    }

    public int Health
    {
        get { return this.health; }
        private set { health = value; }
    }

    public int MaxHealth
    {
        get { return this.maxHealth; }
        private set { maxHealth = value; }
    }

    public Vector2 PlayerLocation
    {
        get { return transform.position; }
    }
}

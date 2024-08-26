using Pathfinding;
using UnityEngine;

public class EnemyUpdate : MonoBehaviour
{
    [Header("Enemy Components")]
    [SerializeField] private AIPath aiPath;
    [SerializeField] private Transform enemyGraphics;
    [SerializeField] private float enemyBuffer = 5.0f;
    [SerializeField] Animator anim;
    GameObject[] enemies;

    [Header("Track Player")]
    [SerializeField] private Transform target;
    private float distanceToTarget = float.MaxValue;
    private float attackRange = 3f;
    private bool isInRange = false;

    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    void Update()
    {
        // ensures sprite is rotated the correct direction of movement
        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            enemyGraphics.localScale = new Vector3(-1f, 1f, 1f);
        }
        if (aiPath.desiredVelocity.x <= -0.01f)
        {
            enemyGraphics.localScale = new Vector3(1f, 1f, 1f);
        }

        distanceToTarget = Vector2.Distance(transform.position, target.position);
        if (distanceToTarget < attackRange)
        {
            EnemyAttack();
        }
        else if (distanceToTarget > attackRange)
        {
            isInRange = false;
            anim.SetBool("isInRange", isInRange);
        }
    }

    private void FixedUpdate()
    {
        // ensures enemies don't move on top of one another
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                float distanceToNextEnemy = Vector2.Distance(enemy.transform.position,
                       transform.position);
                if (distanceToNextEnemy <= enemyBuffer)
                {
                    Vector2 moveDirection = transform.position - enemy.transform.position;
                    transform.Translate(moveDirection * Time.deltaTime);
                }
            }
        }
    }

    private void EnemyAttack()
    {
        isInRange = true;
        anim.SetBool("isInRange", isInRange);
    }
}



using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Idle
    }

    [SerializeField] private int maxHP = 5;
    [SerializeField] private int currentHP;


    public float viewRadius = 60f;
    [Range(0, 360)]
    public float defaultViewAngle = 90f;
    public float currentViewAngle = 90f;
    public float alertedViewAngle = 360f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    private Animator animator;

    private Transform targetPlayer;
    private GameObject[] players;
    private Rigidbody rb;


    [Header("Patrolling")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float waitTimeAtPoint = 2f;

    private int currentPatrolIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    EnemyShooting shooting;
    public float shootCooldown = 1f;
    private float lastShootTime = -Mathf.Infinity;

    private bool isDead = false;



    private EnemyState currentState = EnemyState.Patrolling;


    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        shooting = GetComponent<EnemyShooting>();

        currentHP = maxHP;
    }


    void Update()
    {

        if (isDead) return;

        Transform visiblePlayer = GetVisiblePlayer();

        if (visiblePlayer != null)
        {
            animator.SetBool("Spotted", true);
            LookAtPlayer(visiblePlayer);
            if (Time.time >= lastShootTime + shootCooldown && currentHP != 0)
            {
                shooting.Shoot();
                lastShootTime = Time.time;
            }
            // Optional: stop moving / enter alert mode here
        }
        else
        {
            animator.SetBool("Spotted", false);
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        Vector3 dir = (targetPoint.position - transform.position).normalized;
        dir.y = 0;

        float dist = Vector3.Distance(transform.position, targetPoint.position);

        if (dist <= 0.2f)
        {
            
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = waitTimeAtPoint;
            }

            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                isWaiting = false;
            }

            animator.SetBool("Walking", false);
        }
        else
        {
            // Move toward point
            transform.position += dir * patrolSpeed * Time.deltaTime;
            animator.SetBool("Walking", true);

            // Face direction
            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, 5f * Time.deltaTime);
            }
        }
    }


    Transform GetVisiblePlayer()
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
            float dstToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (dstToPlayer <= viewRadius && Vector3.Angle(transform.forward, dirToPlayer) < currentViewAngle / 2f)
            {
                // Set raycast origin and target with slight Y offset (eye/chest height)
                Vector3 origin = transform.position + Vector3.up * 1.5f; // eye level
                Vector3 target = player.transform.position + Vector3.up * 1.2f;

                if (!Physics.Linecast(origin, target, obstacleMask))
                {
                    if (dstToPlayer < minDistance)
                    {
                        minDistance = dstToPlayer;
                        closest = player.transform;
                    }
                }
            }
        }

        return closest;
    }


    void LookAtPlayer(Transform target)
    {
        Vector3 lookDir = target.position - transform.position;
        lookDir.y = 0;

        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 5f * Time.deltaTime);
        }
    }



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 viewAngleA = DirFromAngle(-currentViewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(currentViewAngle / 2, false);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    }

    Vector3 DirFromAngle(float angleInDegrees, bool global)
    {
        if (!global)
            angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void TakeDamage(int amount)
    {
        currentHP -= 1;
        getAlerted();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {

        if (isDead) return;

        isDead = true;
        currentHP = 0;

        animator.SetBool("Walking", false);
        animator.SetBool("Spotted", false);
        animator.Play("Die");

        if (rb != null)
        {
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

    }

    private void getAlerted()
    {
        currentViewAngle = alertedViewAngle;
        viewRadius = 25f;
    }

    private void leaveAlert()
    {
        currentViewAngle = defaultViewAngle;
    }
}

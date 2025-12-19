using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float attackRange = 2f;
    public float dodgeRange = 10f;
    public float dodgeForce = 5f;
    public LayerMask playerLayer = 1;

    [Header("Combat")]
    public float damage = 20f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    private NavMeshAgent agent;
    private Transform player;
    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        player = FindFirstObjectByType<Camera>().transform;

        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            agent.SetDestination(transform.position);
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            agent.SetDestination(player.position);
        }
        DodgeProjectiles();
    }

    private void Attack()
    {
        Debug.Log("Enemy attacks player!");
        PlayerHealth playerHealth = player.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        lastAttackTime = Time.time;
    }

    private void DodgeProjectiles()
    {
        Vector3 dirToEnemy = (transform.position - player.position).normalized;
        Ray ray = new Ray(player.position, dirToEnemy);

        if (Physics.Raycast(ray, out RaycastHit hit, dodgeRange, ~LayerMask.GetMask("Enemy")))
        {
            if (hit.collider.CompareTag("Projectile"))
            {
                Vector3 dodgeDir = Vector3.Cross(dirToEnemy, Vector3.up).normalized;
                dodgeDir = Random.Range(-1f, 1f) > 0 ? dodgeDir : -dodgeDir;

                rb.isKinematic = false;
                rb.AddForce(dodgeDir * dodgeForce, ForceMode.Impulse);
                rb.isKinematic = true;

                Debug.Log("Enemy dodged projectile!");
            }
        }
    }
}
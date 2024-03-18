using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    [Header("Enemy Brain")]
    [Space]
    [Header("Neccesary")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject projectile;
    [HideInInspector] Transform player;
    [Space]
    [Header("Customizable")]
    [Space]
    [SerializeField, Range(0,100)] float health;
    [SerializeField, Range(0,100)] float sightRange;
    [SerializeField, Range(0,100)] float attackRange;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField, Range(0,50)] float walkPointRange;
    [SerializeField, Range(0,10)] float timeBetweenAttacks;
    [HideInInspector] Vector3 walkPoint;
    [HideInInspector] bool walkPointSet;
    [HideInInspector] bool alreadyAttacked;
    [HideInInspector] bool playerInSightRange;
    [HideInInspector] bool playerInAttackRange;
    void Awake()
    {
        player = GameObject.Find("Player 5").transform;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange) handlePatrolling();
        if (playerInSightRange && !playerInAttackRange) handlePlayerChase();
        if (playerInSightRange && playerInAttackRange) handlePlayerAttack();
    }

    void handlePatrolling()
    {
        if (!walkPointSet){
            searchWalkPoint();
        }
        
        if (walkPointSet){
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f){
            walkPointSet = false;
        }
    }
    void searchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer)){
            walkPointSet = true;
        }
    }

    void handlePlayerChase()
    {
        agent.SetDestination(player.position);
    }

    void handlePlayerAttack()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 150f, ForceMode.Impulse);
            alreadyAttacked = true;
            Invoke(nameof(resetAttack), timeBetweenAttacks);
        }
    }
    private void resetAttack()
    {
        alreadyAttacked = false;
    }

    public void takeDamage(float damage)
    {
        health -= damage;

        if (health <= 0){
            Invoke(nameof(destroyEnemy), 0.5f);
        } 
        
    }
    void destroyEnemy()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}

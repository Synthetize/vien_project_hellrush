using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DemonKing : MonoBehaviour, IDamageable
{   
    public float health = 100;
    public float attackSpeed = 2;
    Animator animator;
    public GameObject fireEffectPrefab;
    FPController playerController;
    NavMeshAgent navMeshAgent;
    Rigidbody rb;
    float turnSpeed = 10f;
    bool isInAttackRange = false;
    float _attackCooldown = 0f;

    Transform footCenter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameObject fireballSpawns;
    void Start()
    {
        animator = GetComponent<Animator>();
        footCenter = transform.Find("FootsCenter");
        playerController = FindFirstObjectByType<FPController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        navMeshAgent.updateRotation = false;
        fireballSpawns = GameObject.FindWithTag("FireballSpawns");
    }

    // Update is called once per frame
    void Update()
    {
        _attackCooldown -= Time.deltaTime;

        if (playerController != null)
        {
            navMeshAgent.SetDestination(playerController.transform.position);
            Vector3 dir = playerController.transform.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
            }

        }


        if (isInAttackRange)
        {
            if (_attackCooldown <= 0f)
            {
                navMeshAgent.isStopped = true;
                animator.SetTrigger("OnAttack");
                _attackCooldown = 1f / Mathf.Max(0.1f, attackSpeed);
            }
        }
        

    }

    public void TakeDamage(float damage)
    {
        //animator.SetTrigger("OnHit");
        Debug.Log("Demon King took " + damage + " damage.");
        health -= damage;
        if (health <= 0)
        {
            navMeshAgent.isStopped = true;
            animator.SetBool("IsDead", true);
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    public void OnAttackAnimationFinished()
    {
        if (fireEffectPrefab != null && footCenter != null)
        {

            // GameObject fireEffect = Instantiate(fireEffectPrefab, footCenter.position, Quaternion.identity);
            // Destroy(fireEffect, 3f);
            Debug.Log("Fireball game object: " + fireballSpawns);
            foreach (Transform obj in fireballSpawns.GetComponentsInChildren<Transform>())
            {
                if (obj != fireballSpawns.transform)
                {
                    Instantiate(fireEffectPrefab, obj.position, obj.rotation);
                }
            }

        }
    }

    public void EnableAgentMovement()
    {
        navMeshAgent.isStopped = false;
    }
    
    public void SetIsInAttackRange(bool value)
    {
        isInAttackRange = value;
    }

}

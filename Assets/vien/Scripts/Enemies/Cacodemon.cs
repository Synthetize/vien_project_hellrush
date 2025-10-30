using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class Cacodemon : MonoBehaviour, IDamageable
{
    public float fireRate = 2f;
    public float viewRange = 10f;
    public float halfFovDeg = 60f;
    public float rotationSpeed = 10f;
    private float turnSpeed = 10f;
    public GameObject fireballPrefab;
    private Player player;
    private Transform cameraTransform;
    private NavMeshAgent agent;
    private FPController playerController;
    private Animator animator;
    private bool isDead = false;

    private int health = 100;

    float _shoot_cooldown = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = FindFirstObjectByType<Player>();
        cameraTransform = player.transform.GetChild(0);
        playerController = FindFirstObjectByType<FPController>();
        //_shoot_cooldown = 1f / Mathf.Max(0.01f, fireRate);
        if (cameraTransform.tag != "Camera")
        {
            Debug.LogError("Player's first child is not the camera.");
            return;
        }
        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        agent.SetDestination(cameraTransform.position);
        agent.updateRotation = true;
        agent.SetDestination(playerController.transform.position);
        Vector3 dir = playerController.transform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
            }


        if (_shoot_cooldown > 0f)
		{
			_shoot_cooldown -= Time.deltaTime;
		}

        if (CanSeePlayer() && _shoot_cooldown <= 0f)
        {
            agent.speed = 1f;
            Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            animator.SetTrigger("OnAttack");
            _shoot_cooldown = 1f / Mathf.Max(0.01f, fireRate);
            agent.speed = 3f;

        }

    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= (int)amount;
        if (health <= 0)
        {
            animator.SetTrigger("OnDeath");
            agent.isStopped = true;
            isDead = true;
            Destroy(gameObject, 3f);
        }
        animator.SetTrigger("OnHit");

    }




    private bool CanSeePlayer()
    {
        Vector3 targetPoint = cameraTransform.position;
        Vector3 delta = targetPoint - transform.position;
        float sqrDist = delta.sqrMagnitude;
        if (sqrDist > viewRange * viewRange)
        {
            Debug.Log("Player is out of range");
            return false;
        }

        Vector3 dir = delta.normalized;
        float cosHalfFov = Mathf.Cos(halfFovDeg * Mathf.Deg2Rad);
        if (Vector3.Dot(transform.forward, dir) < cosHalfFov)
        {
            Debug.Log("Player is out of FOV");
            return false;
        }

        float dist = Mathf.Sqrt(sqrDist);
        if(Physics.Raycast(transform.position, dir, out RaycastHit hitInfo, dist))
		{
			if (hitInfo.collider.tag == "PlayerBody")
            {
                Debug.Log("Player spotted!");
                return true;
            }
		}       
        Debug.Log("Player is occluded");
        return false;



    }



    

}

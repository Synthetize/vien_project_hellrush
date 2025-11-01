using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class DemonKingFireball : MonoBehaviour
{

    private HealthController healthController;
    public float speed = 15f;
    public int damage = 10;
    public int fireballLifetime = 10;
    Animator animator;
    Rigidbody rb;
    Camera FPCamera;
    GameObject parentObject;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        healthController = FindFirstObjectByType<HealthController>();
        Debug.Log(healthController);
        Destroy(gameObject, fireballLifetime);
        parentObject = transform.parent.gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        // Move the fireball forward
        rb.linearVelocity = transform.forward * speed;
    }

	void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //animator.SetTrigger("OnExplosion");
            healthController.TakeDamage(damage);
            Debug.Log("Fireball hit " + other.name);
        }

        
	}
}

        
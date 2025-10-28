using Unity.VisualScripting;
using UnityEngine;

public class Fireball : MonoBehaviour
{

    private HealthController healthController;
    FPController fPController;
    public float speed = 5f;
    public int damage = 10;
    Animator animator;
    Rigidbody rb;
    void Start()
    {
        fPController = FindFirstObjectByType<FPController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        healthController = FindFirstObjectByType<HealthController>();
        Debug.Log(healthController);
        

    }

    // Update is called once per frame
    void Update()
    {
        if (fPController != null)
        {
            // Move the fireball towards the player
            rb.linearVelocity = (fPController.transform.position - transform.position).normalized * speed;

        }
    }

	void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //animator.SetTrigger("OnExplosion");
            Destroy(gameObject, 0.1f);
            healthController.TakeDamage(damage);
            Debug.Log("Fireball hit " + other.name);
        }
        
	}
}

        
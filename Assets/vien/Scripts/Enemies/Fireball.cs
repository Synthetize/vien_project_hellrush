using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Fireball : MonoBehaviour
{

    private HealthController healthController;
    FPController fPController;
    public float speed = 15f;
    public int damage = 10;
    public int fireballLifetime = 10;
    Animator animator;
    Rigidbody rb;
    Camera FPCamera;
    void Start()
    {
        fPController = FindFirstObjectByType<FPController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        FPCamera = fPController.GetComponentInChildren<Camera>();
        healthController = FindFirstObjectByType<HealthController>();
        Debug.Log(healthController);
        Destroy(gameObject, fireballLifetime);

    }

    // Update is called once per frame
    void Update()
    {
        if (fPController != null)
        {
            // Move the fireball towards the player
            rb.linearVelocity = (FPCamera.transform.position - transform.position).normalized * speed;

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

        
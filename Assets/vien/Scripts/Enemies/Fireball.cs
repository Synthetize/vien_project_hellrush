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
    public float negativeYOffset = 2;
    void Start()
    {
        fPController = FindFirstObjectByType<FPController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        FPCamera = fPController.GetComponentInChildren<Camera>();
        healthController = FindFirstObjectByType<HealthController>();
        //Debug.Log(healthController);
        Destroy(gameObject, fireballLifetime);

    }

    // Update is called once per frame
    void Update()
    {
        if (fPController != null)
        {
            // Move the fireball towards the player (don't modify the camera's transform directly)
            Transform cameraTransform = FPCamera.transform;
            Vector3 targetPosition = cameraTransform.position;
            targetPosition.y -= negativeYOffset;

            rb.linearVelocity = (targetPosition - transform.position).normalized * speed;
        }
    }

	void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //animator.SetTrigger("OnExplosion");
            healthController.TakeDamage(damage);
            Debug.Log("Fireball hit " + other.name + " for " + damage + " damage.");
            Destroy(gameObject);

        }
        
	}
}

        
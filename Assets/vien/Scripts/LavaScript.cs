using UnityEngine;

public class LavaScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var healthController = other.gameObject.GetComponent<HealthController>();
            if (healthController != null)
            {
                healthController.TakeDamage(100);
            }
        }
    }
    
}

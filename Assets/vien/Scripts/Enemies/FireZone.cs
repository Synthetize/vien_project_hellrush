using System;
using UnityEngine;

public class FireZoneBehaviour : MonoBehaviour
{   
    public int initialDamage = 10;
    public int damagePerTick = 1;
    public int tickPerSecond = 2; // ticks per second
    private float tickTimer;
    HealthController healthController;
    void Start()
    {
        tickTimer = 0f;
        healthController = FindFirstObjectByType<HealthController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            healthController.TakeDamage(initialDamage);
            tickTimer = 0f;
        }

    }
    void OnTriggerStay(Collider other)
    {
        tickTimer += Time.deltaTime;
        if (other.CompareTag("Player"))
        {
            if (healthController != null)
            {
                if (tickTimer >= tickPerSecond)
                {
                    healthController.TakeDamage(damagePerTick);
                    tickTimer = 0f;
                }
            }
        }
    }

}

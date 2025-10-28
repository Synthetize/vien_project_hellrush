using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private HudController hudController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        hudController = FindFirstObjectByType<HudController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        hudController.UpdateHealth(currentHealth);
    }
}

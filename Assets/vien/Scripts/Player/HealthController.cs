using System.Threading;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private HudController hudController;

    private Player player;
    private EndGame endGame;
    AudioListener audioListener;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = 1000;
        hudController = FindFirstObjectByType<HudController>();
        player = GetComponent<Player>();
        audioListener = FindFirstObjectByType<AudioListener>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {   
            player.enabled = false;
            hudController.ShowGameOver();
            endGame = FindFirstObjectByType<EndGame>();
            endGame.FadeToBlack("BadEnding");
            AudioListener.volume = 0f;

        }

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        hudController.UpdateHealth(currentHealth);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI healthText;
    public RawImage gameOverImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ShowGameOver()
    {
        gameOverImage.gameObject.SetActive(true);
    }
    
    public void UpdateHealth(int currentHealth)
    {
        healthText.text = "Health: " + currentHealth;
    }
    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        ammoText.text = currentAmmo + " / " + maxAmmo;
    }
}

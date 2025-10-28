using TMPro;
using UnityEngine;

public class HudController : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI healthText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

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

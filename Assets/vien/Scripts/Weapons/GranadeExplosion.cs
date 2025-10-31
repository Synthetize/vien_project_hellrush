using UnityEngine;

public class GranadeExplosion : MonoBehaviour
{
    public float explosionRadius = 5f;
    public int explosionDamage = 25;

    // Update is called once per frame
    void Update()
    {

    }

	void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(explosionDamage); // Apply explosion damage
        }
	}
}

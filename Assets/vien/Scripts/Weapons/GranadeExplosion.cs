using System;
using UnityEngine;

public class GranadeExplosion : MonoBehaviour
{
    public float explosionRadius = 5f;
    public int explosionDamage = 50;

    public AudioClip explosionSound;
    AudioSource audioSource;

    public float AudioVolume = 1.0f;

    // Update is called once per frame

    void Start()
    {
        // AudioSource.PlayClipAtPoint(explosionSound, transform.position, Mathf.Clamp01(AudioVolume));
    }
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

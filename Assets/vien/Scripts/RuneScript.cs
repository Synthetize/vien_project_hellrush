using UnityEngine;
using System.Collections;

public class RuneScript : MonoBehaviour
{
    public EnemySpawner enemySpawner;
    private PlayerInventory playerInventory;
    public float floatHalfRange = 0.5f;
    public float floatSpeed = 1f;
    float _initialLocalY;
    public AudioClip audioSource;
    public AudioSource audioSourceComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();
        _initialLocalY = transform.localPosition.y;
    }



    // Update is called once per frame
    void Update()
    {
        MoveUpAndDown();
    }

    void MoveUpAndDown()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHalfRange;
        float newY = _initialLocalY + offset;
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("RuneScript: Player has entered the rune area.");
            AudioSource.PlayClipAtPoint(audioSource, transform.position);
            enemySpawner.StartSpawning();
            playerInventory.AddItem();
            audioSourceComponent.Play();
            gameObject.SetActive(false);
        }
    }
}

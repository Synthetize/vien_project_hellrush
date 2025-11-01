using UnityEngine;

public class RuneScript : MonoBehaviour
{
    public EnemySpawner enemySpawner;
    private PlayerInventory playerInventory;
    public float floatHalfRange = 0.5f;
    public float floatSpeed = 1f;
    float _initialLocalY;

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
            enemySpawner.StartSpawning();
            playerInventory.AddItem();
            Destroy(gameObject);
		}
	}
}

using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{
    public WeaponName weaponName;
    public AudioClip pickUpSound;
    private PlayerLoadoutController playerLoadoutController;
    public GameObject nextPortalCollider;
    private DialogsManager dialogsManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float floatSpeed = 2f;
    public float floatHalfRange = 0.5f;
    private float _initialLocalY;

    void Start()
    {
        _initialLocalY = transform.localPosition.y;
        dialogsManager = FindFirstObjectByType<DialogsManager>();
    }
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
            playerLoadoutController = other.GetComponent<PlayerLoadoutController>();
            dialogsManager.EnqueueAction(dialogsManager.WeaponCollected);
            playerLoadoutController.AddWeaponToLoadout(weaponName);
            AudioSource.PlayClipAtPoint(pickUpSound, transform.position);
            if (nextPortalCollider != null)
            {
                nextPortalCollider.GetComponent<Collider>().enabled = true;
            }

            Destroy(gameObject);
        }
	}
}

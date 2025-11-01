using UnityEngine;

public class DemonicAltar : MonoBehaviour
{
    private DialogsManager dialogsManager;
    bool dialogTriggered = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogsManager = FindFirstObjectByType<DialogsManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

	void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.name);
		if (other.CompareTag("Player") && !dialogTriggered)
        {
            dialogTriggered = true;
            dialogsManager.AltarDialog();
        }
	}
}

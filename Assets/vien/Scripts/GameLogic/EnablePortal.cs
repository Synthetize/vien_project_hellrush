using UnityEngine;

public class EnablePortal : MonoBehaviour
{
    private HellGate_Controller hellGateController;
    bool portalEnabled = false;
    void Start()
    {
        hellGateController = GetComponentInParent<HellGate_Controller>();
    }

    // Update is called once per frame
    void Update()
    {

    }

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
        {
            if (!portalEnabled)
            {
                portalEnabled = true;
                hellGateController.ToggleHellGate();
            }
        }
	}
}

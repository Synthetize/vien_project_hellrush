using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class AltarInteraction : MonoBehaviour
{
    PlayerInventory playerInventory;
    DialogsManager dialogsManager;
    public GameObject interactDisplay;
    DemonicAltar_Controller demonicAltarController;
    bool isInside = false;
    public InputActionReference inputActions;
    bool hasInteracted = false;

    public HellGate_Controller[] spawnPortal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();
        dialogsManager = FindFirstObjectByType<DialogsManager>();
        demonicAltarController = FindFirstObjectByType<DemonicAltar_Controller>();
        interactDisplay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInside && inputActions.action.WasPerformedThisFrame())
        {
            Interaction();
        }

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.name);
        if (other.CompareTag("Player"))
        {
            if(hasInteracted) return;
            isInside = true;
            interactDisplay.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInside = false;
            interactDisplay.SetActive(false);
        }
    }   
    
    public void Interaction()
    {
        if (!isInside) return;

        if (hasInteracted) return;  

        if (playerInventory.GetItemCount() >= 2)
        {
            hasInteracted = true;
            dialogsManager.EnqueueAction(dialogsManager.AltarInteractionTrue);
            demonicAltarController.ToggleDemonicAltar();
            foreach (var portal in spawnPortal)
            {
                portal.ToggleHellGate();
            }   

        }
        else
        {
            dialogsManager.EnqueueAction(dialogsManager.AltarInteractionFalse);
        }
    }
}

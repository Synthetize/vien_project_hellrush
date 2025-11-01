using TMPro;
using UnityEngine;

public class GameLogicManager : MonoBehaviour
{

    private DialogsManager dialogsManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogsManager = FindFirstObjectByType<DialogsManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
}

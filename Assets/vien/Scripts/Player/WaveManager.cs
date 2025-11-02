using UnityEngine;

public class WaveManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    DialogsManager dialogsManager;
    public GameObject weaponToPickup;
    int currentWave = 0;
    private EndGame endGame;
    void Start()
    {
        dialogsManager = FindFirstObjectByType<DialogsManager>();
        endGame = FindFirstObjectByType<EndGame>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    

    public void AdvanceWave()
    {
        currentWave++;
        switch (currentWave)
        {
            case 1:
                dialogsManager.EnqueueAction(dialogsManager.FirstWaveCompleted);
                weaponToPickup.SetActive(true);
                break;
            case 2:
                dialogsManager.EnqueueAction(dialogsManager.SecondWaveCompleted);
                break;
            case 3:
                dialogsManager.EnqueueAction(dialogsManager.BossDefeated);
                endGame.FadeToBlack("GoodEnding");
                break;
            default:
                break;
        }
    }
}

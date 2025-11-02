using UnityEngine;

public class WaveManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    DialogsManager dialogsManager;
    int currentWave = 0;
    void Start()
    {
        dialogsManager = FindFirstObjectByType<DialogsManager>();
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
                break;
            case 2:
                dialogsManager.EnqueueAction(dialogsManager.SecondWaveCompleted);
                break;
            case 3:
                dialogsManager.EnqueueAction(dialogsManager.BossDefeated);
                break;
            default:
                break;
        }
    }
}

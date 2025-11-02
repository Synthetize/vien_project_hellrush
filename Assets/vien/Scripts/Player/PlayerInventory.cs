using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    int itemCount = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void AddItem()
    {
        itemCount++;
    }
    
    public int GetItemCount()
    {
        return itemCount;
    }
}

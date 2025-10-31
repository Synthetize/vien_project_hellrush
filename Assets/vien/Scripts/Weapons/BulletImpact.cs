using UnityEngine;

public class BulletImpact : MonoBehaviour
{
    public float timeToDestroy = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }

}

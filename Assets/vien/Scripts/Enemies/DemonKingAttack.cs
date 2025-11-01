using UnityEngine;
using UnityEngine.AI;

public class DemonKingAttack : MonoBehaviour
{
    DemonKing demonKing;
    void Start()
    {
        demonKing = GetComponentInParent<DemonKing>();
    }
    void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			demonKing.SetIsInAttackRange(true);
		}
	}
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            demonKing.SetIsInAttackRange(false);
        }
    }
}

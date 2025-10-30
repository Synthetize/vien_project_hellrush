using UnityEngine;
using UnityEngine.AI;

public class ExecutionerAttack : MonoBehaviour
{
    ExecutionerDemon executionerDemon;
    void Start()
    {
        executionerDemon = GetComponentInParent<ExecutionerDemon>();
    }
    void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("PlayerBody"))
		{
			executionerDemon.SetIsInAttackRange(true);
		}
	}
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerBody"))
        {
            executionerDemon.SetIsInAttackRange(false);
        }
    }
}

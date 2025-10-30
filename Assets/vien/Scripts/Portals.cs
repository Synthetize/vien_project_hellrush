// ...existing code...
using System.Collections;
using System.Reflection;
using UnityEngine;

public class Portals : MonoBehaviour
{
    public Transform exitPortal;
    [Tooltip("Minimum forward offset from the exit portal")]
    public float forwardOffset = 5f;

    void OnTriggerEnter(Collider other)
    {
        if (exitPortal == null) return;

        // Only teleport objects tagged "Player"
        if (!other.CompareTag("Player")) return;

        Transform targetTransform = other.transform;
        Vector3 targetPosition = exitPortal.position + exitPortal.forward * forwardOffset;
        Quaternion targetRotation = exitPortal.rotation;

        // Handle CharacterController to avoid physics teleport issues
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            targetTransform.position = targetPosition;
            targetTransform.rotation = targetRotation;
            cc.enabled = true;
            return;
        }

        // If there's a Rigidbody, set its position/rotation and clear velocities
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.position = targetPosition;
            rb.rotation = targetRotation;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        // Fallback: move transform directly
        targetTransform.position = targetPosition;
        targetTransform.rotation = targetRotation;
    }
}
// ...existing code...
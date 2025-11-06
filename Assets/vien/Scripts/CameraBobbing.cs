using UnityEngine;

public class CameraBobbing : MonoBehaviour
{
    [Header("Player settings")]
    public CharacterController controller; // Riferimento al CharacterController del player

    [Header("Bobbing settings")]
    public float bobSpeed = 0.18f;         // Velocità del bobbing
    public float bobAmount = 0.05f;        // Altezza massima del bobbing

    [Header("Tilt settings")]
    public float tiltAmount = 2f;          // Grado di inclinazione laterale massima (gradi)
    [Tooltip("Velocità laterale (in unità/s) che produce il tilt massimo")]
    public float lateralSpeedForFullTilt = 5f;
    [Tooltip("Tempo di smoothing: più alto -> transizione più lenta")]
    public float tiltSmoothing = 0.12f;

    private float defaultYPos;
    private float timer = 0f;

    // stato per smoothing tilt
    private float currentTilt = 0f;
    private float tiltVelocity = 0f;

    void Start()
    {
        if (controller == null)
            controller = GetComponentInParent<CharacterController>();

        defaultYPos = transform.localPosition.y;
    }

    void Update()
    {
        if (controller == null) return;

        // --- vertical bobbing (come prima) ---
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            timer += Time.deltaTime * bobSpeed * controller.velocity.magnitude;
            float newY = defaultYPos + Mathf.Sin(timer * Mathf.PI * 2f) * bobAmount;
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
        else
        {
            timer = 0f;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultYPos, transform.localPosition.z);
        }

        // --- compute target tilt based only on lateral (local X) velocity ---
        // Calcoliamo la velocità laterale rispetto al transform del player
        Vector3 localVel = transform.InverseTransformDirection(controller.velocity);
        float lateralSpeed = localVel.x; // positivo = right, negativo = left

        // Normalizziamo la velocità laterale rispetto a lateralSpeedForFullTilt
        float lateralFactor = Mathf.Clamp(lateralSpeed / lateralSpeedForFullTilt, -1f, 1f);

        // Target tilt in gradi: solo dalla componente laterale
        float targetTilt = lateralFactor * tiltAmount;

        // Smoothiamo il tilt (non istantaneo)
        currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt, ref tiltVelocity, tiltSmoothing);

        // Applichiamo il tilt sullo Z locale. Conserviamo il pitch X esistente (può essere gestito da altri script).
        Vector3 localEuler = transform.localEulerAngles;

        // Correzione per angoli >180 (Unity usa 0..360)
        float pitch = localEuler.x;
        if (pitch > 180f) pitch -= 360f;

        // Impostiamo il nuovo euler con il roll smussato
        transform.localEulerAngles = new Vector3(pitch, localEuler.y, currentTilt);
    }
}
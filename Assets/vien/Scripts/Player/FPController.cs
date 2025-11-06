using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Events;
using Unity.VisualScripting;
using Zenject;
using NUnit.Framework;

[RequireComponent(typeof(CharacterController))]
public class FPController : MonoBehaviour
{
    [Header("MovementParameters")]
    private float MaxSpeed => SprintInput ? SprintSpeed : WalkSpeed;
    public float acceleration = 10f;
    public float deceleration = 10f;

    public float WalkSpeed => 10f;
    public float SprintSpeed => 15f;

    public float JumpHeight = 2f;
    public int timedJumps = 0;
    [SerializeField] private bool CanDoubleJump = true;

  

    [Header("LookParameters")]
    public Vector2 lookSensitivity = new Vector2(1, 1);
    public float pitchLimit = 80f;

    [SerializeField] private float currentPitch = 0f;

    public float CurrentPitch {
        get => currentPitch;
        set => currentPitch = Mathf.Clamp(value, -pitchLimit, pitchLimit);
    }
    [Header("Inputs")]
    public Vector2 MoveInput;
    public Vector2 LookInput;
    public bool SprintInput;

    public bool enableInput = true;


    [Header("Camera Parameters")]
    float defaultFOV = 60f;
    float sprintFOV = 90f;
    float cameraFOVSmothing = 5f;

    [Header("Physics Parameters")]
    public float gravityScale = 2f;
    public Vector3 CurrentVelocity { get; private set; }
    public float CurrentSpeed { get; private set; }
    public float VerticalVelocity = 0f;

    public bool wasGrounded = false;
    public bool IsGrounded => _characterController.isGrounded;

    [Header("Audio FX")]
    public AudioClip landSound;
    public AudioClip walkSound;
    public AudioClip sprintSound;
    private AudioSource audioSource;

    [Header("Events")]
    public UnityEvent OnLand;






    public bool Sprinting => SprintInput && CurrentSpeed > 0.1f;
    bool walking = false;


    [Header("References")]
    private CinemachineCamera _fpCamera;
    private CharacterController _characterController;

    [Inject]
    public void Construct([Inject(Id = "FPCamera")] CinemachineCamera fpCamera, CharacterController characterController)
    {
        this._fpCamera = fpCamera;
        this._characterController = characterController;
    }
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayLandSound()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null || landSound == null) return;
        float currentVolume = audioSource.volume;
        audioSource.PlayOneShot(landSound, currentVolume / 2f);
    }
    
    public void SetInputStatus(bool enable)
    {
        enableInput = enable;
    }

    void Update()
    {
        if (!enableInput) return;
        MoveUpdate();
        LookUpdate();
        CameraFOVUpdate();

        if (!wasGrounded && IsGrounded)
        {
            timedJumps = 0;
            OnLand?.Invoke();
        }
        wasGrounded = IsGrounded;


        if (Sprinting && IsGrounded)
        {
            //Debug.Log("sprinting");
            audioSource.clip = sprintSound;
            audioSource.loop = true;
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else if (walking && IsGrounded)
        {
            //Debug.Log("walking");
            audioSource.clip = walkSound;
            audioSource.loop = true;
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            //Debug.Log("stopped");
            audioSource.Stop();
        }
    }
    

    
    private void MoveUpdate()
    {
        Vector3 motion = transform.right * MoveInput.x + transform.forward * MoveInput.y;
        motion.y = 0;
        motion.Normalize();

        if (MoveInput.magnitude > 0)
        {
            CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, motion * MaxSpeed, acceleration * Time.deltaTime);
            walking = true;
        }
        else
        {
            CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, Vector3.zero, deceleration * Time.deltaTime);
            walking = false;
        }

        //gravity to make the controller stick to the ground
        if (_characterController.isGrounded && VerticalVelocity <= 0)
        {
            //important when the player is climbing a slope
            VerticalVelocity = -3f;
        }
        else {
            VerticalVelocity += Physics.gravity.y * gravityScale * Time.deltaTime;
        }


        Vector3 fullVelocity = new Vector3 (CurrentVelocity.x, VerticalVelocity, CurrentVelocity.z);
        CollisionFlags flags = _characterController.Move(fullVelocity * Time.deltaTime);
        if( (flags & CollisionFlags.Above) != 0 && VerticalVelocity > 0)
        {
            VerticalVelocity = 0f;
        }
        CurrentSpeed = CurrentVelocity.magnitude;
    }

    private void LookUpdate()
    {
        Vector2 input = new Vector2(LookInput.x * lookSensitivity.x, LookInput.y * lookSensitivity.y);
        CurrentPitch -= input.y;
        //loking up and down
        _fpCamera.transform.localEulerAngles = new Vector3(CurrentPitch, 0, 0);
        //loking left and right
        transform.Rotate(Vector3.up * input.x);
    }

    void CameraFOVUpdate()
    {
        float targetFOV = defaultFOV;
        if (Sprinting)
        {
            float speedRatio = CurrentSpeed / SprintSpeed;
            targetFOV = Mathf.Lerp(defaultFOV, sprintFOV, speedRatio);
        }
        _fpCamera.Lens.FieldOfView = Mathf.Lerp(_fpCamera.Lens.FieldOfView, targetFOV, Time.deltaTime * cameraFOVSmothing);

    }

    public void TryJump()
    {
        if (!IsGrounded)
        {
            if (CanDoubleJump && timedJumps < 2 && VerticalVelocity > 0)
            {
                return;
            }
            if (!CanDoubleJump || timedJumps >= 2)
            {
                return;
            }
        }

        VerticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y * gravityScale);
        timedJumps += 1;
    }

    public void SetSensitivity(float newSensitivity)
    {
        Debug.Log("Setting sensitivity to " + newSensitivity);
        lookSensitivity = new Vector2(newSensitivity, newSensitivity);
    }

}

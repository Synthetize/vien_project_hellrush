using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SensitivitySlider : MonoBehaviour
{
    public RawImage pauseMenu;
    public TextMeshProUGUI sensitivityValueText;
    public TextMeshProUGUI audioValueText;
    public Slider sensitivitySlider;
    public Slider audioSlider;
    FPController playerController;
    Weapon weapon;
    public float defaultSensitivity = 0.1f;
    public float defaultAudioVolume = 1f;
    AudioListener audioListener;

    public InputActionReference inputAction;
    bool isMenuActive = false;

    void Start()
    {
        playerController = FindFirstObjectByType<FPController>();
        audioListener = FindFirstObjectByType<AudioListener>();
        weapon = FindFirstObjectByType<Weapon>();

        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.AddListener(OnSensitivitySliderChanged);
            float initialSens = (playerController != null) ? playerController.lookSensitivity.x : defaultSensitivity;
            sensitivitySlider.value = initialSens;
            sensitivityValueText.text = sensitivitySlider.value.ToString("F2");
        }
        else
        {
            sensitivityValueText.text = defaultSensitivity.ToString("F2");
        }

        if (audioSlider != null)
        {
            audioSlider.onValueChanged.AddListener(OnAudioSliderChanged);
            audioSlider.value = AudioListener.volume;
            audioValueText.text = audioSlider.value.ToString("F2");
        }
        else
        {
            audioValueText.text = defaultAudioVolume.ToString("F2");
        }
        Debug.Log("Initial sensitivity: " + ((playerController != null) ? playerController.lookSensitivity.x.ToString() : "null"));
        Debug.Log("Initial audio volume: " + AudioListener.volume.ToString());
    }

    void OnEnable()
    {
        if (inputAction == null || inputAction.action == null) return;
        inputAction.action.Enable();
        inputAction.action.performed += OnToggleInput;
    }

    void OnDisable()
    {
        if (inputAction == null || inputAction.action == null) return;
        inputAction.action.performed -= OnToggleInput;
        inputAction.action.Disable();
    }

    // kept for other per-frame logic if needed
    void Update()
    {
    }

    private void OnToggleInput(InputAction.CallbackContext ctx)
    {
        // toggle menu on performed
        if (isMenuActive)
        {
            DisableMenu();
            isMenuActive = false;
        }
        else
        {
            EnableMenu();
            isMenuActive = true;
        }
    }

    private void OnSensitivitySliderChanged(float value)
    {
        sensitivityValueText.text = value.ToString("F2");
        if (playerController != null) playerController.SetSensitivity(value);
    }

    private void OnAudioSliderChanged(float value)
    {
        audioValueText.text = value.ToString("F2");
        AudioListener.volume = Mathf.Clamp01(value);
    }

    public float GetCurrentValue()
    {
        return (sensitivitySlider != null) ? sensitivitySlider.value : defaultSensitivity;
    }

    public void EnableMenu()
    {
        Debug.Log("Enabling sensitivity menu");
        pauseMenu?.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        weapon.SetInputStatus(false);
        playerController.SetInputStatus(false);
        Time.timeScale = 0f;
    }

    public void DisableMenu()
    {
        Debug.Log("Disabling sensitivity menu");
        pauseMenu?.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        weapon.SetInputStatus(true);
        playerController.SetInputStatus(true);
        Time.timeScale = 1f;
    }

  
}
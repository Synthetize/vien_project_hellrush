using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FPController))]
public class Player : MonoBehaviour
{
    public FPController controller;

    void Awake()
    {
        if (controller == null)
        {
            controller = GetComponent<FPController>();
        }
    }

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}
	void OnMove(InputValue value)
    {
        controller.MoveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        controller.LookInput = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        controller.SprintInput = value.isPressed;
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            controller.TryJump();
        }
    }
}

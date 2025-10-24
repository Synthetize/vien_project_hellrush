using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FPController))]
public class Player : MonoBehaviour
{
    private FPController controller;
    private Weapon weapon;
    private PlayerLoadoutController playerLoadoutController;

    void Awake()
    {
        if (controller == null)
        {

            controller = GetComponent<FPController>();
            weapon = GetComponent<Weapon>();
            playerLoadoutController = GetComponent<PlayerLoadoutController>();

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

    void OnAttack(InputValue v)
    {
        weapon.SetFiring(v.isPressed);
    }

    void OnReload(InputValue v)
    {
        weapon.StartReload();
    }

    void OnNextWeapon(InputValue v)
    {
        playerLoadoutController.NextWeapon();
    }

    void OnPreviousWeapon(InputValue v)
    {
        playerLoadoutController.PreviousWeapon();
    }
}

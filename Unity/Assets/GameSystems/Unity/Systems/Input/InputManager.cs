using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : IJumpInput
{
    private CustomInput input = new();

    public event Action JumpEvent;

    InputManager()
    {
        input.Enable();
        input.Player.Jump.performed += OnJump;
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        JumpEvent?.Invoke();
    }
}

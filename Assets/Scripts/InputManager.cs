using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Vector2 move { get; private set; }
    public Vector2 look { get; private set; }
    public bool jump { get; private set; }
    public bool sprint { get; private set; }
    public bool visor { get; private set; }

    public bool analogMovement { get; private set; }

    public bool cursorLocked { get; private set; } = true;
    public bool cursorInputForLook { get; private set; } = true;


    // EVENTS
    public delegate void OnVisorEvent();
    public OnVisorEvent onVisorPressed;



    // INPUT SYSTEM CALLBACKS
    private void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    private void OnLook(InputValue value)
    {
        if (cursorInputForLook) LookInput(value.Get<Vector2>());
    }

    private void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    private void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    private void OnEscape(InputValue value)
    {
        ToggleCursorState();
    }

    private void OnVisor(InputValue value)
    {
        VisorInput(value.isPressed);

        if (onVisorPressed != null) onVisorPressed();
    }




    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    } 

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }

    public void VisorInput(bool newVisorState)
    {
        visor = newVisorState;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
    
    private void ToggleCursorState()
    {
        cursorLocked = !cursorLocked;
        SetCursorState(cursorLocked);
    }
}
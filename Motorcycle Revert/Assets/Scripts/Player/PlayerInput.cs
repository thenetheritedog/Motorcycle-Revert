using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    PlayerManager playerManager;
    PlayerMovement movement;


    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        movement = GetComponent<PlayerMovement>();

    }
    public void OnMove(InputValue inputVal)
    {
        var input = inputVal.Get<Vector2>();
        movement.MoveChange(input);
    }

    public void OnJump(InputValue inputVal)
    {
        var input = inputVal.Get<float>() > 0.5f;
        movement.Jump(input);
    }

    public void OnLook(InputValue inputVal)
    {
        var input = inputVal.Get<Vector2>();
        playerManager.cameraManager.ChangeCameraOffset(input);
    }

    public void OnClick(InputValue inputVal)
    {
        var input = inputVal.Get<float>() > 0.5f;
        playerManager.BreakObject(input);
    }
}

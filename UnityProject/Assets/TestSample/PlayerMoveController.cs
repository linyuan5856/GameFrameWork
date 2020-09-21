using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour, PlayerInputController.IPlayerActions
{
    private PlayerInputController _controller;

    private Keyboard _keyboard;

    // Start is called before the first frame update
    void Start()
    {
        _keyboard = Keyboard.current;
        _controller = new PlayerInputController();
        PlayerInputController.PlayerActions ac = new PlayerInputController.PlayerActions(_controller);
        ac.SetCallbacks(this);

    }

    // Update is called once per frame
    void Update()
    {
        if( _keyboard.qKey.isPressed)
            Debug.Log("key Q is pressed");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
       var move= context.ReadValue<Vector2>();
       Debug.LogWarning($"move->{move}");
    }

    public void OnLook(InputAction.CallbackContext context)
    {
       
    }

    public void OnFire(InputAction.CallbackContext context)
    {
      
    }
}
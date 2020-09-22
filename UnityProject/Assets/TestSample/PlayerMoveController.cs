using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour, PlayerInputController.IPlayerActions
{
    private PlayerInputController _controller;
    private Keyboard _keyboard;

    private Vector2 _move;
    private float _speed = 5;

    // Start is called before the first frame update
    void Awake()
    {
        _keyboard = Keyboard.current;
        _controller = new PlayerInputController();
        PlayerInputController.PlayerActions ac = new PlayerInputController.PlayerActions(_controller);
        ac.SetCallbacks(this);
    }

    void OnEnable()
    {
        _controller.Enable();
    }

    void OnDisable()
    {
        _controller.Disable();
    }


    // Update is called once per frame
    void Update()
    {
        if (_keyboard.qKey.isPressed)
            Debug.Log("key Q is pressed");
        if (_move.magnitude > 0.1f)
        {
            var velocity = new Vector3(_move.x, _move.y);
            transform.Translate(velocity * Time.deltaTime * _speed);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
    }

    public void OnFire(InputAction.CallbackContext context)
    {
    }
}
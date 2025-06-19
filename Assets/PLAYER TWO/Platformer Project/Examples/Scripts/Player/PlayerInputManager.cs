using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public InputActionAsset actions;
    protected InputAction m_movement;

    protected void Awake()
    {
        CacheActions();
    }

    // Start is called before the first frame update
    void Start()
    {
        actions.Enable();
    }

    protected void Update()
    {
    }

    protected void OnEnable()
    {
        actions?.Enable();
    }

    protected void OnDisable()
    {
        actions?.Disable();
    }


    protected virtual void CacheActions()
    {
        m_movement = actions["Movement"];
        var value = m_movement.ReadValue<Vector2>();
    }
}
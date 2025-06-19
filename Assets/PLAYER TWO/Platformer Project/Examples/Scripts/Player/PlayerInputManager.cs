using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public InputActionAsset actions;
    public float m_movementDirctionUnlock;
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
    }

    public virtual Vector3 GetMovementDirection()
    {
        if (Time.time < m_movementDirctionUnlock)
        {
            return Vector3.zero;
        }

        var value = m_movement.ReadValue<Vector2>();

        return GetAxisWithCrossDeadZone(value);
    }

    public virtual Vector3 GetAxisWithCrossDeadZone(Vector2 axis)
    {
        var deadZone = InputSystem.settings.defaultDeadzoneMin;
        axis.x = Mathf.Abs(axis.x) > deadZone ? RemapToDeadzone(axis.x, deadZone) : 0;
        axis.y = Mathf.Abs(axis.y) > deadZone ? RemapToDeadzone(axis.y, deadZone) : 0;
        return new Vector3(axis.x, 0, axis.y);
    }

    // 重新矫正0-1
    private float RemapToDeadzone(float value, float deadzone) => (value - deadzone) / (1 - deadzone);
}
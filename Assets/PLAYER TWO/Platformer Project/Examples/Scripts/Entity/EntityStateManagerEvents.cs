using System;
using UnityEngine.Events;

[System.Serializable]
public class EntityStateManagerEvents
{
    public UnityEvent onChange;
    public UnityEvent<Type> onEnter;
    public UnityEvent<Type> onExit;
}
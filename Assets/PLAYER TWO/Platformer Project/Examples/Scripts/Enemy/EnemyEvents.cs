using System;
using UnityEngine.Events;

[Serializable]
public class EnemyEvents
{
    public UnityEvent OnPlayerSpotted;
    public UnityEvent OnPlayerScaped;
    public UnityEvent OnPlayerContact;
    public UnityEvent OnDamaged;
    public UnityEvent OnDie;
    public UnityEvent OnRespawn;
}
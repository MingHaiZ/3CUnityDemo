using System;
using UnityEngine.Events;

[Serializable]
public class EntityEvents
{
    public UnityEvent OnGroundEnter;
    public UnityEvent OnGroundExit;
    public UnityEvent OnRailsEnter;
    public UnityEvent OnRailsExit;
    public UnityEvent OnHurt;
    public UnityEvent OnJump;
    public UnityEvent OnDie;
    public UnityEvent OnSpin;
    public UnityEvent OnPickUp;
    public UnityEvent OnThrow;
    public UnityEvent OnStompStarted;
    public UnityEvent OnStompFalling;
    public UnityEvent OnStompLanding;
    public UnityEvent OnStompEnding;
    public UnityEvent OnLedgeGrabbed;
    public UnityEvent OnLedgeClimbing;
    public UnityEvent OnAirDive;
    public UnityEvent OnBackflip;
    public UnityEvent OnGlidingStart;
    public UnityEvent OnGlidingStop;
    public UnityEvent OnDashStarted;
    public UnityEvent OnDashEnded;
    
}
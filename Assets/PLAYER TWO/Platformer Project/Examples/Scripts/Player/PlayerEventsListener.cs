using System;
using UnityEngine;

public class PlayerEventsListener : MonoBehaviour
{
    public Player player;
    public PlayerEvents events;


    private void Start()
    {
        InitializePlayer();
        InitializeEventListeners();
    }

    private void InitializeEventListeners()
    {
        player.playerEvents.OnJump.AddListener(() => events.OnJump.Invoke());
        player.playerEvents.OnHurt.AddListener(() => events.OnHurt.Invoke());
        player.playerEvents.OnDie.AddListener(() => events.OnDie.Invoke());
        player.playerEvents.OnSpin.AddListener(() => events.OnSpin.Invoke());
        player.playerEvents.OnPickUp.AddListener(() => events.OnPickUp.Invoke());
        player.playerEvents.OnThrow.AddListener(() => events.OnThrow.Invoke());
        player.playerEvents.OnStompStarted.AddListener(() => events.OnStompStarted.Invoke());
        player.playerEvents.OnStompFalling.AddListener(() => events.OnStompFalling.Invoke());
        player.playerEvents.OnStompLanding.AddListener(() => events.OnStompLanding.Invoke());
        player.playerEvents.OnStompEnding.AddListener(() => events.OnStompEnding.Invoke());
        player.playerEvents.OnLedgeGrabbed.AddListener(() => events.OnLedgeGrabbed.Invoke());
        player.playerEvents.OnLedgeClimbing.AddListener(() => events.OnLedgeClimbing.Invoke());
        player.playerEvents.OnAirDive.AddListener(() => events.OnAirDive.Invoke());
        player.playerEvents.OnBackflip.AddListener(() => events.OnBackflip.Invoke());
        player.playerEvents.OnGlidingStart.AddListener(() => events.OnGlidingStart.Invoke());
        player.playerEvents.OnGlidingStop.AddListener(() => events.OnGlidingStop.Invoke());
        player.playerEvents.OnDashStarted.AddListener(() => events.OnDashStarted.Invoke());
        player.playerEvents.OnDashEnded.AddListener(() => events.OnDashEnded.Invoke());
    }

    private void InitializePlayer()
    {
        if (!player)
        {
            player = GetComponentInParent<Player>();
        }
    }
}
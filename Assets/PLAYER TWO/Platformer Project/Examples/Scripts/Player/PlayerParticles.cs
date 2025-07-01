using System;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    public float walkDustMinSpeed = 3.5f;
    public float landingParticleMinSpeed = 5f;
    public ParticleSystem walkDust;
    public ParticleSystem hurtDust;
    public ParticleSystem landDust;
    public ParticleSystem starsDust;
    protected Player m_player;

    public virtual void Play(ParticleSystem particle)
    {
        if (!particle.isPlaying)
        {
            particle.Play();
        }
    }

    protected virtual void HandleLandParticle()
    {
        if (!m_player.onWater && Mathf.Abs(m_player.velocity.y) >= landingParticleMinSpeed)
        {
            Play(landDust);
        }
    }

    protected virtual void HandleHurtParticle()
    {
        Play(hurtDust);
    }

    protected virtual void HandleWalkParticle()
    {
        if (m_player.isGrounded && !m_player.onWater)
        {
            if (m_player.lateralVelocity.magnitude > walkDustMinSpeed)
            {
                Play(walkDust);
            } else
            {
                Stop(walkDust);
            }
        } else
        {
            Stop(walkDust);
        }
    }

    public virtual void Stop(ParticleSystem particle, bool clear = false)
    {
        if (particle.isPlaying)
        {
            var mode = clear
                ? ParticleSystemStopBehavior.StopEmittingAndClear
                : ParticleSystemStopBehavior.StopEmitting;
            particle.Stop(true, mode);
        }
    }

    protected virtual void Start()
    {
        m_player = GetComponent<Player>();
        m_player.entityEvents.OnGroundEnter.AddListener(HandleLandParticle);
        m_player.playerEvents.OnHurt.AddListener(HandleHurtParticle);
    }

    protected virtual void Update()
    {
        HandleWalkParticle();
    }
}
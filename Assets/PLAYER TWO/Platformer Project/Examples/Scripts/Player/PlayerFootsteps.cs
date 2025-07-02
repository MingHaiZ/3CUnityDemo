using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Player))]
public class PlayerFootsteps : MonoBehaviour
{
    [Serializable]
    public class Surface
    {
        public string tag;
        public AudioClip[] footsteps;
        public AudioClip[] landings;
    }

    public Surface[] surfaces;

    protected Player m_player;
    protected Dictionary<string, AudioClip[]> m_landing = new Dictionary<string, AudioClip[]>();
    protected Dictionary<string, AudioClip[]> m_footsteps = new Dictionary<string, AudioClip[]>();
    protected AudioSource m_audio;
    protected Vector3 m_lastLateralPosition;

    public AudioClip[] defaultLandings;
    public AudioClip[] defaultFootsteps;

    [Header("General Settings")]
    public float footStepVolume = 0.5f;

    public float stepOffset = 1.25f;


    protected virtual void PlayRandomClip(AudioClip[] clips)
    {
        if (clips.Length > 0)
        {
            var index = Random.Range(0, clips.Length);
            m_audio.PlayOneShot(clips[index], footStepVolume);
        }
    }

    protected virtual void Landing()
    {
        if (!m_player.onWater)
        {
            if (m_landing.ContainsKey(m_player.groundHit.collider.tag))
            {
                PlayRandomClip(m_landing[m_player.groundHit.collider.tag]);
            } else
            {
                PlayRandomClip(defaultLandings);
            }
        }
    }

    protected virtual void Start()
    {
        m_player = GetComponent<Player>();
        m_player.entityEvents.OnGroundEnter.AddListener(Landing);

        if (!TryGetComponent(out m_audio))
        {
            m_audio = gameObject.AddComponent<AudioSource>();
        }

        foreach (var surface in surfaces)
        {
            m_footsteps.Add(surface.tag, surface.footsteps);
            m_landing.Add(surface.tag, surface.landings);
        }
    }

    private void Update()
    {
        if (m_player.isGrounded && m_player.states.IsCurrentOfType(typeof(WalkPlayerState)))
        {
            var position = transform.position;
            var lateralPosition = new Vector3(position.x, 0, position.z);
            var distance = (m_lastLateralPosition - lateralPosition).magnitude;
            // 如果一步能迈出1.25单位的距离就播放脚步
            if (distance >= stepOffset)
            {
                if (m_footsteps.ContainsKey(m_player.groundHit.collider.tag))
                {
                    PlayRandomClip(m_footsteps[m_player.groundHit.collider.tag]);
                } else
                {
                    PlayRandomClip(defaultFootsteps);
                }

                m_lastLateralPosition = lateralPosition;
            }
        }
    }
}
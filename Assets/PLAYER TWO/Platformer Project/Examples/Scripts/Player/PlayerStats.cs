using UnityEngine;

public class PlayerStats : EntityStats<PlayerStats>
{
    [Header("Motion stats")]
    public float brakeThreshold = -0.8f;

    public float turningDrag = 28f;
    public float acceleration = 13f;
    public float topSpeed = 6f;
    public float airAcceleration = 32f;

    [Header("Running stats")]
    public float runningAcceleration = 16f;

    public float runningTopSpeed = 7.5f;
    public float runningTurnningDrag = 14f;
}
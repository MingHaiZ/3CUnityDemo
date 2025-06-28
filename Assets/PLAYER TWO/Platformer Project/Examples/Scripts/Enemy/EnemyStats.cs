using UnityEngine;
using UnityEngine.Serialization;

public class EnemyStats : EntityStats<EnemyStats>
{
    [Header("General Settings")]
    public float gravity = 35f;

    public float snapForce = 0.15f;
    public float rotationSpeed = 970f;
    public float deceleration = 28f;
    public float firction = 16f;
    public float turnningDrag = 28f;

    [Header("View Settings")]
    public float spotRange = 5f;

    public float viewRange = 5f;

    [Header("Follow Stats")]
    public float followAcceleration = 10f;

    public float followTopSpeed = 4f;

    [Header("WayPoint Stats")]
    public float waypointMinDistance = 0.5f;

    public bool faceWaypoint = true;
    public float waypointAcceleration = 10f;
    public float waypointTopSpeed = 2f;

    [Header("Contact Attack Settings")]
    public bool canAttackOnContact;

    public bool contactPushback;
    public float contactOffset = 0.15f;
    public int contactDamage = 1;
    public int contactPushBackForce = 1;
    public int contactSteppingToLerance = 1;
}
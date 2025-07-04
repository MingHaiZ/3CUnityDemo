using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Pole : MonoBehaviour, IEntityContact
{
    public Collider collider { get; protected set; }
    public Vector3 center => transform.position;

    private void Awake()
    {
        tag = GameTag.Pole;
        collider = GetComponent<Collider>();
    }

    public Vector3 GetDirectionToRole(Transform other) => GetDirectionToRole(other, out _);

    public Vector3 GetDirectionToRole(Transform other, out float distance)
    {
        var target = new Vector3(center.x, other.position.y, center.z) - other.position;
        distance = target.magnitude;
        return target / distance;
    }

    public void OnEntityContact(Entity entity)
    {
    }

    public Vector3 ClampPointToPoleHeight(Vector3 point, float offset)
    {
        var minHeight = collider.bounds.min.y + offset;
        var maxHeight = collider.bounds.max.y - offset;
        var clampedHeight = Mathf.Clamp(point.y, minHeight, maxHeight);
        return new Vector3(point.x, clampedHeight, point.z);
    }
}
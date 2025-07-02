using UnityEngine;
using UnityEngine.Splines;

public class RailGrindPlayerState : PlayerState
{
    protected bool m_backwards;
    protected float m_speed;
    protected float m_lastDahTime;

    protected override void OnEnter(Player player)
    {
        Evaluate(player, out var point, out var forward, out var upward, out _);
        UpdatePosition(player, point, upward);

        m_backwards = Vector3.Dot(player.transform.forward, forward) < 0;
        m_speed = Mathf.Max(player.lateralVelocity.magnitude, player.stats.current.minGrindInitialSpeed);
        player.velocity = Vector3.zero;
        player.UseCustomCollision(player.stats.current.useCustomCollision);
    }

    protected override void OnExit(Player player)
    {
        player.ExitRail();
        player.UseCustomCollision(false);
    }

    protected override void OnStep(Player player)
    {
    }

    public override void OnContact(Player entity, Collider other)
    {
    }

    protected virtual void Evaluate(Player player, out Vector3 point, out Vector3 forward, out Vector3 upward,
        out float t)
    {
        var origin = player.rails.transform.InverseTransformPoint(player.transform.position);
        SplineUtility.GetNearestPoint(player.rails.Spline, origin, out var nearest, out t);
        point = player.rails.transform.TransformPoint(nearest);
        forward = Vector3.Normalize(player.rails.EvaluateTangent(t));
        upward = Vector3.Normalize(player.rails.EvaluateTangent(t));
    }

    protected virtual void UpdatePosition(Player player, Vector3 point, Vector3 upward)
    {
        player.transform.position = point + upward * GetDistanceToRail(player);
    }

    protected virtual float GetDistanceToRail(Player player)
    {
        return player.originalHeight * 0.5f + player.stats.current.grindRadiusOffset;
    }
}
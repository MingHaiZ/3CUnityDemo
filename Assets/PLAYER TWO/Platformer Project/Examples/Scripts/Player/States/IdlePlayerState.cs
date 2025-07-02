using UnityEngine;

public class IdlePlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
    }

    protected override void OnExit(Player player)
    {
    }

    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.SnapToGround();
        player.Jump();
        player.Fall();
        player.PickAndThrow();
        player.Friction();
        player.Spin();
        // 输出方向
        var inputDirection = player.inputs.GetMovementDirection();

        if (inputDirection.sqrMagnitude > 0 || player.lateralVelocity.sqrMagnitude > 0)
        {
            player.states.Change<WalkPlayerState>();
        }
    }

    public override void OnContact(Player entity, Collider other)
    {
    }
}
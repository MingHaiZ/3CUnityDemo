using UnityEngine;

public class FallPlayerState : PlayerState
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
        player.FaceDirectionSmooth(player.lateralVelocity);
        player.AccelerateToInputDirection();
        player.Jump();
        player.Spin();

        if (player.isGrounded)
        {
            player.states.Change<IdlePlayerState>();
        }
    }

    public override void OnContact(Player player, Collider other)
    {
        player.PushRigidbody(other);
    }
}
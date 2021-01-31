using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSecondaryAttackState : PlayerAbilityState

{
    protected Transform attackPosition;
    private AttackDetails attackDetails;
    private bool canAttack = true;
    private int xInput;
    private bool hasChangedDirection;
    private GameObject arrow;
    protected Arrow arrowScript;

    public PlayerSecondaryAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {

    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }


    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        secondaryAttackInput = player.InputHandler.SecondaryAttackInput;
        xInput = player.InputHandler.NormInputX;
        hasChangedDirection = player.CheckIfHasChangedDirection(xInput);

        if (secondaryAttackInput && canAttack && playerData.arrowCount > 0)
        {
            canAttack = false;
            Shoot();
            player.RemoveArrows(1);
        }

        if (!secondaryAttackInput || hasChangedDirection)
        {
            canAttack = true;
            stateMachine.ChangeState(player.IdleState);
        }
    }


    private void Shoot()
    {
        attackDetails.behindBackAttackMultiplier = playerData.behindBackAttackMultiplier;
        attackDetails.sneekAttackMultiplier = playerData.sneekAttackMultiplier;
        attackDetails.arrow = playerData.arrow;
        attackDetails.arrowSpeed = playerData.arrowSpeed;
        attackDetails.arrowTravelDistance = playerData.arrowTravelDistance;

        // ToDo: Should come from weapon
        attackDetails.damageAmount = 5;
        attackDetails.stunDamageAmount = 2;
        // ----------------------------

        arrow = GameObject.Instantiate(attackDetails.arrow, player.firePoint.position, player.firePoint.rotation);
        arrowScript = arrow.GetComponent<Arrow>();
        arrowScript.FireProjectile(attackDetails.arrowSpeed, attackDetails.arrowTravelDistance, attackDetails);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

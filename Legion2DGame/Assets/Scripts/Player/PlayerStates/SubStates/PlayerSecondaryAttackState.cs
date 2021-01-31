using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSecondaryAttackState : PlayerAbilityState

{
    protected Transform attackPosition;
    private AttackDetails attackDetails;
    private bool secondaryAttackInput;
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


        if (secondaryAttackInput && canAttack)
        {
            canAttack = false;
            Shoot();
        }

        if (!secondaryAttackInput || hasChangedDirection)
        {
            canAttack = true;
            stateMachine.ChangeState(player.IdleState);
        }
    }


    private void Shoot()
    {
        attackDetails.arrow = playerData.arrow;
        attackDetails.arrowSpeed = playerData.arrowSpeed;
        attackDetails.arrowTravelDistance = playerData.arrowTravelDistance;
        attackDetails.arrowDamage = playerData.arrowDamage;

        arrow = GameObject.Instantiate(attackDetails.arrow, player.firePoint.position, player.firePoint.rotation);
        arrowScript = arrow.GetComponent<Arrow>();
        arrowScript.FireProjectile(attackDetails.arrowSpeed, attackDetails.arrowTravelDistance, attackDetails.arrowDamage);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

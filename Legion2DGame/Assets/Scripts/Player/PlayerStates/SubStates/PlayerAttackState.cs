using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState

{
    protected Transform attackPosition;
    private AttackDetails attackDetails;
    private bool primaryAttackInput;
    private bool canAttack = true;
    private int xInput;
    private bool hasChangedDirection;

    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName, Transform attackPosition) : base(player, stateMachine, playerData, animBoolName)
    {
        this.attackPosition = attackPosition;
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

        primaryAttackInput = player.InputHandler.PrimaryAttackInput;
        xInput = player.InputHandler.NormInputX;
        hasChangedDirection = player.CheckIfHasChangedDirection(xInput);

        if (canAttack && primaryAttackInput)
        {
            canAttack = false;
            TriggerAttack();
        }

        if ((!primaryAttackInput && isAnimationFinished) || hasChangedDirection)
        {
            canAttack = true;
            stateMachine.ChangeState(player.IdleState);
        }

    }

    public virtual void TriggerAttack()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackPosition.position, playerData.attackRadius, playerData.whatIsEnemy);
        attackDetails.position = attackPosition.position;
        attackDetails.behindBackAttackMultiplier = playerData.behindBackAttackMultiplier;
        attackDetails.sneekAttackMultiplier = playerData.sneekAttackMultiplier;

        // ToDo: Should come from weapon
        attackDetails.damageAmount = 10; 
        attackDetails.stunDamageAmount = 1;
        // ------------------------------

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.SendMessage("Damage", attackDetails);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

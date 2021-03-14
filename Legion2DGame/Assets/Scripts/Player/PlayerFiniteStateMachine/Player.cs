using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region State Variables

    public PlayerStateMachine StateMachine { get; private set; }
    public bool alive { get; private set; }
    public float maxHealth { get; private set; }
    public float currentHealth { get; private set; }
    public float currentAbility { get; private set; }
    public int currentActiveAbilityOrbs { get; private set; }
    public int arrowCount { get; private set; }
    public float dashCooldown { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallGrabState WallGrabState { get; private set; }
    public PlayerWallClimbState WallClimbState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerLedgeClimbState LedgeClimbState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerSecondaryAttackState SecondaryAttackState { get; private set; }


    [Header("All player defining data")]
    [SerializeField]
    private PlayerData playerData;
    #endregion

    #region Components
    public Animator Anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public Transform DashDirectionIndicator { get; private set; }
    #endregion

    #region Check Transforms

    [Header("Player environment check objects")]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private Transform ledgeCheck;

    #endregion

    #region Particles
    [Header("Player particle system")]
    public ParticleSystem dust;
    #endregion

    #region Other Variables
    public Vector2 CurrentVelocity { get; private set; }
    [Header("Other")]
    public int facingDirection = 1;
    private bool knockback;
    private float knockbackStartTime;
    private float knockbackDuration = 0.2f;
    private Vector2 knockbackDirection;
    private float knockbackStrength;

    [Header("Attack transforms")]
    [SerializeField]
    private Transform meleeAttackPosition;
    public Transform firePoint;

    private Vector2 workspace;
    #endregion

    #region Unity Callback Functions
    private void Awake()
    {
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "inAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        LandState = new PlayerLandState(this, StateMachine, playerData, "land");
        WallSlideState = new PlayerWallSlideState(this, StateMachine, playerData, "wallSlide");
        WallGrabState = new PlayerWallGrabState(this, StateMachine, playerData, "wallGrab");
        WallClimbState = new PlayerWallClimbState(this, StateMachine, playerData, "wallClimb");
        WallJumpState = new PlayerWallJumpState(this, StateMachine, playerData, "inAir");
        LedgeClimbState = new PlayerLedgeClimbState(this, StateMachine, playerData, "ledgeClimbState");
        DashState = new PlayerDashState(this, StateMachine, playerData, "inAir");
        AttackState = new PlayerAttackState(this, StateMachine, playerData, "isAttacking", meleeAttackPosition);
        SecondaryAttackState = new PlayerSecondaryAttackState(this, StateMachine, playerData, "secondaryAttack");
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();
        DashDirectionIndicator = transform.Find("DashDirectionIndicator");
        StateMachine.Initialize(IdleState);

        alive = true;
        facingDirection = 1;

        maxHealth = playerData.totalHealth;
        currentHealth = maxHealth;
        currentAbility = playerData.abilityOrbValue * playerData.abiltyOrbs;
        currentActiveAbilityOrbs = playerData.activeAbilityOrbs;
        arrowCount = playerData.arrowCount;
        dashCooldown = playerData.dashCooldown;

        InvokeRepeating("FillAbilityOverTime", 2.0f, 1f);
    }

    private void Update()
    {
        CurrentVelocity = RB.velocity;
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
        CheckKnockback();
    }
    #endregion

    #region Set Functions

    public void SetVelocityZero()
    {
        RB.velocity = Vector2.zero;
        CurrentVelocity = Vector2.zero;
    }

    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workspace.Set(angle.x * velocity * direction, angle.y * velocity);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocity(float velocity, Vector2 direction)
    {
        workspace = direction * velocity;
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocityX(float velocity)
    {
        workspace.Set(velocity, CurrentVelocity.y);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocityY(float velocity)
    {
        workspace.Set(CurrentVelocity.x, velocity);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public void CheckKnockback()
    {
        if (knockback && Time.time <= knockbackStartTime + knockbackDuration)
        {
            SetVelocity(knockbackStrength, knockbackDirection);
        }
        else
        {
            knockback = false;
            knockbackStrength = 0;
        }
    }

    public void CreateDust()
    {
        dust.Play();
    }

    #endregion

    #region Check Functions

    public bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    }

    public bool CheckIfTouchingWall()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    }

    public bool CheckIfTouchingLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.right * facingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    }

    public bool CheckIfTouchingWallBack()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * -facingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    }

    public void CheckIfShouldFlip(int xInput)
    {
        if (xInput != 0 && xInput != facingDirection)
        {
            Flip();
        }
    }

    public bool CheckIfHasChangedDirection(int xInput)
    {
        if (xInput != 0 && xInput != facingDirection)
        {
            return true;
        }
        return false;
    }

    #endregion

    #region Other Functions

    public Vector2 DetermineCornerPosition()
    {
        RaycastHit2D xHit = Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
        float xDist = xHit.distance;
        workspace.Set(xDist * facingDirection, 0f);
        RaycastHit2D yHit = Physics2D.Raycast(ledgeCheck.position + (Vector3)(workspace), Vector2.down, ledgeCheck.position.y - wallCheck.position.y, playerData.whatIsGround);
        float yDist = yHit.distance;

        workspace.Set(wallCheck.position.x + (xDist * facingDirection), ledgeCheck.position.y - yDist);
        return workspace;
    }

    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    private void AnimtionFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    public void Heal(float ammount)
    {
        if (currentHealth + ammount >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += ammount;
        }
    }

    public void Damage(AttackDetails attackDetails)
    {
        currentHealth -= attackDetails.damageAmount;

        if (attackDetails.pushBack)
        {
            knockback = true;
            knockbackStartTime = Time.time;
            knockbackDirection = (Vector2)transform.position - attackDetails.position;
            knockbackStrength = attackDetails.pushBackForce;
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Death();
        }
    }


    private void FillAbilityOverTime()
    {
        FillAbilityPool(1, false);
    }

    public void FillAbility(float ammount)
    {
        // ToDo:
        // Made function this way because SendMessage function can only send 1 argument, and we need two.
        // Need to find better way of doing this.
        FillAbilityPool(ammount, true);
    }

    private void FillAbilityPool(float ammout, bool addOrb = true)
    {
        currentAbility += ammout;

        if (addOrb && currentAbility > playerData.abilityOrbValue * currentActiveAbilityOrbs)
        {
            AddAbilityOrb(1);
        }

        if (currentAbility > playerData.abilityOrbValue * currentActiveAbilityOrbs)
        {
            currentAbility = playerData.abilityOrbValue * currentActiveAbilityOrbs;
        }
    }

    public void ConsumeAbility(float ammount)
    {
        ConsumeAbilityPool(ammount, true);
    }

    private void ConsumeAbilityPool(float ammount, bool removeOrb = true)
    {
        currentAbility -= ammount;

        if (removeOrb && currentAbility < playerData.abilityOrbValue * (currentActiveAbilityOrbs - 1))
        {
            RemoveAbilityOrb(1);
        }

        if (currentAbility < 0)
        {
            currentAbility = 0;
        }
    }

    public void AddAbilityOrb(int ammount)
    {
        currentActiveAbilityOrbs += ammount;

        if (currentActiveAbilityOrbs > playerData.abiltyOrbs)
        {
            currentActiveAbilityOrbs = playerData.abiltyOrbs;
        }
    }

    public void RemoveAbilityOrb(int ammount)
    {
        currentActiveAbilityOrbs -= ammount;

        if (currentActiveAbilityOrbs < 0)
        {
            currentActiveAbilityOrbs = 0;
        }
    }

    public void AddArrows(int ammount)
    {
        arrowCount += ammount;
    }

    public void RemoveArrows(int ammount)
    {
        arrowCount -= ammount;

        if (arrowCount < 0)
        {
            arrowCount = 0;
        }
    }

    public void Resurrect()
    {
        alive = true;
        gameObject.SetActive(true);
    }

    public void Death()
    {
        alive = false;
        gameObject.SetActive(false);
    }

    #endregion

    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.right * facingDirection * playerData.wallCheckDistance));
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * facingDirection * playerData.wallCheckDistance));

        Gizmos.DrawWireSphere(groundCheck.position, playerData.groundCheckRadius);

        Gizmos.DrawWireSphere(meleeAttackPosition.position, playerData.attackRadius);
    }
}

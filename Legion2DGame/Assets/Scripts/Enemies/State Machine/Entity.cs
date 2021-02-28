using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public FiniteStateMachine stateMachine;

    [Header("Entity data")]
    public D_Entity entityData;
    public EnemyHealthBar healthBar;

    [Header("Drop Items")]
    public GameObject[] dropItems;

    public int facingDirection { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    public AnimationToStatemachine atsm { get; private set; }
    public int lastDamageDirection { get; private set; }

    [Header("Environment checks")]
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private Transform ledgeCheck;
    [SerializeField]
    private Transform playerCheck;
    [SerializeField]
    private Transform groundCheck;

    public float currentHealth { get; private set; }
    private float currentStunResistance;
    private float lastDamageTime;

    private Vector2 velocityWorkspace;

    public bool isAlerted { get; private set; }
    protected bool isStunned;
    protected bool isDead;

    private GameManager GM;

    public virtual void Start()
    {
        facingDirection = 1;
        currentHealth = entityData.maxHealth;
        currentStunResistance = entityData.stunResistance;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        atsm = GetComponent<AnimationToStatemachine>();

        stateMachine = new FiniteStateMachine();
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public virtual void Update()
    {
        stateMachine.currentState.LogicUpdate();

        anim.SetFloat("yVelocity", rb.velocity.y);

        if (Time.time >= lastDamageTime + entityData.stunRecoveryTime)
        {
            ResetStunResistance();
        }
    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    public virtual void SetVelocity(float velocity)
    {
        velocityWorkspace.Set(facingDirection * velocity, rb.velocity.y);
        rb.velocity = velocityWorkspace;
    }

    public virtual void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        velocityWorkspace.Set(angle.x * velocity * direction, angle.y * velocity);
        rb.velocity = velocityWorkspace;
    }

    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, transform.right, entityData.wallCheckDistance, entityData.whatIsGround);
    }

    public virtual bool CheckLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, entityData.ledgeCheckDistance, entityData.whatIsGround);
    }

    public virtual bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, entityData.groundCheckRadius, entityData.whatIsGround);
    }

    public virtual bool CheckPlayerInMinAgroRange()
    {
        return Physics2D.Raycast(playerCheck.position, transform.right, entityData.minAgroDistance, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInMaxAgroRange()
    {
        return Physics2D.Raycast(playerCheck.position, transform.right, entityData.maxAgroDistance, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return Physics2D.Raycast(playerCheck.position, transform.right, entityData.closeRangeActionDistance, entityData.whatIsPlayer);
    }

    public virtual void DamageHop(float velocity)
    {
        velocityWorkspace.Set(rb.velocity.x / 2, velocity);
        rb.velocity = velocityWorkspace;
    }

    public virtual void PushBack(Vector2 position, float velocity)
    {
        Vector2 direction = (rb.position - position).normalized;
        Vector2 pushBackForce = direction * velocity;
        velocityWorkspace.Set(pushBackForce.x, pushBackForce.y);
        rb.velocity = velocityWorkspace;
    }

    public virtual void ResetStunResistance()
    {
        isStunned = false;
        currentStunResistance = entityData.stunResistance;
    }

    public virtual void Damage(AttackDetails attackDetails)
    {
        lastDamageTime = Time.time;

        float damage = attackDetails.damageAmount;
        float stunDamage = attackDetails.stunDamageAmount;

        if (IsSneekAttack())
        {
            damage *= attackDetails.sneekAttackMultiplier;
            stunDamage *= attackDetails.sneekAttackMultiplier;
        }
        else if (IsAttackFromBehind(attackDetails.position))
        {
            damage *= attackDetails.behindBackAttackMultiplier;
            stunDamage *= attackDetails.behindBackAttackMultiplier;
        }

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            isDead = true;
            DropItem();
            GM.EnemyKilled();
            return;
        }

        currentStunResistance -= stunDamage;
        if (currentStunResistance <= 0)
        {
            isStunned = true;
        }

        Instantiate(entityData.hitParticle, transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
        UpdateHealthBar();
        PushBack(attackDetails.position, entityData.damageHopSpeed);
        if (CheckGround())
        {
            DamageHop(entityData.damageHopSpeed);
        }
    }

    private bool IsAttackFromBehind(Vector2 attackPosition)
    {
        if (attackPosition.x > transform.position.x)
        {
            lastDamageDirection = 1;
        }
        else
        {
            lastDamageDirection = -1;
        }

        return lastDamageDirection * facingDirection < 0;
    }
    private bool IsSneekAttack()
    {
        return !isAlerted;
    }

    public virtual void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }

    private void UpdateHealthBar()
    {
        if (healthBar)
        {
            healthBar.currentHealth = currentHealth;
            healthBar.maxHealth = entityData.maxHealth;
        }
    }

    public virtual void DropItem()
    {
        int randomDrop = Random.Range(0, dropItems.Length);
        Instantiate(dropItems[randomDrop], transform.position, Quaternion.identity);
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * facingDirection * entityData.wallCheckDistance));
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * entityData.ledgeCheckDistance));

        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * facingDirection * entityData.closeRangeActionDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * facingDirection * entityData.minAgroDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * facingDirection * entityData.maxAgroDistance), 0.2f);
    }

    public void EntedAlertedPhase()
    {
        isAlerted = true;
    }

    public void ExitAlertedPhase()
    {
        isAlerted = true;
    }
}

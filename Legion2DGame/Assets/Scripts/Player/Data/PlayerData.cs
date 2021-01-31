using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Health")]
    public float totalHealth = 100;

    [Header("Move State")]
    public float movementVelocity = 10f;

    [Header("Jump State")]
    public float jumpVelocity = 15f;
    public int amountOfJumps = 1;

    [Header("Wall Jump State")]
    public float wallJumpVelocity = 20;
    public float wallJumpTime = 0.4f;
    public Vector2 wallJumpAngle = new Vector2(1, 2);

    [Header("In Air State")]
    public float coyoteTime = 0.2f;
    public float variableJumpHeightMultiplier = 0.5f;

    [Header("Wall Slide State")]
    public float wallSlideVelocity = 3f;

    [Header("Wall Climb State")]
    public float wallClimbVelocity = 3f;

    [Header("Ledge Climb State")]
    public Vector2 startOffset;
    public Vector2 stopOffset;

    [Header("Dash State")]
    public float dashCooldown = 0.5f;
    public float maxHoldTime = 1f;
    public float holdTimeScale = 0.25f;
    public float dashTime = 0.2f;
    public float dashVelocity = 30f;
    public float drag = 10f;
    public float dashEndYMultiplier = 0.2f;
    public float distBetweenAfterImages = 0.5f;

    [Header("Check Variables")]
    public float groundCheckRadius = 0.2f;
    public float wallCheckDistance = 0.5f;
    public LayerMask whatIsGround;

    [Header("Melee Attack Variables")]
    public float attackRadius = 0.5f;
    public float damageAmount = 10;
    public float stunDamageAmount = 1;
    public float behindBackAttackMultiplier = 2f;
    public float sneekAttackMultiplier = 3f;

    [Header("Secondary Attack Variables")]
    public GameObject arrow;
    public float attackRange = 0.5f;
    public float damageAmountSecondaryAttack = 5;
    public float arrowDamage = 10;
    public float arrowSpeed = 30;
    public float arrowTravelDistance = 10;

    public LayerMask whatIsEnemy;
}

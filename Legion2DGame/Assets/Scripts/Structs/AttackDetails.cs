using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackDetails
{
    // General
    public Vector2 position;
    public float damageAmount;
    public float stunDamageAmount;
    public float behindBackAttackMultiplier;
    public float sneekAttackMultiplier;
    public bool pushBack;
    public float pushBackForce;
    // Arrow
    public GameObject arrow;
    public float arrowSpeed;
    public float arrowTravelDistance;
}

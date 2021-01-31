using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackDetails
{
    public Vector2 position;
    public float damageAmount;
    public float stunDamageAmount;
    public float behindBackAttackMultiplier;
    public float sneekAttackMultiplier;
    public bool pushBack;
    public float pushBackForce;
    public GameObject arrow;
    public float arrowSpeed;
    public float arrowTravelDistance;
    public float arrowDamage;
}

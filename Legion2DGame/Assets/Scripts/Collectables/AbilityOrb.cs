using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityOrb : Orb
{
    [Header("Orb Specific Stats")]
    public float value;

    public override void HandleCollision(Collision2D collision)
    {
        base.HandleCollision(collision);

        collision.transform.SendMessage("FillAbility", value);
    }
}

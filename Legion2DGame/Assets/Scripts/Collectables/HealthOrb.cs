﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : Orb
{
    [Header("Orb Specific Stats")]
    public float healValue;

    public override void HandleCollision(Collision2D collision)
    {
        base.HandleCollision(collision);

        collision.transform.SendMessage("Heal", healValue);
    }
}

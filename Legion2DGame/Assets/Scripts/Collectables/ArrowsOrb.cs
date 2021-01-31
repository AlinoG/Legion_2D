using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowsOrb : Orb
{
    [Header("Orb Specific Stats")]
    public int min;
    public int max;

    public override void HandleCollision(Collision2D collision)
    {
        base.HandleCollision(collision);

        int randomDrop = Random.Range(min, max);
        collision.transform.SendMessage("AddArrows", randomDrop);
    }

}

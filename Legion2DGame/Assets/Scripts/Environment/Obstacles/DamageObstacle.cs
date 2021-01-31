using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObstacle : MonoBehaviour
{
    protected AttackDetails attackDetails;

    [Header("Stats")]
    public float damage;
    public bool pushBack;
    public float pushBackForce = 10f;

    private void Awake()
    {
        attackDetails.damageAmount = damage;
        attackDetails.position = gameObject.transform.position;
        attackDetails.pushBack = pushBack;
        attackDetails.pushBackForce = pushBackForce;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.SendMessage("Damage", attackDetails);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb
    : MonoBehaviour
{
    [Header("Base Stats")]
    public float absorbSpeed = 7;

    private GameObject player;

    private void Update()
    {
        if (player)
        {
            FlyTowardsPlayer(player);
        }
    }

    public void FlyTowardsPlayer(GameObject player)
    {
        gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position,
            player.transform.position,
            absorbSpeed * Time.deltaTime);
    }

    public virtual void HandleCollision(Collision2D collision)
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            HandleCollision(collision);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            player = collider.gameObject;
        }
    }
}

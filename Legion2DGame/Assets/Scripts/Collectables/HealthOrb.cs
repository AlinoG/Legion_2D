using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    [Header("Stats")]
    public float healthValue;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.SendMessage("Heal", healthValue);
            Destroy(gameObject);
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

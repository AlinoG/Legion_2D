using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform otherSide;
    public bool used;

    private void Start()
    {
        // Set to false because every room has portal by default.
        // This portal should be activated only when necesary!
        gameObject.SetActive(false);
    }

    private void Teleport(GameObject player)
    {
        player.SetActive(false);
        player.transform.position = otherSide.position;
        otherSide.gameObject.GetComponent<Portal>().used = true;
        player.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!used && otherSide && collision.gameObject.tag == "Player")
        {
            Teleport(collision.gameObject);
            used = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            used = false;
        }
    }
}

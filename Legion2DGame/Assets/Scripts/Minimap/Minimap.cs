using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public GameObject player;

    private void LateUpdate()
    {
        Vector3 newPosition = player.transform.position;
        newPosition.z = transform.position.z;
        transform.position = newPosition;
    }
}

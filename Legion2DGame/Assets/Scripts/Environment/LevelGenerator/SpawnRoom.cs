using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoom : MonoBehaviour
{
    public LayerMask whatIsRoom;
    public LevelGenerator levelGenerator;

    // Update is called once per frame
    void Update()
    {
        Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, whatIsRoom);
        if (!roomDetection && levelGenerator.stopGeneration && levelGenerator.spawnSurroundingRooms)
        {
            GameObject room = Instantiate(levelGenerator.GetRandomRoomByType(levelGenerator.GetRandomRoomType()), transform.position, Quaternion.identity);
            room.transform.SetParent(levelGenerator.parent.transform);

            Destroy(gameObject);
        }
    }
}

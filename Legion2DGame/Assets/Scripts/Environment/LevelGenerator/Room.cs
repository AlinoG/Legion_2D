using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int width; // ToDo (Alino): Find better way of finding room size.
    public int height;
    public RoomType type;


    public void RoomDestruction()
    {
        Destroy(gameObject);
    }

    public void SpawnRoomWalls(GameObject horizontalRoomWall, GameObject verticalRoomWall, LayerMask whatIsRoom)
    {
        float checkRadius = 3;

        // Check Right
        Vector2 rightCheckPosition = new Vector2(transform.position.x + width, transform.position.y);
        Collider2D roomDetectionRight = Physics2D.OverlapCircle(rightCheckPosition, checkRadius, whatIsRoom);

        if (!roomDetectionRight)
        {
            SpawnRoomWall(verticalRoomWall, new Vector2(transform.position.x + width / 2, transform.position.y));
        }

        // Check Left
        Vector2 leftCheckPosition = new Vector2(transform.position.x - width, transform.position.y);
        Collider2D roomDetectionLeft = Physics2D.OverlapCircle(leftCheckPosition, checkRadius, whatIsRoom);

        if (!roomDetectionLeft)
        {
            SpawnRoomWall(verticalRoomWall, new Vector2(transform.position.x - width / 2, transform.position.y));
        }

        // Check Up
        Vector2 upCheckPosition = new Vector2(transform.position.x, transform.position.y + height);
        Collider2D roomDetectionUp = Physics2D.OverlapCircle(upCheckPosition, checkRadius, whatIsRoom);

        if (!roomDetectionUp)
        {
            SpawnRoomWall(horizontalRoomWall, new Vector2(transform.position.x, transform.position.y + height / 2));
        }

        // Check Down
        Vector2 downCheckPosition = new Vector2(transform.position.x, transform.position.y - height);
        Collider2D roomDetectionDown = Physics2D.OverlapCircle(downCheckPosition, checkRadius, whatIsRoom);

        if (!roomDetectionDown)
        {
            SpawnRoomWall(horizontalRoomWall, new Vector2(transform.position.x, transform.position.y - height / 2));
        }
    }

    private void SpawnRoomWall(GameObject wall, Vector2 position)
    {
        GameObject room = Instantiate(wall, position, Quaternion.identity);
        room.transform.SetParent(transform);
    }
}
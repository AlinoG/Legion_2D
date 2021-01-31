using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public bool restartGenerator = false;
    public bool stopGeneration = false;

    [Header("Where all rooms are created")]
    public GameObject parent;

    [Header("Master room settings")]
    public LayerMask whatIsRoom;
    public bool haveWalls = true;
    public bool spawnSurroundingRooms = true;
    public int masterRoomWidth = 4;
    public int masterRoomDepth = 4;
    public GameObject verticalRoomWall;
    public GameObject horizontalRoomWall;

    [Header("Entrance portal")]
    public GameObject lvlEntrance;

    [Header("Sub room settings")]
    public GameObject roomSpawnPoint;
    private Vector3[] startingPositions;
    private Vector3[] roomSpawnPostions;

    public GameObject[] leftRightRooms;
    public GameObject[] leftRightBootomRooms;
    public GameObject[] leftRightTopRooms;
    public GameObject[] leftRightBottomTopRooms;
    private GameObject lastRoom;
    private List<GameObject> allRooms = new List<GameObject>();

    private float roomHeight;
    private float roomWidth;
    private float minX;
    private float maxX;
    private float minY;

    private float moveAmountX;
    private float moveAmountY;
    private int direction;
    private int downCounter = 0;

    private GameManager GM;

    private void Start()
    {
        SetStartVariables();
        SetMasterRoomWalls();
        SpawnStartRoom();
    }

    private void Update()
    {
        if (restartGenerator)
        {
            restartGenerator = false;
            DeleteAllRooms();
            SetStartVariables();
            stopGeneration = false;
        }

        if (!spawnSurroundingRooms && GM.lvlGeneratorPortal && lvlEntrance.GetComponent<Portal>().otherSide == null)
        {
            ConnectPortals(lvlEntrance, GM.lvlGeneratorPortal);
        }

        if (!stopGeneration)
        {
            SpawnNextRoom();
        }
    }


    public GameObject GetRandomRoomByType(RoomType type)
    {
        int randomRoom = Random.Range(0, leftRightRooms.Length);

        if (type == RoomType.LeftRight)
        {
            return leftRightRooms[randomRoom];
        }
        else if (type == RoomType.LeftRightBottom)
        {
            return leftRightBootomRooms[randomRoom];
        }
        else if (type == RoomType.LeftRightTop)
        {
            return leftRightTopRooms[randomRoom];
        }
        else
        {
            return leftRightBottomTopRooms[randomRoom];
        }
    }

    public RoomType GetRandomRoomType()
    {
        int random = Random.Range(0, 4);
        return (RoomType)random;
    }

    private void SetStartVariables()
    {
        roomWidth = leftRightRooms[0].GetComponent<Room>().width;
        roomHeight = leftRightRooms[0].GetComponent<Room>().height;

        roomSpawnPostions = CreateRoomSpawnPositions();
        CreateRoomSpawnPoints(roomSpawnPostions);
        startingPositions = roomSpawnPostions.Take(masterRoomWidth).ToArray();

        minX = parent.transform.position.x + roomWidth / 2;
        maxX = parent.transform.position.x + roomWidth * masterRoomWidth - roomWidth / 2;
        minY = startingPositions[0].y - roomHeight * (masterRoomDepth - 1);
        moveAmountX = roomWidth;
        moveAmountY = roomHeight;

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void SetMasterRoomWalls()
    {
        if (!haveWalls || !spawnSurroundingRooms)
        {
            return;
        }

        for (int i = 0; i < masterRoomDepth; i++)
        {
            Vector2 spawnPosition = new Vector2(transform.position.x, transform.position.y - roomHeight / 2 - roomHeight * i);
            SpawnMasterRoomWall(verticalRoomWall, spawnPosition);

            spawnPosition = new Vector2(transform.position.x + roomWidth * masterRoomWidth, spawnPosition.y);
            SpawnMasterRoomWall(verticalRoomWall, spawnPosition);
        }

        for (int i = 0; i < masterRoomWidth; i++)
        {
            Vector2 spawnPosition = new Vector2(transform.position.x + roomWidth / 2 + roomWidth * i, transform.position.y - roomHeight * masterRoomDepth);
            SpawnMasterRoomWall(horizontalRoomWall, spawnPosition);
        }

    }

    private void SpawnMasterRoomWall(GameObject wall, Vector2 position)
    {
        GameObject room = Instantiate(wall, position, Quaternion.identity);
        room.transform.SetParent(parent.transform);
    }

    private void SpawnStartRoom()
    {
        // Select starting position from one of starting position.
        int randomStartPosition = Random.Range(0, startingPositions.Length);
        transform.position = startingPositions[randomStartPosition];
        // Spawn room with opening on all sides - our case, 4th room.
        SpawnRoom(GetRandomRoomByType(RoomType.LeftRightBottomTop), transform.position);

        direction = Random.Range(1, 6); // Last number in range is excluded!
    }

    private Vector3[] CreateRoomSpawnPositions()
    {
        Vector3[] positions = new Vector3[masterRoomWidth * masterRoomDepth];

        int currentIndex = 0;
        for (int i = 0; i < masterRoomDepth; i++)
        {
            if (i == 0)
            {
                Vector3 firstPos = new Vector3(parent.transform.position.x + roomWidth / 2, parent.transform.position.y - roomHeight / 2, parent.transform.position.z);
                positions[0] = firstPos;
            }

            for (int y = i == 0 ? 1 : 0; y < masterRoomWidth; y++)
            {
                if (i != 0 && y == 0)
                {
                    Vector3 newDepthFirstPose = new Vector3(positions[0].x, positions[0].y - roomHeight * i, positions[0].z);
                    currentIndex++;
                    positions[currentIndex] = newDepthFirstPose;
                }
                else
                {
                    Vector3 otherPos = new Vector3(positions[currentIndex].x + roomWidth, positions[currentIndex].y, positions[currentIndex].z);
                    currentIndex++;
                    positions[currentIndex] = otherPos;
                }
            }
        }

        return positions;
    }

    private void CreateRoomSpawnPoints(Vector3[] spawnPositions)
    {
        foreach (var spawnPosition in spawnPositions)
        {
            CreateRoomSpawnPoint(spawnPosition);
        }
    }

    private void CreateRoomSpawnPoint(Vector3 spawnPosition)
    {
        GameObject roomSpawn = Instantiate(roomSpawnPoint, spawnPosition, Quaternion.identity);
        roomSpawn.transform.SetParent(parent.transform);

        SpawnRoom roomSpawnScript = roomSpawn.GetComponent<SpawnRoom>();
        roomSpawnScript.whatIsRoom = LayerMask.GetMask("Room");
        roomSpawnScript.levelGenerator = gameObject.GetComponent<LevelGenerator>();
    }

    private void SpawnNextRoom()
    {
        if (direction == 1 || direction == 2) // SpawnNextRoom Right!
        {
            downCounter = 0;

            if (transform.position.x < maxX)
            {
                Vector2 newPosition = new Vector2(transform.position.x + moveAmountX, transform.position.y);
                transform.position = newPosition;

                SpawnRoom(GetRandomRoomByType(GetRandomRoomType()), transform.position);

                // Direction 3 and 4 (left direction) is removed because if we are moving (spawning rooms) to the right, 
                // we don't want to go left and spawn on top of already spawned room.
                direction = Random.Range(1, 6);
                if (direction == 3)
                {
                    direction = 2;
                }
                else if (direction == 4)
                {
                    direction = 5;
                }
            }
            else
            {
                // But we can go down.
                direction = 5;
            }
        }
        else if (direction == 3 || direction == 4) // SpawnNextRoom Left!
        {
            downCounter = 0;

            if (transform.position.x > minX)
            {
                Vector2 newPosition = new Vector2(transform.position.x - moveAmountX, transform.position.y);
                transform.position = newPosition;

                SpawnRoom(GetRandomRoomByType(GetRandomRoomType()), transform.position);

                // Direction 1 and 2 (right direction) is removed because if we are moving (spawning rooms) to the left, 
                // we don't want to go right and spawn on top of already spawned room.
                direction = Random.Range(3, 6);
            }
            else
            {
                // But we can go down.
                direction = 5;
            }
        }
        else
        { // SpawnNextRoom Down!

            downCounter++;

            if (transform.position.y > minY)
            {
                Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, whatIsRoom);
                if (roomDetection.GetComponent<Room>().type != RoomType.LeftRightBottom && roomDetection.GetComponent<Room>().type != RoomType.LeftRightBottomTop)
                {
                    if (downCounter < masterRoomDepth - 1)
                    {
                        roomDetection.GetComponent<Room>().RoomDestruction();
                        SpawnRoom(GetRandomRoomByType(RoomType.LeftRightBottomTop), transform.position);
                    }
                    else
                    {
                        roomDetection.GetComponent<Room>().RoomDestruction();

                        int randomBottomRoom = Random.Range(1, 4);
                        if (randomBottomRoom == 2)
                        {
                            // Because room type 2 = LeftRightTop has no bottom exit
                            randomBottomRoom = 1;
                        }
                        SpawnRoom(GetRandomRoomByType((RoomType)randomBottomRoom), transform.position);
                    }
                }

                Vector2 newPosition = new Vector2(transform.position.x, transform.position.y - moveAmountY);
                transform.position = newPosition;

                int random = Random.Range(2, 4);
                SpawnRoom(GetRandomRoomByType((RoomType)random), transform.position);

                direction = Random.Range(1, 6);
            }
            else
            {
                stopGeneration = true;
                if (haveWalls)
                {
                    SetSubRoomWalls();
                }

                ConnectPortals(GM.lvlGeneratorPortal, GM.playerSpawnPoint);
            }
        }
    }

    private void SpawnRoom(GameObject gameObject, Vector2 position)
    {
        GameObject room = Instantiate(gameObject, position, Quaternion.identity);
        room.transform.SetParent(parent.transform);

        allRooms.Add(room);

        // ToDo: Remove lastRoom
        lastRoom = room;
    }

    private void SetSubRoomWalls()
    {
        allRooms.RemoveAll(item => item == null);

        foreach (GameObject room in allRooms)
        {
            room.GetComponent<Room>().SpawnRoomWalls(horizontalRoomWall, verticalRoomWall, whatIsRoom);
        }
    }

    private void ConnectPortals(GameObject entrance, GameObject otherSide)
    {
        entrance.SetActive(true);
        entrance.GetComponent<Portal>().otherSide = otherSide.transform;

        otherSide.SetActive(true);
        otherSide.GetComponent<Portal>().otherSide = entrance.transform;
    }

    private void DeleteAllRooms()
    {
        foreach (Transform child in parent.transform)
        {
            if (child.tag != "LevelGenerator")
            {
                // ToDo: Deactivate and reactivate again
                Destroy(child.gameObject);
            }
        }
    }
}

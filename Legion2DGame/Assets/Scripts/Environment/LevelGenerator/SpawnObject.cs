using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour

{
    public GameObject[] objects;

    // Start is called before the first frame update
    void Start()
    {
        int random = Random.Range(0, objects.Length);
        GameObject instance = Instantiate(objects[random], transform.position, Quaternion.identity);

        if (instance.tag == "Portal")
        {
            OnPortalSpawn(instance);
        }
        instance.transform.SetParent(transform);
    }

    private void OnPortalSpawn(GameObject portal)
    {
        GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (GM.lvlGeneratorPortal == null)
        {
            GM.lvlGeneratorPortal = portal;
        }
    }
}

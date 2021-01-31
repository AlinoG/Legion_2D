using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    public float currentHealth = 1f;
    public float maxHealth = 1f;
    public GameObject fill;

    public void Start()
    {
        fill.transform.localScale = new Vector3(currentHealth / maxHealth, fill.transform.localScale.y, fill.transform.localScale.z);
    }

    private void Update()
    {
        float localScaleX = currentHealth / maxHealth;

        if (localScaleX > 0.6)
        {
            fill.GetComponent<SpriteRenderer>().color = new Color(0, 255f, 0);
        }
        else if (localScaleX < 0.6f && localScaleX > 0.3f)
        {
            fill.GetComponent<SpriteRenderer>().color = new Color(255f, 115f, 0);
        }
        else if (localScaleX < 0.3)
        {
            fill.GetComponent<SpriteRenderer>().color = new Color(255f, 0, 0);

        }

        fill.transform.localScale = new Vector3(localScaleX, fill.transform.localScale.y, fill.transform.localScale.z);
    }
}

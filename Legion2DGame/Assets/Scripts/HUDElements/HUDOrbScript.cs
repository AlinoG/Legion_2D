using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDOrbScript : MonoBehaviour
{
    public float maxValue;
    public float currentValue;
    public GameObject background;

    private float minScale;

    private void Start()
    {
        RectTransform rt = (RectTransform)transform;
        minScale = rt.rect.height * (-1);
    }

    void Update()
    {
        float percentage = currentValue / maxValue;

        background.transform.localPosition = new Vector2(0, minScale - (minScale * percentage));
        background.GetComponent<Image>().color = GetImageColor(percentage);
    }

    private Color32 GetImageColor(float percentage)
    {
        Color32 color = new Color32();

        if (percentage < 0.45f)
        {
            color = new Color32(255, 00, 0, 255);
        }
        else if (percentage >= 0.45f && percentage < 0.75f)
        {
            color = new Color32(255, 255, 0, 255);
        }
        else if (percentage >= 0.75f)
        {
            color = new Color32(0, 255, 0, 255);
        }

        return color;
    }
}

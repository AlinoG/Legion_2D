using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        background.transform.localPosition = new Vector2(0, minScale - (minScale * currentValue / maxValue));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDropUI : MonoBehaviour
{
    private Image[] Images;
    private float currentTime;

    void Start()
    {
        Images = GetComponentsInChildren<Image>();
        currentTime = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime >= 0.01f)
        {
            Color temp = Images[0].color;
            temp.a -= Time.deltaTime;
            Images[0].color = Images[1].color = temp;
            currentTime -= Time.deltaTime;
            this.GetComponent<RectTransform>().anchoredPosition += Vector2.up * 50f * Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public SpriteRenderer itemImage;
    private float currentTime;
    private float moveSpeed;

    void Start()
    {
        currentTime = 1f;
        moveSpeed = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += Vector3.up * Time.deltaTime * moveSpeed;

        if(currentTime >= 0.01f)
        {
            Color temp = itemImage.color;
            temp.a -= Time.deltaTime;
            itemImage.color = temp;
            currentTime -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}

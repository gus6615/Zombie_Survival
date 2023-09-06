using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private float currentTime;
    private float EffectTime;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0f;
        EffectTime = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime <= EffectTime)
        {
            currentTime += Time.deltaTime;
        }
        else
            Destroy(this.gameObject);
    }
}

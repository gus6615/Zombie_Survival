using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    static private MapScript data;

    // Use this for initialization
    void Start()
    {
        if (data == null)
        {
            DontDestroyOnLoad(this.gameObject);
            data = this;
        }
        else if (data != null)
        {
            Destroy(this.gameObject);
        }
    }
}

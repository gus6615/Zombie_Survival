using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObejct : MonoBehaviour {

    static private WeaponObejct instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }
    }
}

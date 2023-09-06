using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObejectCtrl : MonoBehaviour
{
    void Start()
    {
        GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();

        for (int i = 0; i < all.Length; i++)
        {
            all[i].SetActive(true);
        }
    }
}

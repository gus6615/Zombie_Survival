using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIObject : MonoBehaviour
{
    static private AIObject instance;
    public GameObject storyAIs;
    public GameObject AIs;
    public GameObject specialAIs;

    static public SpriteRenderer[] storyAIs_ShopImages, AIs_ShopImages, specialAIs_ShopImages;
    static public SpriteRenderer[] storyAIs_WeaponImages, AIs_WeaponImages, specialAIs_WeaponImages;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            storyAIs_ShopImages = storyAIs.GetComponentsInChildren<ItemOrder>()[0].GetComponentsInChildren<SpriteRenderer>();
            AIs_ShopImages = AIs.GetComponentsInChildren<ItemOrder>()[0].GetComponentsInChildren<SpriteRenderer>();
            specialAIs_ShopImages = specialAIs.GetComponentsInChildren<ItemOrder>()[0].GetComponentsInChildren<SpriteRenderer>();

            storyAIs_WeaponImages = storyAIs.GetComponentsInChildren<ItemOrder>()[1].GetComponentsInChildren<SpriteRenderer>();
            AIs_WeaponImages = AIs.GetComponentsInChildren<ItemOrder>()[1].GetComponentsInChildren<SpriteRenderer>();
            specialAIs_WeaponImages = specialAIs.GetComponentsInChildren<ItemOrder>()[1].GetComponentsInChildren<SpriteRenderer>();

            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }
    }
}

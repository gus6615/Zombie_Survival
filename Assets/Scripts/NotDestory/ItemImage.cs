using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemImage : MonoBehaviour
{
    static private ItemImage instance;

    static public Image[] gunImages, gunUIImages, bioGunImages;
    static public Image[] armorImages;
    static public Image[] upgradeImages;
    static public Image[] etcImages, etcUIImages;
    static public Image bulletImage;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;

            Transform[] temp = this.gameObject.GetComponentsInChildren<Transform>();

            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].gameObject.name == "WeaponImage")
                    gunImages = temp[i].GetComponentsInChildren<Image>();
                else if (temp[i].gameObject.name == "WeaponUIImage")
                    gunUIImages = temp[i].GetComponentsInChildren<Image>();
                else if (temp[i].gameObject.name == "ArmorImage")
                    armorImages = temp[i].GetComponentsInChildren<Image>();
                else if (temp[i].gameObject.name == "UpgradeImage")
                    upgradeImages = temp[i].GetComponentsInChildren<Image>();
                else if (temp[i].gameObject.name == "EtcGame.ver")
                    etcImages = temp[i].GetComponentsInChildren<Image>();
                else if (temp[i].gameObject.name == "EtcUI.ver")
                    etcUIImages = temp[i].GetComponentsInChildren<Image>();
                else if(temp[i].gameObject.name == "Bullet")
                    bulletImage = temp[i].GetComponent<Image>();
                else if (temp[i].gameObject.name == "BioWeaponImage")
                    bioGunImages = temp[i].GetComponentsInChildren<Image>();
            }
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }

        
    }
}

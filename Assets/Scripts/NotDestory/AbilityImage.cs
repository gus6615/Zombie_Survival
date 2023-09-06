using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityImage : MonoBehaviour
{
    static private AbilityImage instance;

    static public Image[] weaponAbility;
    static public Image[] armorAbility;
    static public Image[] AIAbility;

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
                if (temp[i].gameObject.name == "WeaponAbility")
                    weaponAbility = temp[i].GetComponentsInChildren<Image>();
                else if (temp[i].gameObject.name == "ArmorAbility")
                    armorAbility = temp[i].GetComponentsInChildren<Image>();
                else if (temp[i].gameObject.name == "AIAbility")
                    AIAbility = temp[i].GetComponentsInChildren<Image>();
            }
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }


    }
}

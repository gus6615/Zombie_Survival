using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item
{
    static public string[] weaponNames;
    static public string[] armorNames;
    static public string[] etcNames;
    static public bool isInit;
    static public Color[] colors;

    public int itemCode;
    // 아이템 코드는 1 ~ WeaponNum 까지가 무기아이템을 의미하며, WeaponNum + 1 ~ WeaponNum + ArmorNum 까지가 방어구아이템이다. 이하 생략
    public string name; // 아이템의 이름
    public Image image, UIImage; // 아이템의 아이콘

    public void SetItem()
    {
        if(itemCode > 0)
        {
            if (itemCode <= SaveScript.weaponNum)
                SetGun();
            else if (itemCode >= SaveScript.weaponNum + 1 && itemCode <= SaveScript.weaponNum + SaveScript.armorNum)
                SetArmor();
            else
                SetEtc();
        }
        else
        {
            SetBioGun();
        }
    }

    public void SetBioGun()
    {
        image = ItemImage.bioGunImages[Mathf.Abs(itemCode) - 1];
    }

    public void SetGun()
    {
        name = weaponNames[itemCode - 1];
        image = ItemImage.gunImages[itemCode - 1];
        UIImage = ItemImage.gunUIImages[itemCode - 1];
    }

    public void SetArmor()
    {
        name = armorNames[itemCode - SaveScript.weaponNum - 1];
        image = ItemImage.armorImages[itemCode - SaveScript.weaponNum - 1];
    }

    public void SetEtc()
    {
        name = etcNames[itemCode - SaveScript.weaponNum - SaveScript.armorNum - 1];
        image = ItemImage.etcImages[itemCode - SaveScript.weaponNum - SaveScript.armorNum - 1];
        UIImage = ItemImage.etcUIImages[itemCode - SaveScript.weaponNum - SaveScript.armorNum - 1];
    }

    public void show()
    {
        Debug.Log("[" + itemCode + "] " + name);
    }

    static public void init() // static 변수들을 초기화
    {
        isInit = true;
        weaponNames = new string[SaveScript.weaponNum];
        armorNames = new string[SaveScript.armorNum];
        etcNames = new string[SaveScript.etcNum];
        colors = new Color[5];

        weaponNames[0] = "P1911";
        weaponNames[1] = "MP5";
        weaponNames[2] = "ChurChill";
        weaponNames[3] = "K2";
        weaponNames[4] = "AS50";

        armorNames[0] = "두꺼운 점퍼";
        armorNames[1] = "질긴 특수 야상";
        armorNames[2] = "개량된 가죽 점퍼";
        armorNames[3] = "개량된 철의상";
        armorNames[4] = "티타늄 슈트";

        etcNames[0] = "가공된 철괴 (레어)";
        etcNames[1] = "깨끗한 원단 (레어)";
        etcNames[2] = "부서진 못 (노멀)";
        etcNames[3] = "부서진 판자 (노멀)";
        etcNames[4] = "빈 통조림 (노멀)";
        etcNames[5] = "붉은 결정석 (에픽)";
        etcNames[6] = "푸른 결정석 (유니크)";

        colors[0] = Color.white;
        colors[1] = new Color(0, 0.7f, 1, 1); // 하늘색
        colors[2] = new Color(1, 0, 1, 1); // 보라색
        colors[3] = new Color(1, 1, 0, 1); // 노란색
        colors[4] = new Color(1, 0, 0, 1); // 빨간색
    }
}

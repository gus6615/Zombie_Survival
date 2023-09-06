using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item
{
    static private float[] _armors;
    static private float[] _HPCures;
    static private float[] _reflectDamages;
    static private int[] _ironNums, _clothNums, _prices;

    static public bool isSaveData;
    public bool isHas; // 현재 가지고 있는가
    public float armor; // 데미지 감소량 (퍼센트)
    public float HPCure; // 자동 회복량
    public bool isHPCureTime; // 자동 회복량이 되었는가?
    public float reflectDamage; // 데미지 반사량 (퍼센트)
    public int ironNum, clothNum, price;

    public Armor(int _itemCode)
    {
        if (!isInit)
            init();

        if (!isSaveData)
            SaveData();

        itemCode = _itemCode;
        SetItem();
        SetValue();
    }

    public void SetValue()
    {
        int index = itemCode - SaveScript.weaponNum - 1;
        armor = _armors[index];
        HPCure = _HPCures[index];
        reflectDamage = _reflectDamages[index];
        ironNum = _ironNums[index];
        clothNum = _clothNums[index];
        price = _prices[index];
    }

    static public void SaveData()
    {
        _armors = new float[SaveScript.armorNum];
        _HPCures = new float[SaveScript.armorNum];
        _reflectDamages = new float[SaveScript.armorNum];
        _ironNums = new int[SaveScript.armorNum];
        _prices = new int[SaveScript.armorNum];
        _clothNums = new int[SaveScript.armorNum];

        _armors[0] = 0.1f;
        _armors[1] = 0.2f;
        _armors[2] = 0.3f;
        _armors[3] = 0.4f;
        _armors[4] = 0.5f;

        _HPCures[0] = 0f;
        _HPCures[1] = 0.3f;
        _HPCures[2] = 0.7f;
        _HPCures[3] = 1f;
        _HPCures[4] = 1.5f;

        _reflectDamages[0] = 0f;
        _reflectDamages[1] = 0f;
        _reflectDamages[2] = 1f;
        _reflectDamages[3] = 2f;
        _reflectDamages[4] = 5f;

        _ironNums[0] = 0;
        _ironNums[1] = 0;
        _ironNums[2] = 20;
        _ironNums[3] = 50;
        _ironNums[4] = 100;

        _clothNums[0] = 10;
        _clothNums[1] = 30;
        _clothNums[2] = 80;
        _clothNums[3] = 150;
        _clothNums[4] = 250;

        _prices[0] = 2000;
        _prices[1] = 7000;
        _prices[2] = 15000;
        _prices[3] = 30000;
        _prices[4] = 50000;
    }
}

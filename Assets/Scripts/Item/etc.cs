using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class etc : Item
{
    public string info; // 아이템 설명
    public int quallity;
    // 아이템의 등급을 나타낸다. 1 = 노멀, 2 = 레어, 3 = 에픽, 4 = 유니크, 5 = 레전더리
    public float dropRate; // 드랍할 확률    
    public int price; // 아이템 판매가격

    static public string[] _info = new string[SaveScript.etcNum];
    static public int[] _quallity = new int[SaveScript.etcNum];
    static public float[] _dropRate = new float[SaveScript.etcNum];
    static public int[] _price = new int[SaveScript.etcNum];
    static bool isSavedData;

    static public int maxQuallity;
    static public int[] itemNumsAsQuality;
    static public float[] FarmingPercentAsQuality, FarmingPlusPercentAsQuality;

    public etc(int _itemCode)
    {
        if (!isInit)
            init();

        if (!isSavedData)
            SavedData();

        itemCode = _itemCode;
        SetItem();
        SetValue();
    }

    public void SetValue()
    {
        int EtcItemCode = itemCode - SaveScript.weaponNum - SaveScript.armorNum - 1;

        info = _info[EtcItemCode];
        quallity = _quallity[EtcItemCode];
        dropRate = _dropRate[EtcItemCode];
        price = _price[EtcItemCode];
    }

    public void SavedData()
    {
        isSavedData = true;

        _info[0] = "정제되어 단단하다. 무기 및 방어구 제작 재료로 쓰인다.";
        _info[1] = "부드럽고 깨끗한 천이다. 방어구 제작 재료로 쓰인다.";
        _info[2] = "구부러진 못이다. 모아두면 쓸만할 것 같다.";
        _info[3] = "부서졌지만 무언가를 막기에는 충분할 것 같다.";
        _info[4] = "알루미늄으로 된 깡통. 곰팡이 냄새가 조금 난다.";
        _info[5] = "바이러스 군체가 결정체로 나타난 형상이다. 일반용병단에서 화폐로 쓰인다.";
        _info[6] = "돌연변이 바이러스 군체가 결정체로 나타난 형상이다. 특수용병단에서 화폐로 쓰인다.";



        _quallity[0] = 1;
        _quallity[1] = 1;
        _quallity[2] = 0;
        _quallity[3] = 0;
        _quallity[4] = 0;
        _quallity[5] = 2;
        _quallity[6] = 3;



        _dropRate[0] = 0.2f;
        _dropRate[1] = 0.2f;
        _dropRate[2] = 0.7f;
        _dropRate[3] = 0.7f;
        _dropRate[4] = 0.7f;
        _dropRate[5] = 0.05f;
        _dropRate[6] = 0.02f;



        _price[0] = 100;
        _price[1] = 100;
        _price[2] = 10;
        _price[3] = 15;
        _price[4] = 30;
        _price[5] = 500;
        _price[6] = 1000;



        maxQuallity = 5;
        itemNumsAsQuality = new int[maxQuallity];
        FarmingPercentAsQuality = new float[maxQuallity];
        FarmingPlusPercentAsQuality = new float[maxQuallity];

        for (int i = 0; i < _quallity.Length; i++)
            itemNumsAsQuality[_quallity[i]]++;

        FarmingPercentAsQuality[0] = 0.1f;
        FarmingPercentAsQuality[1] = 0.02f;
        FarmingPercentAsQuality[2] = 0.001f;
        FarmingPercentAsQuality[3] = 0.0003f;
        FarmingPercentAsQuality[4] = 0.000005f;

        for (int i = 0; i < FarmingPercentAsQuality.Length; i++)
            FarmingPlusPercentAsQuality[i] = FarmingPercentAsQuality[i] * 0.2f;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ability
{
    static private string[] weaponNames;
    static private string[] armorNames;
    static private string[] AINames;

    static private string[] weaponInfos;
    static private string[] armorInfos;
    static private string[] AIInfos;

    static private string[] weaponInfos2;
    static private string[] armorInfos2;

    static private int[] weaponDataAsLevel;
    static private int[] armorDataAsLevel;

    static public bool isInit;

    public int abilityCode;
    public string name;
    public string info, info2;
    public int data;
    public Image image;

    public Ability(int _abilityCode)
    {
        if (!isInit)
            Init();

        abilityCode = _abilityCode;
        SetValue();
    }

    static public void Init()
    {
        isInit = true;

        weaponNames = new string[SaveScript.weaponAbilityNum];
        weaponNames[0] = "강력한 총알";
        weaponNames[1] = "퀵드로우 탄창"; 
        weaponNames[2] = "대용량 탄창";
        weaponNames[3] = "광전사";
        weaponNames[4] = "출혈";
        weaponNames[5] = "흡혈";
        weaponNames[6] = "도벽";
        weaponNames[7] = "수집"; 

        armorNames = new string[SaveScript.armorAbilityNum];
        armorNames[0] = "경량화"; 
        armorNames[1] = "경질화";
        armorNames[2] = "고속 치유";
        armorNames[3] = "과다 반사";

        AINames = new string[SaveScript.AIAbilityNum];
        AINames[0] = "프론트 슈터"; // 앞 공격
        AINames[1] = "백 슈터"; // 뒤 공격
        AINames[2] = "풀 슈터"; // 앞 뒤 공격 

        weaponInfos = new string[SaveScript.weaponAbilityNum];
        weaponInfos[0] = "총기 데미지가 증가합니다.";
        weaponInfos[1] = "재장전 속도가 증가합니다.";
        weaponInfos[2] = "탄창 수가 증가합니다.";
        weaponInfos[3] = "총알을 사용할수록 데미지가 증가합니다. 재장전 시 초기화됩니다.";
        weaponInfos[4] = "크리티컬 확률로 출혈을 일으킵니다. 총 8번의 도트 데미지가 들어갑니다.";
        weaponInfos[5] = "크리티컬 확률로 흡혈합니다.";
        weaponInfos[6] = "크리티컬 확률로 적의 골드를 훔칩니다.";
        weaponInfos[7] = "크리티컬 확률로 총알을 수집합니다.";

        armorInfos = new string[SaveScript.armorAbilityNum];
        armorInfos[0] = "이동속도가 증가합니다.";
        armorInfos[1] = "방어력이 증가합니다.";
        armorInfos[2] = "자동 치유력이 증가합니다.";
        armorInfos[3] = "공격 반사량이 증가합니다.";

        weaponDataAsLevel = new int[SaveScript.weaponAbilityNum];
        weaponDataAsLevel[0] = weaponDataAsLevel[1] = weaponDataAsLevel[2] = 20;
        weaponDataAsLevel[3] = weaponDataAsLevel[7] = 1;
        weaponDataAsLevel[4] = weaponDataAsLevel[5] = weaponDataAsLevel[6] = 10;

        armorDataAsLevel = new int[SaveScript.armorAbilityNum];
        armorDataAsLevel[0] = armorDataAsLevel[2] = 20;
        armorDataAsLevel[1] = 5;
        armorDataAsLevel[3] = 50;

        weaponInfos2 = new string[SaveScript.weaponAbilityNum];
        weaponInfos2[0] = "( 총기 데미지 증가율 : ";
        weaponInfos2[1] = "( 재장전 속도 증가율 : ";
        weaponInfos2[2] = "( 탄창 수 증가율 : ";
        weaponInfos2[3] = "( 중첩 데미지 증가율 : 총기 데미지의 ";
        weaponInfos2[4] = "( 출혈 데미지 증가율 : 총기 데미지의 ";
        weaponInfos2[5] = "( 흡혈 량 : 총기 데미지의 ";
        weaponInfos2[6] = "( 도벽할 수 있는 골드의 비율 : ";
        weaponInfos2[7] = "( 수집할 수 있는 총알의 개수 : 1개 ~ ";

        armorInfos2 = new string[SaveScript.armorAbilityNum];
        armorInfos2[0] = "( 이동속도 증가율 : ";
        armorInfos2[1] = "( 방어력 증가율 : ";
        armorInfos2[2] = "( 자동 치유 증가율 : ";
        armorInfos2[3] = "( 공격 반사 증가율 : ";

        AIInfos = new string[SaveScript.AIAbilityNum];
        AIInfos[0] = "앞에서 등장하는 적들을 공격합니다";
        AIInfos[1] = "뒤에서 등장하는 적들을 공격합니다";
        AIInfos[2] = "앞, 뒤에서 등장하는 적들을 공격합니다";
    }

    public void SetValue()
    {
        int tempCode = 0;

        if(abilityCode <= SaveScript.weaponAbilityNum) // weapon 어빌리티
        {
            tempCode = abilityCode - 1;
            name = weaponNames[tempCode];
            info = weaponInfos[tempCode];
            info2 = weaponInfos2[tempCode];
            data = weaponDataAsLevel[tempCode];
            image = AbilityImage.weaponAbility[tempCode];
        }
        else if(abilityCode <= SaveScript.weaponAbilityNum + SaveScript.armorAbilityNum) // armor 어빌리티
        {
            tempCode = abilityCode - SaveScript.weaponAbilityNum - 1;
            name = armorNames[tempCode];
            info = armorInfos[tempCode];
            info2 = armorInfos2[tempCode];
            data = armorDataAsLevel[tempCode];
            image = AbilityImage.armorAbility[tempCode];
        }
        else // ai 어빌리티
        {
            tempCode = abilityCode - SaveScript.weaponAbilityNum - SaveScript.armorAbilityNum - 1;
            name = AINames[tempCode];
            info = AIInfos[tempCode];
            image = AbilityImage.AIAbility[tempCode];
        }
    }
}

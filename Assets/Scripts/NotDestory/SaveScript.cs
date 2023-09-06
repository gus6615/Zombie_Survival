using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class SaveData
{
    public int HP; // 플레이어 HP
    public int gold; // 플레이어 돈의 양
    public int level; // 플레이어의 레벨
    public int levelUp; // 플레이어 다음 레벨 업에 필요한 경험치 량
    public int exp; // 플레이어의 경험치
    public int stage; // 주 스테이지
    public int stage_level; // 부 스테이지
    public int currentStage; // 최근 스테이지
    public int currentStage_level; // 최근 스테이지 레벨
    public int score; // 점수
    public int storyIndex; // 스토리씬에서 다루어 지는 스토리 순서 변수

    public List<bool> hasGuns; // 총기 확보
    public string[] hasGunsAbilitys; // 총기 업그레이드
    public int equipGun; // 현재 장착준인 무기 인덱스
    public List<bool> hasArmors; // 존재하는 모든 아머
    public int equipArmor; // 현재 장착중인 아머 인덱스
    public string[] hasArmorsAbilitys; // 총기 업그레이드
    public List<int> hasEtcs; // 존재하는 모든 아이템들

    public List<bool> hasAI, hasStoryAI, hasSpecialAI;
    public List<int> AIUpgrade, storyAIUpgrade, specialAIUpgrade;

    // Farming Informations
    public bool[] Farming_stages; // 현재 각 파밍 장소에서 파밍중인가?
    public int[] Farming_currentSize; // 현재 파밍된 량
    public int[] Farming_totalNum; // 각 파밍 장소에서 파밍중인 AI들의 수
    public int[] Farming_totalForce; // 각 파밍 장소에 배치된 AI들의 채집력

    // Player Informations
    public int[] hasGunsBullets; // 현재 가지고 있는 무기 총알
    public bool isTutorial; // 튜토리얼을 해야 하는 경우

    // Player Upgrades
    public int DamageUpgrade; // 데미지 업그레이드 (1UP 당 데미지 5% 증가)
    public int HPUpgrade; // 체력 업그레이드 (1UP 당 체력 5 증가)
    public int MoveSpeedUpgrade; // 이동속도 업그레이드 (1UP 당 이동속도 5% 증가)
    public int CriticalPercentUpgrade; // 크리티컬 확률 업그레이드 (1UP 당 크리티컬 확률 3% 증가) 기본 10%로 고정 --> 수정 시, shoutbutton스크립트 수정
    public int HeadShotDamageUpgrade; // 헤드샷 데미지 업그레이드 (1UP 당 헤드샷 데미지 3% 증가)
    public int ArmorDistroyUpgrade; // 방어구 관통력 업그레이드 (1UP 당 방어구 관통력 3% 증가)
    public int Farming_GoldPlusPercentUpgrade; // 파밍 추가 골드 휙득 확률
    public int Farming_GoldPlusMountUpgrade; // 파밍 추가 골드 량 
    public int Farming_ItemPlusPercentUpgrade; // 파밍 추가 아이템 확률
    public int Farming_ItemPlusMountUpgrade; // 파밍 추가 아이템 수

    public int SniperDamageUpgrade; // 스나이퍼 모드 데미지 업그레이드 (1UP 당 데미지 20% 증가, 플레이어의 데미지에는 영향을 받지 않는다.)
    public int SniperBulletUpgrade; // 스나이퍼 모드 총알 수 업그레이드 (1UP 당 1발 증가)
}

public class SaveScript : MonoBehaviour
{
    static public int weaponNum = 5;
    static public int bioWeaponNum = 1;
    static public int armorNum = 5;
    static public int etcNum = 7;

    static public int weaponAbilityNum = 8;
    static public int armorAbilityNum = 4;
    static public int AIAbilityNum = 3;

    static public int farming_stageNum = 5;
    static public int AINum = 2;
    static public int storyAINum = 2;
    static public int specialAINum = 1;

    static private SaveScript instance;
    static public SaveData saveData;
    static public List<gun> guns;
    static public List<gun> bioGuns;
    static public List<Armor> armors;
    static public List<etc> etcs;
    static public Ability[] gunsAbilitys, armorsAbilitys;

    static public AI[][] Farming_AIs; // 각 파밍 장소에서 파밍중인 AI들
    static public AI[] hasAI; // 현재 기본AI케릭터를 가지고 있는가?
    static public AI[] hasStoryAI; // 현재 스토리AI를 가지고 있는가?
    static public SpecialAI[] hasSpecialAI; // 현재 특수AI를 가지고 있는가?

    static public float[] upgradeDatas; // 각 업그레이드 수치값, 값이 변할 때는 Upgarde 스크립트에서 infos도 변경해줘야 한다.

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
            saveData = new SaveData();
            LoadData();

            guns = new List<gun>();
            guns.Add(new gun(1)); // P92
            guns.Add(new gun(2)); // MP5
            guns.Add(new gun(3)); // Chullchill
            guns.Add(new gun(4)); // K2 
            guns.Add(new gun(5)); // AS50

            bioGuns = new List<gun>();
            bioGuns.Add(new gun(-1));

            armors = new List<Armor>();
            armors.Add(new Armor(6)); 
            armors.Add(new Armor(7));
            armors.Add(new Armor(8));
            armors.Add(new Armor(9));
            armors.Add(new Armor(10));

            etcs = new List<etc>();
            etcs.Add(new etc(11)); // 철괴
            etcs.Add(new etc(12)); // 비단
            etcs.Add(new etc(13)); // 못
            etcs.Add(new etc(14)); // 나무판자
            etcs.Add(new etc(15)); // 통조림
            etcs.Add(new etc(16)); // 붉은보석
            etcs.Add(new etc(17)); // 푸른보석

            gunsAbilitys = new Ability[weaponAbilityNum];
            gunsAbilitys[0] = new Ability(1);
            gunsAbilitys[1] = new Ability(2);
            gunsAbilitys[2] = new Ability(3);
            gunsAbilitys[3] = new Ability(4);
            gunsAbilitys[4] = new Ability(5);
            gunsAbilitys[5] = new Ability(6);
            gunsAbilitys[6] = new Ability(7);
            gunsAbilitys[7] = new Ability(8);

            armorsAbilitys = new Ability[armorAbilityNum];
            armorsAbilitys[0] = new Ability(9);
            armorsAbilitys[1] = new Ability(10);
            armorsAbilitys[2] = new Ability(11);
            armorsAbilitys[3] = new Ability(12);

            Farming_AIs = new AI[farming_stageNum][];
            for (int i = 0; i < farming_stageNum; i++)
                Farming_AIs[i] = new AI[6];

            hasStoryAI = new AI[storyAINum];
            hasAI = new AI[AINum];
            hasSpecialAI = new SpecialAI[specialAINum];

            hasStoryAI[0] = new GameObject().AddComponent<DontDestroyScript>().gameObject.AddComponent<MrKim>();
            hasStoryAI[1] = new GameObject().AddComponent<DontDestroyScript>().gameObject.AddComponent<Alex>();
            hasStoryAI[0].level = saveData.storyAIUpgrade[0];
            hasStoryAI[1].level = saveData.storyAIUpgrade[1];

            hasAI[0] = new GameObject().AddComponent<DontDestroyScript>().gameObject.AddComponent<AI_1>();
            hasAI[1] = new GameObject().AddComponent<DontDestroyScript>().gameObject.AddComponent<AI_2>();
            hasAI[0].level = saveData.AIUpgrade[0];
            hasAI[1].level = saveData.AIUpgrade[1];

            hasSpecialAI[0] = new GameObject().AddComponent<DontDestroyScript>().gameObject.AddComponent<SpecialAI_1>();
            hasSpecialAI[0].level = saveData.specialAIUpgrade[0];

            upgradeDatas = new float[12];
            upgradeDatas[0] = upgradeDatas[2] = upgradeDatas[7] = 0.05f;
            upgradeDatas[3] = upgradeDatas[4] = upgradeDatas[5] = 0.03f;
            upgradeDatas[6] = upgradeDatas[8] = upgradeDatas[10] = 0.2f;
            upgradeDatas[9] = upgradeDatas[11] = 1f;
            upgradeDatas[1] = 5f;
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }
    }

    public void Update()
    {
        if (Farming.isFarming)
            SetFarmingInfo();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    static public void SaveData()
    {
        saveData.score = 0;

        FileStream fileStream = new FileStream(Application.dataPath + "/SaveData.json", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(JsonUtility.ToJson(saveData));
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    static public void LoadData()
    {
        if(File.Exists(Application.dataPath + "/SaveData.json"))
        {
            FileStream fileStream = new FileStream(Application.dataPath + "/SaveData.json", FileMode.Open);
            byte[] data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);
            fileStream.Close();
            string str = Encoding.UTF8.GetString(data);
            JsonUtility.FromJsonOverwrite(str, saveData);
        }
        else
        {
            saveData.HP = 100;
            saveData.gold = 10000000;
            saveData.level = 30;
            saveData.levelUp = 100;
            saveData.exp = 0;
            saveData.score = 0;
            saveData.stage = 0;
            saveData.stage_level = 3;
            saveData.storyIndex = 0;

            // Farming Information
            saveData.Farming_stages = new bool[5];
            saveData.Farming_currentSize = new int[saveData.Farming_stages.Length];
            saveData.Farming_totalForce = new int[saveData.Farming_stages.Length];
            saveData.Farming_totalNum = new int[saveData.Farming_stages.Length];

            // Player Information
            saveData.hasGuns = new List<bool>();
            saveData.hasGuns.Add(true);
            saveData.hasGuns.Add(true);
            saveData.hasGuns.Add(true);
            saveData.hasGuns.Add(true);
            saveData.hasGuns.Add(true);

            saveData.hasGunsBullets = new int[weaponNum];
            saveData.hasGunsBullets[0] = 0;
            saveData.hasGunsBullets[1] = 999;
            saveData.hasGunsBullets[2] = 999;
            saveData.hasGunsBullets[3] = 999;
            saveData.hasGunsBullets[4] = 999;
            saveData.equipGun = 0;

            saveData.hasArmors = new List<bool>();
            saveData.equipArmor = -1;
            saveData.hasArmors.Add(true);
            saveData.hasArmors.Add(false);
            saveData.hasArmors.Add(true);
            saveData.hasArmors.Add(false);
            saveData.hasArmors.Add(true);

            saveData.hasEtcs = new List<int>();
            saveData.hasEtcs.Add(9999);
            saveData.hasEtcs.Add(9999);
            saveData.hasEtcs.Add(9999);
            saveData.hasEtcs.Add(9999);
            saveData.hasEtcs.Add(9999);
            saveData.hasEtcs.Add(9999);
            saveData.hasEtcs.Add(9999);

            saveData.hasGunsAbilitys = new string[weaponNum];
            saveData.hasGunsAbilitys[0] = "11011110";
            saveData.hasGunsAbilitys[1] = "11111111";
            saveData.hasGunsAbilitys[2] = "11111111";
            saveData.hasGunsAbilitys[3] = "11111111";
            saveData.hasGunsAbilitys[4] = "11111111";

            saveData.hasArmorsAbilitys = new string[armorNum];
            for (int i = 0; i < saveData.hasArmorsAbilitys.Length; i++)
                saveData.hasArmorsAbilitys[i] = "1111";

            saveData.isTutorial = false;

            saveData.hasAI = new List<bool>();
            saveData.hasAI.Add(true);
            saveData.hasAI.Add(true);

            saveData.hasStoryAI = new List<bool>();
            saveData.hasStoryAI.Add(true);
            saveData.hasStoryAI.Add(true);

            saveData.hasSpecialAI = new List<bool>();
            saveData.hasSpecialAI.Add(true);

            saveData.AIUpgrade = new List<int>();
            saveData.AIUpgrade.Add(0);
            saveData.AIUpgrade.Add(0);

            saveData.storyAIUpgrade = new List<int>();
            saveData.storyAIUpgrade.Add(0);
            saveData.storyAIUpgrade.Add(0);

            saveData.specialAIUpgrade = new List<int>();
            saveData.specialAIUpgrade.Add(0);

            SaveData();
        }
    }

    public void SetFarmingInfo()
    {
        Farming.isFarming = false;
        CancelInvoke("SetFarmingData");

        for (int i = 0; i < saveData.Farming_stages.Length; i++)
        {
            if (saveData.Farming_stages[i])
            {
                InvokeRepeating("SetFarmingData", 1f, 1f);
                break;
            }
        }
    }

    public void SetFarmingData()
    {
        for (int i = 0; i < saveData.Farming_stages.Length; i++)
        {
            if (saveData.Farming_stages[i] && saveData.Farming_currentSize[i] < Farming.stage_totalWorkSize[i])
            {
                int workForce = 0;
                for (int j = 0; j < 6; j++)
                    if (Farming_AIs[i][j] != null)
                        workForce += Farming_AIs[i][j].workPorce + Farming_AIs[i][j].level * 2;

                saveData.Farming_currentSize[i] += workForce;
                Debug.Log(saveData.Farming_currentSize[i] + ", " + workForce);
            }
        }
    }
}

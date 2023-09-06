using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Upgrade : MonoBehaviour
{
    [SerializeField] private GameObject buttonObject;
    private Button[] buttons; // 0 = 뒤로 가기, 1 ~ 3 = 메뉴 버튼
    [SerializeField] private GameObject contentBox;
    [SerializeField] private Image upgradeImage;
    [SerializeField] private GameObject[] contentPanels; // 0 = Player, 1 = AI, 2 = SpecialAI
    [SerializeField] private GameObject contentPanels_Weapon; 
    [SerializeField] private GameObject selectPanel_Weapon; 
    [SerializeField] private GameObject systemInfo;
    [SerializeField] private Text goldText, goldText_weapon;
    [SerializeField] private GameObject AIUpgrade, weaponUpgrade; // UI Prefab 오브젝트
    [SerializeField] private GameObject[] canDeletedButtons; // 0 = itemplus ,1 = sniperbullet
    [SerializeField] private Animator normalAnimator, weaponAnimator;
    [SerializeField] private GameObject weapon_selectProfile;

    static public bool isBack;
    private int menuIndex;
    private int upgradeIndex;
    private int weaponIndex;
    private string[] upgradeNames, upgradeInfos; // 플레이어 전용
    private int[] upgradeType; // 플레이어 각 업그레이드의 타입
    private bool isWeaponOn; // 현재 무기 방어구 업그레이드 화면인가?
    private bool isWorkAnimator;
    private int[] weaponItemCodes, armorItemCodes; // 능력 별 아이테코드
    

    // Start is called before the first frame update
    void Start()
    {
        upgradeNames = new string[contentPanels[0].GetComponentsInChildren<ItemOrder>().Length];
        upgradeInfos = new string[contentPanels[0].GetComponentsInChildren<ItemOrder>().Length];
        upgradeNames[0] = "데미지";
        upgradeNames[1] = "체력";
        upgradeNames[2] = "이동속도";
        upgradeNames[3] = "크리티컬 확률";
        upgradeNames[4] = "헤드샷 데미지";
        upgradeNames[5] = "방어력 관통력";
        upgradeNames[6] = "[채집] 보너스 골드 확률";
        upgradeNames[7] = "[채집] 보너스 골드 량";
        upgradeNames[8] = "[채집] 보너스 아이템 확률";
        upgradeNames[9] = "[채집] 보너스 아이템 량";
        upgradeNames[10] = "[스나이퍼 모드] 데미지";
        upgradeNames[11] = "[스타이퍼 모드] 총알 수";

        upgradeInfos[0] = "플레이어의 데미지가 5% 상승한다.";
        upgradeInfos[1] = "플레이어의 체력이 5 상승한다.";
        upgradeInfos[2] = "플레이어의 이동속도가 5% 상승한다.";
        upgradeInfos[3] = "플레이어의 크리티컬 확률이 3% 상승한다.";
        upgradeInfos[4] = "플레이어의 헤드샷 데미지가 3% 상승한다.";
        upgradeInfos[5] = "플레이어의 방어력 관통력이 3% 상승한다.";
        upgradeInfos[6] = "직접 채집에서 보너스 골드 휙득 확률이 20% 상승한다.(상대확률)";
        upgradeInfos[7] = "직접 채집에서 보너스 골드 휙득량이 5% 상승한다.";
        upgradeInfos[8] = "직접 채집에서 보너스 아이템 휙득 확률이 20% 상승한다.(상대확률)";
        upgradeInfos[9] = "직접 채집에서 보너스 아이템을 하나 더 휙득한다.";
        upgradeInfos[10] = "스나이퍼 데미지가 20% 상승한다.";
        upgradeInfos[11] = "스나이퍼 총알 수가 1발 증가한다";

        upgradeType = new int[contentPanels[0].GetComponentsInChildren<ItemOrder>().Length];
        upgradeType[0] = SaveScript.saveData.DamageUpgrade;
        upgradeType[1] = SaveScript.saveData.HPUpgrade;
        upgradeType[2] = SaveScript.saveData.MoveSpeedUpgrade;
        upgradeType[3] = SaveScript.saveData.CriticalPercentUpgrade;
        upgradeType[4] = SaveScript.saveData.HeadShotDamageUpgrade;
        upgradeType[5] = SaveScript.saveData.ArmorDistroyUpgrade;
        upgradeType[6] = SaveScript.saveData.Farming_GoldPlusPercentUpgrade;
        upgradeType[7] = SaveScript.saveData.Farming_GoldPlusMountUpgrade;
        upgradeType[8] = SaveScript.saveData.Farming_ItemPlusPercentUpgrade;
        upgradeType[9] = SaveScript.saveData.Farming_ItemPlusMountUpgrade;
        upgradeType[10] = SaveScript.saveData.SniperDamageUpgrade;
        upgradeType[11] = SaveScript.saveData.SniperBulletUpgrade;

        buttons = buttonObject.GetComponentsInChildren<Button>();
        weaponItemCodes = new int[SaveScript.weaponAbilityNum];
        weaponItemCodes[0] = 2;
        weaponItemCodes[1] = 3;
        weaponItemCodes[2] = 4;
        weaponItemCodes[3] = 0;
        weaponItemCodes[4] = 0;
        weaponItemCodes[5] = 0;
        weaponItemCodes[6] = 0;
        weaponItemCodes[7] = 0;

        armorItemCodes = new int[SaveScript.armorAbilityNum];
        armorItemCodes[0] = 2;
        armorItemCodes[1] = 3;
        armorItemCodes[2] = 4;
        armorItemCodes[3] = 0;

        if (SaveScript.saveData.Farming_ItemPlusMountUpgrade == 5)
        {
            Text[] texts = canDeletedButtons[0].GetComponentsInChildren<Text>();
            for (int i = 0; i < texts.Length; i++)
                texts[i].color = new Color(1f, 1f, 1f, 0f);
            Image[] images = canDeletedButtons[0].GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
                images[i].color = new Color(1f, 1f, 1f, 0f);
            canDeletedButtons[0].GetComponent<Button>().interactable = false;
        }
            
        if (SaveScript.saveData.SniperBulletUpgrade == 5)
        {
            Text[] texts = canDeletedButtons[1].GetComponentsInChildren<Text>();
            for (int i = 0; i < texts.Length; i++)
                texts[i].color = new Color(1f, 1f, 1f, 0f);
            Image[] images = canDeletedButtons[1].GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
                images[i].color = new Color(1f, 1f, 1f, 0f);
            canDeletedButtons[1].GetComponent<Button>().interactable = false;
        }

        for (int i = 0; i < contentPanels.Length; i++)
        {
            contentPanels[i].SetActive(false);
        }
        goldText_weapon.text = goldText.text = SaveScript.saveData.gold.ToString();

        normalAnimator.SetBool("isOn", true);
    }

    public void BackShelter()
    {
        isBack = true;
    }

    public void WeaponAndArmorButton()
    {
        if (!isWorkAnimator)
        {
            if (!isWeaponOn) // Weapon And Armor 업그레이드
            {
                EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = Color.blue;
                EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text = "플레이어 ＆ 용병" + "\n" + "업그레이드";
                normalAnimator.SetBool("isOn", false);
                weaponAnimator.SetBool("isOn", true);
            }
            else // 기타 업그레이드
            {
                EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = Color.red;
                EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text = "무기 ＆ 방어구" + "\n" + "업그레이드";
                normalAnimator.SetBool("isOn", true);
                weaponAnimator.SetBool("isOn", false);
            }

            isWeaponOn = !isWeaponOn;
            StartCoroutine("WorkAnimator");
        }
    }

    IEnumerator WorkAnimator()
    {
        isWorkAnimator = true;
        yield return new WaitForSeconds(3f);
        isWorkAnimator = false;
    }

    public void SelectMenu()
    {
        menuIndex = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;
        contentBox.GetComponentInChildren<ScrollRect>().content = contentPanels[menuIndex].GetComponent<RectTransform>();
        contentBox.GetComponentInChildren<ScrollRect>().StopMovement();
        contentPanels[menuIndex].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        for (int i = 0; i < contentPanels.Length; i++)
            contentPanels[i].SetActive(false);
        contentPanels[menuIndex].SetActive(true);

        SetUpgradeInfo();
    }

    public void SelectWeapon()
    {
        ItemOrder[] datas = selectPanel_Weapon.GetComponentsInChildren<ItemOrder>();
        for (int i = 0; i < datas.Length; i++)
        {
            Destroy(datas[i].gameObject);
        }

        SpriteRenderer[] datas2 = contentPanels_Weapon.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < datas2.Length; i++)
        {
            Destroy(datas2[i].gameObject);
        }

        for (int i = 0; i < SaveScript.weaponNum; i++)
        {
            GameObject data = Instantiate(weapon_selectProfile, selectPanel_Weapon.transform);
            data.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 1f);
            data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.guns[i].image.sprite;
            data.GetComponentsInChildren<Image>()[1].GetComponent<RectTransform>().sizeDelta = Vector2.ClampMagnitude(SaveScript.guns[i].image.sprite.bounds.size * 100f, 140f);
            data.GetComponent<Button>().onClick.AddListener(SelectWeaponIcon);
            data.GetComponent<ItemOrder>().order = i;
        }
    }

    public void SelectArmor()
    {
        ItemOrder[] datas = selectPanel_Weapon.GetComponentsInChildren<ItemOrder>();
        for (int i = 0; i < datas.Length; i++)
        {
            Destroy(datas[i].gameObject);
        }

        SpriteRenderer[] datas2 = contentPanels_Weapon.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < datas2.Length; i++)
        {
            Destroy(datas2[i].gameObject);
        }

        for (int i = 0; i < SaveScript.armorNum; i++)
        {
            GameObject data = Instantiate(weapon_selectProfile, selectPanel_Weapon.transform);
            data.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 1f);
            data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.armors[i].image.sprite;
            data.GetComponentsInChildren<Image>()[1].GetComponent<RectTransform>().sizeDelta = Vector2.ClampMagnitude(SaveScript.armors[i].image.sprite.bounds.size * 100f, 140f);
            data.GetComponent<Button>().onClick.AddListener(SelectArmorIcon);
            data.GetComponent<ItemOrder>().order = i;
        }
    }

    public void SelectWeaponIcon()
    {
        weaponIndex = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;

        ItemOrder[] data = selectPanel_Weapon.GetComponentsInChildren<ItemOrder>();
        for (int i = 0; i < data.Length; i++)
        {
            data[i].GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 1f);
        }
        EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

        SetWeaponInfo();
    }

    public void SetWeaponInfo()
    {
        goldText_weapon.text = goldText.text = SaveScript.saveData.gold.ToString();

        SpriteRenderer[] datas = contentPanels_Weapon.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < datas.Length; i++)
        {
            Destroy(datas[i].gameObject);
        }

        for (int i = 0; i < SaveScript.weaponAbilityNum; i++)
        {
            int level = (int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[weaponIndex][i]);

            if(level != 0)
            {
                GameObject data = Instantiate(weaponUpgrade, contentPanels_Weapon.transform);
                Text[] texts = data.GetComponentsInChildren<Text>();
                Image[] images = data.GetComponentsInChildren<Image>();

                data.GetComponentInChildren<Button>().onClick.AddListener(SelectWeaponUpgrade);
                data.GetComponentInChildren<ItemOrder>().order = i;
                images[1].sprite = SaveScript.gunsAbilitys[i].image.sprite;
                texts[0].text = SaveScript.gunsAbilitys[i].name + "' UP [LV." + (level - 1) + "]";
                texts[1].text = SaveScript.gunsAbilitys[i].info + "\n" + SaveScript.gunsAbilitys[i].info2 + ((level - 1) * SaveScript.gunsAbilitys[i].data) + "% )";

                texts[2].text = (5000 * level).ToString();
                if (SaveScript.saveData.gold >= 5000 * level)
                    texts[2].color = Color.green;
                else
                    texts[2].color = Color.red;

                texts[3].text = (level * 100).ToString();
                texts[4].text = "(" + SaveScript.saveData.hasEtcs[weaponItemCodes[i]] + ")";
                if (SaveScript.saveData.hasEtcs[weaponItemCodes[i]] >= level * 100)
                    texts[3].color = Color.green;
                else
                    texts[3].color = Color.red;

                images[4].sprite = SaveScript.etcs[weaponItemCodes[i]].image.sprite;
                images[4].color = new Color(1, 1, 1, 1f);

                if (level == 6)
                {
                    Destroy(data.GetComponentInChildren<Button>().gameObject);

                    for (int j = 0; j < texts.Length; j++)
                        texts[j].color = new Color(1f, 1f, 1f, 0.5f);
                    for (int j = 0; j < images.Length; j++)
                        images[j].color = new Color(1f, 1f, 1f, 0.5f);
                }
            }
        }
    }

    public void SelectArmorIcon()
    {
        weaponIndex = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;

        ItemOrder[] data = selectPanel_Weapon.GetComponentsInChildren<ItemOrder>();
        for (int i = 0; i < data.Length; i++)
        {
            data[i].GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 1f);
        }
        EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1f);

        SetArmorInfo();
    }

    public void SetArmorInfo()
    {
        goldText_weapon.text = goldText.text = SaveScript.saveData.gold.ToString();

        SpriteRenderer[] datas = contentPanels_Weapon.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < datas.Length; i++)
        {
            Destroy(datas[i].gameObject);
        }

        for (int i = 0; i < SaveScript.armorAbilityNum; i++)
        {
            int level = (int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[weaponIndex][i]);

            if(level != 0)
            {
                GameObject data = Instantiate(weaponUpgrade, contentPanels_Weapon.transform);
                Text[] texts = data.GetComponentsInChildren<Text>();
                Image[] images = data.GetComponentsInChildren<Image>();

                data.GetComponentInChildren<Button>().onClick.AddListener(SelectArmorUpgrade);
                data.GetComponentInChildren<ItemOrder>().order = i;
                images[1].sprite = SaveScript.armorsAbilitys[i].image.sprite;
                texts[0].text = SaveScript.armorsAbilitys[i].name + "' UP [LV." + (level - 1) + "]";
                texts[1].text = SaveScript.armorsAbilitys[i].info + "\n" + SaveScript.armorsAbilitys[i].info2 + ((level - 1) * SaveScript.armorsAbilitys[i].data) + "% )";

                texts[2].text = (5000 * level).ToString();
                if (SaveScript.saveData.gold >= 5000 * level)
                    texts[2].color = Color.green;
                else
                    texts[2].color = Color.red;

                texts[3].text = (level * 100).ToString();
                texts[4].text = "(" + SaveScript.saveData.hasEtcs[armorItemCodes[i]] + ")";
                if (SaveScript.saveData.hasEtcs[armorItemCodes[i]] >= level * 100)
                    texts[3].color = Color.green;
                else
                    texts[3].color = Color.red;

                images[4].sprite = SaveScript.etcs[armorItemCodes[i]].image.sprite;
                images[4].color = new Color(1, 1, 1, 1f);

                if (level == 6)
                {
                    Destroy(data.GetComponentInChildren<Button>().gameObject);

                    for (int j = 0; j < texts.Length; j++)
                        texts[j].color = new Color(1f, 1f, 1f, 0.5f);
                    for (int j = 0; j < images.Length; j++)
                        images[j].color = new Color(1f, 1f, 1f, 0.5f);
                }
            }
        }
    }

    public void SelectWeaponUpgrade()
    {
        upgradeIndex  = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;
        GameObject system = Instantiate(systemInfo, this.transform);
        int levelData = int.Parse(SaveScript.saveData.hasGunsAbilitys[weaponIndex]);
        int level = (int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[weaponIndex][upgradeIndex]);
        // levelData = 11111111 과 같이 8자리 int형이다.

        if (SaveScript.saveData.gold >= 5000 * level && SaveScript.saveData.hasEtcs[weaponItemCodes[upgradeIndex]] >= 100 * level)
        {
            SaveScript.saveData.gold -= 5000 * level;
            SaveScript.saveData.hasEtcs[weaponItemCodes[upgradeIndex]] -= 100 * level;

            levelData += (int)Mathf.Pow(10, SaveScript.weaponAbilityNum - upgradeIndex - 1);
            SaveScript.saveData.hasGunsAbilitys[weaponIndex] = levelData.ToString();
            system.GetComponentInChildren<Text>().text = "업그레이드 완료!" + "\n" + SaveScript.gunsAbilitys[upgradeIndex].name + " [Lv." + level + "]";
            SetWeaponInfo();
        }
        else
        {
            system.GetComponentInChildren<Text>().text = "재고가 부족합니다!";
        }
    }

    public void SelectArmorUpgrade()
    {
        upgradeIndex = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;
        GameObject system = Instantiate(systemInfo, this.transform);
        int levelData = int.Parse(SaveScript.saveData.hasArmorsAbilitys[weaponIndex]);
        int level = (int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[weaponIndex][upgradeIndex]);
        // levelData = 11111111 과 같이 8자리 int형이다.

        if (SaveScript.saveData.gold >= 5000 * level && SaveScript.saveData.hasEtcs[armorItemCodes[upgradeIndex]] >= 100 * level)
        {
            SaveScript.saveData.gold -= 5000 * level;
            SaveScript.saveData.hasEtcs[armorItemCodes[upgradeIndex]] -= 100 * level;

            levelData += (int)Mathf.Pow(10, SaveScript.armorAbilityNum - upgradeIndex - 1);
            SaveScript.saveData.hasArmorsAbilitys[weaponIndex] = levelData.ToString();
            system.GetComponentInChildren<Text>().text = "업그레이드 완료!" + "\n" + SaveScript.armorsAbilitys[upgradeIndex].name + " [Lv." + level + "]";
            SetArmorInfo();
            Debug.Log(levelData);
        }
        else
        {
            system.GetComponentInChildren<Text>().text = "재고가 부족합니다!";
        }
    }

    public void SelectUpgrade()
    {
        upgradeIndex = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;
        GameObject system = Instantiate(systemInfo, this.transform);
        bool isCanUpgrade = false;
        int times = 1; // 플레이어가 지불해야 할 금액의 곱

        switch (menuIndex)
        {
            case 0:
                if (upgradeIndex == 9)
                    times = 5;

                if (upgradeIndex == 11)
                    times = 3;

                if (upgradeType[upgradeIndex] >= 10)
                {
                    if (SaveScript.saveData.hasEtcs[6] >= (upgradeType[upgradeIndex] + 1 - 10) * times && SaveScript.saveData.gold >= 1000 * (upgradeType[upgradeIndex] + 1) * times)
                    {
                        isCanUpgrade = true;
                        SaveScript.saveData.hasEtcs[6] -= (upgradeType[upgradeIndex] + 1 - 10) * times;
                    }
                }
                else if (upgradeType[upgradeIndex] >= 5)
                {
                    if (SaveScript.saveData.hasEtcs[5] >= (upgradeType[upgradeIndex] + 1 - 5) * times && SaveScript.saveData.gold >= 1000 * (upgradeType[upgradeIndex] + 1) * times)
                    {
                        isCanUpgrade = true;
                        SaveScript.saveData.hasEtcs[5] -= (upgradeType[upgradeIndex] + 1 - 5) * times;
                    }
                }
                else
                {
                    if (SaveScript.saveData.gold >= 1000 * (upgradeType[upgradeIndex] + 1) * times)
                        isCanUpgrade = true;
                }

                if (isCanUpgrade)
                {
                    SaveScript.saveData.gold -= 1000 * (upgradeType[upgradeIndex] + 1) * times;
                    upgradeType[upgradeIndex]++;
                    switch (upgradeIndex)
                    {
                        case 0:
                            SaveScript.saveData.DamageUpgrade++;
                            break;
                        case 1:
                            SaveScript.saveData.HPUpgrade++;
                            break;
                        case 2:
                            SaveScript.saveData.MoveSpeedUpgrade++;
                            break;
                        case 3:
                            SaveScript.saveData.CriticalPercentUpgrade++;
                            break;
                        case 4:
                            SaveScript.saveData.HeadShotDamageUpgrade++;
                            break;
                        case 5:
                            SaveScript.saveData.ArmorDistroyUpgrade++;
                            break;
                        case 6:
                            SaveScript.saveData.Farming_GoldPlusPercentUpgrade++;
                            break;
                        case 7:
                            SaveScript.saveData.Farming_GoldPlusMountUpgrade++;
                            break;
                        case 8:
                            SaveScript.saveData.Farming_ItemPlusPercentUpgrade++;
                            break;
                        case 9:
                            SaveScript.saveData.Farming_ItemPlusMountUpgrade++;
                            break;
                        case 10:
                            SaveScript.saveData.SniperDamageUpgrade++;
                            break;
                        case 11:
                            SaveScript.saveData.SniperBulletUpgrade++;
                            break;
                    }
                    SetUpgradeInfo();
                    system.GetComponentInChildren<Text>().text = "업그레이드 완료!" + "\n" + upgradeNames[upgradeIndex] + " [Lv." + (upgradeType[upgradeIndex] + 1) + "]";
                }
                else
                {
                    system.GetComponentInChildren<Text>().text = "재고가 부족합니다!";
                }

                if(upgradeIndex == 9 && SaveScript.saveData.Farming_ItemPlusMountUpgrade == 5)
                {
                    Text[] texts = canDeletedButtons[0].GetComponentsInChildren<Text>();
                    for (int i = 0; i < texts.Length; i++)
                        texts[i].color = new Color(1f, 1f, 1f, 0f);
                    Image[] images = canDeletedButtons[0].GetComponentsInChildren<Image>();
                    for (int i = 0; i < images.Length; i++)
                        images[i].color = new Color(1f, 1f, 1f, 0f);
                    canDeletedButtons[0].GetComponent<Button>().interactable = false;
                }
                if (upgradeIndex == 11 && SaveScript.saveData.SniperBulletUpgrade == 5)
                {
                    Text[] texts = canDeletedButtons[1].GetComponentsInChildren<Text>();
                    for (int i = 0; i < texts.Length; i++)
                        texts[i].color = new Color(1f, 1f, 1f, 0f);
                    Image[] images = canDeletedButtons[1].GetComponentsInChildren<Image>();
                    for (int i = 0; i < images.Length; i++)
                        images[i].color = new Color(1f, 1f, 1f, 0f);
                    canDeletedButtons[1].GetComponent<Button>().interactable = false;
                }
                break;
            case 1:
                if(upgradeIndex < SaveScript.storyAINum)
                {
                    if (SaveScript.hasStoryAI[upgradeIndex].level >= 10)
                    {
                        if (SaveScript.saveData.hasEtcs[6] >= (SaveScript.hasStoryAI[upgradeIndex].level + 1 - 10) * times && SaveScript.saveData.gold >= 1000 * (SaveScript.hasStoryAI[upgradeIndex].level + 1) * times)
                        {
                            isCanUpgrade = true;
                            SaveScript.saveData.hasEtcs[6] -= (SaveScript.hasStoryAI[upgradeIndex].level + 1 - 10) * times;
                        }
                    }
                    else if (SaveScript.hasStoryAI[upgradeIndex].level >= 5)
                    {
                        if (SaveScript.saveData.hasEtcs[5] >= (SaveScript.hasStoryAI[upgradeIndex].level + 1 - 5) * times && SaveScript.saveData.gold >= 1000 * (SaveScript.hasStoryAI[upgradeIndex].level + 1) * times)
                        {
                            isCanUpgrade = true;
                            SaveScript.saveData.hasEtcs[5] -= (SaveScript.hasStoryAI[upgradeIndex].level + 1 - 5) * times;
                        }
                    }
                    else
                    {
                        if (SaveScript.saveData.gold >= 1000 * (SaveScript.hasStoryAI[upgradeIndex].level + 1) * times)
                            isCanUpgrade = true;
                    }

                    if (isCanUpgrade)
                    {
                        SaveScript.saveData.gold -= 1000 * (SaveScript.hasStoryAI[upgradeIndex].level + 1) * times;
                        SaveScript.hasStoryAI[upgradeIndex].level++;
                        SaveScript.saveData.storyAIUpgrade[upgradeIndex]++;
                        
                        SetUpgradeInfo();
                        system.GetComponentInChildren<Text>().text = "업그레이드 완료!" + "\n" + SaveScript.hasStoryAI[upgradeIndex].name + " [Lv." + (SaveScript.hasStoryAI[upgradeIndex].level + 1) + "]";
                    }
                    else
                    {
                        system.GetComponentInChildren<Text>().text = "재고가 부족합니다!";
                    }
                }
                else
                {
                    upgradeIndex -= SaveScript.AINum;
                    if (SaveScript.hasAI[upgradeIndex].level >= 10)
                    {
                        if (SaveScript.saveData.hasEtcs[6] >= (SaveScript.hasAI[upgradeIndex].level + 1 - 10) * times && SaveScript.saveData.gold >= 1000 * (SaveScript.hasAI[upgradeIndex].level + 1) * times)
                        {
                            isCanUpgrade = true;
                            SaveScript.saveData.hasEtcs[6] -= (SaveScript.hasAI[upgradeIndex].level + 1 - 10) * times;
                        }
                    }
                    else if (SaveScript.hasAI[upgradeIndex].level >= 5)
                    {
                        if (SaveScript.saveData.hasEtcs[5] >= (SaveScript.hasAI[upgradeIndex].level + 1 - 5) * times && SaveScript.saveData.gold >= 1000 * (SaveScript.hasAI[upgradeIndex].level + 1) * times)
                        {
                            isCanUpgrade = true;
                            SaveScript.saveData.hasEtcs[5] -= (SaveScript.hasAI[upgradeIndex].level + 1 - 5) * times;
                        }
                    }
                    else
                    {
                        if (SaveScript.saveData.gold >= 1000 * (SaveScript.hasAI[upgradeIndex].level + 1) * times)
                            isCanUpgrade = true;
                    }

                    if (isCanUpgrade)
                    {
                        SaveScript.saveData.gold -= 1000 * (SaveScript.hasAI[upgradeIndex].level + 1) * times;
                        SaveScript.hasAI[upgradeIndex].level++;
                        SaveScript.saveData.AIUpgrade[upgradeIndex]++;

                        SetUpgradeInfo();
                        system.GetComponentInChildren<Text>().text = "업그레이드 완료!" + "\n" + SaveScript.hasAI[upgradeIndex].name + " [Lv." + (SaveScript.hasAI[upgradeIndex].level + 1) + "]";
                    }
                    else
                    {
                        system.GetComponentInChildren<Text>().text = "재고가 부족합니다!";
                    }
                }
                break;
            case 2:
                if (SaveScript.hasSpecialAI[upgradeIndex].level >= 10)
                {
                    if (SaveScript.saveData.hasEtcs[6] >= (SaveScript.hasSpecialAI[upgradeIndex].level + 1 - 10) * times && SaveScript.saveData.gold >= 1000 * (SaveScript.hasSpecialAI[upgradeIndex].level + 1) * times)
                    {
                        isCanUpgrade = true;
                        SaveScript.saveData.hasEtcs[6] -= (SaveScript.hasSpecialAI[upgradeIndex].level + 1 - 10) * times;
                    }
                }
                else if (SaveScript.hasSpecialAI[upgradeIndex].level >= 5)
                {
                    if (SaveScript.saveData.hasEtcs[5] >= (SaveScript.hasSpecialAI[upgradeIndex].level + 1 - 5) * times && SaveScript.saveData.gold >= 1000 * (SaveScript.hasSpecialAI[upgradeIndex].level + 1) * times)
                    {
                        isCanUpgrade = true;
                        SaveScript.saveData.hasEtcs[5] -= (SaveScript.hasSpecialAI[upgradeIndex].level + 1 - 5) * times;
                    }
                }
                else
                {
                    if (SaveScript.saveData.gold >= 1000 * (SaveScript.hasSpecialAI[upgradeIndex].level + 1) * times)
                        isCanUpgrade = true;
                }

                if (isCanUpgrade)
                {
                    SaveScript.saveData.gold -= 1000 * (SaveScript.hasSpecialAI[upgradeIndex].level + 1) * times;
                    SaveScript.hasSpecialAI[upgradeIndex].level++;
                    SaveScript.saveData.specialAIUpgrade[upgradeIndex]++;

                    SetUpgradeInfo();
                    system.GetComponentInChildren<Text>().text = "업그레이드 완료!" + "\n" + SaveScript.hasSpecialAI[upgradeIndex].name + " [Lv." + (SaveScript.hasSpecialAI[upgradeIndex].level + 1) + "]";
                }
                else
                {
                    system.GetComponentInChildren<Text>().text = "재고가 부족합니다!";
                }
                break;
        }
    }

    public void SetUpgradeInfo()
    {
        goldText_weapon.text = goldText.text = SaveScript.saveData.gold.ToString();
        int times = 1; // 플레이어가 지불해야 할 금액의 곱

        switch (menuIndex)
        {
            case 0:
                SpriteRenderer[] upgrades = contentPanels[menuIndex].GetComponentsInChildren<SpriteRenderer>();

                for (int i = 0; i < upgradeType.Length; i++)
                {
                    if (i == 9)
                        times = 5;
                    else if (i == 11)
                        times = 3;
                    else
                        times = 1;

                    if((i == 9 && SaveScript.saveData.Farming_ItemPlusMountUpgrade == 5) || (i == 11 && SaveScript.saveData.SniperBulletUpgrade == 5))
                    {

                    }
                    else
                    {
                        upgrades[i].GetComponentsInChildren<Text>()[0].text = upgradeNames[i] + " UP [Lv." + (upgradeType[i] + 1) + "]";
                        upgrades[i].GetComponentsInChildren<Text>()[1].text = upgradeInfos[i];
                        upgrades[i].GetComponentsInChildren<Text>()[2].text = (1000 * (upgradeType[i] + 1) * times).ToString();

                        if (SaveScript.saveData.gold >= 1000 * (upgradeType[i] + 1) * times)
                            upgrades[i].GetComponentsInChildren<Text>()[2].color = Color.green;
                        else
                            upgrades[i].GetComponentsInChildren<Text>()[2].color = Color.red;

                        if (upgradeType[i] >= 10)
                        {
                            upgrades[i].GetComponentsInChildren<Text>()[3].text = ((upgradeType[i] + 1 - 10) * times).ToString();
                            upgrades[i].GetComponentsInChildren<Text>()[4].text = "(" + SaveScript.saveData.hasEtcs[6] + ")";
                            if (SaveScript.saveData.hasEtcs[6] >= (upgradeType[i] + 1 - 10) * times)
                                upgrades[i].GetComponentsInChildren<Text>()[3].color = Color.green;
                            else
                                upgrades[i].GetComponentsInChildren<Text>()[3].color = Color.red;

                            upgrades[i].GetComponentsInChildren<Image>()[4].sprite = SaveScript.etcs[6].image.sprite;
                            upgrades[i].GetComponentsInChildren<Image>()[4].color = new Color(1, 1, 1, 1f);
                        }
                        else if (upgradeType[i] >= 5)
                        {
                            upgrades[i].GetComponentsInChildren<Text>()[3].text = ((upgradeType[i] + 1 - 5) * times).ToString();
                            upgrades[i].GetComponentsInChildren<Text>()[4].text = "(" + SaveScript.saveData.hasEtcs[5] + ")";
                            if (SaveScript.saveData.hasEtcs[5] >= (upgradeType[i] + 1 - 5) * times)
                                upgrades[i].GetComponentsInChildren<Text>()[3].color = Color.green;
                            else
                                upgrades[i].GetComponentsInChildren<Text>()[3].color = Color.red;

                            upgrades[i].GetComponentsInChildren<Image>()[4].sprite = SaveScript.etcs[5].image.sprite;
                            upgrades[i].GetComponentsInChildren<Image>()[4].color = new Color(1, 1, 1, 1f);
                        }
                        else
                        {
                            upgrades[i].GetComponentsInChildren<Text>()[3].text = upgrades[i].GetComponentsInChildren<Text>()[4].text = "";
                            upgrades[i].GetComponentsInChildren<Image>()[4].color = new Color(1, 1, 1, 0f);
                        }
                    }
                }
                break;
            case 1:
                SpriteRenderer[] datas = contentPanels[menuIndex].GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < datas.Length; i++)
                    Destroy(datas[i].gameObject);

                for (int i = 0; i < SaveScript.storyAINum; i++)
                {
                    if (SaveScript.saveData.hasStoryAI[i])
                    {
                        GameObject data = Instantiate(AIUpgrade, contentPanels[menuIndex].transform);

                        data.GetComponentInChildren<Button>().onClick.AddListener(SelectUpgrade);
                        data.GetComponentInChildren<ItemOrder>().order = i;
                        data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.hasStoryAI[i].shop_image.sprite;
                        data.GetComponentsInChildren<Text>()[0].text = "용병 '" + SaveScript.hasStoryAI[i].name + "' UP [LV." + (SaveScript.hasStoryAI[i].level + 1) + "]";
                        data.GetComponentsInChildren<Text>()[1].text = "데미지 20%, 방어력 관통력 5%, 채집력 2 증가";

                        data.GetComponentsInChildren<Text>()[2].text = (1000 * (SaveScript.hasStoryAI[i].level + 1) * times).ToString();
                        if (SaveScript.saveData.gold >= 1000 * (SaveScript.hasStoryAI[i].level + 1) * times)
                            data.GetComponentsInChildren<Text>()[2].color = Color.green;
                        else
                            data.GetComponentsInChildren<Text>()[2].color = Color.red;

                        if (SaveScript.hasStoryAI[i].level >= 10)
                        {
                            data.GetComponentsInChildren<Text>()[3].text = ((SaveScript.hasStoryAI[i].level + 1 - 10) * times).ToString();
                            data.GetComponentsInChildren<Text>()[4].text = "(" + SaveScript.saveData.hasEtcs[6] + ")";
                            if (SaveScript.saveData.hasEtcs[6] >= (SaveScript.hasStoryAI[i].level + 1 - 10) * times)
                                data.GetComponentsInChildren<Text>()[3].color = Color.green;
                            else
                                data.GetComponentsInChildren<Text>()[3].color = Color.red;

                            data.GetComponentsInChildren<Image>()[4].sprite = SaveScript.etcs[6].image.sprite;
                            data.GetComponentsInChildren<Image>()[4].color = new Color(1, 1, 1, 1f);
                        }
                        else if (SaveScript.hasStoryAI[i].level >= 5)
                        {
                            data.GetComponentsInChildren<Text>()[3].text = ((SaveScript.hasStoryAI[i].level + 1 - 5) * times).ToString();
                            data.GetComponentsInChildren<Text>()[4].text = "(" + SaveScript.saveData.hasEtcs[5] + ")";
                            if (SaveScript.saveData.hasEtcs[5] >= (SaveScript.hasStoryAI[i].level + 1 - 5) * times)
                                data.GetComponentsInChildren<Text>()[3].color = Color.green;
                            else
                                data.GetComponentsInChildren<Text>()[3].color = Color.red;

                            data.GetComponentsInChildren<Image>()[4].sprite = SaveScript.etcs[5].image.sprite;
                            data.GetComponentsInChildren<Image>()[4].color = new Color(1, 1, 1, 1f);
                        }
                        else
                        {
                            data.GetComponentsInChildren<Text>()[3].text = data.GetComponentsInChildren<Text>()[4].text = "";
                            data.GetComponentsInChildren<Image>()[4].color = new Color(1, 1, 1, 0f);
                        }
                    }
                }

                for (int i = 0; i < SaveScript.AINum; i++)
                {
                    if (SaveScript.saveData.hasAI[i])
                    {
                        GameObject data = Instantiate(AIUpgrade, contentPanels[menuIndex].transform);

                        data.GetComponentInChildren<Button>().onClick.AddListener(SelectUpgrade);
                        data.GetComponentInChildren<ItemOrder>().order = i + SaveScript.storyAINum;
                        data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.hasAI[i].shop_image.sprite;
                        data.GetComponentsInChildren<Text>()[0].text = "용병 '" + SaveScript.hasAI[i].name + "' UP [LV." + (SaveScript.hasAI[i].level + 1) + "]";
                        data.GetComponentsInChildren<Text>()[1].text = "데미지 20%, 방어력 관통력 5%, 채집력 2 증가";

                        data.GetComponentsInChildren<Text>()[2].text = (1000 * (SaveScript.hasAI[i].level + 1) * times).ToString();
                        if (SaveScript.saveData.gold >= 1000 * (SaveScript.hasAI[i].level + 1) * times)
                            data.GetComponentsInChildren<Text>()[2].color = Color.green;
                        else
                            data.GetComponentsInChildren<Text>()[2].color = Color.red;

                        if (SaveScript.hasAI[i].level >= 10)
                        {
                            data.GetComponentsInChildren<Text>()[3].text = ((SaveScript.hasAI[i].level + 1 - 10) * times).ToString();
                            data.GetComponentsInChildren<Text>()[4].text = "(" + SaveScript.saveData.hasEtcs[6] + ")";
                            if (SaveScript.saveData.hasEtcs[6] >= (SaveScript.hasAI[i].level + 1 - 10) * times)
                                data.GetComponentsInChildren<Text>()[3].color = Color.green;
                            else
                                data.GetComponentsInChildren<Text>()[3].color = Color.red;

                            data.GetComponentsInChildren<Image>()[4].sprite = SaveScript.etcs[6].image.sprite;
                            data.GetComponentsInChildren<Image>()[4].color = new Color(1, 1, 1, 1f);
                        }
                        else if (SaveScript.hasAI[i].level >= 5)
                        {
                            data.GetComponentsInChildren<Text>()[3].text = ((SaveScript.hasAI[i].level + 1 - 5) * times).ToString();
                            data.GetComponentsInChildren<Text>()[4].text = "(" + SaveScript.saveData.hasEtcs[5] + ")";
                            if (SaveScript.saveData.hasEtcs[5] >= (SaveScript.hasAI[i].level + 1 - 5) * times)
                                data.GetComponentsInChildren<Text>()[3].color = Color.green;
                            else
                                data.GetComponentsInChildren<Text>()[3].color = Color.red;

                            data.GetComponentsInChildren<Image>()[4].sprite = SaveScript.etcs[5].image.sprite;
                            data.GetComponentsInChildren<Image>()[4].color = new Color(1, 1, 1, 1f);
                        }
                        else
                        {
                            data.GetComponentsInChildren<Text>()[3].text = data.GetComponentsInChildren<Text>()[4].text = "";
                            data.GetComponentsInChildren<Image>()[4].color = new Color(1, 1, 1, 0f);
                        }
                    }
                }
                break;
            case 2:
                SpriteRenderer[] datas2 = contentPanels[menuIndex].GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < datas2.Length; i++)
                    Destroy(datas2[i].gameObject);

                for (int i = 0; i < SaveScript.specialAINum; i++)
                {
                    if (SaveScript.saveData.hasSpecialAI[i])
                    {
                        GameObject data = Instantiate(AIUpgrade, contentPanels[menuIndex].transform);

                        data.GetComponentInChildren<Button>().onClick.AddListener(SelectUpgrade);
                        data.GetComponentInChildren<ItemOrder>().order = i;
                        data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.hasSpecialAI[i].shop_image.sprite;
                        data.GetComponentsInChildren<Text>()[0].text = "용병 '" + SaveScript.hasSpecialAI[i].name + "' UP [LV." + (SaveScript.hasSpecialAI[i].level + 1) + "]";
                        data.GetComponentsInChildren<Text>()[1].text = "데미지 10%, 채집력 2 증가";

                        data.GetComponentsInChildren<Text>()[2].text = (1000 * (SaveScript.hasSpecialAI[i].level + 1) * times).ToString();
                        if (SaveScript.saveData.gold >= 1000 * (SaveScript.hasSpecialAI[i].level + 1) * times)
                            data.GetComponentsInChildren<Text>()[2].color = Color.green;
                        else
                            data.GetComponentsInChildren<Text>()[2].color = Color.red;

                        if (SaveScript.hasSpecialAI[i].level >= 10)
                        {
                            data.GetComponentsInChildren<Text>()[3].text = ((SaveScript.hasSpecialAI[i].level + 1 - 10) * times).ToString();
                            data.GetComponentsInChildren<Text>()[4].text = "(" + SaveScript.saveData.hasEtcs[6] + ")";
                            if (SaveScript.saveData.hasEtcs[6] >= (SaveScript.hasSpecialAI[i].level + 1 - 10) * times)
                                data.GetComponentsInChildren<Text>()[3].color = Color.green;
                            else
                                data.GetComponentsInChildren<Text>()[3].color = Color.red;

                            data.GetComponentsInChildren<Image>()[4].sprite = SaveScript.etcs[6].image.sprite;
                            data.GetComponentsInChildren<Image>()[4].color = new Color(1, 1, 1, 1f);
                        }
                        else if (SaveScript.hasSpecialAI[i].level >= 5)
                        {
                            data.GetComponentsInChildren<Text>()[3].text = ((SaveScript.hasSpecialAI[i].level + 1 - 5) * times).ToString();
                            data.GetComponentsInChildren<Text>()[4].text = "(" + SaveScript.saveData.hasEtcs[5] + ")";
                            if (SaveScript.saveData.hasEtcs[5] >= (SaveScript.hasSpecialAI[i].level + 1 - 5) * times)
                                data.GetComponentsInChildren<Text>()[3].color = Color.green;
                            else
                                data.GetComponentsInChildren<Text>()[3].color = Color.red;

                            data.GetComponentsInChildren<Image>()[4].sprite = SaveScript.etcs[5].image.sprite;
                            data.GetComponentsInChildren<Image>()[4].color = new Color(1, 1, 1, 1f);
                        }
                        else
                        {
                            data.GetComponentsInChildren<Text>()[3].text = data.GetComponentsInChildren<Text>()[4].text = "";
                            data.GetComponentsInChildren<Image>()[4].color = new Color(1, 1, 1, 0f);
                        }
                    }
                }
                break;
        }
    }
}

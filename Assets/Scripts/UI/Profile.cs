using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Profile : MonoBehaviour
{
    [SerializeField] private GameObject buttonObject;
    private Button[] buttons; // 0 = back, 1 ~ 5 = 메뉴 버튼들
    [SerializeField] private GameObject selectObject;
    private Image selectPanel;
    [SerializeField] private GameObject lineObject;
    [SerializeField] private GameObject profile_imageObject;
    [SerializeField] private GameObject[] contents;
    [SerializeField] private GameObject ability;
    [SerializeField] private GameObject profile_AI;
    [SerializeField] private GameObject armorEquipButton;
    public GameObject selectButtonPref;
    private List<Button> selectButtons; // select 버튼들
    private SpriteRenderer[][] selects;

    private int menuIndex, listIndex;

    static public bool isBack;
    private bool isShowMenu; // 첫 시작시. 페이드 효과 시작을 알리는 변수
    private bool isShowSelect; // 메뉴 클릭시, 페이드 효과 시작을 알리는 변수 
    private int order; // 무기, 방어구, 아이템의 코드 (itemOrder)

    // Start is called before the first frame update
    void Start()
    {
        buttons = buttonObject.GetComponentsInChildren<Button>();
        selectPanel = selectObject.GetComponentInChildren<Image>();
        selectButtons = new List<Button>();
        armorEquipButton.SetActive(false);

        // AI Selector 생성
        for (int i = 0; i < SaveScript.storyAINum; i++)
        {
            if (SaveScript.saveData.hasStoryAI[i])
            {
                GameObject data = Instantiate(profile_AI, contents[2].transform);
                data.name = SaveScript.hasStoryAI[i].name;
                data.GetComponent<ItemOrder>().order = i;
            }
        }

        for (int i = 0; i < SaveScript.AINum; i++)
        {
            if (SaveScript.saveData.hasAI[i])
            {
                GameObject data = Instantiate(profile_AI, contents[3].transform);
                data.name = SaveScript.hasAI[i].name;
                data.GetComponent<ItemOrder>().order = i;
            }
        }

        for (int i = 0; i < SaveScript.specialAINum; i++)
        {
            if (SaveScript.hasSpecialAI[i])
            {
                GameObject data = Instantiate(profile_AI, contents[4].transform);
                data.name = SaveScript.hasSpecialAI[i].name;
                data.GetComponent<ItemOrder>().order = i;
            }
        }

        selects = new SpriteRenderer[contents.Length][];
        for (int i = 0; i < selects.Length; i++)
            selects[i] = new SpriteRenderer[contents[i].GetComponentsInChildren<SpriteRenderer>().Length];

        for (int i = 0; i < selects.Length; i++)
            selects[i] = contents[i].GetComponentsInChildren<SpriteRenderer>();

        for (int i = 1; i < buttons.Length; i++)
            buttons[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        for (int i = 0; i < contents.Length; i++)
            contents[i].gameObject.SetActive(false);

        isShowMenu = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isShowMenu)
            ShowMenu();

        if (isShowSelect)
            ShowSelect();
    }

    public void BackShelter()
    {
        isBack = true;
    }

    public void ShowMenu()
    {
        for (int i = 1; i < buttons.Length; i++)
        {
            Color temp = buttons[i].GetComponent<Image>().color;
            temp.a += Time.deltaTime;
            buttons[i].GetComponent<Image>().color = temp;

            if (buttons[i].GetComponent<Image>().color.a <= 0.3f)
                break;
        }

        if (buttons[buttons.Length - 1].GetComponent<Image>().color.a >= 1f)
            isShowMenu = false;
    }

    public void ShowSelect()
    {
        for (int i = 0; i < selectButtons.Count; i++)
        {
            Color imageTemp = selectButtons[i].GetComponent<Image>().color;
            Color textTemp = selectButtons[i].GetComponentInChildren<Text>().color;
            imageTemp.a += Time.deltaTime;
            textTemp.a += Time.deltaTime;
            selectButtons[i].GetComponent<Image>().color = imageTemp;
            selectButtons[i].GetComponentInChildren<Text>().color = textTemp;

            if (selectButtons[i].GetComponent<Image>().color.a <= 0.3f)
                break;
        }

        if (selectButtons.Count != 0 && selectButtons[selectButtons.Count - 1].GetComponent<Image>().color.a >= 1f)
            isShowSelect = false;
    }

    public void ClickMenu()
    {
        menuIndex = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;
        isShowSelect = true;

        // UI Object active 설정
        for (int i = 0; i < contents.Length; i++)
            contents[i].SetActive(false);
        contents[menuIndex].SetActive(true);

        // select 버튼 초기화
        Button[] temps = selectPanel.GetComponentsInChildren<Button>();
        for (int i = 0; i < temps.Length; i++)
            Destroy(temps[i].gameObject);

        // select 버튼 생성
        selectButtons.Clear();
        for (int i = 0; i < selects[menuIndex].Length; i++)
        {
            GameObject select = Instantiate(selectButtonPref, selectPanel.transform);
            selectButtons.Add(select.GetComponent<Button>());
            selectButtons[i].onClick.AddListener(ClickSelect);
            selectButtons[i].GetComponent<ItemOrder>().order = i;
            select.GetComponentInChildren<Text>().text = selects[menuIndex][i].gameObject.name;
            select.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            select.GetComponentInChildren<Text>().color = new Color(select.GetComponentInChildren<Text>().color.r,
                select.GetComponentInChildren<Text>().color.g, select.GetComponentInChildren<Text>().color.b, 0);
        }

        // content 초기화
        for (int i = 0; i < selects[menuIndex].Length; i++)
            selects[menuIndex][i].gameObject.SetActive(false);
    }

    public void ClickSelect()
    {
        listIndex = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;

        for (int i = 0; i < selects[menuIndex].Length; i++)
            selects[menuIndex][i].gameObject.SetActive(false);
        selects[menuIndex][listIndex].gameObject.SetActive(true);
        SettingSelect();
    }

    public void SettingSelect()
    {
        Image image;
        Text[] texts;
        Slider expSlider;
        AbilityButton[] abilities;
        int AIindex;

        switch (menuIndex)
        {
            case 0:
                switch (listIndex)
                {
                    case 0: // 플레이어 - 스텟
                        texts = selects[menuIndex][listIndex].GetComponentsInChildren<Text>();
                        expSlider = selects[menuIndex][listIndex].GetComponentInChildren<Slider>();

                        texts[0].text = SaveScript.saveData.level.ToString();
                        texts[1].text = "( " + SaveScript.saveData.exp + " / " + SaveScript.saveData.levelUp + " )";

                        texts[2].text = (100 + SaveScript.saveData.DamageUpgrade * 5f + SaveScript.saveData.level * 5f) + "%";
                        texts[3].text = (SaveScript.saveData.HPUpgrade * 5f + SaveScript.saveData.HP).ToString();
                        texts[4].text = (100 + SaveScript.saveData.MoveSpeedUpgrade * 5f) + "%";
                        texts[5].text = (SaveScript.saveData.CriticalPercentUpgrade * 3f) + "%";
                        texts[6].text = (150 + SaveScript.saveData.HeadShotDamageUpgrade * 3f) + "%";
                        texts[7].text = (SaveScript.saveData.ArmorDistroyUpgrade* 3f) + "%";

                        texts[8].text = "추가 확률 " + (SaveScript.saveData.Farming_GoldPlusPercentUpgrade * 20f) + "%";
                        texts[9].text = "추가 확률 " + (SaveScript.saveData.Farming_ItemPlusPercentUpgrade * 20f) + "%";
                        texts[10].text = "추가 휙득량 " + (SaveScript.saveData.Farming_GoldPlusMountUpgrade * 5f) + "%";
                        texts[11].text = "추가 휙득량 " + SaveScript.saveData.Farming_ItemPlusMountUpgrade + "개";

                        expSlider.value = (float)SaveScript.saveData.exp / SaveScript.saveData.levelUp;
                        break;
                    case 1: // 플레이어 - 전투 스텟 설명
                        texts = selects[menuIndex][listIndex].GetComponentsInChildren<Text>();

                        texts[0].text = "실제 데미지는 (총기 데미지) x [ " + (100 + SaveScript.saveData.DamageUpgrade * (SaveScript.upgradeDatas[0] * 100f) + SaveScript.saveData.level * (SaveScript.upgradeDatas[0] * 100f)) + "% ] 로 적용됩니다.";
                        texts[1].text = "체력은 (100) + [ " + ((SaveScript.saveData.level - 1) * SaveScript.upgradeDatas[1] + SaveScript.saveData.HPUpgrade * SaveScript.upgradeDatas[1])
                            + " ] 으로, 총 " + (100 + SaveScript.saveData.level * SaveScript.upgradeDatas[1] + SaveScript.saveData.HPUpgrade * SaveScript.upgradeDatas[1]) + " 으로 적용됩니다";
                        texts[2].text = "이동속도는 (기본 속도) x [ " + (100 + SaveScript.saveData.MoveSpeedUpgrade * (SaveScript.upgradeDatas[2] * 100f)) + "% ] 로 적용됩니다.";
                        texts[3].text = "크리티컬은 경직과 무기 능력 발동에 관여합니다. [현재 " + (SaveScript.saveData.CriticalPercentUpgrade * (SaveScript.upgradeDatas[3] * 100f)) + "%]";
                        texts[4].text = "헤드샷 데미지는 (총기 데미지) x [ " + (100 + SaveScript.saveData.DamageUpgrade * (SaveScript.upgradeDatas[0] * 100f)
                            + SaveScript.saveData.level * (SaveScript.upgradeDatas[0] * 100f)) + "% ] x [ " + (150 + SaveScript.saveData.HeadShotDamageUpgrade * (SaveScript.upgradeDatas[4] * 100f)) + "% ] 로 적용됩니다.";
                        texts[5].text = "방어구 관통력은 좀비의 방어력을 상쇄시킵니다. [현재 " + (SaveScript.saveData.ArmorDistroyUpgrade * (SaveScript.upgradeDatas[5] * 100f)) + "%]";
                        break;
                    case 2: // 플레이어 - 파밍 스텟 설명
                        texts = selects[menuIndex][listIndex].GetComponentsInChildren<Text>();

                        texts[0].text = "보너스 골드 휙득 확률 = (기존 확률) x [ " + (100 + SaveScript.saveData.Farming_GoldPlusPercentUpgrade * (SaveScript.upgradeDatas[6] * 100f)) + "% ] 로 적용됩니다."; 
                        texts[1].text = "보너스 아이템 휙득 확률 = (기존 확률) x [ " + (100 + SaveScript.saveData.Farming_ItemPlusPercentUpgrade * (SaveScript.upgradeDatas[8] * 100f)) + "% ] 로 적용됩니다.";
                        texts[2].text = "보너스 골드 휙득량 = (기존량) x [ " + (100 + SaveScript.saveData.Farming_GoldPlusMountUpgrade * (SaveScript.upgradeDatas[7] * 100f)) + "% ] 로 적용됩니다.";
                        texts[3].text = "보너스 아이템 휙득량 = (1개) + [ " + (SaveScript.saveData.Farming_ItemPlusMountUpgrade * (SaveScript.upgradeDatas[9])) + "개 ] 로 적용됩니다.";
                        break;
                }
                break;
            case 1:
                switch (listIndex)
                {
                    case 0: // 인벤토리 - 무기
                        ItemOrder[] trashes = selects[menuIndex][listIndex].GetComponentsInChildren<GridLayoutGroup>()[0].GetComponentsInChildren<ItemOrder>();
                        AbilityButton[] trashes2 = selects[menuIndex][listIndex].GetComponentsInChildren<GridLayoutGroup>()[1].GetComponentsInChildren<AbilityButton>();

                        texts = selects[menuIndex][listIndex].GetComponentsInChildren<Text>();
                        image = selects[menuIndex][listIndex].GetComponentInChildren<Image>();

                        for (int i = 0; i < texts.Length; i++)
                            texts[i].text = "";

                        image.color = new Color(1, 1, 1, 0);

                        for (int i = 0; i < trashes.Length; i++)
                            Destroy(trashes[i].gameObject);
                        for (int i = 0; i < trashes2.Length; i++)
                            Destroy(trashes2[i].gameObject);

                        for (int i = 0; i < SaveScript.weaponNum; i++)
                        {
                            if (SaveScript.saveData.hasGuns[i])
                            {
                                GameObject data = Instantiate(profile_imageObject, selects[menuIndex][listIndex].GetComponentsInChildren<GridLayoutGroup>()[0].transform);
                                data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.guns[i].UIImage.sprite;
                                data.GetComponentsInChildren<Image>()[2].gameObject.SetActive(false);
                                data.GetComponent<Button>().onClick.AddListener(WeaponClick);
                                data.GetComponent<ItemOrder>().order = i;
                            }
                        }
                        break;
                    case 1: // 인벤토리 - 방어구
                        ItemOrder[] trashes3 = selects[menuIndex][listIndex].GetComponentsInChildren<GridLayoutGroup>()[0].GetComponentsInChildren<ItemOrder>();
                        AbilityButton[] trashes4 = selects[menuIndex][listIndex].GetComponentsInChildren<GridLayoutGroup>()[1].GetComponentsInChildren<AbilityButton>();

                        texts = selects[menuIndex][listIndex].GetComponentsInChildren<Text>();
                        image = selects[menuIndex][listIndex].GetComponentInChildren<Image>();

                        for (int i = 0; i < texts.Length; i++)
                            texts[i].text = "";

                        image.color = new Color(1, 1, 1, 0);

                        for (int i = 0; i < trashes3.Length; i++)
                            Destroy(trashes3[i].gameObject);
                        for (int i = 0; i < trashes4.Length; i++)
                            Destroy(trashes4[i].gameObject);

                        for (int i = 0; i < SaveScript.armorNum; i++)
                        {
                            if (SaveScript.saveData.hasArmors[i])
                            {
                                GameObject data = Instantiate(profile_imageObject, selects[menuIndex][listIndex].GetComponentsInChildren<GridLayoutGroup>()[0].transform);
                                data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.armors[i].image.sprite;
                                data.GetComponent<Button>().onClick.AddListener(ArmorClick);
                                data.GetComponent<ItemOrder>().order = i;

                                if(SaveScript.saveData.equipArmor != i)
                                {
                                    data.GetComponentsInChildren<Image>()[2].gameObject.SetActive(false);
                                }
                            }
                        }
                        break;
                    case 2: // 인벤토리 - 아이템
                        ItemOrder[] trashes5 = selects[menuIndex][listIndex].GetComponentsInChildren<GridLayoutGroup>()[0].GetComponentsInChildren<ItemOrder>();
                        List<etc> items = new List<etc>();
                        items.AddRange(SaveScript.etcs);

                        items.Sort(delegate (etc A, etc B)
                        {
                            if (A.quallity < B.quallity)
                                return 1;
                            else if (A.quallity > B.quallity)
                                return -1;
                            else
                                return 0;
                        });

                        texts = selects[menuIndex][listIndex].GetComponentsInChildren<Text>();
                        image = selects[menuIndex][listIndex].GetComponentInChildren<Image>();

                        for (int i = 0; i < texts.Length; i++)
                            texts[i].text = "";

                        image.color = new Color(1, 1, 1, 0);
                        selects[menuIndex][listIndex].GetComponentsInChildren<Image>()[2].color = Color.white;

                        for (int i = 0; i < trashes5.Length; i++)
                            Destroy(trashes5[i].gameObject);

                        for (int i = 0; i < items.Count; i++)
                        {
                            if (SaveScript.saveData.hasEtcs[items[i].itemCode - SaveScript.weaponNum - SaveScript.armorNum - 1] != 0)
                            {
                                GameObject data = Instantiate(profile_imageObject, selects[menuIndex][listIndex].GetComponentsInChildren<GridLayoutGroup>()[0].transform);
                                data.GetComponent<ItemOrder>().order = items[i].itemCode - SaveScript.weaponNum - SaveScript.armorNum - 1;
                                data.GetComponentsInChildren<Image>()[1].sprite = SaveScript.etcs[data.GetComponent<ItemOrder>().order].image.sprite;
                                data.GetComponentsInChildren<Image>()[2].gameObject.SetActive(false);
                                data.GetComponentInChildren<Text>().text = "x" + SaveScript.saveData.hasEtcs[data.GetComponent<ItemOrder>().order];
                                data.GetComponent<Button>().onClick.AddListener(ItemClick);
                            }
                        }
                        break;
                }
                break;
            case 2:
                abilities = selects[menuIndex][listIndex].GetComponentInChildren<GridLayoutGroup>().GetComponentsInChildren<AbilityButton>();
                AIindex = selects[menuIndex][listIndex].GetComponent<ItemOrder>().order;

                texts = selects[menuIndex][listIndex].GetComponentsInChildren<Text>();
                image = selects[menuIndex][listIndex].GetComponentInChildren<Image>();

                for (int i = 0; i < texts.Length; i++)
                    texts[i].text = "";

                for (int i = 0; i < abilities.Length; i++)
                    Destroy(abilities[i].gameObject);

                image.sprite = SaveScript.hasStoryAI[AIindex].shop_image.sprite;
                selects[menuIndex][listIndex].GetComponentsInChildren<Image>()[1].sprite = SaveScript.hasStoryAI[AIindex].weapon_image.sprite;
                selects[menuIndex][listIndex].GetComponentsInChildren<Image>()[1].GetComponent<RectTransform>().sizeDelta = SaveScript.hasStoryAI[AIindex].weapon_image.sprite.bounds.size * 100f;

                texts[0].text = "이름 : " + SaveScript.hasStoryAI[AIindex].name;
                texts[1].text = "Lv. " + (SaveScript.hasStoryAI[AIindex].level + 1);
                texts[2].text = "채집력 : " + (SaveScript.hasStoryAI[AIindex].workPorce + SaveScript.hasStoryAI[AIindex].level * 2);

                if ((SaveScript.hasStoryAI[AIindex] as ShotAI) != null)
                {
                    texts[3].text = "무기 종류 : " + SaveScript.hasStoryAI[AIindex].weaponName;
                    texts[4].text = "데미지 : " + (SaveScript.hasStoryAI[AIindex] as ShotAI).damage * (1 + SaveScript.hasStoryAI[AIindex].level * 0.2f);
                    texts[5].text = "탄창 : " + (SaveScript.hasStoryAI[AIindex] as ShotAI).bulletNum + "발";
                    texts[6].text = "방어력 관통력 : " + ((SaveScript.hasStoryAI[AIindex] as ShotAI).armorDestroyPercent + SaveScript.hasStoryAI[AIindex].level * 5f) + "%";
                }

                for (int i = 0; i < SaveScript.hasStoryAI[AIindex].abilities.Length; i++)
                {
                    GameObject data = Instantiate(ability, selects[menuIndex][listIndex].GetComponentInChildren<GridLayoutGroup>().transform);
                    data.GetComponent<Image>().sprite = SaveScript.hasStoryAI[AIindex].abilities[i].image.sprite;
                    data.GetComponentsInChildren<Text>()[0].text = SaveScript.hasStoryAI[AIindex].abilities[i].name;
                    data.GetComponentsInChildren<Text>()[1].text = SaveScript.hasStoryAI[AIindex].abilities[i].info;
                }

                break;
            case 3:
                abilities = selects[menuIndex][listIndex].GetComponentInChildren<GridLayoutGroup>().GetComponentsInChildren<AbilityButton>();
                AIindex = selects[menuIndex][listIndex].GetComponent<ItemOrder>().order;

                texts = selects[menuIndex][listIndex].GetComponentsInChildren<Text>();
                image = selects[menuIndex][listIndex].GetComponentInChildren<Image>();

                for (int i = 0; i < texts.Length; i++)
                    texts[i].text = "";

                for (int i = 0; i < abilities.Length; i++)
                    Destroy(abilities[i].gameObject);

                image.sprite = SaveScript.hasAI[AIindex].shop_image.sprite;
                selects[menuIndex][listIndex].GetComponentsInChildren<Image>()[1].sprite = SaveScript.hasAI[AIindex].weapon_image.sprite;
                selects[menuIndex][listIndex].GetComponentsInChildren<Image>()[1].GetComponent<RectTransform>().sizeDelta = SaveScript.hasAI[AIindex].weapon_image.sprite.bounds.size * 100f;

                texts[0].text = "이름 : " + SaveScript.hasAI[AIindex].name;
                texts[1].text = "Lv. " + (SaveScript.hasAI[AIindex].level + 1);
                texts[2].text = "채집력 : " + (SaveScript.hasAI[AIindex].workPorce + SaveScript.hasAI[AIindex].level * 2);

                if ((SaveScript.hasAI[AIindex] as ShotAI) != null)
                {
                    texts[3].text = "무기 종류 : " + SaveScript.hasAI[AIindex].weaponName;
                    texts[4].text = "데미지 : " + (SaveScript.hasAI[AIindex] as ShotAI).damage * (1 + SaveScript.hasAI[AIindex].level * 0.2f);
                    texts[5].text = "탄창 : " + (SaveScript.hasAI[AIindex] as ShotAI).bulletNum + "발";
                    texts[6].text = "방어력 관통력 : " + ((SaveScript.hasAI[AIindex] as ShotAI).armorDestroyPercent + SaveScript.hasAI[AIindex].level * 5f) + "%";
                }

                for (int i = 0; i < SaveScript.hasAI[AIindex].abilities.Length; i++)
                {
                    GameObject data = Instantiate(ability, selects[menuIndex][listIndex].GetComponentInChildren<GridLayoutGroup>().transform);
                    data.GetComponent<Image>().sprite = SaveScript.hasAI[AIindex].abilities[i].image.sprite;
                    data.GetComponentsInChildren<Text>()[0].text = SaveScript.hasAI[AIindex].abilities[i].name;
                    data.GetComponentsInChildren<Text>()[1].text = SaveScript.hasAI[AIindex].abilities[i].info;
                }
                break;
            case 4:
                abilities = selects[menuIndex][listIndex].GetComponentInChildren<GridLayoutGroup>().GetComponentsInChildren<AbilityButton>();
                AIindex = selects[menuIndex][listIndex].GetComponent<ItemOrder>().order;

                texts = selects[menuIndex][listIndex].GetComponentsInChildren<Text>();
                image = selects[menuIndex][listIndex].GetComponentInChildren<Image>();

                for (int i = 0; i < texts.Length; i++)
                    texts[i].text = "";

                for (int i = 0; i < abilities.Length; i++)
                    Destroy(abilities[i].gameObject);

                image.sprite = SaveScript.hasSpecialAI[AIindex].shop_image.sprite;
                selects[menuIndex][listIndex].GetComponentsInChildren<Image>()[1].sprite = SaveScript.hasSpecialAI[AIindex].weapon_image.sprite;
                selects[menuIndex][listIndex].GetComponentsInChildren<Image>()[1].GetComponent<RectTransform>().sizeDelta = SaveScript.hasSpecialAI[AIindex].weapon_image.sprite.bounds.size * 100f;

                texts[0].text = "이름 : " + SaveScript.hasSpecialAI[AIindex].name;
                texts[1].text = "Lv. " + (SaveScript.hasSpecialAI[AIindex].level + 1);
                texts[2].text = "채집력 : " + (SaveScript.hasSpecialAI[AIindex].workPorce + SaveScript.hasSpecialAI[AIindex].level * 2);
                texts[3].text = "무기 종류 : " + SaveScript.hasSpecialAI[AIindex].weaponName;
                texts[4].text = "데미지 : " + SaveScript.hasSpecialAI[AIindex].damage * (1 + SaveScript.hasSpecialAI[AIindex].level * 0.1f);
                texts[7].text = "[스킬 정보]\n" + SaveScript.hasSpecialAI[AIindex].skillInfo;

                for (int i = 0; i < SaveScript.hasSpecialAI[AIindex].abilities.Length; i++)
                {
                    GameObject data = Instantiate(ability, selects[menuIndex][listIndex].GetComponentInChildren<GridLayoutGroup>().transform);
                    data.GetComponent<Image>().sprite = SaveScript.hasSpecialAI[AIindex].abilities[i].image.sprite;
                    data.GetComponentsInChildren<Text>()[0].text = SaveScript.hasSpecialAI[AIindex].abilities[i].name;
                    data.GetComponentsInChildren<Text>()[1].text = SaveScript.hasSpecialAI[AIindex].abilities[i].info;
                }
                break;
        }
    }

    public void WeaponClick()
    {
        order = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;
        Text[] texts = selects[menuIndex][listIndex].GetComponentsInChildren<Text>();
        Image image = selects[menuIndex][listIndex].GetComponentInChildren<Image>();
        AbilityButton[] trashes = selects[menuIndex][listIndex].GetComponentsInChildren<GridLayoutGroup>()[1].GetComponentsInChildren<AbilityButton>();

        for (int i = 0; i < trashes.Length; i++)
            Destroy(trashes[i].gameObject);

        for (int i = 0; i < SaveScript.saveData.hasGunsAbilitys[order].Length; i++)
        {
            int level = (int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[order][i]);

            if (!(level == 1 || level == 0))
            {
                GameObject data = Instantiate(ability, selects[menuIndex][listIndex].GetComponentsInChildren<GridLayoutGroup>()[1].transform);
                data.GetComponent<Image>().sprite = SaveScript.gunsAbilitys[i].image.sprite;
                data.GetComponentsInChildren<Text>()[0].text = SaveScript.gunsAbilitys[i].name + " [Lv." + (level - 1) + "]";
                if(i != 7)
                    data.GetComponentsInChildren<Text>()[1].text = SaveScript.gunsAbilitys[i].info + "\n" + SaveScript.gunsAbilitys[i].info2 + ((level - 1) * SaveScript.gunsAbilitys[i].data) + "% )";
                else
                    data.GetComponentsInChildren<Text>()[1].text = SaveScript.gunsAbilitys[i].info + "\n" + SaveScript.gunsAbilitys[i].info2 + ((level - 1) * SaveScript.gunsAbilitys[i].data) + "개 )";
            }
        }

        image.sprite = SaveScript.guns[order].UIImage.sprite;
        image.color = new Color(1, 1, 1, 1);
        texts[0].text = "< " + SaveScript.guns[order].name + ">";
        texts[1].text = (SaveScript.guns[order].damage * (1 + 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[order][0]) - 1))).ToString();
        if(order == 0)
            texts[2].text = "∞";
        else
            texts[2].text = SaveScript.saveData.hasGunsBullets[order] + " / " + Mathf.Round(SaveScript.guns[order].bulletNum * (1 + 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[order][2]) - 1)));
        texts[3].text = SaveScript.guns[order].shootDelayTime + " 초";

        if((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[order][1]) == 6)
            texts[4].text = "0 초";
        else
            texts[4].text = (SaveScript.guns[order].reloadingTime * (1 - 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[order][1]) - 1))) + " 초";
    }

    public void ArmorClick()
    {
        order = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;
        Text[] texts = selects[menuIndex][listIndex].GetComponentsInChildren<Text>();
        Image image = selects[menuIndex][listIndex].GetComponentInChildren<Image>();
        AbilityButton[] trashes = selects[menuIndex][listIndex].GetComponentsInChildren<GridLayoutGroup>()[1].GetComponentsInChildren<AbilityButton>();
        
        if(SaveScript.saveData.equipArmor != order)
        {
            armorEquipButton.SetActive(true);
            armorEquipButton.GetComponentInChildren<Button>().GetComponent<ItemOrder>().order = order;
        }
        else
        {
            armorEquipButton.SetActive(false);
        }

        for (int i = 0; i < trashes.Length; i++)
            Destroy(trashes[i].gameObject);

        for (int i = 0; i < SaveScript.saveData.hasArmorsAbilitys[order].Length; i++)
        {
            int level = (int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[order][i]);

            if (!(level == 1 || level == 0))
            {
                GameObject data = Instantiate(ability, selects[menuIndex][listIndex].GetComponentsInChildren<GridLayoutGroup>()[1].transform);
                data.GetComponent<Image>().sprite = SaveScript.armorsAbilitys[i].image.sprite;
                data.GetComponentsInChildren<Text>()[0].text = SaveScript.armorsAbilitys[i].name;
                data.GetComponentsInChildren<Text>()[1].text = SaveScript.armorsAbilitys[i].info;
            }
        }

        image.sprite = SaveScript.armors[order].image.sprite;
        image.color = new Color(1, 1, 1, 1);
        texts[0].text = "< " + SaveScript.armors[order].name + ">";
        texts[1].text = (SaveScript.armors[order].armor + (SaveScript.armorsAbilitys[1].data * 0.01f * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[order][1]) - 1))) * 100f + " %";
        texts[2].text = (SaveScript.armors[order].HPCure + (SaveScript.armorsAbilitys[2].data * 0.01f * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[order][2]) - 1))) + " / 1 초";
        texts[3].text = (SaveScript.armors[order].reflectDamage + (SaveScript.armorsAbilitys[3].data * 0.01f * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[order][3]) - 1))) * 100f + " %";
    }

    public void EquipArmorButton()
    {
        SaveScript.saveData.equipArmor = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;
        SettingSelect();
        ArmorClick();
    }

    public void ItemClick()
    {
        order = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;
        Text[] texts = selects[menuIndex][listIndex].GetComponentsInChildren<Text>();
        Image[] images = selects[menuIndex][listIndex].GetComponentsInChildren<Image>();

        images[0].sprite = SaveScript.etcs[order].image.sprite;
        images[0].color = new Color(1, 1, 1, 1);
        images[2].color = Item.colors[SaveScript.etcs[order].quallity];
        texts[0].text = SaveScript.etcs[order].name;
        texts[1].text = "x" + SaveScript.saveData.hasEtcs[order];
        texts[2].text = SaveScript.etcs[order].info;
    }
}

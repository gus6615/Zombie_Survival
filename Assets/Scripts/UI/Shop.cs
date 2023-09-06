using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Shop : MonoBehaviour
{
    [SerializeField] private RectTransform contentObject;
    [SerializeField] private GameObject contentPanel;
    [SerializeField] private GameObject shopSelector;
    [SerializeField] private Button[] shopButtons;
    [SerializeField] private GameObject infoObecjt;
    [SerializeField] private GameObject[] infos;
    [SerializeField] private GameObject ability;
    [SerializeField] private GameObject systemInfo;

    private Vector2 savedContentVec;
    private Vector2 goalContentVec;
    private List<GameObject> shopSelectors;
    static public List<int> etcCodes;
    private Color color;

    private Image info_image;
    private Text[] info_Texts;
    private GameObject info_Panel;
    private Text[] sell_Texts;
    private Button sell_Button;
    static public Slider sold_slider;
    static public Text sold_Text;
    static public Text[] sold_goldTexts;

    static public bool isBack;
    private bool isInit; // 제일 처음 UI효과 담당 변수
    private bool isNext; // content 이동에 따른 변수
    private bool isContentShow;
    private bool isShopButtonsShow;
    private bool isCreateContents;
    private int index;
    static public int order;

    void Start()
    {
        infoObecjt.SetActive(false);
        savedContentVec = contentObject.anchoredPosition;
        goalContentVec = Vector2.up * 1200f;
        contentObject.anchoredPosition = savedContentVec + goalContentVec;

        for (int i = 0; i < shopButtons.Length; i++)
            shopButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);

        isShopButtonsShow = true;
        isContentShow = true;
        index = 0;
    }

    private void Update()
    {
        if (isShopButtonsShow)
        {
            ShowButtons();
        }

        if (isContentShow)
        {
            ShowContent();
        }
    }

    public void ShowButtons()
    {
        for (int i = 0; i < shopButtons.Length; i++)
        {
            Color temp = shopButtons[i].GetComponent<Image>().color;
            temp.a += Time.deltaTime;
            shopButtons[i].GetComponent<Image>().color = temp;

            if (shopButtons[i].GetComponent<Image>().color.a <= 0.3f)
                break;
        }

        if (shopButtons[shopButtons.Length - 1].GetComponent<Image>().color.a >= 1f)
            isShopButtonsShow = false;
    }

    public void ShowContent()
    {
        if (!isInit) // 초반 content UI등장
        {
            if(Vector2.Distance(contentObject.anchoredPosition, savedContentVec) >= 0.5f)
                contentObject.anchoredPosition += (savedContentVec - contentObject.anchoredPosition) * Time.deltaTime * 3f;
            else
            {
                contentObject.anchoredPosition = savedContentVec;
                isInit = true;
                isContentShow = false;
            }
        }
        else // 상점 메뉴에 따른 content UI등장
        {
            if (!isNext)
            {
                if (Vector2.Distance(contentObject.anchoredPosition, savedContentVec + goalContentVec) >= 50f)
                    contentObject.anchoredPosition += ((savedContentVec + goalContentVec) - contentObject.anchoredPosition) * Time.deltaTime * 3f;
                else
                {
                    contentObject.anchoredPosition = savedContentVec + goalContentVec;
                    isNext = true;
                }
            }
            else
            {
                if (!isCreateContents)
                {
                    SetContents();
                    isCreateContents = true;
                }

                if (Vector2.Distance(contentObject.anchoredPosition, savedContentVec) >= 0.5f)
                    contentObject.anchoredPosition += (savedContentVec - contentObject.anchoredPosition) * Time.deltaTime * 3f;
                else
                {
                    contentObject.anchoredPosition = savedContentVec;
                    isNext = false;
                    isContentShow = false;
                    isCreateContents = false;
                }
            }
        }
    }

    public void BackShelter()
    {
        isBack = true;
    }

    public void OpenWeapon()
    {
        isContentShow = true;
        isNext = isCreateContents = false;
        infoObecjt.SetActive(false);
        index = 0;
    }

    public void OpenArmor()
    {
        isContentShow = true;
        isNext = isCreateContents = false;
        infoObecjt.SetActive(false);
        index = 1;
    }

    public void OpenAI()
    {
        isContentShow = true;
        isNext = isCreateContents = false;
        infoObecjt.SetActive(false);
        index = 2;
    }

    public void OpenSpecialAI()
    {
        isContentShow = true;
        isNext = isCreateContents = false;
        infoObecjt.SetActive(false);
        index = 3;
    }

    public void OpenSold()
    {
        isContentShow = true;
        isNext = isCreateContents = false;
        infoObecjt.SetActive(false);
        index = 4;
    }

    public void SetContents() 
    {
        if(shopSelectors != null)
        {
            for (int i = 0; i < shopSelectors.Count; i++)
                if(shopSelectors[i] != null)
                    Destroy(shopSelectors[i].gameObject);
            shopSelectors = null;
        }

        switch (index)
        {
            case 0:
                shopSelectors = new List<GameObject>();
                for (int i = 0; i < SaveScript.weaponNum - 1; i++)
                {
                    shopSelectors.Add(Instantiate(shopSelector, Vector3.zero, new Quaternion(0, 0, 0, 0), contentPanel.transform));
                    shopSelectors[i].GetComponentInChildren<Text>().text = SaveScript.guns[i + 1].name;
                    shopSelectors[i].GetComponent<ItemOrder>().order = i + 1;
                    shopSelectors[i].GetComponent<Button>().onClick.AddListener(ClickShopSelector);

                    if (SaveScript.saveData.hasGuns[i + 1])
                        shopSelectors[i].GetComponentsInChildren<Image>()[1].gameObject.SetActive(true);
                    else
                        shopSelectors[i].GetComponentsInChildren<Image>()[1].gameObject.SetActive(false);
                }
                break;
            case 1:
                shopSelectors = new List<GameObject>();
                for (int i = 0; i < SaveScript.armorNum; i++)
                {
                    shopSelectors.Add(Instantiate(shopSelector, Vector3.zero, new Quaternion(0, 0, 0, 0), contentPanel.transform));
                    shopSelectors[i].GetComponentInChildren<Text>().text = SaveScript.armors[i].name;
                    shopSelectors[i].GetComponent<ItemOrder>().order = i;
                    shopSelectors[i].GetComponent<Button>().onClick.AddListener(ClickShopSelector);

                    if (SaveScript.saveData.hasArmors[i])
                        shopSelectors[i].GetComponentsInChildren<Image>()[1].gameObject.SetActive(true);
                    else
                        shopSelectors[i].GetComponentsInChildren<Image>()[1].gameObject.SetActive(false);
                }
                break;
            case 2:
                shopSelectors = new List<GameObject>();
                for (int i = 0; i < SaveScript.AINum; i++)
                {
                    shopSelectors.Add(Instantiate(shopSelector, Vector3.zero, new Quaternion(0, 0, 0, 0), contentPanel.transform));
                    shopSelectors[i].GetComponentInChildren<Text>().text = SaveScript.hasAI[i].name;
                    shopSelectors[i].GetComponent<ItemOrder>().order = i;
                    shopSelectors[i].GetComponent<Button>().onClick.AddListener(ClickShopSelector);

                    if (SaveScript.saveData.hasAI[i])
                        shopSelectors[i].GetComponentsInChildren<Image>()[1].gameObject.SetActive(true);
                    else
                        shopSelectors[i].GetComponentsInChildren<Image>()[1].gameObject.SetActive(false);
                }
                break;
            case 3:
                shopSelectors = new List<GameObject>();
                for (int i = 0; i < SaveScript.specialAINum; i++)
                {
                    shopSelectors.Add(Instantiate(shopSelector, Vector3.zero, new Quaternion(0, 0, 0, 0), contentPanel.transform));
                    shopSelectors[i].GetComponentInChildren<Text>().text = SaveScript.hasSpecialAI[i].name;
                    shopSelectors[i].GetComponent<ItemOrder>().order = i;
                    shopSelectors[i].GetComponent<Button>().onClick.AddListener(ClickShopSelector);

                    if (SaveScript.saveData.hasSpecialAI[i])
                        shopSelectors[i].GetComponentsInChildren<Image>()[1].gameObject.SetActive(true);
                    else
                        shopSelectors[i].GetComponentsInChildren<Image>()[1].gameObject.SetActive(false);
                }
                break;
            case 4:
                shopSelectors = new List<GameObject>();
                etcCodes = new List<int>();
                int count = 0;
                for (int j = 0; j < 5; j++) // 등급 별 구분
                {       
                    for (int i = 0; i < SaveScript.saveData.hasEtcs.Count; i++)
                    {
                        if (SaveScript.saveData.hasEtcs[i] != 0 && SaveScript.etcs[i].quallity == j)
                        {
                            shopSelectors.Add(Instantiate(shopSelector, Vector3.zero, new Quaternion(0, 0, 0, 0), contentPanel.transform));
                            etcCodes.Add(SaveScript.etcs[i].itemCode);
                            shopSelectors[count].GetComponentInChildren<Text>().text = SaveScript.etcs[i].name;
                            shopSelectors[count].GetComponentInChildren<Text>().color = Item.colors[SaveScript.etcs[i].quallity];
                            shopSelectors[count].GetComponent<ItemOrder>().order = count;
                            shopSelectors[count].GetComponentsInChildren<Image>()[1].gameObject.SetActive(false);
                            shopSelectors[count++].GetComponent<Button>().onClick.AddListener(ClickShopSelector);
                        }
                    }
                }
                break;
        }
    }

    public void ClickShopSelector()
    {
        order = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;
        for (int i = 0; i < shopSelectors.Count; i++)
        {
            if(shopSelectors[i] != null)
            {
                if (i == order)
                    shopSelectors[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                else
                    shopSelectors[i].GetComponent<Image>().color = new Color(1, 1, 1, 1f);
            }
        }
        infoObecjt.SetActive(true);
        SetInfo();
    }

    public void SetInfo()
    {
        for (int i = 0; i < infos.Length; i++)
        {
            if (i == index)
                infos[i].SetActive(true);
            else
                infos[i].SetActive(false);
        }

        RectTransform[] temp = infos[index].GetComponentsInChildren<RectTransform>();
        Text skillInfo_text = null; // 특수 용병 전용 변수

        if (index == 0 || index == 1)
        {
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].gameObject.name == "Image")
                    info_image = temp[i].gameObject.GetComponent<Image>();
                else if (temp[i].gameObject.name == "Stat")
                    info_Texts = temp[i].gameObject.GetComponentsInChildren<Text>();
                else if (temp[i].gameObject.name == "Panel")
                    info_Panel = temp[i].gameObject;
                else if (temp[i].gameObject.name == "Sell")
                {
                    sell_Texts = temp[i].gameObject.GetComponentsInChildren<Text>();
                    sell_Button = temp[i].gameObject.GetComponentInChildren<Button>();
                }
            }
        }
        else if(index == 2)
        {
            SpriteRenderer[] statObjects = null;

            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].gameObject.name == "Image")
                    info_image = temp[i].gameObject.GetComponent<Image>();
                else if (temp[i].gameObject.name == "Stat")
                    statObjects = temp[i].gameObject.GetComponentsInChildren<SpriteRenderer>();
                else if (temp[i].gameObject.name == "Panel")
                    info_Panel = temp[i].gameObject;
                else if (temp[i].gameObject.name == "Sell")
                {
                    sell_Texts = temp[i].gameObject.GetComponentsInChildren<Text>();
                    sell_Button = temp[i].gameObject.GetComponentInChildren<Button>();
                }
            }

            for (int i = 0; i < statObjects.Length; i++)
                statObjects[i].gameObject.SetActive(false);
            statObjects[SaveScript.hasAI[order].type].gameObject.SetActive(true);
            info_Texts = statObjects[SaveScript.hasAI[order].type].GetComponentsInChildren<Text>();

            AbilityButton[] deleteAbility = info_Panel.GetComponentsInChildren<AbilityButton>();

            for (int i = 0; i < deleteAbility.Length; i++)
                Destroy(deleteAbility[i].gameObject);
        }
        else if (index == 3)
        {
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].gameObject.name == "Image")
                    info_image = temp[i].gameObject.GetComponent<Image>();
                else if (temp[i].gameObject.name == "Stat")
                    info_Texts = temp[i].gameObject.GetComponentsInChildren<Text>();
                else if (temp[i].gameObject.name == "Panel")
                    info_Panel = temp[i].gameObject;
                else if (temp[i].gameObject.name == "Sell")
                {
                    sell_Texts = temp[i].gameObject.GetComponentsInChildren<Text>();
                    sell_Button = temp[i].gameObject.GetComponentInChildren<Button>();
                }
                else if (temp[i].gameObject.name == "AbilityInfo")
                    skillInfo_text = temp[i].gameObject.GetComponent<Text>();
            }
        }
        else
        {
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].gameObject.name == "Image")
                    info_image = temp[i].gameObject.GetComponent<Image>();
                else if (temp[i].gameObject.name == "Stat")
                    info_Texts = temp[i].gameObject.GetComponentsInChildren<Text>();
                else if (temp[i].gameObject.name == "Slider")
                    sold_slider = temp[i].gameObject.GetComponent<Slider>();
                else if (temp[i].gameObject.name == "SoldNumText")
                    sold_Text = temp[i].gameObject.GetComponent<Text>();
                else if (temp[i].gameObject.name == "GoldObject")
                    sold_goldTexts = temp[i].gameObject.GetComponentsInChildren<Text>();
            }
        }

        switch (index)
        {
            case 0: // 무기
                info_image.sprite = SaveScript.guns[order].UIImage.sprite;
                info_Texts[0].text = SaveScript.guns[order].name;
                info_Texts[1].text = "공격력 : " + SaveScript.guns[order].damage * (1 + 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[order][0]) - 1));
                info_Texts[2].text = "탄창 : " + Mathf.Round(SaveScript.guns[order].bulletNum * (1 + 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[order][2]) - 1))) + " 발";
                info_Texts[3].text = "공격속도 : " + SaveScript.guns[order].shootDelayTime + "초";
                if((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[order][1]) == 6)
                    info_Texts[4].text = "재장전속도 : 0초";
                else
                    info_Texts[4].text = "재장전속도 : " + (SaveScript.guns[order].reloadingTime * (1 - 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[order][1]) - 1))) + "초";

                if (SaveScript.saveData.hasGuns[order])
                {
                    sell_Texts[0].text = "[탄창 구매]";
                    sell_Texts[1].text = (SaveScript.guns[order].price * 0.2f).ToString();
                    sell_Texts[2].text = "(" + SaveScript.saveData.gold + " 원)";
                    if (SaveScript.saveData.gold >= SaveScript.guns[order].price * 0.2f)
                        sell_Texts[2].color = Color.green;
                    else
                        sell_Texts[2].color = Color.red;

                    sell_Texts[3].text = (SaveScript.guns[order].ironBarNum * 0.2f).ToString();
                    sell_Texts[4].text = "(" + SaveScript.saveData.hasEtcs[0] + ")";
                    if (SaveScript.saveData.hasEtcs[0] >= SaveScript.guns[order].ironBarNum * 0.2f)
                        sell_Texts[4].color = Color.green;
                    else
                        sell_Texts[4].color = Color.red;

                    sell_Texts[5].color = Color.green;
                    sell_Texts[5].text = "총알[" + SaveScript.saveData.hasGunsBullets[order].ToString() + "발] 소유";
                }
                else
                {
                    sell_Texts[0].text = "[총기 구매]";
                    sell_Texts[1].text = SaveScript.guns[order].price.ToString();
                    sell_Texts[2].text = "(" + SaveScript.saveData.gold + " 원)";
                    if (SaveScript.saveData.gold >= SaveScript.guns[order].price)
                        sell_Texts[2].color = Color.green;
                    else
                        sell_Texts[2].color = Color.red;

                    sell_Texts[3].text = SaveScript.guns[order].ironBarNum.ToString();
                    sell_Texts[4].text = "(" + SaveScript.saveData.hasEtcs[0] + ")";
                    if (SaveScript.saveData.hasEtcs[0] >= SaveScript.guns[order].ironBarNum)
                        sell_Texts[4].color = Color.green;
                    else
                        sell_Texts[4].color = Color.red;

                    sell_Texts[5].color = new Color(0, 0, 0, 0);
                    sell_Texts[5].text = "";
                }
                break;
            case 1: // 방어구
                info_image.sprite = SaveScript.armors[order].image.sprite;
                info_Texts[0].text = SaveScript.armors[order].name;
                info_Texts[1].text = "방어력 : " + (SaveScript.armors[order].armor + (SaveScript.armorsAbilitys[1].data * 0.01f * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[order][1]) - 1))) * 100f + " %";
                info_Texts[2].text = "치유량 : " + (SaveScript.armors[order].HPCure + (SaveScript.armorsAbilitys[2].data * 0.01f * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[order][2]) - 1))) + " / 1 초";
                info_Texts[3].text = "공격반사량 : " + (SaveScript.armors[order].reflectDamage + (SaveScript.armorsAbilitys[3].data * 0.01f * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[order][3]) - 1))) * 100f + " %";

                if (SaveScript.saveData.hasArmors[order])
                {
                    sell_Texts[0].text = "[이미 소유한 방어구입니다.]";
                    sell_Texts[1].text = "0";
                    sell_Texts[2].text = "";
                    sell_Texts[3].text = "0";
                    sell_Texts[4].text = "";
                    sell_Texts[5].text = "0";
                    sell_Texts[6].text = "";
                    sell_Button.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                }
                else
                {
                    sell_Texts[0].text = "[방어구 구매]";

                    sell_Texts[1].text = SaveScript.armors[order].price.ToString();
                    sell_Texts[2].text = "(" + SaveScript.saveData.gold + " 원)";
                    if (SaveScript.saveData.gold >= SaveScript.armors[order].price)
                        sell_Texts[2].color = Color.green;
                    else
                        sell_Texts[2].color = Color.red;

                    sell_Texts[3].text = SaveScript.armors[order].ironNum.ToString();
                    sell_Texts[4].text = "(" + SaveScript.saveData.hasEtcs[0] + ")";
                    if (SaveScript.saveData.hasEtcs[0] >= SaveScript.armors[order].ironNum)
                        sell_Texts[4].color = Color.green;
                    else
                        sell_Texts[4].color = Color.red;

                    sell_Texts[5].text = SaveScript.armors[order].clothNum.ToString();
                    sell_Texts[6].text = "(" + SaveScript.saveData.hasEtcs[1] + ")";
                    if (SaveScript.saveData.hasEtcs[1] >= SaveScript.armors[order].clothNum)
                        sell_Texts[6].color = Color.green;
                    else
                        sell_Texts[6].color = Color.red;

                    sell_Button.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
                break;
            case 2: // 일반용병
                info_image.sprite = SaveScript.hasAI[order].shop_image.sprite;
                info_Texts[0].text = "이름 : " + SaveScript.hasAI[order].name;
                info_Texts[2].text = "무기 종류 : " + SaveScript.hasAI[order].weaponName;
                info_Texts[3].text = "공격력 : " + (SaveScript.hasAI[order] as ShotAI).damage * (1 + SaveScript.hasAI[order].level * 0.2f);
                info_Texts[4].text = "탄창 수 : " + (SaveScript.hasAI[order] as ShotAI).bulletNum;
                info_Texts[5].text = "채집력 : " + SaveScript.hasAI[order].workPorce + SaveScript.hasAI[order].level * 2;

                for (int i = 0; i < SaveScript.hasAI[order].abilities.Length; i++)
                {
                    GameObject abilityTemp = Instantiate(ability, info_Panel.transform);
                    abilityTemp.GetComponent<Image>().sprite = SaveScript.hasAI[order].abilities[i].image.sprite;
                    abilityTemp.GetComponentsInChildren<Text>()[0].text = SaveScript.hasAI[order].abilities[i].name;
                    abilityTemp.GetComponentsInChildren<Text>()[1].text = SaveScript.hasAI[order].abilities[i].info;
                }

                if (SaveScript.saveData.hasAI[order])
                {
                    sell_Texts[0].text = "[이미 소유한 용병입니다.]";
                    sell_Texts[1].text = "0";
                    sell_Texts[2].text = "";
                    sell_Texts[3].text = "0";
                    sell_Texts[4].text = "";
                    sell_Button.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                }
                else
                {
                    sell_Texts[0].text = "[용병 구매]";

                    sell_Texts[1].text = SaveScript.hasAI[order].price.ToString();
                    sell_Texts[2].text = "(" + SaveScript.saveData.gold + " 원)";
                    if (SaveScript.saveData.gold >= SaveScript.hasAI[order].price)
                        sell_Texts[2].color = Color.green;
                    else
                        sell_Texts[2].color = Color.red;

                    sell_Texts[3].text = SaveScript.hasAI[order].redJam.ToString();
                    sell_Texts[4].text = "(" + SaveScript.saveData.hasEtcs[5] + ")";
                    if (SaveScript.saveData.hasEtcs[5] >= SaveScript.hasAI[order].redJam)
                        sell_Texts[4].color = Color.green;
                    else
                        sell_Texts[4].color = Color.red;

                    sell_Button.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
                break;
            case 3: // 특수 용병 
                info_image.sprite = SaveScript.hasSpecialAI[order].shop_image.sprite;
                info_Texts[0].text = "이름 : " + SaveScript.hasSpecialAI[order].name;
                info_Texts[1].text = SaveScript.hasSpecialAI[order].typeInfo;
                info_Texts[2].text = "공격력 : " + SaveScript.hasSpecialAI[order].damage * (1 + SaveScript.hasSpecialAI[order].level * 0.1f);
                info_Texts[3].text = "채집력 : " + SaveScript.hasSpecialAI[order].workPorce + SaveScript.hasSpecialAI[order].level * 2;
                skillInfo_text.text = SaveScript.hasSpecialAI[order].skillInfo;

                if (SaveScript.saveData.hasSpecialAI[order])
                {
                    sell_Texts[0].text = "[이미 소유한 용병입니다.]";
                    sell_Texts[1].text = "0";
                    sell_Texts[2].text = "";
                    sell_Texts[3].text = "0";
                    sell_Texts[4].text = "";
                    sell_Texts[5].text = "0";
                    sell_Texts[6].text = "";
                    sell_Button.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                }
                else
                {
                    sell_Texts[0].text = "[용병 구매]";

                    sell_Texts[1].text = SaveScript.hasSpecialAI[order].price.ToString();
                    sell_Texts[2].text = "(" + SaveScript.saveData.gold + " 원)";
                    if (SaveScript.saveData.gold >= SaveScript.hasSpecialAI[order].price)
                        sell_Texts[2].color = Color.green;
                    else
                        sell_Texts[2].color = Color.red;

                    sell_Texts[3].text = SaveScript.hasSpecialAI[order].redJam.ToString();
                    sell_Texts[4].text = "(" + SaveScript.saveData.hasEtcs[5] + ")";
                    if (SaveScript.saveData.hasEtcs[5] >= SaveScript.hasSpecialAI[order].redJam)
                        sell_Texts[4].color = Color.green;
                    else
                        sell_Texts[4].color = Color.red;

                    sell_Texts[5].text = SaveScript.hasSpecialAI[order].blueJam.ToString();
                    sell_Texts[6].text = "(" + SaveScript.saveData.hasEtcs[6] + ")";
                    if (SaveScript.saveData.hasEtcs[6] >= SaveScript.hasSpecialAI[order].blueJam)
                        sell_Texts[6].color = Color.green;
                    else
                        sell_Texts[6].color = Color.red;

                    sell_Button.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
                break;
            case 4: // 팔기
                info_image.sprite = SaveScript.etcs[etcCodes[order] - SaveScript.weaponNum - SaveScript.armorNum - 1].UIImage.sprite;
                info_Texts[0].text = SaveScript.etcs[etcCodes[order] - SaveScript.weaponNum - SaveScript.armorNum - 1].name;
                info_Texts[0].color = Item.colors[SaveScript.etcs[etcCodes[order] - SaveScript.weaponNum - SaveScript.armorNum - 1].quallity];
                info_Texts[1].text = "보유 수 : " + SaveScript.saveData.hasEtcs[etcCodes[order] - SaveScript.weaponNum - SaveScript.armorNum - 1] + " 개";
                info_Texts[2].text = "가격 : " + SaveScript.etcs[etcCodes[order] - SaveScript.weaponNum - SaveScript.armorNum - 1].price;
                sold_slider.value = 0;
                sold_slider.maxValue = SaveScript.saveData.hasEtcs[etcCodes[order] - SaveScript.weaponNum - SaveScript.armorNum - 1];

                SoldSlider.soldNum = 0;
                sold_Text.text = 0.ToString();
                sold_goldTexts[0].text = sold_goldTexts[1].text = SaveScript.saveData.gold + " 원";
                break;
        }
    }

    public void SellContent()
    {
        GameObject system;

        switch (index)
        {
            case 0:
                system = Instantiate(systemInfo, this.transform);
                if (SaveScript.saveData.hasGuns[order]) // 탄창 구매
                {
                    if(SaveScript.saveData.gold >= (int)(SaveScript.guns[order].price * 0.2f) && SaveScript.saveData.hasEtcs[0] >= (int)(SaveScript.guns[order].ironBarNum * 0.2f)) // 구매
                    {
                        int bulletNum = (int)Mathf.Round(SaveScript.guns[order].bulletNum * (1 + 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[order][2]) - 1)));

                        SaveScript.saveData.gold -= (int)(SaveScript.guns[order].price * 0.2f);
                        SaveScript.saveData.hasEtcs[0] -= (int)(SaveScript.guns[order].ironBarNum * 0.2f);
                        SaveScript.saveData.hasGunsBullets[order] += bulletNum;
                        
                        system.GetComponentInChildren<Text>().text = "탄창(" + bulletNum + "발) 구매 완료!" + "\n" + "[총 " + SaveScript.saveData.hasGunsBullets[order] + " 발]";
                        SetInfo();
                    }
                    else
                        system.GetComponentInChildren<Text>().text = "재고가 부족합니다!";
                }
                else // 총기 구매
                {
                    if (SaveScript.saveData.gold >= SaveScript.guns[order].price && SaveScript.saveData.hasEtcs[0] >= SaveScript.guns[order].ironBarNum) // 구매
                    {
                        SaveScript.saveData.gold -= SaveScript.guns[order].price;
                        SaveScript.saveData.hasEtcs[0] -= SaveScript.guns[order].ironBarNum;
                        SaveScript.saveData.hasGunsBullets[order] = SaveScript.guns[order].bulletNum * 5;
                        SaveScript.saveData.hasGuns[order] = true;

                        system.GetComponentInChildren<Text>().text = "총기 구매 완료!" + "\n" + "[총 " + SaveScript.saveData.hasGunsBullets[order] + " 발]";
                        SetContents();
                        SetInfo();
                    }
                    else
                        system.GetComponentInChildren<Text>().text = "재고가 부족합니다!";
                }
                break;
            case 1:
                if (!SaveScript.saveData.hasArmors[order])
                {
                    system = Instantiate(systemInfo, infoObecjt.transform);

                    if (SaveScript.saveData.gold >= SaveScript.armors[order].price && SaveScript.saveData.hasEtcs[0] >= SaveScript.armors[order].ironNum
                        && SaveScript.saveData.hasEtcs[1] >= SaveScript.armors[order].clothNum) // 구매
                    {
                        SaveScript.saveData.gold -= SaveScript.armors[order].price;
                        SaveScript.saveData.hasEtcs[0] -= SaveScript.armors[order].ironNum;
                        SaveScript.saveData.hasEtcs[1] -= SaveScript.armors[order].clothNum;

                        system.GetComponentInChildren<Text>().text = "방어구 구매 완료!";
                        SetContents();
                        SetInfo();
                    }
                    else
                        system.GetComponentInChildren<Text>().text = "재고가 부족합니다!";
                }
                break;
            case 2: // 일반용병 구매
                if (!SaveScript.saveData.hasAI[order])
                {
                    system = Instantiate(systemInfo, infoObecjt.transform);

                    if (SaveScript.saveData.gold >= SaveScript.hasAI[order].price && SaveScript.saveData.hasEtcs[5] >= SaveScript.hasAI[order].redJam) // 구매
                    {
                        SaveScript.saveData.gold -= SaveScript.hasAI[order].price;
                        SaveScript.saveData.hasEtcs[5] -= SaveScript.hasAI[order].redJam;

                        system.GetComponentInChildren<Text>().text = "용병 구매 완료!";
                        SetContents();
                        SetInfo();
                    }
                    else
                        system.GetComponentInChildren<Text>().text = "재고가 부족합니다!";
                }
                break;
            case 3: // 특수용병 구매
                if (!SaveScript.saveData.hasSpecialAI[order])
                {
                    system = Instantiate(systemInfo, infoObecjt.transform);

                    if (SaveScript.saveData.gold >= SaveScript.hasSpecialAI[order].price && SaveScript.saveData.hasEtcs[5] >= SaveScript.hasSpecialAI[order].redJam
                        && SaveScript.saveData.hasEtcs[6] >= SaveScript.hasSpecialAI[order].blueJam) // 구매
                    {
                        SaveScript.saveData.gold -= SaveScript.hasSpecialAI[order].price;
                        SaveScript.saveData.hasEtcs[5] -= SaveScript.hasSpecialAI[order].redJam;
                        SaveScript.saveData.hasEtcs[6] -= SaveScript.hasSpecialAI[order].blueJam;

                        system.GetComponentInChildren<Text>().text = "용병 구매 완료!";
                        SetContents();
                        SetInfo();
                    }
                    else
                        system.GetComponentInChildren<Text>().text = "재고가 부족합니다!";
                }
                break;
        }
    }

    public void SoldContent()
    {
        SaveScript.saveData.hasEtcs[etcCodes[order] - SaveScript.weaponNum - SaveScript.armorNum - 1] -= SoldSlider.soldNum;
        SaveScript.saveData.gold += SaveScript.etcs[etcCodes[order] - SaveScript.weaponNum - SaveScript.armorNum - 1].price * SoldSlider.soldNum;
        if(SaveScript.saveData.hasEtcs[etcCodes[order] - SaveScript.weaponNum - SaveScript.armorNum - 1] == 0)
            infoObecjt.SetActive(false);

        SetContents();
        SetInfo();
        GameObject system = Instantiate(systemInfo, this.transform);
        system.GetComponentInChildren<Text>().text = "판매 완료! 현재 골드는 '" + SaveScript.saveData.gold + " 원' 입니다.";
    }
}

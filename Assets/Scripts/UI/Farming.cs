using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Farming : MonoBehaviour
{
    [SerializeField] private GameObject buttonObject;
    private Button[] buttons;
    [SerializeField] private GameObject backgroundImageObject;
    private Image[] backgroundImages;
    [SerializeField] private GameObject allStageInfoObject;
    private Image[] allStageInfoImages;
    [SerializeField] private GameObject currentInfoObject;
    private Text[] cur_texts;
    [SerializeField] private GameObject noOpenImage;
    private Text noOpenInfoText;
    [SerializeField] private GameObject teamPanel;
    [SerializeField] private GameObject AISelector;
    [SerializeField] private Text teamInfo;
    [SerializeField] private GameObject AIselectorPrefab;
    [SerializeField] private GameObject SlotContent;
    [SerializeField] private GameObject SelectContent;
    [SerializeField] private GameObject noAIText;

    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private GameObject reward_contentPanel;

    [SerializeField] private GameObject playerFarming, autoFarming;
    [SerializeField] private Button[] pageButtons;
    [SerializeField] private GameObject goldInfo;
    static public Text goldText;

    // PlayerFarming 변수들
    [SerializeField] private GameObject touchObject;
    private SpriteRenderer[] touchObjectStages;
    private Image[][] touchObjectImages;
    private Image[] currentTouchObjectImage;
    static public Text[] dropInfoText;
    static public Slider objectHpSlider;

    static public bool isInit; // static 데이터 초기화 하였는가?
    static public int[] stage_totalWorkSize; // 각 스테이지의 고유한 총 수집력
    static public int[][] stage_golds; // 각 스테이지의 보상 골드
    static public int[] stage_levels; // 각 스테이지의 오픈 레벨
    static public string[] stage_names; // 각 스테이지의 이름
    static public string[] stage_infos; // 각 스테이지의 보상정보
    static public bool[] stage_isOpens; // 각 스테이지의 오픈 여부
    static public bool isBack;
    static public bool isFarming;
    static public bool isCancle;
    static public bool isRecreate; // Player Farming에서 오브젝트를 파괴한 경우
    private bool isPlayerFarming;

    static public int index; // 현재 스테이지의 위치 인덱스
    public int slotIndex; // 1 ~ 6번 슬롯의 인덱스
    public int[] AItypes; // 0 = hasAI, 1 = hasStoryAI, 2 = hasSpecialAI
    static private int infoTextCount; // info 텍스트 현재 출력된 위치

    // Start is called before the first frame update
    void Start()
    {
        buttons = buttonObject.GetComponentsInChildren<Button>();
        backgroundImages = backgroundImageObject.GetComponentsInChildren<Image>();
        allStageInfoImages = allStageInfoObject.GetComponentsInChildren<Image>();
        cur_texts = currentInfoObject.GetComponentsInChildren<Text>();
        noOpenInfoText = noOpenImage.GetComponentInChildren<Text>();
        goldText = goldInfo.GetComponentInChildren<Text>();

        dropInfoText = playerFarming.GetComponentsInChildren<Text>();
        objectHpSlider = playerFarming.GetComponentInChildren<Slider>();
        touchObjectStages = touchObject.GetComponentsInChildren<SpriteRenderer>();
        touchObjectImages = new Image[SaveScript.saveData.Farming_stages.Length][];
        for (int i = 0; i < SaveScript.saveData.Farming_stages.Length; i++)
            touchObjectImages[i] = touchObjectStages[i].GetComponentsInChildren<Image>();

        currentTouchObjectImage = new Image[SaveScript.saveData.Farming_stages.Length];
        for (int i = 0; i < currentTouchObjectImage.Length; i++)
            currentTouchObjectImage[i] = touchObjectImages[i][Random.Range(0, touchObjectImages[i].Length)];

        for (int i = 0; i < touchObjectStages.Length; i++)
            touchObjectStages[i].gameObject.SetActive(false);
        touchObjectStages[index].gameObject.SetActive(true);

        for (int i = 0; i < touchObjectImages.Length; i++)
            for (int j = 0; j < touchObjectImages[i].Length; j++)
                touchObjectImages[i][j].gameObject.SetActive(false);

        currentTouchObjectImage[index].gameObject.SetActive(true);

        rewardPanel.SetActive(false);
        teamPanel.SetActive(false);
        playerFarming.SetActive(true);
        autoFarming.SetActive(false);
        isPlayerFarming = true;

        for (int i = 0; i < dropInfoText.Length; i++)
            dropInfoText[i].text = "";
        
        infoTextCount = 0;
        goldText.text = SaveScript.saveData.gold + "원";

        if (!isInit)
            Init();

        OpenStagePerLevel();
        SetCurrentStage();
        SetTimeInfo();
        SetTeamInfo();
        SetRewardInfo();
    }

    private void Update()
    {
        if (isRecreate)
            StartCoroutine("RecreateObject");
    }

    static public void SetInfoText(string data, Color color)
    {
        if(infoTextCount < 5)
        {
            dropInfoText[infoTextCount].text = data;
            dropInfoText[infoTextCount].color = color;

            infoTextCount++;
        }
        else
        {
            for (int i = 0; i < infoTextCount; i++)
            {
                if (i != 4)
                {
                    dropInfoText[i].text = dropInfoText[i + 1].text;
                    dropInfoText[i].color = dropInfoText[i + 1].color;
                }
                else
                {
                    dropInfoText[i].text = data;
                    dropInfoText[i].color = color;
                }
            }
        }
    }

    IEnumerator RecreateObject()
    {
        isRecreate = false;

        yield return new WaitForSeconds(0.25f);

        currentTouchObjectImage[index] = touchObjectImages[index][Random.Range(0, touchObjectImages[index].Length)];
        SetCurrentStage();
    }

    static public void Init()
    {
        isInit = true;

        stage_totalWorkSize = new int[SaveScript.saveData.Farming_stages.Length];
        stage_totalWorkSize[0] = 10000;
        stage_totalWorkSize[1] = 50000;
        stage_totalWorkSize[2] = 100000;
        stage_totalWorkSize[3] = 300000;
        stage_totalWorkSize[4] = 1000000;

        stage_golds = new int[SaveScript.saveData.Farming_stages.Length][];
        stage_golds[0] = new int[2];
        stage_golds[0][0] = 500;
        stage_golds[0][1] = 1000;
        stage_golds[1] = new int[2];
        stage_golds[1][0] = 1000;
        stage_golds[1][1] = 5000;
        stage_golds[2] = new int[2];
        stage_golds[2][0] = 5000;
        stage_golds[2][1] = 15000;
        stage_golds[3] = new int[2];
        stage_golds[3][0] = 15000;
        stage_golds[3][1] = 35000;
        stage_golds[4] = new int[2];
        stage_golds[4][0] = 35000;
        stage_golds[4][1] = 100000;

        stage_levels = new int[SaveScript.saveData.Farming_stages.Length];
        stage_levels[0] = 5;
        stage_levels[1] = 15;
        stage_levels[2] = 30;
        stage_levels[3] = 50;
        stage_levels[4] = 80;

        stage_names = new string[SaveScript.saveData.Farming_stages.Length];
        stage_names[0] = "낡은 오두막";
        stage_names[1] = "편의점";
        stage_names[2] = "중형 마트";
        stage_names[3] = "백화점";
        stage_names[4] = "군시설";

        stage_infos = new string[SaveScript.saveData.Farming_stages.Length];
        stage_infos[0] = "골드(" + stage_golds[0][0] + " ~ " + stage_golds[0][1] + "), 노멀 아이템";
        stage_infos[1] = "골드(" + stage_golds[1][0] + " ~ " + stage_golds[1][1] + "), 노멀 ~ 레어 아이템";
        stage_infos[2] = "골드(" + stage_golds[2][0] + " ~ " + stage_golds[2][1] + "), 노멀 ~ 에픽 아이템";
        stage_infos[3] = "골드(" + stage_golds[3][0] + " ~ " + stage_golds[3][1] + "), 노멀 ~ 유니크 아이템";
        stage_infos[4] = "골드(" + stage_golds[4][0] + " ~ " + stage_golds[4][1] + "), 노멀 ~ 레전드리 아이템";

        stage_isOpens = new bool[SaveScript.saveData.Farming_stages.Length];
    } 

    public void BackShelter()
    {
        isBack = true;
    }

    public void SetIsFarming()
    {
        for (int i = 0; i < SaveScript.saveData.Farming_stages.Length; i++)
        {
            if (SaveScript.saveData.Farming_stages[i])
            {
                isFarming = true;
                break;
            }
            else
                isFarming = false;
        }
    }

    public void SetTeamInfo() // 팀의 모든 텍스트 및 이미지 정보 초기화 및 수정
    {
        // 슬롯 이미지 설정
        ItemOrder[] order = SlotContent.GetComponentsInChildren<ItemOrder>();
        for (int i = 0; i < 6; i++)
        {
            order[i].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 0);
            order[i].GetComponentsInChildren<Image>()[2].color = new Color(1, 1, 1, 0);
            order[i].GetComponentsInChildren<Image>()[3].color = new Color(0, 0, 0, 0);
            order[i].GetComponentInChildren<Text>().text = "";

            if (SaveScript.Farming_AIs[index][i] != null)
            // 해당 슬롯에 AI가 존재한다.
            {
                order[i].GetComponentsInChildren<Image>()[3].color = new Color(1, 1, 1, 1);
                order[i].GetComponentsInChildren<Image>()[3].sprite = SaveScript.Farming_AIs[index][i].shop_image.sprite;
                order[i].GetComponentInChildren<Text>().text = "채집력 : " + (SaveScript.Farming_AIs[index][i].workPorce + SaveScript.Farming_AIs[index][i].level * 2);
            }
            else
            {
                order[i].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 1);
                order[i].GetComponentInChildren<Text>().text = "";
            }
        }

        // 팀 배치도 수정
        teamInfo.text = "인원 : " + SaveScript.saveData.Farming_totalNum[index] + " / 6,   총 채집력 : " + SaveScript.saveData.Farming_totalForce[index];
    }

    public void SetTimeInfo() // 스테이지의 진행도를 표시
    {
        CancelInvoke("ResetInfo");

        for (int i = 0; i < SaveScript.saveData.Farming_stages.Length; i++)
        {
            if (SaveScript.saveData.Farming_stages[i] != false)
            {
                InvokeRepeating("ResetInfo", 1f, 1f);
                break;
            }
        }

        if (SaveScript.saveData.Farming_currentSize[index] < stage_totalWorkSize[index])
        {
            buttons[3].gameObject.SetActive(false);

            if (!SaveScript.saveData.Farming_stages[index])
                ShowNonInfo();
            else
            {
                int dataTime = (stage_totalWorkSize[index] - SaveScript.saveData.Farming_currentSize[index]) / SaveScript.saveData.Farming_totalForce[index];
                int emptyTime_h = dataTime / 3600;
                int emptyTime_m = dataTime / 60 - emptyTime_h * 60;
                int emptyTime_s = dataTime % 60;

                if (emptyTime_h != 0)
                    cur_texts[5].text = "남은 시간 : " + emptyTime_h + "시 " + emptyTime_m + "분 " + emptyTime_s + "초";
                else if (emptyTime_m != 0)
                    cur_texts[5].text = "남은 시간 : " + emptyTime_m + "분 " + emptyTime_s + "초";
                else
                    cur_texts[5].text = "남은 시간 : " + emptyTime_s + "초";

                cur_texts[6].text = "진행 상황 : " + SaveScript.saveData.Farming_currentSize[index] + " / " + stage_totalWorkSize[index];
                cur_texts[7].text = "인원 : " + SaveScript.saveData.Farming_totalNum[index] + " / 6,   총 채집력 : " + SaveScript.saveData.Farming_totalForce[index];
            }
        }
        else
        {
            cur_texts[5].text = "[ 채집 완료 ]";
            cur_texts[6].text = "진행 상황 : " + stage_totalWorkSize[index] + " / " + stage_totalWorkSize[index];
            cur_texts[7].text = "인원 : " + SaveScript.saveData.Farming_totalNum[index] + " / 6,   총 채집력 : " + SaveScript.saveData.Farming_totalForce[index];

            buttons[3].gameObject.SetActive(true);
        }
    }

    public void ResetInfo() // 파밍 상태 정보 갱신 및 호출
    {
        SetRewardInfo();
        if (SaveScript.saveData.Farming_stages[index])
        {
            if (stage_totalWorkSize[index] > SaveScript.saveData.Farming_currentSize[index])
            {
                // 정보 계산
                int dataTime = (stage_totalWorkSize[index] - SaveScript.saveData.Farming_currentSize[index]) / SaveScript.saveData.Farming_totalForce[index];
                int emptyTime_h = dataTime / 3600;
                int emptyTime_m = dataTime / 60 - emptyTime_h * 60;
                int emptyTime_s = dataTime % 60;

                if (emptyTime_h != 0)
                    cur_texts[5].text = "남은 시간 : " + emptyTime_h + "시 " + emptyTime_m + "분 " + emptyTime_s + "초";
                else if (emptyTime_m != 0)
                    cur_texts[5].text = "남은 시간 : " + emptyTime_m + "분 " + emptyTime_s + "초";
                else
                    cur_texts[5].text = "남은 시간 : " + emptyTime_s + "초";

                cur_texts[6].text = "진행 상황 : " + SaveScript.saveData.Farming_currentSize[index] + " / " + stage_totalWorkSize[index];
                cur_texts[7].text = "인원 : " + SaveScript.saveData.Farming_totalNum[index] + " / 6,   총 채집력 : " + SaveScript.saveData.Farming_totalForce[index];

                buttons[3].gameObject.SetActive(false);
            }
            else
            {
                // 보상 수령 확인 알림
                buttons[3].gameObject.SetActive(true);

                cur_texts[5].text = "[ 채집 완료 ]";
                cur_texts[6].text = "진행 상황 : " + stage_totalWorkSize[index] + " / " + stage_totalWorkSize[index];
                cur_texts[7].text = "인원 : " + SaveScript.saveData.Farming_totalNum[index] + " / 6,   총 채집력 : " + SaveScript.saveData.Farming_totalForce[index];
            }
        }
    }

    public void SetCurrentStage() // index에 따른 스테이지 설정
    {
        // PlayerFarming Setting
        if (isPlayerFarming)
        {
            for (int i = 0; i < touchObjectImages.Length; i++)
                for (int j = 0; j < touchObjectImages[i].Length; j++)
                    touchObjectImages[i][j].gameObject.SetActive(false);

            for (int i = 0; i < touchObjectStages.Length; i++)
                touchObjectStages[i].gameObject.SetActive(false);

            touchObjectStages[index].gameObject.SetActive(true);
            currentTouchObjectImage[index].gameObject.SetActive(true);
            currentTouchObjectImage[index].GetComponent<FarmingObject>().isInit = true;
            currentTouchObjectImage[index].GetComponent<FarmingObject>().colorA = 0f;
            currentTouchObjectImage[index].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        // PageButton Setting
        pageButtons[0].gameObject.SetActive(true);
        pageButtons[1].gameObject.SetActive(true);

        if (index == 0)
            pageButtons[0].gameObject.SetActive(false);
        else if (index == SaveScript.saveData.Farming_stages.Length - 1)
            pageButtons[1].gameObject.SetActive(false);

        // Stage BackGround Setting
        for (int i = 0; i < SaveScript.saveData.Farming_stages.Length; i++)
            backgroundImages[i].gameObject.SetActive(false);
        backgroundImages[index].gameObject.SetActive(true);

        // Stage Infos Setting
        cur_texts[0].text = "- " + stage_names[index] + " -";
        cur_texts[1].text = "총 채집량 : " + stage_totalWorkSize[index];
        cur_texts[3].text = stage_infos[index];

        // 스테이지가 닫혀 있을 경우
        if (!stage_isOpens[index])
        {
            for (int i = 2; i < 4; i++)
                buttons[i].gameObject.SetActive(false);
            currentInfoObject.SetActive(false);
            noOpenImage.SetActive(true);
            noOpenInfoText.text = "주의! 플레이어 Lv." + stage_levels[index] + " 이상 출입 가능";
            currentTouchObjectImage[index].gameObject.SetActive(false);
        }
        else
        {
            for (int i = 2; i < 4; i++)
                buttons[i].gameObject.SetActive(true);
            currentInfoObject.SetActive(true);
            noOpenImage.SetActive(false);
        }
    }

    public void ShowNonInfo() // 현재 파밍지역이 아닐 경우 출력되는 함수
    {
        cur_texts[5].text = "[ 채집 중이 아님 ]";
        cur_texts[6].text = "진행 상황 : " + SaveScript.saveData.Farming_currentSize[index] + " / " + stage_totalWorkSize[index];
        cur_texts[7].text = "인원 : " + SaveScript.saveData.Farming_totalNum[index] + " / 6,   총 채집력 : " + SaveScript.saveData.Farming_totalForce[index];

        buttons[3].gameObject.SetActive(false);
    }

    public void OpenStagePerLevel() // 플레이어 레벨에 따른 스테이지 개방 여부
    {
        if (SaveScript.saveData.level >= stage_levels[0])
            stage_isOpens[0] = true;
        else
            stage_isOpens[0] = false;

        if (SaveScript.saveData.level >= stage_levels[1])
            stage_isOpens[1] = true;
        else
            stage_isOpens[1] = false;

        if (SaveScript.saveData.level >= stage_levels[2])
            stage_isOpens[2] = true;
        else
            stage_isOpens[2] = false;

        if (SaveScript.saveData.level >= stage_levels[3])
            stage_isOpens[3] = true;
        else
            stage_isOpens[3] = false;

        if (SaveScript.saveData.level >= stage_levels[4])
            stage_isOpens[4] = true;
        else
            stage_isOpens[4] = false;
    }

    public void PageUp() // 다음 스테이지 이동 버튼
    {
        index++;
        SetCurrentStage();
        SetTimeInfo();
        SetRewardInfo();
    }

    public void PageDown() // 이전 스테이지 이동 버튼
    {
        index--;
        SetCurrentStage();
        SetTimeInfo();
        SetRewardInfo();
    }

    public void OpenTeamUI() // 채집 팀 오픈 버튼
    {
        teamPanel.SetActive(true);
        AISelector.SetActive(false);
        goldInfo.SetActive(false);

        SetTeamInfo();
    }

    public void CloseTeamUI()
    {
        teamPanel.SetActive(false);
        goldInfo.SetActive(true);
    }

    public void PlayerFarmingButton()
    {
        playerFarming.SetActive(true);
        autoFarming.SetActive(false);
        isPlayerFarming = true;
        for (int i = 0; i < dropInfoText.Length; i++)
            dropInfoText[i].text = "";
        infoTextCount = 0;

        SetCurrentStage();
    }

    public void AutoFarmingButton()
    {
        playerFarming.SetActive(false);
        autoFarming.SetActive(true);
        isPlayerFarming = false;
        for (int i = 0; i < dropInfoText.Length; i++)
            dropInfoText[i].text = "";
        infoTextCount = 0;

        SetCurrentStage();
        SetTimeInfo();
    }

    public void SlotButton() // 1 ~ 6번 슬롯의 기능을 담당, 슬롯콘텐트UI를 보여준다.
    {
        slotIndex = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;
        // order은 순서를 의미하며, order번째 슬롯임을 알려준다. (order = 0 ~ 5의 값을 가진다.)

        for (int i = 0; i < 6; i++)
        {
            SlotContent.GetComponentsInChildren<ItemOrder>()[i].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 1);
            SlotContent.GetComponentsInChildren<ItemOrder>()[i].GetComponentsInChildren<Image>()[2].color = new Color(1, 1, 1, 0);
        }

        if (SaveScript.Farming_AIs[index][slotIndex] == null)
        {
            SlotContent.GetComponentsInChildren<ItemOrder>()[slotIndex].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 0);
            SlotContent.GetComponentsInChildren<ItemOrder>()[slotIndex].GetComponentsInChildren<Image>()[2].color = new Color(1, 1, 1, 1);
            AISelector.SetActive(true);

            // 슬롯 버튼 초기화
            Button[] AISelectButtons = AISelector.GetComponentsInChildren<Button>();
            for (int i = 0; i < AISelectButtons.Length; i++)
                Destroy(AISelectButtons[i].gameObject);

            // AI 선택창 설정
            GameObject[] slots = new GameObject[SaveScript.storyAINum + SaveScript.AINum + SaveScript.specialAINum];
            AItypes = new int[SaveScript.storyAINum + SaveScript.AINum + SaveScript.specialAINum];
            int count = 0;
            for (int i = 0; i < SaveScript.storyAINum; i++)
            {
                if (SaveScript.saveData.hasStoryAI[i] && !SaveScript.hasStoryAI[i].isFarming)
                {
                    slots[count] = Instantiate(AIselectorPrefab, Vector3.zero, new Quaternion(0, 0, 0, 0), SelectContent.transform);
                    AItypes[i] = 0;
                    slots[count].GetComponentsInChildren<Image>()[2].sprite = SaveScript.hasStoryAI[i].shop_image.sprite;
                    slots[count].GetComponentsInChildren<Text>()[0].text = SaveScript.hasStoryAI[i].name;
                    slots[count].GetComponentsInChildren<Text>()[0].color = SaveScript.hasStoryAI[i].color;
                    slots[count].GetComponentsInChildren<Text>()[1].text = "채집력 : " + (SaveScript.hasStoryAI[i].workPorce + SaveScript.hasStoryAI[i].level * 2);
                    slots[count].GetComponent<Button>().onClick.AddListener(SelectAI);
                    slots[count].GetComponent<ItemOrder>().order = count;
                    count++;
                }
                else
                    AItypes[i] = -1;
            }

            for (int i = 0; i < SaveScript.AINum; i++)
            {
                if (SaveScript.saveData.hasAI[i] && !SaveScript.hasAI[i].isFarming)
                {
                    slots[count] = Instantiate(AIselectorPrefab, Vector3.zero, new Quaternion(0, 0, 0, 0), SelectContent.transform);
                    AItypes[i + SaveScript.storyAINum] = 1;
                    slots[count].GetComponentsInChildren<Image>()[2].sprite = SaveScript.hasAI[i].shop_image.sprite;
                    slots[count].GetComponentsInChildren<Text>()[0].text = SaveScript.hasAI[i].name;
                    slots[count].GetComponentsInChildren<Text>()[0].color = SaveScript.hasAI[i].color;
                    slots[count].GetComponentsInChildren<Text>()[1].text = "채집력 : " + (SaveScript.hasAI[i].workPorce + SaveScript.hasAI[i].level * 2).ToString();
                    slots[count].GetComponent<Button>().onClick.AddListener(SelectAI);
                    slots[count].GetComponent<ItemOrder>().order = count;
                    count++;
                }
                else
                    AItypes[i + SaveScript.storyAINum] = -1;
            }

            for (int i = 0; i < SaveScript.specialAINum; i++)
            {
                if (SaveScript.saveData.hasSpecialAI[i] && !SaveScript.hasSpecialAI[i].isFarming)
                {
                    slots[count] = Instantiate(AIselectorPrefab, Vector3.zero, new Quaternion(0, 0, 0, 0), SelectContent.transform);
                    AItypes[i + SaveScript.storyAINum + SaveScript.AINum] = 2;
                    slots[count].GetComponentsInChildren<Image>()[2].sprite = SaveScript.hasSpecialAI[i].shop_image.sprite;
                    slots[count].GetComponentsInChildren<Text>()[0].text = SaveScript.hasSpecialAI[i].name;
                    slots[count].GetComponentsInChildren<Text>()[0].color = SaveScript.hasSpecialAI[i].color;
                    slots[count].GetComponentsInChildren<Text>()[1].text = "채집력 : " + (SaveScript.hasSpecialAI[i].workPorce + SaveScript.hasSpecialAI[i].level * 2).ToString();
                    slots[count].GetComponent<Button>().onClick.AddListener(SelectAI);
                    slots[count].GetComponent<ItemOrder>().order = count;
                    count++;
                }
                else
                    AItypes[i + SaveScript.storyAINum + SaveScript.AINum] = -1;
            }

            // 배치할 AI가 없을 경우
            if (slots[0] == null)
                noAIText.SetActive(true);
            else
                noAIText.SetActive(false);
        }
        else
        {
            AISelector.SetActive(false);
            SlotContent.GetComponentsInChildren<ItemOrder>()[slotIndex].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 1);
            SlotContent.GetComponentsInChildren<ItemOrder>()[slotIndex].GetComponentsInChildren<Image>()[2].color = new Color(1, 1, 1, 0);
            SlotContent.GetComponentsInChildren<ItemOrder>()[slotIndex].GetComponentsInChildren<Image>()[3].color = new Color(0, 0, 0, 0);
            SlotContent.GetComponentsInChildren<ItemOrder>()[slotIndex].GetComponentsInChildren<Image>()[3].sprite = null;

            SaveScript.saveData.Farming_totalForce[index] -= SaveScript.Farming_AIs[index][slotIndex].workPorce + SaveScript.Farming_AIs[index][slotIndex].level * 2;
            SaveScript.saveData.Farming_totalNum[index]--;
            SaveScript.Farming_AIs[index][slotIndex].isFarming = false;
            SaveScript.Farming_AIs[index][slotIndex] = null;

            if(SaveScript.saveData.Farming_totalNum[index] == 0)
                SaveScript.saveData.Farming_stages[index] = false;

            SetTeamInfo();
            SetTimeInfo();
            SetIsFarming();
        }
    }

    public void SelectAI() // AI를 선택하여 팀 배치
    {
        ItemOrder[] slots = SlotContent.GetComponentsInChildren<ItemOrder>();
        int tempOrder = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order; // 슬롯의 위치 인덱스
        int countTrue = 0, countFalse = 0;
        int order; // 실제 선택될 AI 인덱스

        for (int i = 0; i < AItypes.Length; i++)
        {
            if (AItypes[i] != -1)
                countTrue++;
            else
                countFalse++;

            if (tempOrder == countTrue - 1)
                break;
        }

        if (countTrue + countFalse <= SaveScript.storyAINum)
        {
            order = countTrue + countFalse - 1;
            SaveScript.Farming_AIs[index][slotIndex] = SaveScript.hasStoryAI[order];
            SaveScript.saveData.Farming_totalForce[index] += SaveScript.hasStoryAI[order].workPorce + SaveScript.hasStoryAI[order].level * 2;
            SaveScript.saveData.Farming_totalNum[index]++;
            SaveScript.hasStoryAI[order].isFarming = true;
            slots[slotIndex].GetComponentsInChildren<Image>()[3].sprite = SaveScript.hasStoryAI[order].shop_image.sprite;
            slots[slotIndex].GetComponentInChildren<Text>().text = "채집력 : " + (SaveScript.hasStoryAI[order].workPorce + SaveScript.hasStoryAI[order].level * 2);
        }
        else if(countTrue + countFalse <= SaveScript.storyAINum + SaveScript.AINum)
        {
            order = countTrue + countFalse - SaveScript.storyAINum - 1;
            SaveScript.Farming_AIs[index][slotIndex] = SaveScript.hasAI[order];
            SaveScript.saveData.Farming_totalForce[index] += SaveScript.hasAI[order].workPorce + SaveScript.hasAI[order].level * 2;
            SaveScript.saveData.Farming_totalNum[index]++;
            SaveScript.hasAI[order].isFarming = true;
            slots[slotIndex].GetComponentsInChildren<Image>()[3].sprite = SaveScript.hasAI[order].shop_image.sprite;
            slots[slotIndex].GetComponentInChildren<Text>().text = "채집력 : " + (SaveScript.hasAI[order].workPorce + SaveScript.hasAI[order].level * 2);
        }
        else
        {
            order = countTrue + countFalse - SaveScript.storyAINum - SaveScript.AINum - 1;
            SaveScript.Farming_AIs[index][slotIndex] = SaveScript.hasSpecialAI[order];
            SaveScript.saveData.Farming_totalForce[index] += SaveScript.hasSpecialAI[order].workPorce + SaveScript.hasSpecialAI[order].level * 2;
            SaveScript.saveData.Farming_totalNum[index]++;
            SaveScript.hasSpecialAI[order].isFarming = true;
            slots[slotIndex].GetComponentsInChildren<Image>()[3].sprite = SaveScript.hasSpecialAI[order].shop_image.sprite;
            slots[slotIndex].GetComponentInChildren<Text>().text = "채집력 : " + (SaveScript.hasSpecialAI[order].workPorce + SaveScript.hasSpecialAI[order].level * 2);
        }

        SaveScript.saveData.Farming_stages[index] = true;
        AISelector.SetActive(false);
        SetTeamInfo();
        SetTimeInfo();
        SetIsFarming();
    }
    
    public void GetReward() // 해당 스테이지 보상 휙득 버튼
    {
        rewardPanel.SetActive(true);
        goldInfo.SetActive(false);
        rewardPanel.GetComponentInChildren<Text>().text = "- [ " + stage_names[index] + " ] 보상 -";
        SaveScript.saveData.Farming_currentSize[index] = 0;

        // 보상 프리팹 초기화
        ItemOrder[] rewardTemps = reward_contentPanel.GetComponentsInChildren<ItemOrder>();
        for (int i = 0; i < rewardTemps.Length; i++)
            Destroy(rewardTemps[i].gameObject);

        // 보상 목록 표시
        int gold = Random.Range(stage_golds[index][0], stage_golds[index][1]);
        List<Item> items = new List<Item>();
        int[] nums;

        switch (index)
        {
            case 4:
            case 3:
            case 2:
            case 1:
            case 0:
                int count = 0;
                while (Percent.GetRandFlag(Mathf.Pow(0.5f, count)))
                {
                    if (count != 3)
                    {
                        int code = Random.Range(13, 16);
                        for (int i = 0; i < items.Count; i++)
                        {
                            while (items[i].itemCode == code)
                            {
                                i = 0;
                                code = Random.Range(13, 16);
                            }
                        }

                        items.Add(new etc(code));
                    }
                    else
                        break;

                    count++;
                }
                break;
        }

        switch (index)
        {
            case 4:
            case 3:
            case 2:
            case 1:
                int count = 0;
                while (Percent.GetRandFlag(Mathf.Pow(0.5f, count)))
                {
                    if (count != 2)
                    {
                        int code = Random.Range(11, 13);
                        for (int i = 0; i < items.Count; i++)
                        {
                            while (items[i].itemCode == code)
                            {
                                i = 0;
                                code = Random.Range(11, 13);
                            }
                        }

                        items.Add(new etc(code));
                    }
                    else
                        break;

                    count++;
                }
                break;
        }

        switch (index)
        {
            case 4:
            case 3:
            case 2:
                int count = 0;
                while (Percent.GetRandFlag(Mathf.Pow(0.5f, count)))
                {
                    if (count != 1)
                    {
                        int code = Random.Range(16, 17);
                        for (int i = 0; i < items.Count; i++)
                        {
                            while (items[i].itemCode == code)
                            {
                                i = 0;
                                code = Random.Range(16, 17);
                            }
                        }

                        items.Add(new etc(code));
                    }
                    else
                        break;

                    count++;
                }
                break;
        }

        switch (index)
        {
            case 4:
            case 3:
                int count = 0;
                while (Percent.GetRandFlag(Mathf.Pow(0.5f, count)))
                {
                    if (count != 1)
                    {
                        int code = Random.Range(17, 18);
                        for (int i = 0; i < items.Count; i++)
                        {
                            while (items[i].itemCode == code)
                            {
                                i = 0;
                                code = Random.Range(17, 18);
                            }
                        }

                        items.Add(new etc(code));
                    }
                    else
                        break;

                    count++;
                }
                break;
        }

        switch (index)
        {
            case 4:
                break;
        }

        // 보상 정리 및 재고 확정

        nums = new int[items.Count];

        for (int i = 0; i < items.Count + 1; i++)
        {
            GameObject item = Instantiate(rewardPrefab, reward_contentPanel.transform);
            if (i == 0)
            {
                item.GetComponentInChildren<Text>().text = gold + " 원";
                SaveScript.saveData.gold += gold;
                goldText.text = SaveScript.saveData.gold + "원";
            }
            else
            {
                switch((items[i - 1] as etc).quallity)
                {
                    case 0:
                        nums[i - 1] = Random.Range(5 * (index + 2) * (index + 2), 5 * (index + 3) * (index + 3));
                        break;
                    case 1:
                        nums[i - 1] = Random.Range(8 * (index * 2), 8 * ((index + 1) * 2));
                        break;
                    case 2:
                        nums[i - 1] = Random.Range(index * (index + 1), (index * 2) * index);
                        break;
                    case 3:
                        nums[i - 1] = Random.Range(index * 2 - 2, index * 2);
                        break;
                    case 4:
                        nums[i - 1] = Random.Range(0, 2);
                        break;
                }

                item.GetComponentsInChildren<Image>()[1].sprite = items[i - 1].UIImage.sprite;
                item.GetComponentInChildren<Text>().text = "x" + nums[i - 1];
                SaveScript.saveData.hasEtcs[items[i - 1].itemCode - SaveScript.saveData.hasGuns.Count - SaveScript.saveData.hasArmors.Count - 1] += nums[i - 1];
            }
        }
    }

    public void GetAllReward() // 모든 스테이지 보상 휙득 버튼
    {
        rewardPanel.SetActive(true);
        goldInfo.SetActive(false);
        rewardPanel.GetComponentInChildren<Text>().text = "- 모든 보상 -";

        // 보상 프리팹 초기화
        ItemOrder[] rewardTemps = reward_contentPanel.GetComponentsInChildren<ItemOrder>();
        for (int i = 0; i < rewardTemps.Length; i++)
            Destroy(rewardTemps[i].gameObject);

        // 보상 목록 표시
        int gold = 0;
        List<Item> items = new List<Item>();
        List<int> nums = new List<int>();
        int itemIndex = 0; // 아이템의 순차적 인덱스를 나타내는 변수

        for (int i = 0; i < SaveScript.saveData.Farming_stages.Length; i++)
        {
            if(SaveScript.saveData.Farming_currentSize[i] >= stage_totalWorkSize[i])
            {
                gold += Random.Range(stage_golds[i][0], stage_golds[i][1]);
                SaveScript.saveData.Farming_currentSize[i] = 0;

                switch (i)
                {
                    case 4:
                    case 3:
                    case 2:
                    case 1:
                    case 0:
                        int count = 0;
                        while (Percent.GetRandFlag(Mathf.Pow(0.5f, count)))
                        {
                            if (count != etc.itemNumsAsQuality[i])
                            {
                                int code = Random.Range(13, 16);
                                items.Add(new etc(code));

                                switch ((items[itemIndex++] as etc).quallity)
                                {
                                    case 0:
                                        nums.Add(Random.Range(5 * (i + 2) * (i + 2), 5 * (i + 3) * (i + 3)));
                                        break;
                                    case 1:
                                        nums.Add(Random.Range(8 * (i * 2), 8 * ((i + 1) * 2)));
                                        break;
                                    case 2:
                                        nums.Add(Random.Range(i * (i + 1), (i * 2) * i));
                                        break;
                                    case 3:
                                        nums.Add(Random.Range(i * 2 - 2, i * 2));
                                        break;
                                    case 4:
                                        nums.Add(Random.Range(0, 2));
                                        break;
                                }
                            }
                            else
                                break;

                            count++;
                        }
                        break;
                }

                switch (i)
                {
                    case 4:
                    case 3:
                    case 2:
                    case 1:
                        int count = 0;
                        while (Percent.GetRandFlag(Mathf.Pow(0.5f, count)))
                        {
                            if (count != etc.itemNumsAsQuality[i])
                            {
                                int code = Random.Range(11, 13);
                                items.Add(new etc(code));

                                switch ((items[itemIndex++] as etc).quallity)
                                {
                                    case 0:
                                        nums.Add(Random.Range(5 * (i + 2) * (i + 2), 5 * (i + 3) * (i + 3)));
                                        break;
                                    case 1:
                                        nums.Add(Random.Range(8 * (i * 2), 8 * ((i + 1) * 2)));
                                        break;
                                    case 2:
                                        nums.Add(Random.Range(i * (i + 1), (i * 2) * i));
                                        break;
                                    case 3:
                                        nums.Add(Random.Range(i * 2 - 2, i * 2));
                                        break;
                                    case 4:
                                        nums.Add(Random.Range(0, 2));
                                        break;
                                }
                            }
                            else
                                break;

                            count++;
                        }
                        break;
                }

                switch (i)
                {
                    case 4:
                    case 3:
                    case 2:
                        int count = 0;
                        while (Percent.GetRandFlag(Mathf.Pow(0.5f, count)))
                        {
                            if (count != etc.itemNumsAsQuality[i])
                            {
                                int code = Random.Range(16, 17);
                                items.Add(new etc(code));

                                switch ((items[itemIndex++] as etc).quallity)
                                {
                                    case 0:
                                        nums.Add(Random.Range(5 * (i + 2) * (i + 2), 5 * (i + 3) * (i + 3)));
                                        break;
                                    case 1:
                                        nums.Add(Random.Range(8 * (i * 2), 8 * ((i + 1) * 2)));
                                        break;
                                    case 2:
                                        nums.Add(Random.Range(i * (i + 1), (i * 2) * i));
                                        break;
                                    case 3:
                                        nums.Add(Random.Range(i * 2 - 2, i * 2));
                                        break;
                                    case 4:
                                        nums.Add(Random.Range(0, 2));
                                        break;
                                }
                            }
                            else
                                break;

                            count++;
                        }
                        break;
                }

                switch (i)
                {
                    case 4:
                    case 3:
                        int count = 0;
                        while (Percent.GetRandFlag(Mathf.Pow(0.5f, count)))
                        {
                            if (count != etc.itemNumsAsQuality[i])
                            {
                                int code = Random.Range(17, 18);
                                items.Add(new etc(code));

                                switch ((items[itemIndex++] as etc).quallity)
                                {
                                    case 0:
                                        nums.Add(Random.Range(5 * (i + 2) * (i + 2), 5 * (i + 3) * (i + 3)));
                                        break;
                                    case 1:
                                        nums.Add(Random.Range(8 * (i * 2), 8 * ((i + 1) * 2)));
                                        break;
                                    case 2:
                                        nums.Add(Random.Range(i * (i + 1), (i * 2) * i));
                                        break;
                                    case 3:
                                        nums.Add(Random.Range(i * 2 - 2, i * 2));
                                        break;
                                    case 4:
                                        nums.Add(Random.Range(0, 2));
                                        break;
                                }
                            }
                            else
                                break;

                            count++;
                        }
                        break;
                }

                switch (i)
                {
                    case 4:
                        break;
                }
            }
        }

        // 보상 정리 및 재고 확정
        for (int i = 0; i < items.Count; i++)
        {
            etc data = items[i] as etc;
            for (int j = 0; j < items.Count; j++)
                Debug.Log((items[j] as etc).name + nums[j]);
            Debug.Log("----------------------");

            for (int j = i + 1; j < items.Count; j++)
            {
                if(data.itemCode == items[j].itemCode)
                {
                    nums[i] += nums[j];
                    nums.RemoveAt(j);
                    items.RemoveAt(j--);
                }
            }
        }

        for (int i = 0; i < items.Count + 1; i++)
        {
            GameObject item = Instantiate(rewardPrefab, reward_contentPanel.transform);
            if (i == 0)
            {
                item.GetComponentInChildren<Text>().text = gold + " 원";
                SaveScript.saveData.gold += gold;
                goldText.text = SaveScript.saveData.gold + "원";
            }
            else
            { 
                item.GetComponentsInChildren<Image>()[1].sprite = items[i - 1].UIImage.sprite;
                item.GetComponentInChildren<Text>().text = "x" + nums[i - 1];
                SaveScript.saveData.hasEtcs[items[i - 1].itemCode - SaveScript.weaponNum - SaveScript.weaponNum - 1] += nums[i - 1];
            }
        }
    }

    public void CloseReward()
    {
        rewardPanel.SetActive(false);
        goldInfo.SetActive(true);

        SetCurrentStage();
        SetTimeInfo();
        SetTeamInfo();
        SetRewardInfo();
    }

    public void SetRewardInfo()
    {
        buttons[1].gameObject.SetActive(false);

        for (int i = 0; i < SaveScript.saveData.Farming_stages.Length; i++)
        {
            if(SaveScript.saveData.Farming_currentSize[i] >= stage_totalWorkSize[i])
            {
                allStageInfoImages[2 * i + 1].color = new Color(1, 1, 1, 1);
                buttons[1].gameObject.SetActive(true);
            }
            else
                allStageInfoImages[2 * i + 1].color = new Color(1, 1, 1, 0);
        }
    }
}

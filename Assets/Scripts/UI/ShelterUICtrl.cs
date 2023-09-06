using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShelterUICtrl : MonoBehaviour
{
    public Image startButton;
    public GameObject modeSelector;
    public GameObject storyPanel;
    public GameObject mainUI;
    public Text goldText;
    private Button[] stageButtons;
    [SerializeField] private GameObject stageButtonObject;
    [SerializeField] private GameObject stagePanel;
    [SerializeField] private GameObject stageObject;
    private int stageData;

    // 레벨 및 경험치 UI 관련 변수들
    public GameObject levelInfo;
    static public Slider expSlider;
    static public Text levelText, expText, levelUpText;

    private bool isExpOn; // 경험치 휙득 감지
    private bool isFadeExp;
    private Color expTempColor;
    private bool isLevelUpOn; //레벨 업 감지
    private bool isFadeLevelUp;
    private Color levelUpColor;

    // 씬 이동 관련 변수들

    private bool isStartButtonPressed;
    
    static public bool isGotoGameScene;
    static public bool isGotoShopScene;
    static public bool isGotoFarmingScene;
    static public bool isGotoUpgradeScene;
    static public bool isGotoProfileScene;
    static public bool isGotoQuastScene;

    // Start is called before the first frame update
    void Start()
    {
        ALLUISetting(false);
        storyPanel.gameObject.SetActive(false);
        stageButtons = stageButtonObject.GetComponentsInChildren<Button>(); 
        goldText.text = SaveScript.saveData.gold + " 원";

        expSlider = levelInfo.GetComponentInChildren<Slider>();
        levelText = levelInfo.GetComponentsInChildren<Text>()[0];
        expText = levelInfo.GetComponentsInChildren<Text>()[1];
        levelUpText = levelInfo.GetComponentsInChildren<Text>()[2];

        levelUpText.gameObject.SetActive(false);
        expText.gameObject.SetActive(false);
        expText.text = "";
        stagePanel.SetActive(false);
        stageObject.SetActive(false);

        SetLevelInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStartButtonPressed)
        { 
            if (Input.GetKeyDown(KeyCode.Escape))
                startButtonOn();
        }

        // 경험치 알림
        if (isExpOn)
        {
            expText.gameObject.SetActive(true);
            expTempColor = Color.white;
            expText.color = expTempColor;
            isExpOn = false;
        }

        if (expText.gameObject.activeSelf)
        {
            StartCoroutine(WaitExp(1f));
            if (isFadeExp)
            {
                if (expTempColor.a >= 0f)
                {
                    expTempColor.a -= Time.deltaTime * 0.5f;
                    expText.color = expTempColor;
                }
                else
                {
                    isFadeExp = false;
                    expText.gameObject.SetActive(false);
                    expText.color = Color.white;
                }
            }
        }

        // 레벨 업 알림
        if (isLevelUpOn)
        {
            levelUpText.gameObject.SetActive(true);
            levelUpColor = Color.yellow;
            levelUpText.color = levelUpColor;
            isLevelUpOn = false;
        }

        if (levelUpText.gameObject.activeSelf)
        {
            StartCoroutine(WaitLevelUp(2f));
            if (isFadeLevelUp)
            {
                if (levelUpColor.a >= 0f)
                {
                    levelUpColor.a -= Time.deltaTime * 0.5f;
                    levelUpText.color = levelUpColor;
                }
                else
                {
                    isFadeLevelUp = false;
                    levelUpText.gameObject.SetActive(false);
                    levelUpText.color = Color.yellow;
                }
            }
        }
    }

    public void ALLUISetting(bool data) // 모든 로비 UI를 끄거나 킨다. ( 컨텐츠 UI는 제외 )
    {
        modeSelector.gameObject.SetActive(data);
        isStartButtonPressed = false;

        if (data)
            startButton.color = Color.white;
        else
            startButton.color = Color.gray;
    }

    public void startButtonOn()
    {
        if (isStartButtonPressed) // 선택창 끄기
        {
            startButton.color = Color.gray;
            modeSelector.gameObject.SetActive(false);
        }
        else // 선택창 키기
        {
            startButton.color = Color.white;
            modeSelector.gameObject.SetActive(true);
        }

        
        isStartButtonPressed = !isStartButtonPressed;
    }

    public void BackGroundButtonOn()
    {
        ALLUISetting(false);
    }

    public void StoryButtonOn()
    {
        storyPanel.gameObject.SetActive(true);
        mainUI.gameObject.SetActive(false);
        SetStage();
    }

    public void ScoreButtonOn()
    {
        
    }

    public void BackButtonOn()
    {
        storyPanel.gameObject.SetActive(false);
        mainUI.gameObject.SetActive(true);
        startButtonOn();
    }

    public void SetStage()
    {
        if(SaveScript.saveData.isTutorial)
            stageButtons[0].gameObject.SetActive(true);
        else
            stageButtons[0].gameObject.SetActive(false);

        int clolsedNum = stageButtons.Length - (5 * SaveScript.saveData.stage + SaveScript.saveData.stage_level + 1); // 닫혀야 할 버튼의 갯수

        for (int i = 0; i < clolsedNum; i++)
        {
            stageButtons[stageButtons.Length - 1 - i].gameObject.SetActive(false);
        }
    }

    public void GoToShop()
    {
        isGotoShopScene = true;
    }

    public void GoToFarming()
    {
        isGotoFarmingScene = true;
    }

    public void GoToUpgrade()
    {
        isGotoUpgradeScene = true;
    }

    public void GoToProfile()
    {
        isGotoProfileScene = true;
    }

    public void GoToQuast()
    {
        isGotoQuastScene = true;
    }

    public void GetStageInfo()
    {
        stageData = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;

        stagePanel.SetActive(true);
        stageObject.SetActive(true);
        stageObject.GetComponentsInChildren<Text>()[0].text = stageData / Stage.stageLevelLength + 1 + "-" + stageData % Stage.stageLevelLength + " 스테이지, [ " + Stage.stageName[stageData] + " ]";
        stageObject.GetComponentsInChildren<Text>()[1].text = Stage.stageDangerInfo[stageData];
        stageObject.GetComponentsInChildren<Text>()[2].text = Stage.stageDropItemInfo[stageData];
        stageObject.GetComponentsInChildren<Text>()[3].text = Stage.stageZombieInfo[stageData];
    }

    public void GoToGame()
    {
        SaveScript.saveData.currentStage = stageData / Stage.stageLevelLength;
        SaveScript.saveData.currentStage_level = stageData % Stage.stageLevelLength;
        isGotoGameScene = true;
    }

    public void StageBack()
    {
        stagePanel.SetActive(false);
        stageObject.SetActive(false);
    }

    public void SetLevelInfo()
    {
        if (SaveScript.saveData.exp >= SaveScript.saveData.levelUp)
        {
            SaveScript.saveData.exp -= SaveScript.saveData.levelUp;
            SaveScript.saveData.level++;
            SaveScript.saveData.levelUp = (int)(23 * Mathf.Pow(SaveScript.saveData.level, 1.8f) + 77);
            SaveScript.saveData.HP += 5;
            isLevelUpOn = true;
        }

        isExpOn = true;
        levelText.text = "Lv." + SaveScript.saveData.level;
        expSlider.value = (float)SaveScript.saveData.exp / SaveScript.saveData.levelUp;
    }


    IEnumerator WaitExp(float time)
    {
        yield return new WaitForSeconds(time);
        isFadeExp = true;
    }

    IEnumerator WaitLevelUp(float time)
    {
        yield return new WaitForSeconds(time);
        isFadeLevelUp = true;
    }
}

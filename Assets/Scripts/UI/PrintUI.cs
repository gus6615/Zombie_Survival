using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrintUI : MonoBehaviour {

    public Text bulletText;
    public Text reloadingText;
    public Text HPText;
    public Slider HPSlider;
    public Text goldText;
    public Text ScoreText;
    public Slider bulletSlider;
    public Image gunImage;
    public GameObject scopeObject;
    public GameObject nonScopeObject;
    public GameObject print_ScopeUI;
    public GameObject ScopeBulletUI;
    public Text goldAbilityText, bulletAbilityText;
    private bool isGoldAbility, isBulletAbility;
    public GameObject scoreObject;
 
    public Image[] warningImages; // 0 = 왼쪽, 1 = 오른쪽

    // 아이템 휙득 알림

    public GameObject itemInfoObject;
    private Image itemInfoImage;
    private Text itemInfoText;
    private Color tempImage;
    private Color tempText;
    private bool isFadeItem;
    public bool isItemOn; // 아이템 휘득 감지

    // 레벨 및 경험치 알림

    public Slider expSlider;
    public Text levelText;
    public Text expText;
    private bool isExpOn; // 경험치 휙득 감지
    private bool isFadeExp;
    private Color expTempColor;
    public Text levelUpText;
    private bool isLevelUpOn; //레벨 업 감지
    private bool isFadeLevelUp;
    private Color levelUpColor;

    // 버튼 UI들
    public Button changeWeaponButton, changeModeButton, specialAIButton, specialAILeftButton, specialAIRightButton;
    public GameObject changeWeaponStopImage, changeModeStopImage, specialAIStopImage;

    private PlayerScript player;

	// Use this for initialization
	void Start () {
        // 무한 스코어 모드 일 경우
        if (StageController.isScoreMode)
        {
            scoreObject.SetActive(true);
            ScoreText.text = SaveScript.saveData.score.ToString();
        }
        else
        {
            scoreObject.SetActive(false);
        }

        player = FindObjectOfType<PlayerScript>();
        if (SaveScript.saveData.equipGun == 0)
            bulletText.text = SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum + " / ∞";
        else
            bulletText.text = SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum + " / " + SaveScript.saveData.hasGunsBullets[SaveScript.saveData.equipGun];

        HPText.text = player.HP + " / " + player.MaxHP;
        HPSlider.value = HPSlider.maxValue = player.MaxHP;
        goldText.text = SaveScript.saveData.gold + " 원";
        bulletSlider.value = bulletSlider.maxValue = SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum;
        gunImage.sprite = SaveScript.guns[SaveScript.saveData.equipGun].image.sprite;

        scopeObject.SetActive(false);
        reloadingText.gameObject.SetActive(false);
        warningImages[0].gameObject.SetActive(false);
        warningImages[1].gameObject.SetActive(false);
        itemInfoObject.SetActive(false);
        itemInfoImage = itemInfoObject.GetComponentInChildren<Image>();
        itemInfoText = itemInfoObject.GetComponentInChildren<Text>();

        levelUpText.gameObject.SetActive(false);
        expText.gameObject.SetActive(false);
        expText.text = "";

        changeModeStopImage.gameObject.SetActive(false);
        changeWeaponStopImage.gameObject.SetActive(false);
        specialAIStopImage.gameObject.SetActive(false);

        SetLevelInfo();
    }

    private void Update()
    {
        // 휙득 아이템 알림
        if (isItemOn)
        {
            itemInfoObject.SetActive(true);
            tempImage = Color.white;
            tempText = itemInfoText.color;
            itemInfoImage.color = tempImage;
            itemInfoText.color = tempText;
            isItemOn = false;
        }

        if (itemInfoObject.activeSelf)
        {
            StartCoroutine(WaitItem(1f));
            if (isFadeItem)
            {
                if (tempImage.a >= 0f)
                {
                    tempImage.a -= Time.deltaTime * 0.5f;
                    tempText.a -= Time.deltaTime * 0.5f;
                    itemInfoImage.color = tempImage;
                    itemInfoText.color = tempText;
                }
                else
                {
                    isFadeItem = false;
                    itemInfoObject.SetActive(false);
                    itemInfoImage.color = Color.white;
                    itemInfoText.color = Color.black;
                }
            }
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
                if(expTempColor.a >= 0f)
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

        // 무기 능력 도벽 및 수집 텍스트
        if (isGoldAbility)
        {
            Color temp = goldAbilityText.color;

            if(temp.a > 0.01f)
            {
                temp.a -= Time.deltaTime;
                goldAbilityText.color = temp;
            }
            else
            {
                temp.a = 0f;
                goldAbilityText.color = temp;
                isGoldAbility = false;
            }
        }
        else
        {
            goldAbilityText.color = new Color(1f, 1f, 1f, 0f);
        }

        if (isBulletAbility)
        {
            Color temp = bulletAbilityText.color;

            if (temp.a > 0.01f)
            {
                temp.a -= Time.deltaTime;
                bulletAbilityText.color = temp;
            }
            else
            {
                temp.a = 0f;
                bulletAbilityText.color = temp;
                isBulletAbility = false;
            }
        }
        else
        {
            bulletAbilityText.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    IEnumerator WaitItem(float time)
    {
        yield return new WaitForSeconds(time);
        isFadeItem = true;
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

    public void ShowWaring()
    {

    }

    public void SetScoreInfo()
    {
        ScoreText.text = SaveScript.saveData.score.ToString();
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

    public void GoldAbilityTextOn(int gold)
    {
        goldAbilityText.text = "+" + gold + "원";
        goldAbilityText.color = new Color(1f, 1f, 1f, 1f);
        isGoldAbility = true;
    }

    public void BulletAbilityTextOn(int num)
    {
        bulletAbilityText.text = "+" + num;
        bulletAbilityText.color = new Color(1f, 1f, 1f, 1f);
        isBulletAbility = true;
    }
}

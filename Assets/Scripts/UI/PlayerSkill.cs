using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerSkill : MonoBehaviour
{
    private PlayerScript playerScript;
    private PrintUI printUI;
    private ChangeWeaponButton changeWeapon;

    private bool[] isCoolTimeOns, isSkillOns;
    private float[] coolTimes;
    private float[] currentTimes;
    private int[] currentOrders;

    public Image[] skillImages;
    private Text[] skillTexts;

    // BloodCtrl 관련 변수
    [SerializeField] private GameObject bloodPanel;
    private Animator bloodAnimator;
    static public List<Zombie> zombiesList;
    static public List<AI> AIList;
    static public List<LongAttackCircle> longAttackCircleList;
    static public List<LongAttackEffect> longAttackEffectList;
    static public List<GunEffect> gunEffectList;

    private float bloodTime;
    static public float bloodCtrlPercent;

    // BioWeapon 관련 변수
    static public bool isBioWeaponOn;
    static public int weaponType;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = FindObjectOfType<PlayerScript>();
        printUI = FindObjectOfType<PrintUI>();
        changeWeapon = FindObjectOfType<ChangeWeaponButton>();

        zombiesList = new List<Zombie>();
        AIList = new List<AI>();
        longAttackCircleList = new List<LongAttackCircle>();
        longAttackEffectList = new List<LongAttackEffect>();
        gunEffectList = new List<GunEffect>();

        isCoolTimeOns = new bool[2];
        isSkillOns = new bool[isCoolTimeOns.Length];
        coolTimes = new float[isCoolTimeOns.Length];
        currentTimes = new float[isCoolTimeOns.Length];
        currentOrders = new int[isCoolTimeOns.Length];
        skillTexts = GetComponentsInChildren<Text>();

        for (int i = 0; i < skillTexts.Length; i++)
            skillTexts[i].text = "";

        bloodPanel.SetActive(false);
        bloodAnimator = bloodPanel.GetComponent<Animator>();
        bloodTime = 10f;
        bloodCtrlPercent = 0.2f;

        coolTimes[0] = 3f;
        coolTimes[1] = 10f;

        for (int i = 0; i < isCoolTimeOns.Length; i++)
        {
            skillImages[i].fillAmount = 1f;
            skillTexts[i].text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        SkillCoolTime();

        if (isSkillOns[0])
            BloodCtrl();

        if (isSkillOns[1])
            BioWeaponOn();
    }

    public void SkillButton()
    {
        if (playerScript.isStart)
        {
            int type = EventSystem.current.currentSelectedGameObject.GetComponent<ItemOrder>().order;

            switch (type)
            {
                case 0:
                    if (!isSkillOns[type] && !isCoolTimeOns[type])
                    {
                        isSkillOns[type] = true;
                        bloodPanel.SetActive(true);
                        skillImages[type].fillAmount = 0f;
                        printUI.changeModeButton.enabled = false;
                        printUI.specialAIButton.enabled = false;
                        printUI.specialAILeftButton.enabled = false;
                        printUI.specialAIRightButton.enabled = false;
                        printUI.specialAIStopImage.SetActive(true);
                        printUI.changeModeStopImage.SetActive(true);
                    }
                    break;
                case 1:
                    if (!isCoolTimeOns[type])
                    {
                        if (!isSkillOns[type])
                        {
                            isBioWeaponOn = true;
                            isSkillOns[type] = true;
                            skillImages[type].fillAmount = 0f;
                            weaponType = 0;
                            SaveScript.bioGuns[weaponType].currentBulletNum = SaveScript.bioGuns[weaponType].bulletNum;
                            changeWeapon.SettingBioWeapon(weaponType);
                            printUI.changeWeaponButton.enabled = false;
                            printUI.changeWeaponStopImage.SetActive(true);
                        }
                        else
                        {
                            isBioWeaponOn = false;
                            isSkillOns[1] = false;
                            isCoolTimeOns[1] = true;
                            currentTimes[1] = coolTimes[1];
                            changeWeapon.EndBioWeapon();
                            printUI.changeWeaponButton.enabled = true;
                            printUI.changeWeaponStopImage.SetActive(false);
                        }
                    }

                    break;
            }
        }
    }

    public void BioWeaponOn()
    {
        if(SaveScript.bioGuns[weaponType].currentBulletNum == 0)
        {
            isBioWeaponOn = false;
            isSkillOns[1] = false;
            isCoolTimeOns[1] = true;
            currentTimes[1] = coolTimes[1];
            changeWeapon.EndBioWeapon();
            printUI.changeWeaponButton.enabled = true;
            printUI.changeWeaponStopImage.SetActive(false);
        }
    }

    public void BloodCtrl()
    {
        currentTimes[0] += Time.deltaTime;
        CheckLists(bloodCtrlPercent, true);
        
        if (currentOrders[0] == 0)
        {
            if (currentTimes[0] > 1f)
            {
                currentOrders[0] = 1;
                bloodAnimator.SetInteger("FadeInt", 1);
            }
        }

        if(currentOrders[0] == 1)
        {
            if(currentTimes[0] >= bloodTime - 1f && currentTimes[0] < bloodTime)
            {
                bloodAnimator.SetInteger("FadeInt", 2);
            }
            else if(currentTimes[0] >= bloodTime)
            {
                currentOrders[0] = 2;
            }
        }

        // 종료
        if(currentOrders[0] == 2)
        {
            CheckLists(1f / bloodCtrlPercent, false);
            isCoolTimeOns[0] = true;
            isSkillOns[0] = false;
            currentOrders[0] = 0;
            currentTimes[0] = coolTimes[0];
            bloodPanel.SetActive(false);
            bloodAnimator.SetInteger("FadeInt", 0);

            printUI.changeModeButton.enabled = true;
            printUI.specialAIButton.enabled = true;
            printUI.specialAILeftButton.enabled = true;
            printUI.specialAIRightButton.enabled = true;
            printUI.specialAIStopImage.SetActive(false);
            printUI.changeModeStopImage.SetActive(false);
        }
    }

    public void SkillCoolTime()
    {
        for (int i = 0; i < isCoolTimeOns.Length; i++)
        {
            if (isCoolTimeOns[i])
            {
                currentTimes[i] -= Time.deltaTime;

                if(currentTimes[i] < 0f)
                {
                    isCoolTimeOns[i] = false;
                    currentTimes[i] = 0f;
                    skillImages[i].fillAmount = 1f;
                    skillTexts[i].text = "";
                }
                else
                {
                    skillImages[i].fillAmount = 1f - (currentTimes[i] / coolTimes[i]);
                    skillTexts[i].text = ((int)currentTimes[i]).ToString();
                }
            }
        }
    }

    private void CheckLists(float bloodCtrlPercent, bool isOn)
    {
        // 좀비 설정
        for (int i = 0; i < zombiesList.Count; i++)
        {
            if (zombiesList[i] != null)
            {
                if (zombiesList[i].isBloodCtrlOn != isOn && zombiesList[i].animator != null)
                {
                    zombiesList[i].isBloodCtrlOn = isOn;
                    zombiesList[i].moveSpeed *= bloodCtrlPercent;
                    zombiesList[i].currentAttackTime /= bloodCtrlPercent;
                    zombiesList[i].animator.speed *= bloodCtrlPercent;

                    if (zombiesList[i].type == 0)
                    {
                        zombiesList[i].attackTime /= bloodCtrlPercent;
                        zombiesList[i].attackDamageTime /= bloodCtrlPercent;
                    }
                    else if (zombiesList[i].type == 1)
                    {
                        zombiesList[i].longAttackStartTime /= bloodCtrlPercent;
                        zombiesList[i].longAttackTime /= bloodCtrlPercent;
                    }
                }
            }
            else
            {
                zombiesList.RemoveAt(i--);
            }
        }

        // Effect 및 Circle 설정
        for (int i = 0; i < longAttackEffectList.Count; i++)
        {
            if(longAttackEffectList[i] != null)
            {
                if(longAttackEffectList[i].isBloodCtrlOn != isOn && longAttackEffectList[i].gameObject.activeSelf)
                {
                    longAttackEffectList[i].isBloodCtrlOn = isOn;
                    longAttackEffectList[i].animator.speed *= bloodCtrlPercent;
                    longAttackEffectList[i].damageTime /= bloodCtrlPercent;
                    longAttackEffectList[i].duringTime /= bloodCtrlPercent;
                    longAttackEffectList[i].currentTime /= bloodCtrlPercent;
                }
            }
            else
            {
                longAttackEffectList.RemoveAt(i--);
            }
        }

        for (int i = 0; i < longAttackCircleList.Count; i++)
        {
            if (longAttackCircleList[i] != null)
            {
                if(longAttackCircleList[i].isBloodCtrlOn != isOn)
                {
                    longAttackCircleList[i].isBloodCtrlOn = isOn;
                    longAttackCircleList[i].duringTime /= bloodCtrlPercent;
                    longAttackCircleList[i].createTime /= bloodCtrlPercent;
                }
            }
            else
            {
                longAttackCircleList.RemoveAt(i--);
            }
        }

        // AI 설정
        for (int i = 0; i < AIControler.AIs.Length; i++)
        {
            if (AIControler.AIs[i].isBloodCtrlOn != isOn)
            {
                AIControler.AIs[i].isBloodCtrlOn = isOn;

                switch (AIControler.AIs[i].type)
                {
                    case 0:
                        ShotAI data = AIControler.AIs[i] as ShotAI;
                        data.animator.speed *= bloodCtrlPercent;
                        data.moveSpeed *= bloodCtrlPercent;
                        data.ReloadTime /= bloodCtrlPercent;
                        data.ShoutDelayTime /= bloodCtrlPercent;
                        data.currentTime /= bloodCtrlPercent;
                        break;
                }
            }
        }

        for (int i = 0; i < AIControler.StoryAIs.Length; i++)
        {
            if (AIControler.StoryAIs[i].isBloodCtrlOn != isOn)
            {
                AIControler.StoryAIs[i].isBloodCtrlOn = isOn;

                switch (AIControler.StoryAIs[i].type)
                {
                    case 0:
                        ShotAI data = AIControler.StoryAIs[i] as ShotAI;
                        data.animator.speed *= bloodCtrlPercent;
                        data.moveSpeed *= bloodCtrlPercent;
                        data.ReloadTime /= bloodCtrlPercent;
                        data.ShoutDelayTime /= bloodCtrlPercent;
                        data.currentTime /= bloodCtrlPercent;
                        break;
                }
            }
        }

        // GunEffect 설정
        for (int i = 0; i < gunEffectList.Count; i++)
        {
            if(gunEffectList[i] != null)
            {
                if(gunEffectList[i].isBloodCtrlOn != isOn)
                {
                    gunEffectList[i].isBloodCtrlOn = isOn;
                    gunEffectList[i].time *= bloodCtrlPercent;

                    if (gunEffectList[i].GetComponent<Rigidbody2D>() != null)
                    {
                        gunEffectList[i].GetComponent<Rigidbody2D>().gravityScale *= bloodCtrlPercent;
                    }
                }
            }
            else
            {
                gunEffectList.RemoveAt(i--);
            }
        }
    }
}

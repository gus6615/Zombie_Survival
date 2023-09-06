using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialAICtrl : MonoBehaviour
{
    [SerializeField] private GameObject skillImage;
    static public Text coolTimeText;
    static private new Camera camera;
    private PlayerScript player;
    static public Button[] buttons;
    static public SpecialAI currentAI;

    private int data; // indexBox[index]
    private int index; // data의 indexBox에서의 위치
    private int[] indexBox; // SaveScript.saveData.hasSpecialAI가 [x,o,o,x,x,o] 일 경우, indexBox는 [1,2,5] 가 된다.
    static public bool isWork; // 스킬이 사용되었음을 알림

    // Start is called before the first frame update
    void Start()
    {
        skillImage.SetActive(false);
        coolTimeText = GetComponentInChildren<Text>();
        coolTimeText.text = "";
        camera = FindObjectOfType<Camera>();
        player = FindObjectOfType<PlayerScript>();
        buttons = GetComponentsInChildren<Button>();
        
        int count = 0;
        bool[] datas = new bool[SaveScript.specialAINum];
        for (int i = 0; i < datas.Length; i++)
        {
            if (SaveScript.saveData.hasSpecialAI[i])
                datas[i] = SaveScript.saveData.hasSpecialAI[i];
            else
                datas[i] = false;
        }

        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i])
                count++;
        }
        if (count != 0)
            indexBox = new int[count];

        if(indexBox != null)
        {
            data = indexBox[index];

            for (int j = 0; j < indexBox.Length; j++)
            {
                for (int i = 0; i < datas.Length; i++)
                {
                    if (datas[i])
                    {
                        indexBox[j] = i;
                        datas[i] = false;
                        break;
                    }
                }
            }
        }

        int length = 0;
        for (int i = 0; i < SaveScript.specialAINum; i++)
        {
            if (SaveScript.saveData.hasSpecialAI[i])
                length++;
        }

        if (length == 0)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
        else if(length == 1)
        {
            buttons[1].gameObject.SetActive(false);
            buttons[2].gameObject.SetActive(false);
        }

        currentAI = AIControler.SpecialAIs[data].gameObject.GetComponent<SpecialAI>();
    }

    private void Update()
    {
        if (player.isStart && !isWork && indexBox != null)
        {
            SkillCoolTime();
            SetInfo();
        }
    }

    public void AIButtonOn()
    {
        if (player.isStart && !player.isDead && !CameraCtrl.isChange && !ChangeModeButton.isScopeMode)
        {
            if (!isWork && currentAI.isCanUse)
            {
                isWork = true;
                StartCoroutine("skillImageOn");

                // 오브젝트 멈춤
                PlayerScript.animator.speed = 0;
                MapManager.isNotWork = true;
                AIControler.SpecialAIWork();

                // 좀비들 멈춤
                Zombie[] zombies = FindObjectsOfType<Zombie>();
                for (int i = 0; i < zombies.Length; i++)
                {
                    zombies[i].gameObject.GetComponent<Animator>().speed = 0;
                }
            }
        }
    }

    public void LeftButtonOn()
    {
        if (index == 0)
            index = indexBox.Length - 1;
        else
            index--;
        data = indexBox[index];
        currentAI = AIControler.SpecialAIs[data].gameObject.GetComponent<SpecialAI>();
    }

    public void RightButtonOn()
    {
        if (index == indexBox.Length - 1)
            index = 0;
        else
            index++;
        data = indexBox[index];
        currentAI = AIControler.SpecialAIs[data].gameObject.GetComponent<SpecialAI>();
    }

    IEnumerator skillImageOn()
    {
        skillImage.SetActive(true);
        yield return new WaitForSeconds(2f);
        currentAI.gameObject.SetActive(true);
        currentAI.isWork = true;
        currentAI.isStartAct = true;
        yield return new WaitForSeconds(1f);
        skillImage.SetActive(false);
    }

    public void SkillCoolTime()
    {
        for (int i = 0; i < indexBox.Length; i++)
        {
            SpecialAI data = AIControler.SpecialAIs[indexBox[i]].gameObject.GetComponent<SpecialAI>();

            if (data.currentCoolTime < 0.01f) // 스킬 사용 가능
            {
                data.isCanUse = true;
                data.currentCoolTime = 0f;
            }
            else if (currentAI.currentCoolTime > 0.01f) // 스킬 쿨타임 적용 중
            {
                data.isCanUse = false;
                data.currentCoolTime -= Time.deltaTime;
            }
        }
    }

    public void SetInfo()
    {
        if (currentAI.isCanUse)
        {
            coolTimeText.text = "";
            buttons[0].GetComponent<Image>().fillAmount = 1f;
        }
        else
        {
            coolTimeText.text = ((int)currentAI.currentCoolTime).ToString();
            buttons[0].GetComponent<Image>().fillAmount = 1f - (currentAI.currentCoolTime / currentAI.coolTime);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryScript : MonoBehaviour
{
    [SerializeField] private GameObject[] storyTypeObject;
    [SerializeField] private Text toonText, tellingText, nameText;
    [SerializeField] private Image toonImage, backgroundImage;
    [SerializeField] private Image leftImage, rightImage;

    static public bool isSellectedStory; // 플레이어가 직접 선택하여 보는 경우
    static private int sceneNumber; // 스토리의 분별을 담당, 진행 형식과 배경, 대사 등 모든 요소를 결정 짓는 변수
    private int sceneType; // 0 = 배경 전달 형식(toon형식), 1 = 대화 전달 형식(telling형식)
    private List<string> dialogs; // 대화를 담고 있다.
    public int currentDialogNum; // 최근에 진행된 대화의 순서
    public bool isDialoging; // 현재 대화가 진행중임을 나타낸다. 조작 불가
    static public bool isNext; // true가 되면 씬이 변경된다.

    // Start is called before the first frame update
    void Start()
    {
        dialogs = new List<string>();

        SettingScene();
        StartCoroutine(ChangeDialog(sceneType, dialogs[currentDialogNum]));
    }

    // Update is called once per frame
    void Update()
    {
        if (BlindScript.isEndChange)
        {
            ShowScene();
        }
    }

    public void SettingScene()
    {
        if (isSellectedStory)
            isSellectedStory = false;
        else
            sceneNumber = SaveScript.saveData.storyIndex;

        switch (sceneNumber)
        {
            case 0:
                dialogs.Add("만약 사람이 괴물들이 득실대는 세상에서" + "\n" + "살아남아야 한다면 어떻게 행동해야 할 것인가.");
                dialogs.Add("어떤 사람들은 식량과 물을 구축하여" + "\n" + "방에서 구조를 기다리는 은둔자가 되거나");
                dialogs.Add("인류의 생존을 위해 강인한 기사가" + "\n" + "되려는 사람들도 있을 것이다.");
                dialogs.Add("이 이야기는 은둔자고, 기사도 아닌...");
                dialogs.Add("사랑하는 연인을 구하기 위한 한 남자의 긴 여정의 이야기이다.");
                sceneType = 0;
                break;
        }

        for (int i = 0; i < storyTypeObject.Length; i++)
            storyTypeObject[i].SetActive(false);
        storyTypeObject[sceneType].SetActive(true);
    }

    public void Skip()
    {
        isNext = true;
    }

    static public void SelectStory(int data)
    {
        isSellectedStory = true;
        sceneNumber = data;
    }

    public void ShowScene()
    {
        if (currentDialogNum == dialogs.Count && Input.anyKeyDown)
            isNext = true;

        switch (sceneNumber)
        {
            case 0: // 게임 첫 시작
                if (!isDialoging)
                {
                    if (Input.anyKeyDown)
                    {
                        // ChangeImage(sceneType, );
                        StartCoroutine(ChangeDialog(sceneType, dialogs[currentDialogNum]));
                    }
                }
                break;

            case 1:
                break;

        }
    }

    IEnumerator ChangeDialog(int type, string data)
    {
        if(type == 0)
        {
            isDialoging = true;
            currentDialogNum++;
            toonText.text = "";
            char[] temp;

            for (int i = 0; i < data.Length; i++)
            {
                temp = data.ToCharArray();
                toonText.text += temp[i];

                yield return new WaitForSeconds(0.05f);
            }

            if (currentDialogNum != dialogs.Count)
                isDialoging = false;
        }
        else
        {
            isDialoging = true;
            currentDialogNum++;
            tellingText.text = "";
            char[] temp;

            for (int i = 0; i < data.Length; i++)
            {
                temp = data.ToCharArray();
                tellingText.text += temp[i];

                yield return new WaitForSeconds(0.05f);
            }

            if (currentDialogNum != dialogs.Count)
                isDialoging = false;
        }
    }

    public void ChangeImage(int type, Image data)
    {
        if (type == 0)
        {

        }
        else
        {

        }
    }
}

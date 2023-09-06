using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Anima2D;

public class StageController : MonoBehaviour
{
    [SerializeField] private GameObject[] zombies;
    [SerializeField] private Image[] countsText;
    [SerializeField] private Image stageDistanceImage;
    [SerializeField] private GameObject[] clearObjects; // 클리어 UI를 담고 있는 오브젝트 (0 = 승리, 1 = 패배, 2 = 보상 UI)
    [SerializeField] private GameObject playerWeaponObject; // 플레이어의 무기이미지를 담고 있는 오브젝트
    private SkinnedMeshRenderer[] playerWeapons;
    private Image[] clearImages; // 클리어시 생겨날 보상 UI 이미지들
    private Text[] clearTexts; // 클리어시 생겨날 보상 UI 텍스트들
    public Stage stage;

    private PlayerScript playerScript;
    private new Camera camera;
    private Transform startTr;
    private Tutorial tutorial;
    private List<Zombie> backZombies;

    private float cameraDis;
    private int countIndex;
    private bool isStart; // 카운트가 시작이 된 경우 true
    private bool isCount; // 카운트 효과가 시작이 된 경우 true
    private bool isGameStart; // 게임시작이 된 경우 true
    private bool isClear; // 클리어를 모두 완료한 경우 true ( 클리어 UI가 동작을 완료한 경우에 true가 됨. )
    static public bool isClearStart; // 클리어 UI가 뜨기 전, 즉 최초의 상태 ( 다른 스크립트에서 접근 )
    static public bool isScoreMode;
    public bool isGoToShelter;
    private float stageDistanceStartPosX;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = FindObjectOfType<PlayerScript>();
        camera = FindObjectOfType<Camera>();
        tutorial = FindObjectOfType<Tutorial>();
        playerWeapons = playerWeaponObject.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        backZombies = new List<Zombie>();
        clearImages = clearObjects[2].GetComponentsInChildren<Image>();
        clearTexts = clearObjects[2].GetComponentsInChildren<Text>();

        isClearStart = false;

        for (int i = 0; i < countsText.Length; i++)
            countsText[i].gameObject.SetActive(false);

        Image[] images = clearObjects[0].transform.parent.GetComponentsInChildren<Image>();
        Text[] texts = clearObjects[0].transform.parent.GetComponentsInChildren<Text>();

        for (int i = 0; i < images.Length; i++) // 클리어 이미지 초기화
            images[i].color = new Color(1f, 1f, 1f, 0f);

        for (int i = 0; i < texts.Length; i++) // 클리어 텍스트 초기화
            texts[i].color = new Color(0f, 0f, 0f, 0f);

        for (int i = 0; i < clearObjects.Length; i++)
            clearObjects[i].SetActive(false);

        for (int i = 0; i < SaveScript.saveData.hasGuns.Count; i++) // 플레이어의 모든 총기 탄창 정비
        {
            if (SaveScript.saveData.hasGuns[i])
            {
                if(i == 0)
                {
                    SaveScript.guns[i].currentBulletNum = SaveScript.guns[i].bulletNum;
                }
                else
                {
                    int bulletNum = (int)Mathf.Round(SaveScript.guns[i].bulletNum * (1 + 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[i][2]) - 1)));

                    if (bulletNum <= SaveScript.saveData.hasGunsBullets[i])
                    {
                        SaveScript.guns[i].currentBulletNum = bulletNum;
                        SaveScript.saveData.hasGunsBullets[i] -= bulletNum;
                    }
                    else
                    {
                        SaveScript.guns[i].currentBulletNum = SaveScript.saveData.hasGunsBullets[i];
                        SaveScript.saveData.hasGunsBullets[i] = 0;
                    }
                }
            }
        }

        cameraDis = camera.orthographicSize * camera.pixelWidth / camera.pixelHeight;
        stage = FindObjectOfType<Stage>();

        isStart = true;
        stageDistanceStartPosX = stageDistanceImage.rectTransform.localPosition.x; // 약 -92

        CameraCtrl.ChangeCameraSize(SaveScript.guns[SaveScript.saveData.equipGun].shotDis);
    }

    // Update is called once per frame
    void Update()
    {
        SetDistance();

        if (stage.currentZombieNum != 0) // 필드 좀비 생성
            CreateZombie();

        if ((tutorial == null || tutorial.isTutorialDone) && BlindScript.isEndChange)
        {
            if (isStart) // 카운트 다운
                StartCoroutine("CountDown");

            if (isCount) // 카운트 다운 효과
                ChangeCountText();

            if (isGameStart && !isClear) // 게임 시작
            { 
                if (!playerScript.isDead)
                {
                    if (!playerScript.isStart && !SpecialAICtrl.isWork)
                    {
                        playerScript.isStart = true;
                    }

                    if (stage.isCreateBackZombieOn && !SpecialAICtrl.isWork) // 후방 좀비 생성
                        StartCoroutine("CreateBackZombie");
                }
                else // 사망 이벤트
                    ClearStage(false);
            }

            if (playerScript.transform.position.x >= stage.distance && !isClear) // 클리어
                ClearStage(true); 
        }
    }

    public void CreateZombie()
    {
        float randPosX;
        float randPosY;

        List<Zombie> list = new List<Zombie>();

        for (int i = 0; i < stage.zombiesNum.Length; i++) // 좀비 종류 수
        {
            float distance = (stage.distance - CameraCtrl.cameraRadius * 1.5f) / stage.zombiesNum[i];

            for (int count = 0; count < stage.zombiesNum[i]; count++) // 한 종류의 좀비 수
            {
                if (count + 1 >= stage.zombiesNum[i])
                    randPosX = Random.Range(playerScript.transform.position.x + CameraCtrl.cameraRadius * 1.5f + count * distance, stage.distance - 5f);
                else
                    randPosX = Random.Range(playerScript.transform.position.x + CameraCtrl.cameraRadius * 1.5f + count * distance,
                        playerScript.transform.position.x + CameraCtrl.cameraRadius * 1.5f + (count + 1) * distance);

                randPosY = Random.Range(playerScript.transform.position.y - 1f, playerScript.transform.position.y + 1f);

                Zombie data = Instantiate(zombies[i], new Vector3(randPosX, randPosY, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Zombie>();
                list.Add(data);
                PlayerSkill.zombiesList.Add(data);
                stage.currentZombieNum--;
            }
        }

        // 좀비들의 이미지 우선순위 설정
        list.Sort(delegate(Zombie A, Zombie B)
        {
            if (A.orderTr.position.y > B.orderTr.position.y)
                return 1;
            else if (A.orderTr.position.y < B.orderTr.position.y)
                return -1;
            else
                return 0;
        });

        for (int i = 0; i < list.Count; i++)
        {
            ImageOrder[] temps = list[i].GetComponentsInChildren<ImageOrder>(); // imageorder을 가지고 있는 spritemesh만 부른다.
            for (int j = 0; j < temps.Length; j++)
                temps[j].GetComponent<SpriteMeshInstance>().sortingOrder = list.Count - i;
        }
    }

    IEnumerator CreateBackZombie()
    {
        int type = 0;

        while (Percent.GetRandFlag(1f / Mathf.Pow(2, type + 1)))
            type++;
        if (type >= stage.zombiesNum.Length)
            type = stage.zombiesNum.Length - 1;
        stage.isCreateBackZombieOn = false;

        GameObject data = Instantiate(zombies[type], new Vector2(playerScript.transform.position.x - CameraCtrl.cameraRadius * 1.9f,
            Random.Range(playerScript.transform.position.y - 1f, playerScript.transform.position.y + 1f)), new Quaternion(0, 0, 0, 0));
        data.GetComponent<Zombie>().isAllWork = true;
        backZombies.Add(data.GetComponent<Zombie>());
        PlayerSkill.zombiesList.Add(data.GetComponent<Zombie>());

        backZombies.Sort(delegate (Zombie A, Zombie B)
        {
            if (A != null && B != null)
            {
                if (A.orderTr.position.y > B.orderTr.position.y)
                    return 1;
                else if (A.orderTr.position.y < B.orderTr.position.y)
                    return -1;
                else
                    return 0;
            }
            else
                return 0;
        });

        for (int i = 0; i < backZombies.Count; i++)
        {
            if (backZombies[i] != null)
            {
                ImageOrder[] temps = backZombies[i].GetComponentsInChildren<ImageOrder>();
                for (int j = 0; j < temps.Length; j++)
                    temps[j].GetComponent<SpriteMeshInstance>().sortingOrder = backZombies.Count - i;
            }
        }

        yield return new WaitForSeconds(Random.Range(1f, stage.createBackZombieTime) + type);
        if(playerScript.isStart)
            stage.isCreateBackZombieOn = true;
    }

    IEnumerator CountDown()
    {
        isStart = false;

        for (int count = 2; count >= 0; count--)
        {
            isCount = true;
            countIndex = count;
            for (int i = 0; i < countsText.Length; i++)
            {
                if(i == countIndex)
                    countsText[i].gameObject.SetActive(true);
                else
                    countsText[i].gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(1f);
        }

        countsText[countIndex].gameObject.SetActive(false);
        isCount = false;
        isGameStart = true;
    }

    public void ChangeCountText()
    {
        if(countsText[countIndex].color.a >= 0.02f)
        {
            Color temp = countsText[countIndex].color;
            temp.a -= Time.deltaTime;
            countsText[countIndex].color = temp;

            float currentScale = countsText[countIndex].rectTransform.localScale.x + Time.deltaTime * 0.2f;
            countsText[countIndex].rectTransform.localScale = new Vector3(currentScale, currentScale, currentScale);

            countsText[countIndex].rectTransform.localPosition += new Vector3(0, - Time.deltaTime * 10f, 0);
        }
    }

    public void ClearStage(bool _isClear)
    {
        if (stage.stageNum == SaveScript.saveData.stage && stage.stageLevelNum == SaveScript.saveData.stage_level && !playerScript.isDead) // 스테이지 확장
        {
            SaveScript.saveData.stage_level++;

            if(SaveScript.saveData.stage_level == 5)
            {
                SaveScript.saveData.stage_level = 0;
                SaveScript.saveData.stage++;
            }
        }

        isClearStart = true;
        CameraCtrl.AimSetCamera(0f);
        CameraCtrl.mediatedAimDis = 0;
        MoveCtrl.moveVec = Vector2.zero;

        if (_isClear)
        {
            clearObjects[0].SetActive(true);
            clearObjects[1].SetActive(false);
        }
        else
        {
            clearObjects[0].SetActive(false);
            clearObjects[1].SetActive(true);
        }

        clearObjects[2].SetActive(true);
        playerScript.isStart = false;
        PlayerScript.moveSpeed = 0f;

        // ClearLogo UI 설정
        clearTexts[0].text = "+" + playerScript.stageGold + "원"; // 돈
        clearTexts[1].text = "+" + playerScript.stageExp; // 경험치

        // ItemSetting
        int count = 0;
        for (int i = 0; i < playerScript.stageItems.Length; i++)
        {
            if(playerScript.stageItems[i] != 0)
            {
                clearTexts[2 + count].text = "+" + playerScript.stageItems[i] + "개";
                clearImages[2 + count++].sprite = SaveScript.etcs[i].image.sprite;
            }
        }

        for (int i = 0; i < 9 - count; i++)
        {
            clearTexts[2 + count + i].text = "";
            clearImages[2 + count + i].sprite = null;
            clearImages[2 + count + i].color = new Color(1f, 1f, 1f, 0f);
        }

        Image[] images = clearObjects[0].transform.parent.GetComponentsInChildren<Image>();
        Text[] texts = clearObjects[0].transform.parent.GetComponentsInChildren<Text>();

        for (int i = 0; i < images.Length; i++) // 클리어 이미지 띄우기
        {
            if (images[i].color.a <= 0.85f)
            {
                Color temp = images[i].color;
                temp.a += Time.deltaTime;
                images[i].color = temp;
            }
            else
                isClear = true;
        }

        for (int i = 0; i < texts.Length; i++) // 클리어 텍스트 띄우기
        {
            if (texts[i].color.a <= 0.85f)
            {
                Color temp = texts[i].color;
                temp.a += Time.deltaTime;
                texts[i].color = temp;
            }
            else
                isClear = true;
        }
    }

    public void ClearButton()
    {
        ShoutButtonCtrl.isReload = false; // 리로드 버그 방지
        for (int i = 0; i < SaveScript.weaponNum; i++) // 총알 재정비
        {
            if (SaveScript.saveData.hasGuns[i])
            {
                if(i != 0)
                {
                    SaveScript.saveData.hasGunsBullets[i] += SaveScript.guns[i].currentBulletNum;
                    SaveScript.guns[i].currentBulletNum = 0;
                }
            }
        }
               
        isGoToShelter = true;
    }

    public void SetDistance() // 플레이어의 거리를 알려주는 회색 점을 움직이도록 하는 메소드
    {
        float percent = Mathf.Abs(stageDistanceStartPosX) * 2 / (stage.distance + MoveCtrl.backDis); // 스테이지 거리 대비 StagePosImage의 상대적 X 값

        stageDistanceImage.rectTransform.localPosition = new Vector3(stageDistanceStartPosX + percent * (playerScript.transform.position.x + MoveCtrl.backDis), 0, 0);
    }

    public Stage GetStage()
    {
        return stage;
    }
}

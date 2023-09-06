using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeModeButton : MonoBehaviour
{
    private PrintUI printUI;
    static private new Camera camera;
    private RectTransform rectTransform; // 스코프 이미지의 렉트트렌스폼
    private PlayerScript player;
    static public Image scopeImage; 
    private Color temp;
    static public Vector3 startPos; // 터치가 끝날 때의 카메라 위치 벡터
    private Vector3 playerTr;
    static public Vector2 shotEffectVec; // 총기 반동 벡터

    static public bool isScopeMode;
    static public bool isChangeColor;
    static public bool isScopeMove;
    static public bool isBack;
    private float savedImageX, savedImageY;
    static public float scopeMoveSpeed; // 스코프 감도

    // Scope Bullet UI 관련 변수
    [SerializeField] private GameObject scopeBulletUI;
    [SerializeField] private GameObject scopeBulletPrefab;
    private RectTransform ScopeBullet_RectTr;
    private GameObject scopeBulletPanel;
    static public List<GameObject> scopeBullets;
    private Vector2 scopeBullet_savedPos, scopeBullet_GoalPos;
    private bool isStartScopeBullet, isEndScopeBullet;

    static public bool isCoolTime;
    static public float sniperCoolTime; // 기본 재사용 시간
    static public float sniperCurrentCoolTime; // 최근 재사용 시간
    private Text coolTimeText;
    private Image modeChangeImage;

    private void Start()
    {
        RectTransform[] temps = FindObjectOfType<StageController>().GetComponentsInChildren<RectTransform>();
        for (int i = 0; i < temps.Length; i++)
            if(temps[i].gameObject.name == "Scope")
            {
                scopeImage = temps[i].GetComponent<Image>();
                break;
            }
        printUI = FindObjectOfType<PrintUI>();
        player = FindObjectOfType<PlayerScript>();
        playerTr = player.transform.localScale;
        camera = FindObjectOfType<Camera>();
        rectTransform = scopeImage.GetComponent<RectTransform>();
        startPos =  CameraCtrl.cameraPos;

        temp = new Color(1, 1, 1, 0);
        scopeImage.color = temp;
        savedImageX = camera.ScreenToWorldPoint(scopeImage.transform.position).x;
        savedImageY = camera.ScreenToWorldPoint(scopeImage.transform.position).y;
        scopeMoveSpeed = 1f;
        isScopeMode = isScopeMove = isBack = false;

        scopeBulletPanel = scopeBulletUI.GetComponentInChildren<GridLayoutGroup>().gameObject;
        scopeBullets = new List<GameObject>();
        scopeBullet_savedPos = new Vector2(-200f, 70f);
        ScopeBullet_RectTr = scopeBulletUI.GetComponent<RectTransform>();
        scopeBullet_GoalPos = new Vector2(0f, 70f);
        isCoolTime = false;
        sniperCoolTime = 5f;
        coolTimeText = FindObjectOfType<ChangeModeButton>().GetComponentInChildren<Text>();
        coolTimeText.text = "";
        modeChangeImage = FindObjectOfType<ChangeModeButton>().GetComponent<Image>();

        ScopeBullet_RectTr.anchoredPosition = scopeBullet_savedPos;
    }

    private void Update()
    {
        if (isChangeColor)
            ChangeColor();

        if (isStartScopeBullet)
            StartScopeUI();

        if (isEndScopeBullet)
            EndScopeUI();

        // 스나이퍼 모드
        if (isCoolTime)
            SkillCoolTime();

        if (isScopeMode)
        {
            if (isScopeMove)
                MoveAim(); // 조준점 이동

            if (ScopeShotButton.sniperCurrentBulletNum == 0)
                ButtonOn();
        }
    }

    public void SkillCoolTime()
    {
        if (sniperCurrentCoolTime < 0.01f) // 스킬 사용 가능
        {
            sniperCurrentCoolTime = 0f;
            coolTimeText.text = "";
            modeChangeImage.fillAmount = 1f;
            isCoolTime = false;
        }
        else if (sniperCurrentCoolTime > 0.01f) // 스킬 쿨타임 적용 중
        {
            sniperCurrentCoolTime -= Time.deltaTime;
            coolTimeText.text = ((int)sniperCurrentCoolTime).ToString();
            modeChangeImage.fillAmount = 1f - (sniperCurrentCoolTime / sniperCoolTime);
        }
    }

    public void ButtonOn()
    {
        if (player.isStart && !isChangeColor)
        {
            bool isWork = false;

            if (isScopeMode) // 집중 사격 모드 OFF
            {
                isWork = true;
                sniperCurrentCoolTime = sniperCoolTime;
                ScopeShotButton.sniperCurrentBulletNum = ScopeShotButton.sniperBulletNum;
                ScopeShotButton.isShottingOn = false;
                ScopeShotButton.isShout = false;
                isCoolTime = true;
                CameraCtrl.ChangeCameraSize(SaveScript.guns[SaveScript.saveData.equipGun].shotDis);
                EffectScript.SettingFade(printUI.print_ScopeUI, 1f, 0.5f);

                printUI.nonScopeObject.SetActive(true);
                printUI.scopeObject.SetActive(false);

                isStartScopeBullet = false;
                isEndScopeBullet = true;
                startPos = CameraCtrl.cameraPos;
                temp = new Color(1, 1, 1, 1);

                if (ShoutButtonCtrl.isReload)
                    ShoutButtonCtrl.isReload = false;
            }
            else if (!isCoolTime)// 집중 사격 모드 ON
            {
                isWork = true;
                temp = new Color(1, 1, 1, 0);
                CameraCtrl.ChangeCameraSize(15f);
                EffectScript.SettingFade(printUI.print_ScopeUI, 0f, 0.5f);

                printUI.nonScopeObject.SetActive(false);
                printUI.scopeObject.SetActive(true);
                SettingBullet();

                isStartScopeBullet = true;
                isEndScopeBullet = false;
            }

            if (isWork)
            {
                isScopeMode = !isScopeMode;
                isScopeMove = false;
                isChangeColor = true;
                camera.transform.localPosition = CameraCtrl.cameraPos;
                player.transform.localScale = new Vector3(playerTr.x, playerTr.y, playerTr.z);
            }
        }
    }

    public void ChangeColor()
    {
        if (isScopeMode)
        {
            if (scopeImage.color.a <= 1f)
            {
                temp = new Color(1, 1, 1, temp.a + Time.deltaTime);
                scopeImage.color = temp;
            }
            else
            {
                scopeImage.color = new Color(1, 1, 1, 1);
                isChangeColor = false;
            }
        }
        else
        {
            if (scopeImage.color.a >= 0f)
            {
                temp = new Color(1, 1, 1, temp.a - Time.deltaTime);
                scopeImage.color = temp;
            }
            else
            {
                scopeImage.color = new Color(1, 1, 1, 0);
                isChangeColor = false;
            }
        }
    }

    public void MoveAim()
    {
        camera.transform.localPosition = startPos + (Vector3)ScopeMoveCtrl.moveVec;

        if (camera.transform.localPosition.x >= 100f)
            camera.transform.localPosition = new Vector3(100f, camera.transform.localPosition.y, -10f);
        else if (camera.transform.localPosition.x <= -100f)
            camera.transform.localPosition = new Vector3(-100f, camera.transform.localPosition.y, -10f);

        if (camera.transform.localPosition.y >= 80f)
            camera.transform.localPosition = new Vector3(camera.transform.localPosition.x, 30f, -10f);
        else if(camera.transform.localPosition.y <= -5f)
            camera.transform.localPosition = new Vector3(camera.transform.localPosition.x, -5f, -10f);

    }

    private void StartScopeUI()
    {
        float data = (scopeBullet_GoalPos.x - ScopeBullet_RectTr.anchoredPosition.x) * 5f * Time.deltaTime;
        ScopeBullet_RectTr.anchoredPosition += new Vector2(data, 0f);

        if(scopeBullet_GoalPos.x < ScopeBullet_RectTr.anchoredPosition.x)
        {
            isStartScopeBullet = false;
            ScopeBullet_RectTr.anchoredPosition = scopeBullet_GoalPos;
        }
    }

    private void EndScopeUI()
    {
        float data = (ScopeBullet_RectTr.anchoredPosition.x - scopeBullet_savedPos.x) * 5f * Time.deltaTime;
        ScopeBullet_RectTr.anchoredPosition -= new Vector2(data, 0f);

        if (scopeBullet_savedPos.x > ScopeBullet_RectTr.anchoredPosition.x)
        {
            isEndScopeBullet = false;
            ScopeBullet_RectTr.anchoredPosition = scopeBullet_savedPos;
        }
    }

    private void SettingBullet()
    {
        for (int i = 0; i < scopeBulletPanel.transform.childCount; i++)
            Destroy(scopeBulletPanel.transform.GetChild(i).gameObject);

        scopeBullets.Clear();

        for (int i = 0; i < ScopeShotButton.sniperBulletNum; i++)
        {
            scopeBullets.Add(Instantiate(scopeBulletPrefab, scopeBulletPanel.transform));
        }
    }
}

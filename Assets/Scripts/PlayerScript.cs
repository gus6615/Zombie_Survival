using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Anima2D;

public class PlayerScript : MonoBehaviour
{
    static public Animator animator;
    private PrintUI printUI;
    public GameObject DamageObject;
    static private new Camera camera;
    private List<AI> AIs; // 게임 내 존재하는 모든 AI들
    private SpriteMeshInstance[] player_images; // 플레이어 이미지들
    [SerializeField] private GameObject prefabSpace;

    static public float moveSpeed, jumpSpeed;
    static public float isAttackedDis;
    static public bool isGetHP;
    public bool isStart;
    public float HP, MaxHP; // HP는 현재 체력, MaxHP는 체력의 최대치
    public bool isDead;
    public int stageGold; // 보상 골드
    public int[] stageItems; // 보상 아이템들 숫자 (index 는 weapon + armor - etcItemCode 이다.)
    public int stageExp; // 보상 exp

    public bool isImageFadeOn;
    private int fadeCount;
    private Color fadeColor;
    private bool isFadeOff;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        printUI = FindObjectOfType<PrintUI>();
        camera = FindObjectOfType<Camera>();
        fadeColor = Color.white;
        player_images = GetComponentsInChildren<SpriteMeshInstance>();

        isAttackedDis = 3f;
        isStart = false;
        MaxHP = SaveScript.saveData.HP + (SaveScript.saveData.HPUpgrade * SaveScript.upgradeDatas[1] + (SaveScript.saveData.level - 1) * SaveScript.upgradeDatas[1]);
        HP = MaxHP;
        stageItems = new int[SaveScript.etcNum];

        // AI들 이미지 우선순위 결정

        AIs = new List<AI>();
        AIs.AddRange(FindObjectOfType<StageController>().GetComponentsInChildren<AI>());
        AIs.Sort(delegate (AI A, AI B)
        {
            if (A.transform.Find("FootPosition").position.y > B.transform.Find("FootPosition").position.y)
                return 1;
            else if (A.transform.Find("FootPosition").position.y < B.transform.Find("FootPosition").position.y)
                return -1;
            else
                return 0;
        });

        for (int i = 0; i < AIs.Count; i++)
        {
            ImageOrder[] temps = AIs[i].GetComponentsInChildren<ImageOrder>(); // imageorder을 가지고 있는 spritemesh만 부른다.
            ItemOrder weapon = AIs[i].GetComponentInChildren<ItemOrder>();

            for (int j = 0; j < temps.Length; j++)
                temps[j].GetComponent<SpriteMeshInstance>().sortingOrder = (AIs.Count - i) * 2;
            weapon.GetComponent<SpriteMeshInstance>().sortingOrder = (AIs.Count - i) * 2 - 1;
        }

        SetImageOrder();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart && !SpecialAICtrl.isWork)
        {
            this.transform.position += Vector3.right * moveSpeed * Time.deltaTime + Vector3.up * jumpSpeed * Time.deltaTime;
            animator.SetBool("isStart", true);

            if (isImageFadeOn)
                SetImageFade();

            if (SaveScript.saveData.equipArmor != -1) // 방어구 착용 시
            {
                if (SaveScript.armors[SaveScript.saveData.equipArmor].HPCure != 0 || 
                    (int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[SaveScript.saveData.equipArmor][2]) > 1) // 회복
                {
                    StartCoroutine("HPCuring");
                    // 공격 반사는 좀비 스크립트에 존재한다.
                }
            }

            if (isGetHP) // 흡혈 효과
            {
                isGetHP = false;
                if(HP != MaxHP)
                {
                    float realGetHP = Mathf.Round(SaveScript.guns[SaveScript.saveData.equipGun].damage * SaveScript.gunsAbilitys[5].data * 0.01f
                        * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][5]) - 1) * 10f) / 10f;

                    GameObject damageObject = Instantiate(DamageObject, camera.WorldToScreenPoint(this.transform.position + Vector3.up * 0.5f), new Quaternion(0, 0, 0, 0), prefabSpace.transform);
                    Image[] tempImage = damageObject.GetComponentsInChildren<Image>(); // 0 = 헤드샷, 1 = 출혈
                    for (int j = 0; j < tempImage.Length; j++) // 데미지 이미지 false로 초기화
                        tempImage[j].gameObject.SetActive(false);

                    tempImage[2].gameObject.SetActive(true);
                    damageObject.GetComponent<DamageEffect>().isStart = true;
                    damageObject.GetComponentInChildren<Text>().text = realGetHP.ToString();
                    damageObject.GetComponentInChildren<Text>().color = Color.green;

                    HP += realGetHP;
                    if (HP >= MaxHP)
                        HP = MaxHP;

                    printUI.HPText.text = HP + " / " + MaxHP;
                    printUI.HPSlider.value = HP;
                }
            }
        }
        else
        {
            animator.SetBool("isStart", false);
        }

        if (HP <= 0f)
            isDead = true;
   
        if (isDead)
        {
            moveSpeed = jumpSpeed = 0f;
            animator.SetBool("isAttacked", false);
            animator.SetBool("isDead", true);
            ShoutButtonCtrl.isTouch = false;
            printUI.HPText.text = "0 / " + MaxHP;
        }
    }

    public void SetImageFade()
    {
        SpriteMeshInstance[] datas = GetComponentsInChildren<SpriteMeshInstance>();

        if (fadeCount < 4)
        {
            if (fadeColor.a < 0.3f)
                isFadeOff = true;

            if (!isFadeOff)
                fadeColor.a -= Time.deltaTime * 5f;
            else
            {
                fadeColor.a += Time.deltaTime * 5f;
                if(fadeColor.a >= 1f)
                {
                    fadeCount++;
                    isFadeOff = false;
                }
            }
                
            for (int i = 0; i < datas.Length; i++)
            {
                datas[i].color = fadeColor;
            }
        }
        else
        {
            isImageFadeOn = false;
            isFadeOff = false;
            fadeCount = 0;
            fadeColor = Color.white;

            for (int i = 0; i < datas.Length; i++)
            {
                datas[i].color = fadeColor;
            }
        }
    }

    public void SetImageOrder() // Y축에 따른 이미지 우선순위
    {
        for (int i = 0; i < AIs.Count; i++)
        {
            if (AIs[i].transform.Find("FootPosition").position.y > this.transform.Find("FootPosition").position.y)
            {
                for (int j = 0; j < player_images.Length; j++)
                    player_images[j].sortingOrder = AIs[i].GetComponentInChildren<SpriteMeshInstance>().sortingOrder + 1;
                break;
            }

            if(i == AIs.Count - 1)
                for (int j = 0; j < player_images.Length; j++)
                    player_images[j].sortingOrder = 1;
        }
    }

    IEnumerator HPCuring()
    {
        if (!SaveScript.armors[SaveScript.saveData.equipArmor].isHPCureTime)
        {
            SaveScript.armors[SaveScript.saveData.equipArmor].isHPCureTime = true;

            if(HP != MaxHP)
            {
                float realGetHP = SaveScript.armors[SaveScript.saveData.equipArmor].HPCure * (1 + SaveScript.armorsAbilitys[2].data * 0.01f
                    * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[SaveScript.saveData.equipArmor][2]) - 1));

                GameObject damageObject = Instantiate(DamageObject, camera.WorldToScreenPoint(this.transform.position + Vector3.up * 0.5f), new Quaternion(0, 0, 0, 0), prefabSpace.transform);
                Image[] tempImage = damageObject.GetComponentsInChildren<Image>(); // 0 = 헤드샷, 1 = 출혈
                for (int j = 0; j < tempImage.Length; j++) // 데미지 이미지 false로 초기화
                    tempImage[j].gameObject.SetActive(false);

                tempImage[2].gameObject.SetActive(true);
                damageObject.GetComponent<DamageEffect>().isStart = true;
                damageObject.GetComponentInChildren<Text>().text = (Mathf.Round(realGetHP * 10f) / 10f).ToString();
                damageObject.GetComponentInChildren<Text>().color = Color.green;

                HP += realGetHP;
                if (HP >= MaxHP)
                    HP = MaxHP;

                printUI.HPText.text = Mathf.Round(HP * 10f) / 10f + " / " + MaxHP;
                printUI.HPSlider.value = HP;
            }

            yield return new WaitForSeconds(1f);
            SaveScript.armors[SaveScript.saveData.equipArmor].isHPCureTime = false;
        }
    }
}

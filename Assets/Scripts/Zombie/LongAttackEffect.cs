using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LongAttackEffect : MonoBehaviour
{
    [SerializeField] private GameObject DamageObject;
    private PrintUI printUI;
    public Animator animator;
    private PlayerScript player;

    public Vector2 damageVec;
    public bool isBloodCtrlOn;
    public int type;
    public int damage;
    public float duringTime;
    public float damageTime;

    public float currentTime;
    private bool isDamage;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerScript>();
        printUI = FindObjectOfType<PrintUI>();

        ItemOrder[] datas = this.GetComponentsInChildren<ItemOrder>();
        for (int i = 0; i < datas.Length; i++)
            datas[i].gameObject.SetActive(false);
        datas[type].gameObject.SetActive(true);

        animator.SetInteger("isType", type);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if(currentTime >= damageTime && !isDamage)
        {
            isDamage = true;
            
            if(Mathf.Abs(player.transform.position.x - transform.position.x) <= damageVec.x && Mathf.Abs(player.transform.position.y - 2.5f - transform.position.y) <= damageVec.y * 0.5f)
            {
                float tempDamage = Mathf.Round(damage * 10f) / 10f;
                if (SaveScript.saveData.equipArmor != -1)
                {
                    tempDamage = damage * (1 - (SaveScript.armors[SaveScript.saveData.equipArmor].armor + SaveScript.armorsAbilitys[1].data * 0.01f
                    * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[SaveScript.saveData.equipArmor][1]) - 1)));
                    if (tempDamage < 1)
                        tempDamage = 1;
                }
               
                player.isImageFadeOn = true;

                GameObject damageObject = Instantiate(DamageObject, Camera.main.WorldToScreenPoint(player.transform.position + Vector3.up * 0.5f), new Quaternion(0, 0, 0, 0),
                    FindObjectOfType<StageController>().GetComponentInChildren<Canvas>().GetComponentInChildren<SpriteRenderer>().gameObject.transform);
                Image[] tempImage = damageObject.GetComponentsInChildren<Image>(); // 0 = 헤드샷, 1 = 출혈
                for (int j = 0; j < tempImage.Length; j++) // 데미지 이미지 false로 초기화
                    tempImage[j].gameObject.SetActive(false);

                damageObject.GetComponent<DamageEffect>().isStart = true;
                damageObject.GetComponentInChildren<Text>().text = (Mathf.Round(tempDamage * 10f) / 10f).ToString();
                damageObject.GetComponentInChildren<Text>().color = Color.red;
                damageObject.GetComponentInChildren<Text>().fontSize = 96;

                player.HP -= Mathf.Round(tempDamage * 10f) / 10f;
                printUI.HPText.text = Mathf.Round(player.HP * 10f) / 10f + " / " + player.MaxHP;
                printUI.HPSlider.value = player.HP;
            }
        }

        if(currentTime >= duringTime)
        {
            Destroy(this.gameObject);
        }
    }
}

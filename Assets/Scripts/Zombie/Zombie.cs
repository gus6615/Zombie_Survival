using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zombie : MonoBehaviour {

    public Animator animator;
    protected PlayerScript player;
    protected new Camera camera;
    [SerializeField] GameObject HP_bar;
    [SerializeField] GameObject itemDrop;
    [SerializeField] protected GameObject DamageObject;
    [SerializeField] GameObject longAttackCircle;
    [SerializeField] GameObject longAttackEffect;
    protected GameObject HP_bar_data;
    protected Image filled;
    protected Image background;
    protected RectTransform canvas;
    protected PrintUI printUI;
    public Transform orderTr;

    private static Zombie instance;
    public Vector3 HP_bar_Vector;

    public float HP;
    protected float maxHP;
    public float armorPercent;
    public int gold;
    protected int exp;
    protected int damage;
    public int type; // 0 = 근접 좀비, 1 = 원거리 좀비
    protected etc[] etcs;
    public float moveSpeed;
    public float attackTime;
    public float currentAttackTime;
    public float attackDamageTime;
    protected float attackDis;
    public bool isHeadShot;
    public bool isWork;
    public bool isAttack;
    protected bool isManyAttack;
    protected bool isAttackOn;
    public bool isPlayerHPDown;
    public bool criticalHit;
    public bool isCritical;
    public bool isDead;
    public bool isHPBar;
    public bool isSetHPBar;
    public bool isDotDamage;
    public bool isDotDamaging;
    public bool isAllWork;
    protected bool isEnd; // isDead의 최종형. true일 경우 모든 함수 정지
    public bool isBloodCtrlOn; // 플레이어 스킬인 BloodCtrl이 적용되었는가?

    // 원거리 공격 관련 변수
    public float longAttackStartTime; // AttackStart 애니메이션의 길이
    public float longAttackTime; // Attack 애니메이션의 길이
    protected bool isLongAttackOn;
    protected int longAttackOrder; // 애니메이션의 진행 상태 (0 = 준비, 1 = 공격, 2 = 준비 취소, 3 = 마무리)
    public float circleCreateTime; // 원이 생성되는 시간 및 삭제되는 시간
    protected float circleDuringTime; // 원이 지속되는 시간
    protected float effectDruingTime; // 이펙트가 지속되는 시간
    protected float effectDamageTime; // 이펙트 데미지가 들어가는 시간
    protected float goalPosTime; // 공격 지점 위치값이 확정될 시간
    protected int longAttackType; // 투사체의 종류 (0 = 지네좀비,...)
    protected Vector2 circleSize; // 원의 크기 
    protected Vector2 goalPos; // 타격 위치 벡터
    protected bool isGoalPosSet; 

    public Zombie GetZombie()
    {
        return instance;
    }

    public void SetZombie(Zombie data)
    {
        instance = data;
    }

    public void GetGold()
    {
        StartCoroutine("GetGoldIE");
        SaveScript.saveData.exp += exp;
        player.stageExp += exp;
        printUI.expText.text = "+" + exp + "exp";
        printUI.SetLevelInfo();
    }

    IEnumerator GetGoldIE()
    {
        int currentGold = SaveScript.saveData.gold;
        int goalGold = SaveScript.saveData.gold + gold;
        player.stageGold += gold;

        while (currentGold != goalGold)
        {
            currentGold++;
            SaveScript.saveData.gold = currentGold;
            printUI.goldText.text = SaveScript.saveData.gold + " 원";
            yield return new WaitForSeconds(0.02f);
            isEnd = true;
        }
    }

    public void Set_HP_bar()
    {
        if(isSetHPBar)
        {
            if (!isHPBar)
            {
                HP_bar_data = Instantiate(HP_bar, camera.WorldToScreenPoint(this.transform.position + HP_bar_Vector),
                new Quaternion(0, 0, 0, 0), FindObjectOfType<StageController>().GetComponentInChildren<Canvas>().GetComponentInChildren<SpriteRenderer>().gameObject.transform);
                filled = HP_bar_data.GetComponentsInChildren<Image>()[1];
                background = HP_bar_data.GetComponentsInChildren<Image>()[0];
                isHPBar = true;
            }

            if (HP >= 0f)
                filled.fillAmount = HP / maxHP;
            else
                filled.fillAmount = 0f;

            isSetHPBar = false;
        }

        if (isHPBar)
        {
            filled.transform.position = camera.WorldToScreenPoint(this.transform.position + HP_bar_Vector);
            background.transform.position = camera.WorldToScreenPoint(this.transform.position + HP_bar_Vector);
        }

        if (isDead || !player.isStart)
        {
            if(HP_bar_data != null)
                Destroy(HP_bar_data.gameObject);
            isHPBar = false;
        }
    }

    IEnumerator GetItem()
    {
        for (int index = 0; index < etcs.Length + 1; index++)
        {
            if (index == 0) // 돈주머니
            {
                Instantiate(itemDrop, this.transform.position + Vector3.up, new Quaternion(0, 0, 0, 0));
                yield return new WaitForSeconds(0.5f);
            }
            else if (Percent.GetRandFlag(etcs[index - 1].dropRate)) // 아이템 주머니 ( index 는 1부터 )
            {
                GameObject item = Instantiate(itemDrop, this.transform.position + Vector3.up, new Quaternion(0, 0, 0, 0));
                item.GetComponent<ItemDrop>().itemImage.sprite = etcs[index - 1].image.sprite;
                player.stageItems[etcs[index - 1].itemCode - (SaveScript.weaponNum + SaveScript.armorNum + 1)]++;

                SaveScript.saveData.hasEtcs[etcs[index - 1].itemCode - SaveScript.saveData.hasGuns.Count - SaveScript.saveData.hasArmors.Count - 1]++;

                if(etcs[index - 1].quallity >= 1) // 에픽 아이템 이상일 경우
                {
                    printUI.isItemOn = true;
                    printUI.itemInfoObject.GetComponentInChildren<Image>().sprite = etcs[index - 1].image.sprite;
                    printUI.itemInfoObject.GetComponentInChildren<Text>().text = "'" + etcs[index - 1].name + "' 휙득!";
                    printUI.itemInfoObject.GetComponentInChildren<Text>().color = Item.colors[etcs[index - 1].quallity];
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    IEnumerator IsDotDamage()
    {
        float dotDamage = SaveScript.guns[SaveScript.saveData.equipGun].damage * SaveScript.gunsAbilitys[4].data * 0.01f
                        * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][4]) - 1);
        isDotDamage = false;
        isDotDamaging = true;

        for (int i = 0; i < 8; i++)
        {
            if (isDead)
                break;

            GameObject damageObject = Instantiate(DamageObject, camera.WorldToScreenPoint(this.transform.position + HP_bar_Vector + Vector3.up * 0.5f), new Quaternion(0, 0, 0, 0),
                            FindObjectOfType<StageController>().GetComponentInChildren<Canvas>().GetComponentInChildren<SpriteRenderer>().gameObject.transform);
            Image[] tempImage = damageObject.GetComponentsInChildren<Image>(); // 0 = 헤드샷, 1 = 출혈
            for (int j = 0; j < tempImage.Length; j++) // 데미지 이미지 false로 초기화
                tempImage[j].gameObject.SetActive(false);

            tempImage[1].gameObject.SetActive(true);
            damageObject.GetComponent<DamageEffect>().isStart = true;
            damageObject.GetComponentInChildren<Text>().text = (Mathf.Round(dotDamage * 10f) / 10f).ToString();
            damageObject.GetComponentInChildren<Text>().color = new Color(1, 0, 1, 1); // 보라색

            HP -= dotDamage;
            isSetHPBar = true;

            filled.color = new Color(0.4f, 0.4f, 0.4f, 1);

            yield return new WaitForSeconds(0.25f);
        }

        if (filled != null)
            filled.color = Color.white;
        isDotDamaging = false;
    }

    protected void Dead()
    {
        isDead = true;
        isWork = false;
        isAttack = false;
        moveSpeed = 0f;

        GetGold();
        StartCoroutine("GetItem");

        int Rand;
        if (isHeadShot)
            Rand = 2;
        else
            Rand = 1;

        animator.SetInteger("isDead", Rand);
        for (int i = 0; i < this.gameObject.GetComponentsInChildren<PolygonCollider2D>().Length; i++)
        {
            Destroy(this.gameObject.GetComponentsInChildren<PolygonCollider2D>()[i]);
        }
        for (int i = 0; i < this.gameObject.GetComponentsInChildren<CircleCollider2D>().Length; i++)
        {
            Destroy(this.gameObject.GetComponentsInChildren<CircleCollider2D>()[i]);
        }
        for (int i = 0; i < this.gameObject.GetComponentsInChildren<BoxCollider2D>().Length; i++)
        {
            Destroy(this.gameObject.GetComponentsInChildren<BoxCollider2D>()[i]);
        }
    }

    protected void ShortAttack()
    {
        if (!isAttackOn)
        {
            animator.SetBool("isWalk", false);
            if (isManyAttack)
                animator.SetInteger("isAttack", Random.Range(1, 3));
            else
                animator.SetBool("isAttack", true);
        }

        isAttackOn = true;
        currentAttackTime += Time.deltaTime;

        if (currentAttackTime >= attackDamageTime && !player.isDead && !isPlayerHPDown)
        {
            isPlayerHPDown = true;

            if(Mathf.Abs(player.transform.position.x - transform.position.x) <= attackDis)
            {
                player.isImageFadeOn = true;
                float tempDamage = Mathf.Round(damage * 10f) / 10f;

                if (SaveScript.saveData.equipArmor == -1)
                    player.HP -= Mathf.Round(damage * 10f) / 10f;
                else
                {
                    tempDamage = damage * (1 - (SaveScript.armors[SaveScript.saveData.equipArmor].armor + SaveScript.armorsAbilitys[1].data * 0.01f
                        * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[SaveScript.saveData.equipArmor][1]) - 1)));
                    if (tempDamage < 1)
                        tempDamage = 1;
                    player.HP -= tempDamage;

                    if (SaveScript.armors[SaveScript.saveData.equipArmor].reflectDamage != 0 ||
                        (int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[SaveScript.saveData.equipArmor][3]) > 1) // 공격 반사
                    {
                        float reflectDamage = damage * (SaveScript.armors[SaveScript.saveData.equipArmor].reflectDamage + SaveScript.armorsAbilitys[3].data * 0.01f
                            * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[SaveScript.saveData.equipArmor][3]) - 1));
                        HP -= reflectDamage;

                        isSetHPBar = true;

                        GameObject data = Instantiate(DamageObject, camera.WorldToScreenPoint(this.transform.position + HP_bar_Vector + Vector3.up * 0.5f), new Quaternion(0, 0, 0, 0),
                            FindObjectOfType<StageController>().GetComponentInChildren<Canvas>().GetComponentInChildren<SpriteRenderer>().gameObject.transform);
                        Image[] images = data.GetComponentsInChildren<Image>(); // 0 = 헤드샷, 1 = 출혈
                        for (int j = 0; j < images.Length; j++) // 데미지 이미지 false로 초기화
                            images[j].gameObject.SetActive(false);

                        data.GetComponent<DamageEffect>().isStart = true;
                        data.GetComponentInChildren<Text>().text = (Mathf.Round(reflectDamage * 10f) / 10f).ToString();
                        data.GetComponentInChildren<Text>().color = Color.red;
                        data.GetComponentInChildren<Text>().fontSize = 96;
                    }
                }

                GameObject damageObject = Instantiate(DamageObject, camera.WorldToScreenPoint(player.transform.position + Vector3.up * 0.5f), new Quaternion(0, 0, 0, 0),
                    FindObjectOfType<StageController>().GetComponentInChildren<Canvas>().GetComponentInChildren<SpriteRenderer>().gameObject.transform);
                Image[] tempImage = damageObject.GetComponentsInChildren<Image>(); // 0 = 헤드샷, 1 = 출혈
                for (int j = 0; j < tempImage.Length; j++) // 데미지 이미지 false로 초기화
                    tempImage[j].gameObject.SetActive(false);

                damageObject.GetComponent<DamageEffect>().isStart = true;
                damageObject.GetComponentInChildren<Text>().text = (Mathf.Round(tempDamage * 10f) / 10f).ToString();
                damageObject.GetComponentInChildren<Text>().color = Color.red;
                damageObject.GetComponentInChildren<Text>().fontSize = 96;

                printUI.HPText.text = Mathf.Round(player.HP * 10f) / 10f + " / " + player.MaxHP;
                printUI.HPSlider.value = player.HP;
            }
        }

        if (currentAttackTime >= attackTime)
        {
            isPlayerHPDown = false;
            currentAttackTime = 0f;
            isAttack = false;
            isAttackOn = false;
        }
    }

    protected void LongAttack()
    {
        currentAttackTime += Time.deltaTime;

        // 어택 준비 자세(1)
        if (longAttackOrder == 0)
        {
            animator.SetBool("isAttackStart", true);
            animator.SetBool("isWalk", false);
            longAttackOrder = 1;
        }

        // 어택 자세(2)
        if (currentAttackTime > longAttackStartTime && longAttackOrder == 1)
        {
            animator.SetBool("isAttack", true);
            longAttackOrder = 2;
        }

        if(isLongAttackOn)
        {
            if(currentAttackTime > longAttackStartTime + goalPosTime && !isGoalPosSet)
            {
                goalPos = player.transform.position;
                isGoalPosSet = true;
            }
                
            if (currentAttackTime > longAttackStartTime + longAttackTime)
            {
                currentAttackTime = longAttackStartTime;
                isAttack = false;
                isGoalPosSet = false;

                float tempY = 0f;
                if (goalPos.y >= MoveCtrl.standardY)
                    tempY = MoveCtrl.standardY + MoveCtrl.radiusY - 2.5f;
                else
                    tempY = MoveCtrl.standardY - MoveCtrl.radiusY - 2.5f;

                LongAttackCircle circle = Instantiate(longAttackCircle, new Vector3(goalPos.x, tempY, 0f), new Quaternion(0, 0, 0, 0)).GetComponent<LongAttackCircle>();
                LongAttackEffect effect = Instantiate(longAttackEffect, new Vector3(goalPos.x, tempY, 0f), new Quaternion(0, 0, 0, 0)).GetComponent<LongAttackEffect>();
                PlayerSkill.longAttackCircleList.Add(circle);
                PlayerSkill.longAttackEffectList.Add(effect);
                
                circle.transform.localScale = circleSize * 2f;
                circle.duringTime = circleDuringTime;
                circle.createTime = circleCreateTime;
                circle.attackEffect = effect.gameObject;
                effect.damageVec = circle.transform.localScale;
                effect.damage = damage;
                effect.type = longAttackType;
                effect.duringTime = effectDruingTime;
                effect.damageTime = effectDamageTime;
                effect.gameObject.SetActive(false);
            }
        }
        else
        {
            // 어택 종료 자세(3)
            if (currentAttackTime > longAttackStartTime && longAttackOrder == 2)
            {
                animator.SetBool("isAttack", false);
                longAttackOrder = 3;
            }

            // 마무리 자세(4)
            if (currentAttackTime > 2f * longAttackStartTime && longAttackOrder == 3)
            {
                animator.SetBool("isAttackStart", false);
                animator.SetBool("isWalk", true);
                currentAttackTime = 0f;
                isAttack = false;
                longAttackOrder = 0;
            }
        }
    }

    protected void Move() // 근거리 좀비 이동 함수
    {
        if (Vector3.Distance(this.transform.position, player.transform.position) > attackDis)
        {
            animator.SetBool("isWalk", true);

            // 근거리 좀비일 경우
            if(longAttackOrder == 0)
            {
                if (isManyAttack)
                    animator.SetInteger("isAttack", 0);
                else
                    animator.SetBool("isAttack", false);
            }
                
            isLongAttackOn = false;

            if(longAttackOrder != 3)
            {
                if (this.transform.position.x > player.transform.position.x)
                {
                    this.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
                    this.transform.localScale = new Vector3(1f, 1f, 1f);
                }
                else
                {
                    this.transform.position += Vector3.right * moveSpeed * Time.deltaTime;
                    this.transform.localScale = new Vector3(-1f, 1f, 1f);
                }
            }
        }
        else
        {
            if(longAttackOrder != 3)
            {
                isAttack = true;
                isLongAttackOn = true;
            }
        }
    }
}

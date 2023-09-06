using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThreeHandsZombie : Zombie
{
    bool isPaging;
    bool isPageOn;
    float paseHP;
    [SerializeField] private GameObject firstArm;
    private PolygonCollider2D[] firstArmCols;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerScript>();
        camera = FindObjectOfType<Camera>();
        printUI = FindObjectOfType<PrintUI>();
        HP_bar_Vector = new Vector3(0f, 3f, 0);

        Canvas[] tempCanvas = FindObjectsOfType<Canvas>();
        for (int i = 0; i < tempCanvas.Length; i++)
            if (tempCanvas[i].gameObject.name == "UICanvas")
                canvas = tempCanvas[i].GetComponent<RectTransform>();

        maxHP = HP = 500f;
        exp = 135;
        paseHP = 200f;
        armorPercent = 0.2f;
        damage = 15;
        gold = Random.Range(100, 151);
        moveSpeed = 1f;
        attackTime = 3f;
        attackDamageTime = 0.75f;
        attackDis = 3f;
        isWork = false;

        etcs = new etc[4];
        etcs[0] = new etc(11);
        etcs[1] = new etc(12);
        etcs[2] = new etc(16);
        etcs[3] = new etc(17);

        firstArmCols = firstArm.GetComponentsInChildren<PolygonCollider2D>();
    }

    private void Update()
    {
        if ((Mathf.Abs(player.transform.position.x - transform.position.x) <= CameraCtrl.cameraRadius || isAllWork) && !isEnd)
        {
            Set_HP_bar();

            if (player.isStart && !SpecialAICtrl.isWork)
            {
                if (HP <= paseHP && !isPageOn)
                    StartCoroutine("PaseUp");

                if (isPaging)
                {
                    this.transform.position += new Vector3(this.transform.localScale.x, 0, 0) * Time.deltaTime * 2f;
                }

                if (HP <= 0f && !isDead) // 사망
                {
                    Dead();
                }

                if (isDotDamage && !isDotDamaging) // 출혈 효과
                    StartCoroutine("IsDotDamage");

                if (criticalHit || isPaging)
                    if (HP > 0f && !isCritical && !isAttack && !isPaging)
                        StartCoroutine("critical");
                    else
                    {
                        isAttack = false;
                        isAttackOn = false;
                    }
                else
                {
                    if (Mathf.Abs(player.transform.position.x - this.transform.position.x) < 1.5f * CameraCtrl.cameraRadius && HP > 0f)
                        isWork = true;
                    else if (this.transform.position.x <= player.transform.position.x)
                        isWork = true;

                    if (isAttack && !isDead) // 공격
                    {
                        ShortAttack();
                    }

                    if (isWork && !isAttack) // 이동
                    {
                        Move();
                    }
                }
            }
            else
            {
                if (HP_bar_data != null && isDead)
                {
                    Destroy(HP_bar_data.gameObject);
                    isHPBar = false;
                }
            }
        }
    }

    IEnumerator critical()
    {
        isCritical = true;
        isAttack = false;
        isWork = false;
        animator.SetBool("isCritical", true);

        //int random = Random.Range(0, SoundCtrl.zombieSounds.Length);
        //SoundCtrl.zombieSounds[random].Play();

        yield return new WaitForSeconds(0.5f);

        isCritical = false;
        criticalHit = false;
        isWork = true;
        animator.SetBool("isCritical", false);
    }

    IEnumerator PaseUp()
    {
        isPageOn = true;
        isPaging = true;
        isCritical = false;
        isAttack = false;
        isWork = false;
        animator.SetBool("isPase", true);
        if (!isBloodCtrlOn)
            moveSpeed = 2.5f;
        else
            moveSpeed = 2.5f * PlayerSkill.bloodCtrlPercent;
        damage = 10;
        for (int i = 0; i < firstArmCols.Length; i++)
            Destroy(firstArmCols[i]);

        yield return new WaitForSeconds(1f);

        isWork = true;
        isPaging = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderZombie : Zombie
{
    bool isPaging;
    bool isPageOn;
    float paseHP;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerScript>();
        camera = FindObjectOfType<Camera>();
        printUI = FindObjectOfType<PrintUI>();
        HP_bar_Vector = new Vector3(0f, 2f, 0);

        Canvas[] tempCanvas = FindObjectsOfType<Canvas>();
        for (int i = 0; i < tempCanvas.Length; i++)
            if (tempCanvas[i].gameObject.name == "UICanvas")
                canvas = tempCanvas[i].GetComponent<RectTransform>();

        maxHP = HP = 400f;
        exp = 215;
        paseHP = 200f;
        armorPercent = 0.1f;
        damage = 12;
        gold = Random.Range(80, 131);
        moveSpeed = 1f;
        attackTime = 2f;
        attackDamageTime = 1f;
        attackDis = 3.5f;
        isWork = false;

        etcs = new etc[4];
        etcs[0] = new etc(11);
        etcs[1] = new etc(12);
        etcs[2] = new etc(16);
        etcs[3] = new etc(17);
    }

    private void Update()
    {
        if ((Mathf.Abs(player.transform.position.x - transform.position.x) <= CameraCtrl.cameraRadius || isAllWork) && !isEnd)
        {
            Set_HP_bar();

            if (player.isStart && !SpecialAICtrl.isWork)
            {
                if (HP <= paseHP && HP >= 0f && !isPageOn)
                    StartCoroutine("PaseUp");

                if (isPaging)
                    moveSpeed = 0f;

                if (HP <= 0f && !isDead) // 사망
                {
                    Dead();
                }

                if (isAttack && !isDead) // 공격
                {
                    ShortAttack();
                }

                if (isWork && !isAttack) // 이동
                {
                    Move();
                }

                if (isDotDamage && !isDotDamaging) // 출혈 효과
                    StartCoroutine("IsDotDamage");

                if (Mathf.Abs(player.transform.position.x - this.transform.position.x) < 1.5f * CameraCtrl.cameraRadius && HP > 0f)
                    isWork = true;
                else if (this.transform.position.x <= player.transform.position.x)
                    isWork = true;
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

    IEnumerator PaseUp()
    {
        isPageOn = true;
        isPaging = true;
        isAttack = false;
        isWork = false;
        animator.SetBool("isPase", true);

        yield return new WaitForSeconds(1f);

        isWork = true;
        isPaging = false;
        if (!isBloodCtrlOn)
            moveSpeed = 0.8f;
        else
            moveSpeed = 0.8f * PlayerSkill.bloodCtrlPercent;
        damage = 10;
    }
}

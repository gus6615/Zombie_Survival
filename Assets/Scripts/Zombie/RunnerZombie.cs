using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunnerZombie : Zombie {

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerScript>();
        camera = FindObjectOfType<Camera>();
        canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        printUI = FindObjectOfType<PrintUI>();
        HP_bar_Vector = new Vector3(0f, 1.5f, 0);

        Canvas[] tempCanvas = FindObjectsOfType<Canvas>();
        for (int i = 0; i < tempCanvas.Length; i++)
            if (tempCanvas[i].gameObject.name == "UICanvas")
                canvas = tempCanvas[i].GetComponent<RectTransform>();

        maxHP = HP = 60f;
        exp = 25;
        armorPercent = 0.1f;
        damage = 5;
        gold = Random.Range(30, 51);
        moveSpeed = 3.0f;
        attackTime = 1.0f;
        attackDamageTime = 0.5f;
        attackDis = 1.4f;
        isWork = false;
        isManyAttack = true;

        etcs = new etc[4];
        etcs[0] = new etc(11);
        etcs[1] = new etc(12);
        etcs[2] = new etc(14);
        etcs[3] = new etc(16);
    }

    private void Update()
    {
        if ((Mathf.Abs(player.transform.position.x - transform.position.x) <= CameraCtrl.cameraRadius || isAllWork) && !isEnd)
        {
            if (isDead && Mathf.Abs(this.transform.position.x - player.transform.position.x) >= CameraCtrl.cameraRadius * 2f)
                Destroy(this.gameObject);
            Set_HP_bar();

            if (player.isStart && !SpecialAICtrl.isWork)
            {
                if (HP <= 0f && !isDead) // 사망
                {
                    Dead();
                }

                if (isDotDamage && !isDotDamaging) // 출혈 효과
                    StartCoroutine("IsDotDamage");

                if (criticalHit)
                    if (HP > 0f && !isCritical && !isAttack)
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

        yield return new WaitForSeconds(0.5f);

        isCritical = false;
        criticalHit = false;
        isWork = true;
        animator.SetBool("isCritical", false);
    }
}

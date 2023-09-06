using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalZombie : Zombie {

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerScript>();
        camera = FindObjectOfType<Camera>();
        printUI = FindObjectOfType<PrintUI>();
        HP_bar_Vector = new Vector3(-0.1f, 1f, 0);

        Canvas[] tempCanvas = FindObjectsOfType<Canvas>();
        for (int i = 0; i < tempCanvas.Length; i++)
            if (tempCanvas[i].gameObject.name == "UICanvas")
                canvas = tempCanvas[i].GetComponent<RectTransform>();

        maxHP = HP = 40f;
        exp = 10;
        armorPercent = 0.05f;
        damage = 8;
        gold = Random.Range(10, 21);
        moveSpeed = 0.5f;
        attackTime = 2.333333f;
        attackDamageTime = 1.5f;
        attackDis = 1.1f;
        isWork = false;
        isManyAttack = true;

        etcs = new etc[4];
        etcs[0] = new etc(11);
        etcs[1] = new etc(12);
        etcs[2] = new etc(13);
        etcs[3] = new etc(16);
    }

    private void Update()
    {
        if ((Mathf.Abs(player.transform.position.x - transform.position.x) <= CameraCtrl.cameraRadius || isAllWork) && !isEnd)
        {
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
        animator.SetBool("IsCritical", true);

        //int random = Random.Range(0, SoundCtrl.zombieSounds.Length);
        //SoundCtrl.zombieSounds[random].Play();

        yield return new WaitForSeconds(0.5f);

        isCritical = false;
        criticalHit = false;
        isWork = true;
        animator.SetBool("IsCritical", false);
    }
}

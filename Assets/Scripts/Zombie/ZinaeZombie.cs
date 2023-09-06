using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZinaeZombie : Zombie
{
    private void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerScript>();
        camera = FindObjectOfType<Camera>();
        printUI = FindObjectOfType<PrintUI>();
        HP_bar_Vector = new Vector3(0f, 8f, 0);

        Canvas[] tempCanvas = FindObjectsOfType<Canvas>();
        for (int i = 0; i < tempCanvas.Length; i++)
            if (tempCanvas[i].gameObject.name == "UICanvas")
                canvas = tempCanvas[i].GetComponent<RectTransform>();

        maxHP = HP = 6000f;
        exp = 120;
        armorPercent = 0.45f;
        damage = 30;
        gold = Random.Range(150, 201);
        moveSpeed = 0.5f;
        attackDis = 18f;
        type = 1;
        isWork = false;

        longAttackStartTime = 1f;
        longAttackTime = 5f;
        circleCreateTime = 1f;
        circleDuringTime = 1f;
        effectDruingTime = 2f;
        effectDamageTime = 0.5f;
        goalPosTime = 3f;
        longAttackType = 0;
        circleSize = new Vector2(2f, 1f);

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
                if (HP <= 0f && !isDead) // 사망
                {
                    Dead();
                }

                if (isWork && !isAttack) // 이동
                {
                    Move();
                }

                if ((isAttack || longAttackOrder != 0) && !isDead) // 공격
                {
                    LongAttack();
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
}

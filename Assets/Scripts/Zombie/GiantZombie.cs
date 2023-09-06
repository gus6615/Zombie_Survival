using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiantZombie : Zombie
{
    private void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerScript>();
        camera = FindObjectOfType<Camera>();
        printUI = FindObjectOfType<PrintUI>();
        HP_bar_Vector = new Vector3(0f, 7f, 0);

        Canvas[] tempCanvas = FindObjectsOfType<Canvas>();
        for (int i = 0; i < tempCanvas.Length; i++)
            if (tempCanvas[i].gameObject.name == "UICanvas")
                canvas = tempCanvas[i].GetComponent<RectTransform>();

        maxHP = HP = 800f;
        exp = 80;
        armorPercent = 0.3f;
        damage = 20;
        gold = Random.Range(80, 101);
        moveSpeed = 0.5f;
        attackTime = 5f;
        attackDamageTime = 2.1f;
        attackDis = 5.8f;
        isWork = false;

        etcs = new etc[4];
        etcs[0] = new etc(11);
        etcs[1] = new etc(12);
        etcs[2] = new etc(15);
        etcs[3] = new etc(16);
    }

    private void Update()
    {
        if((Mathf.Abs(player.transform.position.x - transform.position.x) <= CameraCtrl.cameraRadius || isAllWork) && !isEnd)
        {
            Set_HP_bar();

            if (player.isStart && !SpecialAICtrl.isWork)
            {
                if (HP <= 0f && !isDead) // 사망
                {
                    Dead();
                }

                if (isAttack && !isDead) // 공격
                {
                    ShortAttack();
                }

                if (isDotDamage && !isDotDamaging) // 출혈 효과
                    StartCoroutine("IsDotDamage");

                if (Mathf.Abs(player.transform.position.x - this.transform.position.x) < 1.5f * CameraCtrl.cameraRadius && HP > 0f)
                    isWork = true;
                else if (this.transform.position.x <= player.transform.position.x)
                    isWork = true;

                if (isWork && !isAttack) // 이동
                {
                    Move();
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
}

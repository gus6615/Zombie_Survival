using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Anima2D;

public class SpecialAI_1 : SpecialAI
{
    public bool isSkill; // 땅에 내려오고 스킬 시전 순간 확인
    public bool isAttack;

    // Start is called before the first frame update
    void Start()
    {
        name = "사천";
        type = -1;
        workPorce = 16;
        level = SaveScript.hasSpecialAI[0].level;
        weaponName = "붉은 칼날";

        price = 5000;
        redJam = 10;
        blueJam = 5;
        skillInfo = "어깨에 돋아난 날카로운 칼날로 일직선 상에 존재하는 적들에게 큰 피해를 줍니다.";
        typeInfo = "공격 분야";
        damage = 300;
        coolTime = 3f;
        fallTime = 2f;
        skillTime = 1f;
        distance = 14f;
        isCanUse = true;
        savedPos = this.transform.position;

        if (SceneManager.GetActiveScene().name == "GameScene" && gameObject.name != "New Game Object")
        {
            canvas.gameObject.SetActive(false);
            animator = GetComponent<Animator>();
            audio = GetComponent<AudioSource>();
            playerScript = FindObjectOfType<PlayerScript>();
            player = playerScript.gameObject;
            camera = FindObjectOfType<Camera>();
        }

        abilities = new Ability[0];

        shop_image = AIObject.specialAIs_ShopImages[0];
        weapon_image = AIObject.specialAIs_WeaponImages[0];

        color = new Color(1f, 0.8f, 0.8f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && gameObject.name != "New Game Object")
        {
            if (isWork)
            {
                if (isStartAct)
                    StartCoroutine("AttackStart");

                if (isAttack)
                    StartCoroutine("Attack");

                if (isSkill)
                {
                    // 오른쪽으로 달려가는 모션
                    if (mediatedDisX <= camera.orthographicSize * camera.pixelWidth / camera.pixelHeight * 4f)
                    {
                        float temp = camera.orthographicSize * camera.pixelWidth / camera.pixelHeight * 4f - mediatedDisY;
                        mediatedDisX += Time.deltaTime * temp * 3f;
                        this.transform.position += Vector3.right * Time.deltaTime * temp * 3f;
                    }
                }
                    
                // 위에서 떨어지는 모션
                if (mediatedDisY <= camera.orthographicSize * 2f)
                {
                    float temp = camera.orthographicSize * 2f - mediatedDisY;
                    mediatedDisY += Time.deltaTime * temp * 3f;
                    this.transform.position -= Vector3.up * Time.deltaTime * temp * 3f;
                }
            }
        }
    }

    IEnumerator AttackStart()
    {
        animator.SetBool("isSkill", true);
        isStartAct = false;
        CameraCtrl.ChangeCameraSize(distance);
        transform.position = player.transform.position + new Vector3(-distance * camera.pixelWidth / camera.pixelHeight * 0.8f, distance * 2f);
        yield return new WaitForSeconds(fallTime);
        GetComponent<AudioSource>().Play();
        canvas.gameObject.SetActive(true);
        canvas.GetComponentInChildren<Image>().rectTransform.anchoredPosition = camera.WorldToScreenPoint(this.transform.position) - new Vector3(480f, 270f, 0f);
        isSkill = true;
        isAttack = true;
    }

    IEnumerator Attack()
    {
        isAttack = false;
        yield return new WaitForSeconds(skillTime);
        HitDamage();
        mediatedDisX = mediatedDisY = 0;
        currentCoolTime = coolTime;
        isSkill = false;
        isWork = false;
        this.transform.position = savedPos;
        SpecialAICtrl.isWork = false;

        // 오브젝트 초기화
        PlayerScript.animator.speed = 1;
        MapManager.isNotWork = false;
        AIControler.SpecialAINotWork();

        // 좀비들 초기화
        Zombie[] zombies = FindObjectsOfType<Zombie>();
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i].gameObject.GetComponent<Animator>().speed = 1;
        }

        CameraCtrl.ChangeCameraSize(SaveScript.guns[SaveScript.saveData.equipGun].shotDis);
        animator.SetBool("isSkill", false);
        canvas.gameObject.SetActive(false);
    }

    public void HitDamage()
    {
        // 플레이어 기준 가로 일직선 공격
        RaycastHit2D[] temps = Physics2D.BoxCastAll(player.transform.position + Vector3.left * camera.orthographicSize * camera.pixelWidth / camera.pixelHeight, 
            new Vector2(1f, 16f), 0f, Vector2.right, camera.orthographicSize * camera.pixelWidth / camera.pixelHeight * 2f, 16384);
        for (int i = 0; i < temps.Length; i++)
        {
            Zombie zombie = temps[i].transform.gameObject.GetComponentInParent<Zombie>();
            if(zombie != null)
            {
                GameObject damageOb = Instantiate(damageObject, camera.WorldToScreenPoint(zombie.transform.position), new Quaternion(0, 0, 0, 0),
                            FindObjectOfType<StageController>().GetComponentInChildren<Canvas>().GetComponentInChildren<SpriteRenderer>().gameObject.transform);
                float realDamage = damage * (1 - zombie.armorPercent) * (1 + level * 0.05f);
                if (realDamage <= 0f)
                    realDamage = damage;

                zombie.HP -= realDamage;
                zombie.isSetHPBar = true;
                zombie.isWork = true;

                Image[] tempImage = damageOb.GetComponentsInChildren<Image>(); // 0 = 헤드샷, 1 = 출혈
                for (int j = 0; j < tempImage.Length; j++) // 데미지 이미지 false로 초기화
                    tempImage[j].gameObject.SetActive(false);
                damageOb.GetComponent<DamageEffect>().fadeSpeed = 1f;
                damageOb.GetComponent<DamageEffect>().isStart = true;

                Text text = damageOb.GetComponentInChildren<Text>();
                text.text = realDamage.ToString();
                text.color = Color.yellow;
                text.fontSize = 148;
            }
        }

        
    }
}

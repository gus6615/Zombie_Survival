using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class ShotAI : AI {

    [SerializeField] protected GameObject head;
    [SerializeField] protected GameObject reloadObject;
    [SerializeField] protected RectTransform canvas;
    [SerializeField] protected GameObject rightHandTr;
    [SerializeField] protected GameObject leftHandTr;
    [SerializeField] protected GameObject orbitalEffect;
    [SerializeField] protected GameObject[] shotEffect;
    [SerializeField] protected GameObject bulletShell;
    [SerializeField] protected GameObject damageEffect;
    [SerializeField] protected GameObject bloodEffect;
    [SerializeField] protected GameObject blockEffect;

    [SerializeField] protected Transform weaponDot;
    protected Vector2 weaponVec;
    protected Vector2 weaponDisVec;
    public Vector2 zoomVec;
    public Vector2 zoomVecSave;
    public Vector2 bullet_start;
    protected Vector2 standVec;
    protected float distance;
    protected Color effectColor; // 총기 이펙트 색상

    public float damage;
    public float headShotPercent;
    public float armorDestroyPercent;
    public int bulletNum;
    public int currentBulletNum;
    public float ShoutDelayTime;
    public float ReloadTime;
    public float moveSpeed;
    public float initAngle; // 머리 초기 각도
    public float angle; // 머리 각도
    public float range; // 적 인식 범위
    public bool isShout;
    public bool isReload;
    public int isFront; // 플레이어를 기준으로 앞, 뒤, 앞뒤를 공격하는 여부에 대한 변수 ( 1 = 앞, 2 = 뒤, 3 = 앞, 뒤 )
    public bool isAttackFlag;
    public bool isBack; // 후퇴해야 하는가?
    public float backDis; // AI가 뒤로 오는 거리
    public float currentBackDis; // 최근 AI가 뒤로 온 거리

    public bool isReloadUI;
    public GameObject reload;
    Image filled;
    public float currentTime;

    protected void SetMove(Collider2D collider)
    {
        switch (isFront)
        {
            case 1: // 앞쪽 공격
                if (collider != null)
                {
                    animator.SetBool("isRun", false);

                    // 플레이어 기준 후퇴 범위 수정
                    if (this.transform.position.x > player.transform.position.x + 1f)
                    {
                        if (collider.transform.position.x - this.transform.position.x <= range * 0.25f) // 무사격 후퇴
                        {
                            isBack = true;
                            if (Mathf.Abs(this.transform.position.x - player.transform.position.x) < 10f)
                            {
                                this.transform.position += new Vector3(-moveSpeed * 0.25f, 0, 0) * Time.deltaTime;
                                animator.SetBool("isWalk", true);
                            }
                        }
                        else if (collider.transform.position.x - this.transform.position.x <= range * 0.75f) // 사격 후퇴
                        {
                            animator.SetBool("isWalk", true);
                            this.transform.position += new Vector3(-moveSpeed * 0.25f, 0, 0) * Time.deltaTime;
                        }

                        // 플레이어와 일정 거리만큼 좁혀 졌을 경우
                        if (Mathf.Abs(this.transform.position.x - player.transform.position.x) < 10f)
                        {
                            isBack = false;
                        }
                    }
                    else
                    {
                        isBack = false;
                        currentBackDis = 0f;
                        this.transform.localScale = new Vector3(1f, 1f, 1f);
                        animator.SetBool("isRun", false);
                        animator.SetBool("isWalk", false);
                    }

                    // 후퇴
                    if (isBack)
                    {
                        if (currentBackDis < backDis)
                        {
                            animator.SetBool("isWalk", false);
                            animator.SetBool("isRun", true);
                            currentBackDis += moveSpeed * Time.deltaTime;
                            this.transform.position += new Vector3(-moveSpeed, 0, 0) * Time.deltaTime;
                            this.transform.localScale = new Vector3(-1f, 1f, 1f);
                        }
                        else
                        {
                            currentBackDis = 0f;
                            this.transform.localScale = new Vector3(1f, 1f, 1f);
                            isBack = false;
                        }
                    }
                    else
                    {
                        currentBackDis = 0f;
                        this.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                }
                else
                {
                    if (!animator.GetBool("isHold"))
                    {
                        if (isReload)
                            this.transform.position += new Vector3(moveSpeed * 0.75f, 0, 0) * Time.deltaTime;
                        else
                            this.transform.position += new Vector3(moveSpeed, 0, 0) * Time.deltaTime;

                        animator.SetBool("isRun", true);
                        animator.SetBool("isWalk", false);
                    }

                    this.transform.localScale = new Vector3(1f, 1f, 1f);
                    currentBackDis = 0f;
                    isBack = false;
                }

                if (this.transform.position.x > FindObjectOfType<StageController>().stage.distance)
                {
                    animator.SetBool("isRun", false);
                    animator.SetBool("isHold", true);
                }
                else
                {
                    animator.SetBool("isHold", false);
                }
                break;
            case 2: // 뒤쪽 공격
                if (collider != null)
                {
                    animator.SetBool("isRun", false);

                    // 플레이어 기준 후퇴 범위 수정
                    if (this.transform.position.x + 1f < player.transform.position.x)
                    {
                        if (this.transform.position.x - collider.transform.position.x <= range * 0.25f) // 무사격 후퇴
                        {
                            isBack = true;
                            if (Mathf.Abs(this.transform.position.x - player.transform.position.x) < 10f)
                            {
                                this.transform.position += new Vector3(moveSpeed * 0.25f, 0, 0) * Time.deltaTime;
                                animator.SetBool("isWalk", true);
                            }
                        }
                        else if (this.transform.position.x - collider.transform.position.x <= range * 0.75f) // 사격 후퇴
                        {
                            isBack = false;
                            animator.SetBool("isWalk", true);
                            this.transform.position += new Vector3(moveSpeed * 0.25f, 0, 0) * Time.deltaTime;
                        }

                        // 플레이어와 일정 거리만큼 좁혀 졌을 경우
                        if (Mathf.Abs(this.transform.position.x - player.transform.position.x) < 10f)
                        {
                            isBack = false;
                        }
                    }
                    else
                    {
                        isBack = false;
                        currentBackDis = 0f;
                        this.transform.localScale = new Vector3(-1f, 1f, 1f);
                        animator.SetBool("isRun", false);
                        animator.SetBool("isWalk", false);
                    }

                    // 후퇴
                    if (isBack)
                    {
                        if (currentBackDis < backDis)
                        {
                            animator.SetBool("isWalk", false);
                            animator.SetBool("isRun", true);
                            currentBackDis += moveSpeed * Time.deltaTime;
                            this.transform.position += new Vector3(moveSpeed, 0, 0) * Time.deltaTime;
                            this.transform.localScale = new Vector3(1f, 1f, 1f);
                        }
                        else
                        {
                            currentBackDis = 0f;
                            this.transform.localScale = new Vector3(-1f, 1f, 1f);
                            isBack = false;
                        }
                    }
                    else
                    {
                        currentBackDis = 0f;
                        this.transform.localScale = new Vector3(-1f, 1f, 1f);
                    }
                }
                else
                {
                    if (!animator.GetBool("isHold"))
                    {
                        if (isReload)
                            this.transform.position += new Vector3(-moveSpeed * 0.75f, 0, 0) * Time.deltaTime;
                        else
                            this.transform.position += new Vector3(-moveSpeed, 0, 0) * Time.deltaTime;

                        animator.SetBool("isRun", true);
                        animator.SetBool("isWalk", false);
                    }

                    this.transform.localScale = new Vector3(-1f, 1f, 1f);
                    currentBackDis = 0f;
                    isBack = false;
                }

                if (this.transform.position.x < -10f)
                {
                    animator.SetBool("isRun", false);
                    animator.SetBool("isHold", true);
                }
                else
                {
                    animator.SetBool("isHold", false);
                }
                break;
            case 3: // 양쪽 공격
                this.transform.position += new Vector3(moveSpeed, 0, 0) * Time.deltaTime;
                break;
        }
    }

    protected bool SetAttack(Collider2D datas)
    {
        Zombie zombie = datas.gameObject.GetComponent<Zombie>();

        zoomVec = datas.transform.position +(Vector3)datas.offset - this.transform.position;
        zoomVec = Vector2.ClampMagnitude(zoomVec * 10f, 50f);
        angle = Mathf.Asin(zoomVec.normalized.y) / Mathf.PI * 180f + Random.Range(-2.5f, 2.5f);
        zoomVec = new Vector2(Mathf.Cos(angle / 180f * Mathf.PI), Mathf.Sin(angle / 180f * Mathf.PI)).normalized * 50f;
        if (isFront == 2)
            zoomVec = new Vector2(-zoomVec.x, zoomVec.y);

        switch (isFront)
        {
            case 1: // 앞쪽 공격
                if (zombie.transform.position.x > this.transform.position.x)
                {
                    this.transform.localScale = new Vector3(1f, 1f, 1f);
                    isAttackFlag = true;
                }
                else
                    isAttackFlag = false;
                break;
            case 2: // 뒤쪽 공격
                if (zombie.transform.position.x < this.transform.position.x)
                {
                    isAttackFlag = true;
                    this.transform.localScale = new Vector3(-1f, 1f, 1f);
                }
                else
                    isAttackFlag = false;
                break;
            default: // 앞, 뒤쪽 공격
                break;
        }

        if (isAttackFlag)
        {
            isAttackFlag = false;

            if (!isShout && !isReload)
                StartCoroutine("Shouting");

            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator Shouting()
    {
        zoomVecSave = zoomVec;
        
        isShout = true;
        Vector2 verticalZoomVec = Vector2.Perpendicular(zoomVec).normalized * weaponVec.y;
        float verticalAmount = 1 - Quaternion.Euler(zoomVec).y * (1f / 0.34f); // 0 ~ 1의 값, 팔의 각도가 0일 경우 0, 90도 일 경우 1의 값을 가짐.

        if (isFront == 2)
        {
            bullet_start = (Vector2)(this.transform.position) + (zoomVec.normalized * distance) - verticalZoomVec * verticalAmount;
            angle = Mathf.Asin(-zoomVec.normalized.y) / 3.14f * 180f;
        }
        else
        {
            bullet_start = (Vector2)(this.transform.position) + (zoomVec.normalized * distance) + verticalZoomVec * verticalAmount;
            angle = Mathf.Asin(zoomVec.normalized.y) / 3.14f * 180f;
        }
            
        audio.Play();
        ShotEffect();
        currentBulletNum--;
        
        yield return new WaitForSeconds(ShoutDelayTime);

        isShout = false;
    }

    public void Reloading()
    {
        currentTime += Time.deltaTime;
        
        if(currentTime < ReloadTime)
        {
            if (!isReload)
            {
                isReload = true;
                isShout = false;
                animator.SetBool("isReload", true);
                animator.updateMode = AnimatorUpdateMode.Normal;
            }
        }
        else
        {
            currentBulletNum = bulletNum;
            isReload = false;
            animator.SetBool("isReload", false);
            animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            Destroy(reload.gameObject);
            currentTime = 0f;
            isReloadUI = false;
        }
    }

    public void ShotEffect()
    {
        float orbitalDistance; // orbitalEffect의 길이
        float moveVecF = (0.5f - Mathf.Abs(zoomVec.normalized.y - 0.5f)) * 4f; // 총알이 발사되는 각도에 따른 총알 유효범위 조정
        RaycastHit2D raycastHit = Physics2D.Raycast(bullet_start, zoomVec.normalized, range + moveVecF);

        if (raycastHit)
        {
            orbitalDistance = Vector2.Distance(bullet_start, (Vector2)raycastHit.transform.position);

            GameObject hit = raycastHit.collider.gameObject;

            if (hit.tag == "Head" || hit.tag == "Body" || hit.tag == "Zombie" || hit.tag == "Block")
            {
                Zombie zombie = hit.GetComponentInParent<Zombie>();

                if (zombie != null) // 총알이 좀비와 충돌할 경우
                {
                    if (zombie.GetZombie() == null)
                        zombie.SetZombie(zombie);

                    if (zombie == zombie.GetZombie()) // 겹쳐진 적에 대한 오류 보완
                    {
                        GameObject damageObject = Instantiate(damageEffect, camera.WorldToScreenPoint(raycastHit.point), new Quaternion(0, 0, 0, 0),
                            FindObjectOfType<StageController>().GetComponentInChildren<Canvas>().GetComponentInChildren<SpriteRenderer>().gameObject.transform);
                        float realDamage = damage * (1 - (zombie.armorPercent - armorDestroyPercent)) * (1 + level * 0.2f);
                        if (realDamage <= 0f)
                            realDamage = damage * (1 + level * 0.2f);

                        Image[] tempImage = damageObject.GetComponentsInChildren<Image>(); // 0 = 헤드샷, 1 = 출혈
                        for (int i = 0; i < tempImage.Length; i++) // 데미지 이미지 false로 초기화
                            tempImage[i].gameObject.SetActive(false);
                        damageObject.GetComponent<DamageEffect>().isStart = true;

                        if (hit.tag == "Head") // 부위별 피격 데미지
                        {
                            zombie.HP -= (Mathf.Round(realDamage * 1.5f * 10f) / 10f);
                            zombie.isHeadShot = true;
                            tempImage[0].gameObject.SetActive(true);
                            damageObject.GetComponentInChildren<Text>().text = (Mathf.Round(realDamage * 1.5f * 10f) / 10f).ToString();
                            damageObject.GetComponentInChildren<Text>().color = Color.red;
                            Instantiate(bloodEffect, raycastHit.point, Quaternion.Euler(0, 0, 0));
                        }
                        else if(hit.tag == "Body" || hit.tag == "Zombie")
                        {
                            zombie.HP -= (Mathf.Round(realDamage * 10f) / 10f);
                            zombie.isHeadShot = false;
                            damageObject.GetComponentInChildren<Text>().text = (Mathf.Round(realDamage * 10f) / 10f).ToString();
                            damageObject.GetComponentInChildren<Text>().color = Color.white;
                            Instantiate(bloodEffect, raycastHit.point, Quaternion.Euler(0, 0, 0));
                        }
                        else if(hit.tag == "Block")
                        {
                            Instantiate(blockEffect, raycastHit.point, Quaternion.Euler(0, 0, 0));
                            damageObject.GetComponentInChildren<Text>().text = "";
                        }

                        zombie.isSetHPBar = true;
                        zombie.isWork = true;
                        zombie.isAllWork = true;
                    }

                    zombie.SetZombie(null);
                }
            }
        }
        else
            orbitalDistance = camera.orthographicSize * camera.pixelWidth / camera.pixelHeight + moveVecF;

        int rand = Random.Range(0, 3);

        if (this.transform.localScale.x < 0f)
        {
            orbitalDistance = -orbitalDistance;
            shotEffect[rand].transform.localScale = new Vector3(-1, 1, 1);
            
        }
        else
            shotEffect[rand].transform.localScale = new Vector3(1, 1, 1);

        orbitalEffect.transform.localScale = new Vector3(orbitalDistance, 1, 1);

        GameObject orbital = Instantiate(orbitalEffect, bullet_start, Quaternion.Euler(0, 0, angle));
        orbital.GetComponent<SpriteRenderer>().color = effectColor;
        PlayerSkill.gunEffectList.Add(orbital.GetComponent<GunEffect>());

        GameObject fire = Instantiate(shotEffect[rand], bullet_start, Quaternion.Euler(0, 0, angle));
        fire.GetComponent<SpriteRenderer>().color = effectColor;
        PlayerSkill.gunEffectList.Add(fire.GetComponent<GunEffect>());

        GameObject shell = Instantiate(bulletShell, bullet_start + weaponDisVec, Quaternion.Euler(0, 0, Random.Range(0f, 90f)));
        shell.GetComponent<SpriteRenderer>().color = effectColor;
        PlayerSkill.gunEffectList.Add(shell.GetComponent<GunEffect>());
    }

    public Vector2 WorldPosToCanvas(Vector3 data)
    {
        Vector2 temp = (Vector2)camera.WorldToScreenPoint(data);

        return temp;
    }

    public void ReloadUI()
    {
        if (!isReloadUI)
        {
            reload = Instantiate(reloadObject, camera.WorldToScreenPoint(this.transform.position + Vector3.up * 1f),
                new Quaternion(0, 0, 0, 0), FindObjectOfType<StageController>().GetComponentInChildren<Canvas>().GetComponentInChildren<SpriteRenderer>().gameObject.transform);
            filled = reload.GetComponentsInChildren<Image>()[1];
            reload.SetActive(true);
            filled.fillAmount = 0f;

            isReloadUI = true;
        }

        if(filled != null)
        {
            reload.transform.position = camera.WorldToScreenPoint(this.transform.position + Vector3.up * 1f);

            if (currentTime < ReloadTime)
                filled.fillAmount += Time.deltaTime / ReloadTime;
            else
                filled.fillAmount = 1f;
        }
    }
}

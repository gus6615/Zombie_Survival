using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Anima2D;

public class ShoutButtonCtrl : MonoBehaviour, IDragHandler, IEndDragHandler
{
    static public bool isShottingOn;

    [SerializeField] private RectTransform ButtonImageTr;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject player_rightHand;
    [SerializeField] private GameObject player_leftHand;
    [SerializeField] private GameObject player_rightHandImage;
    [SerializeField] private GameObject player_leftHandImage;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject[] shotEffect;
    [SerializeField] private GameObject orbitalEffect;
    [SerializeField] private GameObject bulletShell;
    [SerializeField] private GameObject bloodEffect;
    [SerializeField] private GameObject blockEffect;
    [SerializeField] private GameObject damageEffect;
    [SerializeField] private GameObject pointer;
    [SerializeField] private GameObject itemDrop;

    private Animator animator;
    private PlayerScript playerScript;
    private new Camera camera;
    private PrintUI printUI;
    [SerializeField] private GameObject weapon;
    static private ItemOrder[] bioWeapon_dots;
    static private ImageOrder[] weapon_dots;
    private Color effectColor;

    private Vector3 player_rightHand_posSave;
    private Vector3 player_leftHand_posSave;
    public Vector2 zoomVec; // 첫 터치 지점에서 현재 터치 지점사이의 벡터
    public Vector2 bullet_start;
    private Vector2 bullet_shellVec;
    private Vector3 playerTr;
    static private Vector3 shoutVec;
    private Color buttonColor;
    public float radius;
    static private float distance;
    private float angle;
    static public bool isTouch;
    static public bool isReload;
    private bool isShout;
    private bool isNotAutoReload;

    private void Start()
    {
        radius = ButtonImageTr.rect.width * 0.5f;
        playerTr = player.transform.localScale;
        animator = player.gameObject.GetComponent<Animator>();
        playerScript = player.gameObject.GetComponent<PlayerScript>();
        player_rightHand_posSave = player_rightHand.transform.localPosition;
        player_leftHand_posSave = player_leftHand.transform.localPosition;
        buttonColor = ButtonImageTr.GetComponent<Image>().color;
        camera = FindObjectOfType<Camera>();
        printUI = FindObjectOfType<PrintUI>();
        bioWeapon_dots = weapon.GetComponentsInChildren<ItemOrder>();
        weapon_dots = weapon.GetComponentsInChildren<ImageOrder>();
        effectColor = new Color(1f, 1f, 1f, 1f); // 흰색
        pointer.SetActive(false);

        SetShotInfo(); // 총기에 따른 격발점 및 탄피 거리 조정
    }

    private void Update()
    {
        if (playerScript.isDead)
        {
            zoomVec = Vector2.zero;
            angle = 0;
            player_rightHand.transform.localPosition = player_rightHand_posSave;
            player_leftHand.transform.localPosition = player_leftHand_posSave;
        }

        if (!playerScript.isDead && playerScript.isStart)
        {
            // 얼굴 각도 관련
            if (isTouch || isReload)
            {
                if (zoomVec.x < 0)
                {
                    head.transform.localScale = new Vector3(-1f, 1f, 1f);
                    angle = -Mathf.Asin(zoomVec.normalized.y) / 3.14f * 180f;
                }
                else
                {
                    head.transform.localScale = new Vector3(1f, 1f, 1f);
                    angle = Mathf.Asin(zoomVec.normalized.y) / 3.14f * 180f;
                }

            }
            else
            {
                angle = 0;
                head.transform.localScale = new Vector3(1f, 1f, 1f);
            }

            head.transform.rotation = Quaternion.Euler(0, 0, angle);

            if (isTouch)
            { 
                if(zoomVec.x >= 0f )
                {
                    player_rightHandImage.transform.localScale = new Vector3(1f, 1f, 1f);
                    player_leftHand.GetComponent<IkLimb2D>().flip = false;
                    player_leftHandImage.GetComponentsInChildren<Bone2D>()[0].transform.localScale = new Vector3(1f, 1f, 1f);
                    player_leftHandImage.GetComponentsInChildren<Bone2D>()[1].transform.localScale = new Vector3(1f, 1f, 1f);
                    player_rightHand.transform.localPosition = new Vector3(Mathf.Abs(zoomVec.x), zoomVec.y, 0);
                    player_leftHand.transform.localPosition = new Vector3(Mathf.Abs(Vector2.ClampMagnitude(zoomVec, 2f).x), Vector2.ClampMagnitude(zoomVec, 2f).y);

                    if (playerScript.transform.localScale.x < 0f)
                    {
                        player_rightHandImage.transform.localScale = new Vector3(-1f, 1f, 1f);
                        player_leftHand.GetComponent<IkLimb2D>().flip = true;
                        player_leftHandImage.GetComponentsInChildren<Bone2D>()[0].transform.localScale = new Vector3(-1f, 1f, 1f);
                        player_leftHandImage.GetComponentsInChildren<Bone2D>()[1].transform.localScale = new Vector3(-1f, 1f, 1f);
                        player_rightHand.transform.localPosition = new Vector3(Mathf.Abs(zoomVec.x), -zoomVec.y, 0);
                        player_leftHand.transform.localPosition = new Vector3(Mathf.Abs(Vector2.ClampMagnitude(zoomVec, 2f).x), -Vector2.ClampMagnitude(zoomVec, 2f).y);
                    }
                }
                else
                {
                    player_rightHandImage.transform.localScale = new Vector3(-1f, 1f, 1f);
                    player_leftHand.GetComponent<IkLimb2D>().flip = true;
                    player_leftHandImage.GetComponentsInChildren<Bone2D>()[0].transform.localScale = new Vector3(-1f, 1f, 1f);
                    player_leftHandImage.GetComponentsInChildren<Bone2D>()[1].transform.localScale = new Vector3(-1f, 1f, 1f);
                    player_rightHand.transform.localPosition = new Vector3(Mathf.Abs(zoomVec.x), -zoomVec.y, 0);
                    player_leftHand.transform.localPosition = new Vector3(Mathf.Abs(Vector2.ClampMagnitude(zoomVec, 2f).x), -Vector2.ClampMagnitude(zoomVec, 2f).y);

                    if (playerScript.transform.localScale.x < 0f)
                    {
                        player_rightHandImage.transform.localScale = new Vector3(1f, 1f, 1f);
                        player_leftHand.GetComponent<IkLimb2D>().flip = false;
                        player_leftHandImage.GetComponentsInChildren<Bone2D>()[0].transform.localScale = new Vector3(1f, 1f, 1f);
                        player_leftHandImage.GetComponentsInChildren<Bone2D>()[1].transform.localScale = new Vector3(1f, 1f, 1f);
                        player_rightHand.transform.localPosition = new Vector3(Mathf.Abs(zoomVec.x), zoomVec.y, 0);
                        player_leftHand.transform.localPosition = new Vector3(Mathf.Abs(Vector2.ClampMagnitude(zoomVec, 2f).x), Vector2.ClampMagnitude(zoomVec, 2f).y);
                    }
                }
            }

            // 사격 및 리로드 관련
            if (!isReload)
            {
                // 공격
                if (isShottingOn && !isShout)
                {
                    if((PlayerSkill.isBioWeaponOn && SaveScript.bioGuns[PlayerSkill.weaponType].currentBulletNum != 0)
                        || SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum != 0)
                        StartCoroutine("Shouting");
                }

                // 리로드 - 수동
                if (isNotAutoReload && SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum != SaveScript.guns[SaveScript.saveData.equipGun].bulletNum && SaveScript.saveData.hasGunsBullets[SaveScript.saveData.equipGun] != 0)
                {
                    isNotAutoReload = false;
                    StartCoroutine("Reload");
                }

                // 리로드 - 자동
                if (SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum == 0 && (SaveScript.saveData.hasGunsBullets[SaveScript.saveData.equipGun] != 0 
                    || SaveScript.saveData.equipGun == 0) && !PlayerSkill.isBioWeaponOn)
                {
                    StartCoroutine("Reload");
                }
            }
        }
    }

    public void ReloadButton()
    {
        isNotAutoReload = true;
    }

    static public void SetShotInfo()
    {
        if (!PlayerSkill.isBioWeaponOn)
        {
            shoutVec = weapon_dots[SaveScript.saveData.equipGun].transform.position - Vector3.up * 0.5f;
            distance = Vector2.Distance(Vector3.up * 0.5f, weapon_dots[SaveScript.saveData.equipGun].transform.position);
        }
        else
        {
            shoutVec = bioWeapon_dots[PlayerSkill.weaponType].transform.position - Vector3.up * 0.5f;
            distance = Vector2.Distance(Vector3.up * 0.5f, bioWeapon_dots[PlayerSkill.weaponType].transform.position);
        }
    }

    public void OnDrag(PointerEventData e)
    {
        if (!playerScript.isDead && playerScript.isStart && !SpecialAICtrl.isWork)
        {
            if (!ChangeModeButton.isScopeMode)
            {
                zoomVec = e.position - (Vector2)ButtonImageTr.transform.position;
                zoomVec = Vector2.ClampMagnitude(zoomVec, radius);
                animator.SetBool("isShout", true);
                isTouch = true;

                pointer.SetActive(true);
                pointer.GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
                pointer.GetComponent<RectTransform>().anchoredPosition = zoomVec;
                CameraCtrl.AimSetCamera(zoomVec.x / (radius * 2f)); // 에이밍에 따른 카메라 이동

                if (Vector2.Distance(e.position, ButtonImageTr.transform.position) >= radius)
                {
                    isShottingOn = true;
                    pointer.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                }
                else
                    isShottingOn = false;
            }

            ButtonImageTr.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!playerScript.isDead && playerScript.isStart && !SpecialAICtrl.isWork)
        {
            pointer.SetActive(false);
            pointer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            pointer.GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
            CameraCtrl.AimSetCamera(0f);
            CameraCtrl.mediatedAimDis = 0f;

            ButtonImageTr.GetComponent<Image>().color = buttonColor;

            isTouch = false;
            isShottingOn = false;

            player_rightHandImage.transform.localScale = new Vector3(1f, 1f, 1f);
            player_leftHand.GetComponent<IkLimb2D>().flip = false;
            player_leftHandImage.GetComponentsInChildren<Bone2D>()[0].transform.localScale = new Vector3(1f, 1f, 1f);
            player_leftHandImage.GetComponentsInChildren<Bone2D>()[1].transform.localScale = new Vector3(1f, 1f, 1f);
            player_rightHand.transform.localPosition = new Vector3(Mathf.Abs(zoomVec.x), zoomVec.y, 0);
            player_leftHand.transform.localPosition = new Vector3(Mathf.Abs(Vector2.ClampMagnitude(zoomVec, 2f).x), Vector2.ClampMagnitude(zoomVec, 2f).y);
            animator.SetBool("isShout", false);
            zoomVec = Vector2.zero;
        }
    }

    IEnumerator Shouting()
    {
        // 돌격사격
        if (isTouch)
        {
            isShout = true;
            Vector2 verticalZoomVec = Vector2.Perpendicular(zoomVec).normalized * shoutVec.y;
            float verticalAmount = 1 - Quaternion.Euler(zoomVec).y * (1f / 0.34f); // 0 ~ 1의 값, 팔의 각도가 0일 경우 0, 90도 일 경우 1의 값을 가짐.

            if (zoomVec.x < 0f)
                verticalZoomVec = -verticalZoomVec;

            bullet_start = (Vector2)(player.transform.position) + (zoomVec.normalized * distance) + verticalZoomVec * verticalAmount;

            if (!PlayerSkill.isBioWeaponOn)
            {
                bullet_shellVec = bullet_start - zoomVec.normalized * SaveScript.guns[SaveScript.saveData.equipGun].bulletShellDis;
                SaveScript.guns[SaveScript.saveData.equipGun].shoutSound.Play();
                Shotting(SaveScript.guns[SaveScript.saveData.equipGun].type); // 총알 궤적, 총구 화염, 총 탄피 효과 및 피격 감지

                if (SaveScript.saveData.equipGun == 0)
                    printUI.bulletText.text = --SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum + " / ∞";
                else
                    printUI.bulletText.text = --SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum + " / " + SaveScript.saveData.hasGunsBullets[SaveScript.saveData.equipGun];

                printUI.bulletSlider.value = SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum;

                yield return new WaitForSeconds(SaveScript.guns[SaveScript.saveData.equipGun].shootDelayTime);
                isShout = false;
            }
            else
            {
                bullet_shellVec = bullet_start - zoomVec.normalized * SaveScript.bioGuns[PlayerSkill.weaponType].bulletShellDis;
                SaveScript.bioGuns[PlayerSkill.weaponType].shoutSound.Play();
                Shotting(SaveScript.bioGuns[PlayerSkill.weaponType].type); // 총알 궤적, 총구 화염, 총 탄피 효과 및 피격 감지

                printUI.bulletText.text = (--SaveScript.bioGuns[PlayerSkill.weaponType].currentBulletNum).ToString();
                printUI.bulletSlider.value = SaveScript.bioGuns[PlayerSkill.weaponType].currentBulletNum;

                yield return new WaitForSeconds(SaveScript.bioGuns[PlayerSkill.weaponType].shootDelayTime);
                isShout = false;
            }
        }
    }

    IEnumerator Reload()
    {
        if (ChangeWeaponButton.isChangeWeapon) // 리로드 2번 되는 현상 방지
            ChangeWeaponButton.isChangeWeapon = false;

        isReload = true;
        printUI.reloadingText.gameObject.SetActive(true);
        SaveScript.guns[SaveScript.saveData.equipGun].reloadSound.Play();
        yield return new WaitForSeconds(SaveScript.guns[SaveScript.saveData.equipGun].reloadingTime * (1 - 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][1]) - 1)));

        if (!ChangeWeaponButton.isChangeWeapon) // 무기 교체가 아닐 경우에만
        {
            if(SaveScript.saveData.equipGun == 0)
            {
                SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum = SaveScript.guns[SaveScript.saveData.equipGun].bulletNum;
            }
            else
            {
                if (SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum + SaveScript.saveData.hasGunsBullets[SaveScript.saveData.equipGun] >= SaveScript.guns[SaveScript.saveData.equipGun].bulletNum) // 총알이 [ x / 충분한 양 ] 일 경우
                {
                    int usedBulletNum = (int)Mathf.Round(SaveScript.guns[SaveScript.saveData.equipGun].bulletNum
                        * (1 + 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][2]) - 1))) - SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum;
                    SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum = (int)Mathf.Round(SaveScript.guns[SaveScript.saveData.equipGun].bulletNum 
                        * (1 + 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][2]) - 1)));
                    SaveScript.saveData.hasGunsBullets[SaveScript.saveData.equipGun] -= usedBulletNum;
                }
                else // 총알이 [ x / 부족한 양 ] 일 경우
                {
                    SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum += SaveScript.saveData.hasGunsBullets[SaveScript.saveData.equipGun];
                    SaveScript.saveData.hasGunsBullets[SaveScript.saveData.equipGun] = 0;
                }
            }

            if (SaveScript.saveData.equipGun == 0)
                printUI.bulletText.text = SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum + " / ∞";
            else
                printUI.bulletText.text = SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum + " / " + SaveScript.saveData.hasGunsBullets[SaveScript.saveData.equipGun];
            printUI.bulletSlider.value = SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum;
        }

        ChangeWeaponButton.isChangeWeapon = false;
        isReload = false;
        printUI.reloadingText.gameObject.SetActive(false);
    }

    public void Shotting(int type)
    {
        if (type == 1) // 한발 사격
        {
            OneShot(zoomVec);
        }
        else if (type == 2) // 다발 사격
        {
            Vector2 temp;

            for (int i = 0; i < 6; i++)
            {
                float x = zoomVec.normalized.x, y = zoomVec.normalized.y;
                temp = zoomVec.normalized + new Vector2(Random.Range(x - 0.3f, x + 0.3f), Random.Range(y - 0.3f, y + 0.3f)).normalized;
                OneShot(temp);
            }
        }
        else if (type == 3) // 저격 사격
        {
            float currentDamage = SaveScript.guns[SaveScript.saveData.equipGun].damage * (1 + 0.05f * (SaveScript.saveData.level - 1)) * (1 + 0.05f * (SaveScript.saveData.DamageUpgrade - 1));

            do
            {
                currentDamage = OneShot(currentDamage);
            }
            while (currentDamage > 0f);
        }

        Instantiate(bulletShell, bullet_shellVec, Quaternion.Euler(0, 0, Random.Range(0f, 90f))); // 탄피 생성
    }

    public void OneShot(Vector2 _zoomVec)
    {
        float orbitalDistance; // orbitalEffect의 길이
        float moveVecF = (0.5f - Mathf.Abs(_zoomVec.normalized.y - 0.5f)) * 4f; // 총알이 발사되는 각도에 따른 총알 유효범위 조정
        RaycastHit2D raycastHit = Physics2D.Raycast(bullet_start, _zoomVec.normalized,
            camera.orthographicSize * camera.pixelWidth / camera.pixelHeight + moveVecF + CameraCtrl.mediatedAimDis);
        float _angle = Mathf.Asin(_zoomVec.normalized.y) / 3.14f * 180f;
        if (_zoomVec.x < 0)
            _angle = -_angle;

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
                            FindObjectOfType<StageController>().GetComponentInChildren<Canvas>().gameObject.transform);
                        float realDamage = SaveScript.guns[SaveScript.saveData.equipGun].damage * (1 - (zombie.armorPercent - SaveScript.saveData.ArmorDistroyUpgrade * SaveScript.upgradeDatas[5]))
                            * (1 + SaveScript.upgradeDatas[0] * (SaveScript.saveData.level - 1)) * (1 + SaveScript.upgradeDatas[0] * (SaveScript.saveData.DamageUpgrade - 1))
                            * (1 + 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][0]) - 1));
                        if (zombie.armorPercent <= SaveScript.saveData.ArmorDistroyUpgrade * SaveScript.upgradeDatas[5])
                            realDamage = SaveScript.guns[SaveScript.saveData.equipGun].damage * (1 + SaveScript.upgradeDatas[0] * (SaveScript.saveData.level - 1)) 
                                * (1 + SaveScript.upgradeDatas[0] * (SaveScript.saveData.DamageUpgrade - 1)) * (1 + 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][0]) - 1));

                        // 광전사 특성
                        if(char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][3]) > 1 && !PlayerSkill.isBioWeaponOn)
                        {
                            int count = (int)Mathf.Round(SaveScript.guns[SaveScript.saveData.equipGun].bulletNum 
                                * (1 + 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][2]) - 1))) - SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum;
                            realDamage = realDamage * (float)(1 + 0.01f * count * (char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][3]) - 1));
                        }

                        bool isBlock = false;
                        Image[] tempImage = damageObject.GetComponentsInChildren<Image>(); // 0 = 헤드샷, 1 = 출혈
                        for (int i = 0; i < tempImage.Length; i++) // 데미지 이미지 false로 초기화
                            tempImage[i].gameObject.SetActive(false);

                        if (hit.tag == "Head") // 부위별 피격 데미지
                        {
                            zombie.HP -= Mathf.Round(realDamage * (1.5f + SaveScript.saveData.HeadShotDamageUpgrade * SaveScript.upgradeDatas[4]) * 10f) / 10f;
                            zombie.isHeadShot = true;
                            tempImage[0].gameObject.SetActive(true);
                            damageObject.GetComponentInChildren<Text>().text = (Mathf.Round(realDamage * (1.5f + SaveScript.saveData.HeadShotDamageUpgrade * SaveScript.upgradeDatas[4]) * 10f) / 10f).ToString();
                            damageObject.GetComponentInChildren<Text>().color = Color.red;
                            Instantiate(bloodEffect, raycastHit.point, Quaternion.Euler(0, 0, 0));
                        }
                        else if (hit.tag == "Body" || hit.tag == "Zombie")
                        {
                            zombie.HP -= (Mathf.Round(realDamage * 10f) / 10f);
                            zombie.isHeadShot = false;
                            damageObject.GetComponentInChildren<Text>().text = (Mathf.Round(realDamage * 10f) / 10f).ToString();
                            damageObject.GetComponentInChildren<Text>().color = Color.white;
                            Instantiate(bloodEffect, raycastHit.point, Quaternion.Euler(0, 0, 0));
                        }
                        else if (hit.tag == "Block")
                        {
                            damageObject.GetComponentInChildren<Text>().text = "";
                            Instantiate(blockEffect, raycastHit.point, Quaternion.Euler(0, 0, 0));
                            isBlock = true;
                        }

                        if (zombie.HP <= 0f)
                        {
                            Collider2D[] col = zombie.GetComponentsInChildren<Collider2D>();

                            for (int i = 0; i < col.Length; i++)
                                Destroy(col[i]);
                        }

                        if (!isBlock && !PlayerSkill.isBioWeaponOn)
                        {
                            // 출혈 효과
                            if ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][4]) != 1)
                            {
                                zombie.isDotDamage = Percent.GetRandFlag(SaveScript.saveData.CriticalPercentUpgrade * SaveScript.upgradeDatas[3]);
                            }

                            // 흡혈 효과
                            if ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][5]) != 1)
                            {
                                PlayerScript.isGetHP = Percent.GetRandFlag(SaveScript.saveData.CriticalPercentUpgrade * SaveScript.upgradeDatas[3]);
                            }

                            // 도벽 효과
                            if ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][6]) != 1)
                            {
                                if (Percent.GetRandFlag(SaveScript.saveData.CriticalPercentUpgrade * SaveScript.upgradeDatas[3]))
                                {
                                    int gold = (int)(zombie.gold * (0.1f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][6]) - 1)));
                                    printUI.GoldAbilityTextOn(gold);
                                    SaveScript.saveData.gold += gold;
                                    printUI.goldText.text = SaveScript.saveData.gold + " 원";
                                    playerScript.stageGold += gold;
                                }
                            }

                            // 수집 효과
                            if ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][7]) != 1)
                            {
                                if (Percent.GetRandFlag(SaveScript.saveData.CriticalPercentUpgrade * SaveScript.upgradeDatas[3]))
                                {
                                    int num = Random.Range(1, (int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][7]));
                                    printUI.BulletAbilityTextOn(num);
                                    SaveScript.saveData.hasGunsBullets[SaveScript.saveData.equipGun] += num;
                                    if (SaveScript.saveData.equipGun == 0)
                                        printUI.bulletText.text = SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum + " / ∞";
                                    else
                                        printUI.bulletText.text = SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum + " / " + SaveScript.saveData.hasGunsBullets[SaveScript.saveData.equipGun];
                                }
                            }
                        }

                        damageObject.GetComponent<DamageEffect>().isStart = true;

                        zombie.isSetHPBar = true;
                        zombie.isWork = true;
                        zombie.isAllWork = true;
                        zombie.criticalHit = Percent.GetRandFlag(SaveScript.saveData.CriticalPercentUpgrade * SaveScript.upgradeDatas[3]);
                    }

                    zombie.SetZombie(null);
                }
            }
        }
        else
            orbitalDistance = camera.orthographicSize * camera.pixelWidth / camera.pixelHeight + moveVecF + CameraCtrl.mediatedAimDis;

        int rand = Random.Range(0, 3);

        if (_zoomVec.x <= 0f)
        {
            orbitalDistance = -orbitalDistance;
            shotEffect[rand].transform.localScale = new Vector3(-1, 1, 1);
        }
        else
            shotEffect[rand].transform.localScale = new Vector3(1, 1, 1);

        orbitalEffect.transform.localScale = new Vector3(orbitalDistance, 1, 1);
        Instantiate(orbitalEffect, bullet_start, Quaternion.Euler(0, 0, _angle)).GetComponent<SpriteRenderer>().color = effectColor;
        Instantiate(shotEffect[rand], bullet_start, Quaternion.Euler(0, 0, _angle)).GetComponent<SpriteRenderer>().color = effectColor;
    }

    public float OneShot(float _damage)
    {
        float orbitalDistance; // orbitalEffect의 길이
        float moveVecF = (0.5f - Mathf.Abs(zoomVec.normalized.y - 0.5f)) * 4f; // 총알이 발사되는 각도에 따른 총알 유효범위 조정
        RaycastHit2D raycastHit = Physics2D.Raycast(bullet_start, zoomVec.normalized,
            camera.orthographicSize * camera.pixelWidth / camera.pixelHeight + moveVecF + CameraCtrl.mediatedAimDis);
        float _angle = Mathf.Asin(zoomVec.normalized.y) / 3.14f * 180f;
        if (zoomVec.x < 0)
            _angle = -_angle;

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
                        float tempHP = zombie.HP; // 총알을 맞기 전의 좀비 체력
                        GameObject damageObject = Instantiate(damageEffect, camera.WorldToScreenPoint(raycastHit.point), new Quaternion(0, 0, 0, 0),
                            FindObjectOfType<StageController>().GetComponentInChildren<Canvas>().gameObject.transform);
                        float realDamage = _damage * (1 - (zombie.armorPercent - SaveScript.saveData.ArmorDistroyUpgrade * SaveScript.upgradeDatas[5]))
                            * (1 + SaveScript.upgradeDatas[0] * (SaveScript.saveData.level - 1)) * (1 + SaveScript.upgradeDatas[0] * (SaveScript.saveData.DamageUpgrade - 1))
                            * (1 + 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][0]) - 1));
                        if (zombie.armorPercent <= SaveScript.saveData.ArmorDistroyUpgrade * SaveScript.upgradeDatas[5])
                            realDamage = _damage * (1 + SaveScript.upgradeDatas[0] * (SaveScript.saveData.level - 1)) * (1 + SaveScript.upgradeDatas[0] * (SaveScript.saveData.DamageUpgrade - 1))
                                * (1 + 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][0]) - 1));

                        // 광전사 특성
                        if (char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][3]) > 1 && !PlayerSkill.isBioWeaponOn)
                        {
                            int count = (int)Mathf.Round(SaveScript.guns[SaveScript.saveData.equipGun].bulletNum
                                * (1 + 0.2f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][2]) - 1))) - SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum;
                            realDamage = realDamage * (float)(1 + 0.01f * count * (char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][3]) - 1));
                        }

                        bool isBlock = false;
                        Image[] tempImage = damageObject.GetComponentsInChildren<Image>(); // 0 = 헤드샷, 1 = 출혈
                        for (int i = 0; i < tempImage.Length; i++) // 데미지 이미지 false로 초기화
                            tempImage[i].gameObject.SetActive(false);

                        if (hit.tag == "Head") // 부위별 피격 데미지
                        {
                            zombie.HP -= Mathf.Round(realDamage * (1.5f + SaveScript.saveData.HeadShotDamageUpgrade * SaveScript.upgradeDatas[4]) * 10f) / 10f;
                            zombie.isHeadShot = true;
                            tempImage[0].gameObject.SetActive(true);
                            damageObject.GetComponentInChildren<Text>().text = (Mathf.Round(realDamage * (1.5f + SaveScript.saveData.HeadShotDamageUpgrade * SaveScript.upgradeDatas[4]) * 10f) / 10f).ToString();
                            damageObject.GetComponentInChildren<Text>().color = Color.red;
                        }
                        else if (hit.tag == "Body" || hit.tag == "Zombie")
                        {
                            zombie.HP -= (Mathf.Round(realDamage * 10f) / 10f);
                            zombie.isHeadShot = false;
                            damageObject.GetComponentInChildren<Text>().text = (Mathf.Round(realDamage * 10f) / 10f).ToString();
                            damageObject.GetComponentInChildren<Text>().color = Color.white;
                            Instantiate(bloodEffect, raycastHit.point, Quaternion.Euler(0, 0, 0));
                        }
                        else if (hit.tag == "Block")
                        {
                            damageObject.GetComponentInChildren<Text>().text = "";
                            Instantiate(blockEffect, raycastHit.point, Quaternion.Euler(0, 0, 0));
                            isBlock = true;
                        }

                        if (zombie.HP <= 0f)
                        {
                            Collider2D[] col = zombie.GetComponentsInChildren<Collider2D>();

                            for (int i = 0; i < col.Length; i++)
                                Destroy(col[i]);
                        }

                        if (!isBlock && !PlayerSkill.isBioWeaponOn)
                        {
                            // 출혈 효과
                            if ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][4]) != 1)
                            {
                                zombie.isDotDamage = Percent.GetRandFlag(SaveScript.saveData.CriticalPercentUpgrade * SaveScript.upgradeDatas[3]);
                            }

                            // 흡혈 효과
                            if ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][5]) != 1)
                            {
                                PlayerScript.isGetHP = Percent.GetRandFlag(SaveScript.saveData.CriticalPercentUpgrade * SaveScript.upgradeDatas[3]);
                            }

                            // 도벽 효과
                            if ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][6]) != 1)
                            {
                                if (Percent.GetRandFlag(SaveScript.saveData.CriticalPercentUpgrade * SaveScript.upgradeDatas[3]))
                                {
                                    int gold = (int)(zombie.gold * (0.1f * ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][6]) - 1)));
                                    printUI.GoldAbilityTextOn(gold);
                                    SaveScript.saveData.gold += gold;
                                    printUI.goldText.text = SaveScript.saveData.gold + " 원";
                                    playerScript.stageGold += gold;
                                }
                            }

                            // 수집 효과
                            if ((int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][7]) != 1)
                            {
                                if (Percent.GetRandFlag(SaveScript.saveData.CriticalPercentUpgrade * SaveScript.upgradeDatas[3]))
                                {
                                    int num = Random.Range(1, (int)char.GetNumericValue(SaveScript.saveData.hasGunsAbilitys[SaveScript.saveData.equipGun][7]));
                                    printUI.BulletAbilityTextOn(num);
                                    SaveScript.saveData.hasGunsBullets[SaveScript.saveData.equipGun] += num;
                                    if (SaveScript.saveData.equipGun == 0)
                                        printUI.bulletText.text = SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum + " / ∞";
                                    else
                                        printUI.bulletText.text = SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum + " / " + SaveScript.saveData.hasGunsBullets[SaveScript.saveData.equipGun];
                                }
                            }
                        }

                        damageObject.GetComponent<DamageEffect>().isStart = true;

                        zombie.isSetHPBar = true;
                        zombie.isWork = true;
                        zombie.isAllWork = true;
                        zombie.criticalHit = Percent.GetRandFlag(SaveScript.saveData.CriticalPercentUpgrade * SaveScript.upgradeDatas[3]);
                        _damage -= tempHP;
                    }

                    zombie.SetZombie(null);
                }
            }
            else
                _damage = -1f;
        }
        else
        {
            _damage = -1f;
            orbitalDistance = camera.orthographicSize * camera.pixelWidth / camera.pixelHeight + moveVecF + CameraCtrl.mediatedAimDis;
        }

        int rand = Random.Range(0, 3);

        if (zoomVec.x <= 0f)
        {
            orbitalDistance = -orbitalDistance;
            shotEffect[rand].transform.localScale = new Vector3(-1, 1, 1);
        }
        else
            shotEffect[rand].transform.localScale = new Vector3(1, 1, 1);

        orbitalEffect.transform.localScale = new Vector3(orbitalDistance, 1, 1);
        Instantiate(orbitalEffect, bullet_start, Quaternion.Euler(0, 0, _angle)).GetComponent<SpriteRenderer>().color = effectColor;
        Instantiate(shotEffect[rand], bullet_start, Quaternion.Euler(0, 0, _angle)).GetComponent<SpriteRenderer>().color = effectColor;


        return _damage;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScopeShotButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject bloodEffect;
    [SerializeField] private GameObject damageEffect;
    [SerializeField] private GameObject blockEffect;
    private new Camera camera;
    private PlayerScript playerScript;

    static public bool isShottingOn;
    static public bool isShout;

    // 집중 사격 모드 변수
    static public int sniperBulletNum; // 기본 총알 수
    static public int sniperCurrentBulletNum; // 최근 총알 수
    static public float sniperReShotTime; // 한발한발 사이에 존재하는 시간차
    static public int sniperDamage;

    private void Start()
    {
        camera = FindObjectOfType<Camera>();
        playerScript = FindObjectOfType<PlayerScript>();

        sniperBulletNum = 5 + (SaveScript.saveData.SniperBulletUpgrade * (int)SaveScript.upgradeDatas[11]);
        sniperCurrentBulletNum = sniperBulletNum;
        sniperReShotTime = 1f;
        sniperDamage = (int)(50 * (1 + SaveScript.saveData.SniperDamageUpgrade * SaveScript.upgradeDatas[10]));
    }

    private void Update()
    {
        if (!playerScript.isDead && playerScript.isStart)
        {
            if (isShottingOn && !isShout && sniperCurrentBulletNum != 0)
            {
                StartCoroutine("Shouting");
            }
        }
    }

    public void OnPointerDown(PointerEventData e)
    {
        isShottingOn = true;
    }

    public void OnPointerUp(PointerEventData e)
    {
        isShottingOn = false;
    }

    IEnumerator Shouting()
    {
        // 집중사격
        if (ChangeModeButton.isScopeMode)
        {
            Vector2 shotPos = new Vector2(camera.transform.position.x, camera.transform.position.y);
            ChangeModeButton.shotEffectVec = new Vector2(Random.Range(-1f, 1f), Random.Range(2f, 3f));
            Destroy(ChangeModeButton.scopeBullets[ChangeModeButton.scopeBullets.Count - 1].gameObject);
            ChangeModeButton.scopeBullets.RemoveAt(ChangeModeButton.scopeBullets.Count - 1);

            isShout = true;
            SaveScript.guns[4].shoutSound.Play();

            ModeShot(shotPos);

            // 총알 UI 및 정보 처리
            sniperCurrentBulletNum--;

            yield return new WaitForSeconds(sniperReShotTime);
            isShout = false;
        }
    }

    public void ModeShot(Vector2 posVec)
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(posVec, Vector2.right, 0.01f);
        if (raycastHit)
        {
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
                        float realDamage = sniperDamage * (1 - (zombie.armorPercent - SaveScript.saveData.ArmorDistroyUpgrade * 0.03f))
                            * (1 + 0.05f * (SaveScript.saveData.level - 1)) * (1 + 0.05f * (SaveScript.saveData.DamageUpgrade - 1));
                        if (zombie.armorPercent <= SaveScript.saveData.ArmorDistroyUpgrade * 0.03f)
                            realDamage = sniperDamage * (1 + 0.05f * (SaveScript.saveData.level - 1)) * (1 + 0.05f * (SaveScript.saveData.DamageUpgrade - 1));

                        Image[] tempImage = damageObject.GetComponentsInChildren<Image>(); // 0 = 헤드샷, 1 = 출혈
                        for (int i = 0; i < tempImage.Length; i++) // 데미지 이미지 false로 초기화
                            tempImage[i].gameObject.SetActive(false);

                        if (hit.tag == "Head") // 부위별 피격 데미지
                        {
                            zombie.HP -= Mathf.Round(realDamage * (1.5f + SaveScript.saveData.HeadShotDamageUpgrade * 0.03f) * 10f) / 10f;
                            zombie.isHeadShot = true;
                            tempImage[0].gameObject.SetActive(true);
                            damageObject.GetComponentInChildren<Text>().text = (Mathf.Round(realDamage * (1.5f + SaveScript.saveData.HeadShotDamageUpgrade * 0.03f) * 10f) / 10f).ToString();
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
                        }

                        if (zombie.HP <= 0f)
                        {
                            Collider2D[] col = zombie.GetComponentsInChildren<Collider2D>();

                            for (int i = 0; i < col.Length; i++)
                                Destroy(col[i]);
                        }

                        damageObject.GetComponent<DamageEffect>().isStart = true;

                        zombie.isSetHPBar = true;
                        zombie.isWork = true;
                        zombie.isAllWork = true;
                        zombie.criticalHit = Percent.GetRandFlag(0.1f + SaveScript.saveData.CriticalPercentUpgrade * 0.03f);
                    }

                    zombie.SetZombie(null);
                }
            }
        }
    }
}

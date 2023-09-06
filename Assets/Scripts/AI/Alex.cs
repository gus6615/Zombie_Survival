using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Alex : ShotAI {

    // Use this for initialization
    void Start()
    {
        name = "케이";
        type = 0;
        workPorce = 20000;
        level = SaveScript.hasStoryAI[1].level;

        if (SceneManager.GetActiveScene().name == "GameScene" && gameObject.name != "New Game Object")
        {
            animator = GetComponent<Animator>();
            audio = GetComponent<AudioSource>();
            playerScript = FindObjectOfType<PlayerScript>();
            player = playerScript.gameObject;
            camera = FindObjectOfType<Camera>();

            weaponVec = (Vector2)(weaponDot.position - this.transform.position);
            distance = Vector2.Distance(this.transform.position, weaponDot.position);
            weaponDisVec = new Vector2(0.75f, 0f);
        }

        damage = 10f;
        headShotPercent = 1.5f;
        armorDestroyPercent = 0f;
        bulletNum = 25;
        currentBulletNum = bulletNum;
        ShoutDelayTime = 0.1f;
        ReloadTime = 3.0f;
        initAngle = 0f;
        range = CameraCtrl.cameraRadius * 1f;
        isFront = 2;
        moveSpeed = 2.2f;
        backDis = range * 0.5f;
        weaponName = "기관 단총";

        effectColor = new Color(0.8f, 1f, 1f, 0.6f); // 하늘색
        effectColor = new Color(0.8f, 1f, 1f, 1f); 
        abilities = new Ability[1];
        abilities[0] = new Ability(14);

        shop_image = AIObject.storyAIs_ShopImages[1];
        weapon_image = AIObject.storyAIs_WeaponImages[1];
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && gameObject.name != "New Game Object")
        {
            if (isReload) // 리로드 UI 기능
                ReloadUI();

            if (!playerScript.isDead && playerScript.isStart && !SpecialAICtrl.isWork)
            {
                Collider2D[] objectsCols = Physics2D.OverlapCircleAll(this.transform.position, range, 16384);
                Collider2D collider = null;
                bool isEnemy = false;

                if (objectsCols.Length >= 1) // 오브젝트 포착
                {
                    for (int i = 0; i < objectsCols.Length; i++)
                    {
                        if (objectsCols[i].transform.position.x + 2f < this.transform.position.x)
                        {
                            isEnemy = true;
                            collider = objectsCols[i];
                            break;
                        }
                    }

                    for (int i = 0; i < objectsCols.Length; i++)
                    {
                        if (objectsCols[i].transform.position.x + 2f < this.transform.position.x &&
                            Mathf.Abs(objectsCols[i].transform.position.x - this.transform.position.x) < Mathf.Abs(collider.transform.position.x - this.transform.position.x))
                        {
                            collider = objectsCols[i];
                        }
                    }

                    if (!isShout && !isReload && !isBack)
                    {
                        if (collider != null)
                        {
                            if (SetAttack(collider))
                                animator.SetBool("isAim", true);
                            else
                                animator.SetBool("isAim", false);
                        }
                    }
                }

                if (isEnemy && !isBack)
                {
                    if (isShout)
                    {
                        leftHandTr.transform.localPosition = new Vector3(Mathf.Abs(Vector2.ClampMagnitude(-zoomVecSave, 1.25f).x), Vector2.ClampMagnitude(zoomVecSave, 1.25f).y)
                            - leftHandTr.transform.parent.transform.localPosition;
                        rightHandTr.transform.localPosition = new Vector3(-zoomVecSave.x, zoomVecSave.y, 0) - rightHandTr.transform.parent.transform.localPosition;
                    }
                    else
                    {
                        leftHandTr.transform.localPosition = new Vector3(Mathf.Abs(Vector2.ClampMagnitude(-zoomVec, 1.25f).x), Vector2.ClampMagnitude(zoomVec, 1.25f).y)
                            - leftHandTr.transform.parent.transform.localPosition;
                        rightHandTr.transform.localPosition = new Vector3(-zoomVec.x, zoomVec.y, 0) - rightHandTr.transform.parent.transform.localPosition; ;
                    }

                    head.transform.rotation = Quaternion.Euler(0, 0, angle + initAngle);
                }
                else
                {
                    animator.SetBool("isAim", false);
                    if (this.transform.localScale.x < 0f)
                        head.transform.rotation = Quaternion.Euler(0, 0, initAngle);
                    else
                        head.transform.rotation = Quaternion.Euler(0, 0, -initAngle);
                }

                if (currentBulletNum == 0)
                    Reloading();

                SetMove(collider);
            }
            else
            {
                animator.SetBool("isWalk", false);
            }
        }
    }
}

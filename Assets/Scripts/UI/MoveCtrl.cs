using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveCtrl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform touchPad;
    [SerializeField] private RectTransform touchCircle;
    private PlayerScript playerScript;
    private Animator animator;

    public bool isMove;
    static public Vector2 moveVec;
    static public float standardY, radiusY; // 기준점 Y 값, Y값의 폭
    private float radius;
    private float moveSpeed; // 기본 이동속도
    static public float backDis; // 뒤로 갈수 있는 거리

    private Vector3 playerTr;

    // Use this for initialization
    void Start() {
        playerScript = FindObjectOfType<PlayerScript>();
        playerTr = playerScript.transform.localScale;
        animator = playerScript.GetComponent<Animator>();

        radius = touchPad.rect.width * 0.4f;
        moveSpeed = 2.5f;
        standardY = playerScript.transform.position.y;
        radiusY = 1f;
        backDis = 10f;
    }

    void Update()
    {
        if (playerScript.isStart && !playerScript.isDead)
        {
            if (moveVec != Vector2.zero)
                Move();
        }
    }

    public void OnBeginDrag(PointerEventData e)
    {
        
    }

    public void OnDrag(PointerEventData e)
    {
        if (playerScript.isStart && !playerScript.isDead)
        {
            moveVec = e.position - (Vector2)touchPad.transform.position;
            moveVec = Vector2.ClampMagnitude(moveVec, radius);

            if (moveVec.x >= 0f)
                playerScript.transform.localScale = new Vector3(playerTr.x, playerTr.y, playerTr.z);
            else
                playerScript.transform.localScale = new Vector3(-playerTr.x, playerTr.y, playerTr.z);
            touchCircle.GetComponent<RectTransform>().anchoredPosition = touchPad.GetComponent<RectTransform>().anchoredPosition + moveVec;
            animator.SetBool("isWalk", true);
        }
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (playerScript.isStart && !playerScript.isDead)
        {
            if(moveVec.x >= 0f)
                playerScript.transform.localScale = new Vector3(playerTr.x, playerTr.y, playerTr.z);
            else
                playerScript.transform.localScale = new Vector3(-playerTr.x, playerTr.y, playerTr.z);
            moveVec = Vector2.zero;
            touchCircle.GetComponent<RectTransform>().anchoredPosition = touchPad.GetComponent<RectTransform>().anchoredPosition;
            animator.SetBool("isWalk", false);
            PlayerScript.moveSpeed = PlayerScript.jumpSpeed = 0f;
        }
    }

    public void Move()
    {
        animator.SetBool("isWalk", true);

        // X축 움직임

        if (playerScript.transform.position.x > -backDis)
        {
            Collider2D[] objects = Physics2D.OverlapBoxAll(playerScript.transform.position, new Vector2(1f, 10f), 0f, 16384);

            if (objects.Length < 1)
            {
                if(SaveScript.saveData.equipArmor == -1)
                {
                    if (moveVec.x >= 0f)
                        PlayerScript.moveSpeed = moveSpeed * (1 + SaveScript.upgradeDatas[2] * SaveScript.saveData.MoveSpeedUpgrade);
                    else
                        PlayerScript.moveSpeed = -moveSpeed * (1 + SaveScript.upgradeDatas[2] * SaveScript.saveData.MoveSpeedUpgrade);
                }
                else
                {
                    if (moveVec.x >= 0f)
                        PlayerScript.moveSpeed = moveSpeed * (1 + SaveScript.upgradeDatas[2] * SaveScript.saveData.MoveSpeedUpgrade
                            + SaveScript.armorsAbilitys[0].data * 0.01f * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[SaveScript.saveData.equipArmor][0]) - 1));
                    else
                        PlayerScript.moveSpeed = -moveSpeed * (1 + SaveScript.upgradeDatas[2] * SaveScript.saveData.MoveSpeedUpgrade
                            + SaveScript.armorsAbilitys[0].data * 0.01f * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[SaveScript.saveData.equipArmor][0]) - 1));
                }
            }
            else
            {
                if (moveVec.x >= 0f)
                {
                    if (SaveScript.saveData.equipArmor == -1)
                        PlayerScript.moveSpeed = moveSpeed * (1 + SaveScript.upgradeDatas[2] * SaveScript.saveData.MoveSpeedUpgrade);
                    else
                        PlayerScript.moveSpeed = moveSpeed * (1 + SaveScript.upgradeDatas[2] * SaveScript.saveData.MoveSpeedUpgrade
                            + SaveScript.armorsAbilitys[0].data * 0.01f * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[SaveScript.saveData.equipArmor][0]) - 1));

                    for (int i = 0; i < objects.Length; i++)
                    {
                        if (objects[i].transform.position.x > playerScript.transform.position.x)
                        {
                            animator.SetBool("isWalk", false);
                            PlayerScript.moveSpeed = 0f;
                            break;
                        }
                    }
                }
                else
                {
                    if (SaveScript.saveData.equipArmor == -1)
                        PlayerScript.moveSpeed = -moveSpeed * (1 + SaveScript.upgradeDatas[2] * SaveScript.saveData.MoveSpeedUpgrade);
                    else
                        PlayerScript.moveSpeed = -moveSpeed * (1 + SaveScript.upgradeDatas[2] * SaveScript.saveData.MoveSpeedUpgrade
                            + SaveScript.armorsAbilitys[0].data * 0.01f * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[SaveScript.saveData.equipArmor][0]) - 1));

                    for (int i = 0; i < objects.Length; i++)
                    {
                        if (objects[i].transform.position.x < playerScript.transform.position.x)
                        {
                            animator.SetBool("isWalk", false);
                            PlayerScript.moveSpeed = 0f;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            animator.SetBool("isWalk", false);
            PlayerScript.moveSpeed = 0f;
            if(moveVec.x >= 0f)
            {
                if (SaveScript.saveData.equipArmor == -1)
                    PlayerScript.moveSpeed = moveSpeed * (1 + SaveScript.upgradeDatas[2] * SaveScript.saveData.MoveSpeedUpgrade);
                else
                    PlayerScript.moveSpeed = moveSpeed *(1 + SaveScript.upgradeDatas[2] * SaveScript.saveData.MoveSpeedUpgrade
                        + SaveScript.armorsAbilitys[0].data * 0.01f * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[SaveScript.saveData.equipArmor][0]) - 1));
            }
        }

        // Y축 움직임
        playerScript.SetImageOrder();

        if (playerScript.transform.localPosition.y > standardY + radiusY)
        {
            playerScript.transform.localPosition = new Vector3(playerScript.transform.localPosition.x, standardY + radiusY, playerScript.transform.localPosition.z);
            PlayerScript.jumpSpeed = 0;
        }
        else if (playerScript.transform.localPosition.y < standardY - radiusY)
        {
            playerScript.transform.localPosition = new Vector3(playerScript.transform.localPosition.x, standardY - radiusY, playerScript.transform.localPosition.z);
            PlayerScript.jumpSpeed = 0;
        }

        if (Mathf.Abs(playerScript.transform.localPosition.y - standardY) <= radiusY)
        {
            if(Mathf.Abs(moveVec.normalized.y) >= 0.2f)
                animator.SetBool("isWalk", true);

            if (SaveScript.saveData.equipArmor == -1)
                PlayerScript.jumpSpeed = moveVec.normalized.y * (1 + SaveScript.upgradeDatas[2] * SaveScript.saveData.MoveSpeedUpgrade);
            else
                PlayerScript.jumpSpeed = moveVec.normalized.y * (1 + SaveScript.upgradeDatas[2] * SaveScript.saveData.MoveSpeedUpgrade 
                    + SaveScript.armorsAbilitys[0].data * 0.01f * ((int)char.GetNumericValue(SaveScript.saveData.hasArmorsAbilitys[SaveScript.saveData.equipArmor][0]) - 1));
        }
    }
}

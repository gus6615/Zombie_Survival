using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScopeMoveCtrl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    static public Vector2 startVec; // 최초 터치 위치 (카메라 로컬 위치)
    static public Vector2 moveVec; // startVec을 기준으로 이동되는 벡터
    static public float percent; // moveVec 의 크기 조절

    // Start is called before the first frame update
    void Start()
    {
        percent = 0.1f;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (ChangeModeButton.isScopeMode && !ChangeModeButton.isChangeColor && !CameraCtrl.isChange)
        {
            ChangeModeButton.isScopeMove = true;
            startVec = e.position;
        }
        else
        {
            startVec = Vector3.zero;
            moveVec = Vector2.zero;
        }
    }

    public void OnDrag(PointerEventData e)
    {
        if (ChangeModeButton.isScopeMode && !ChangeModeButton.isChangeColor && !CameraCtrl.isChange)
        {
            moveVec = (e.position - startVec) * percent;
        }
        else
        {
            startVec = Vector2.zero;
            moveVec = Vector2.zero;
        }
    }

    public void OnEndDrag(PointerEventData e)
    {
        ChangeModeButton.startPos = FindObjectOfType<Camera>().transform.localPosition;
        startVec = Vector2.zero;
        moveVec = Vector2.zero;
    }
}

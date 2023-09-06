using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour {

    static public new Camera camera;
    static private PlayerScript playerScript;
    static private float moveSpeed; // 카메라 이동 속도
    static public float mediatedAimDis; // 카메라의 이동에 따른 추가 에임 벡터 길이
    
    // 무기 교체시 카메라 사이즈 변경 관련 변수들
    static private float cameraRate; // 화면 비율
    static public Vector3 initCameraPos; // 변하기전 카메라 위치
    static private float initCameraSize; // 변하기전 카메라 크기
    static private float goalSize; // 변한후 카메라 크기

    static public bool isChange;

    // 플레이어가 줌을 할 시 카메라 이동 관련 변수들
    static public float cameraRadius; // 카메라 가로 크기의 반
    static private float aimPercent; // 에이밍을 당긴 정도
    static public Vector3 cameraPos; // 카메라 위치 정보

    static public bool isAim;

    // Use this for initialization
    void Start () {
        camera = GetComponent<Camera>();
        playerScript = FindObjectOfType<PlayerScript>();
        moveSpeed = 2.5f;
        mediatedAimDis = 0;

        cameraRate = (float)camera.pixelWidth / camera.pixelHeight;
        initCameraPos = camera.transform.localPosition;
        initCameraSize = camera.orthographicSize;
        cameraRadius = camera.orthographicSize * cameraRate * 1.2f;
        cameraPos = camera.transform.localPosition;
    }
	
	// Update is called once per frame
	void Update () {
        if (isChange)
        {
            float leftSize = goalSize - camera.orthographicSize;

            if (Mathf.Abs(leftSize) >= 0.03f)
            {
                camera.transform.localPosition += Vector3.up * leftSize * cameraRate * Time.deltaTime * moveSpeed;
                camera.orthographicSize += leftSize * Time.deltaTime * moveSpeed;
                cameraPos += Vector3.up * leftSize * cameraRate * Time.deltaTime * moveSpeed;
            }
            else
            {
                camera.transform.localPosition += Vector3.up * (goalSize - initCameraSize) * cameraRate;
                camera.orthographicSize = goalSize;
                cameraRadius = camera.orthographicSize * cameraRate * 1.2f;
                isChange = false;
            }

            initCameraPos = new Vector3(0, camera.transform.localPosition.y, camera.transform.localPosition.z);
            initCameraSize = camera.orthographicSize;
            ChangeModeButton.startPos = cameraPos;
        }

        if (isAim)
        {
            cameraRadius = camera.orthographicSize * cameraRate * 1.2f;
            mediatedAimDis = cameraRadius;
            float gab = cameraRadius * aimPercent - cameraPos.x; // 값이 양수면 카메라의 위치가 앞으로, 음수면 뒤로 이동한다.
            cameraPos += Vector3.right * gab * Time.deltaTime * moveSpeed;

            if (playerScript.transform.localScale.x >= 0)
                camera.transform.localPosition = cameraPos;
            else
                camera.transform.localPosition = new Vector3(-cameraPos.x, cameraPos.y, cameraPos.z);

            if (gab >= 0.001f)
                gab -= Time.deltaTime * moveSpeed;
            else if (gab <= -0.001f)
                gab += Time.deltaTime * moveSpeed;
            else
                gab = 0;

            if (gab == 0)
                isAim = false;
        }
    }

    static public void ChangeCameraSize(float size)
    {
        goalSize = size;
        isChange = true;
    }

    static public void AimSetCamera(float _percent)
    {
        isAim = true;
        aimPercent = _percent;

        if (playerScript.transform.localScale.x >= 0)
            camera.transform.localPosition = cameraPos;
        else
            camera.transform.localPosition = new Vector3(-cameraPos.x, cameraPos.y, cameraPos.z);
    }
}

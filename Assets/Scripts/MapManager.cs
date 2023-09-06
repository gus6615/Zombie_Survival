using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    [SerializeField] private new Camera camera;
    private GameObject[] mapSprites;
    private GameObject[] copy_mapSprites;
    private PlayerScript playerScript;
    private Vector2[] mapVectors;
    private bool[] isSecondMap;
    private Stage stage;
    static public bool isNotWork;

	// Use this for initialization
	void Start () {
        stage = FindObjectOfType<StageController>().GetStage();
        mapVectors = new Vector2[stage.maps.Length];
        mapSprites = new GameObject[stage.maps.Length];
        copy_mapSprites = new GameObject[stage.maps.Length];
        isSecondMap = new bool[stage.maps.Length];
        playerScript = FindObjectOfType<PlayerScript>();

        SetMap();

        for (int i = 0; i < mapSprites.Length; i++)
        {
            if (mapSprites[i].GetComponent<SpriteRenderer>().sprite != null)
            {
                mapVectors[i] = (Vector2)mapSprites[i].GetComponent<SpriteRenderer>().bounds.extents;
                copy_mapSprites[i] = Instantiate(mapSprites[i], mapSprites[i].transform.position + new Vector3(mapVectors[i].x * 2, 0, 0), new Quaternion(0, 0, 0, 0));
            }
            else
                mapVectors[i] = Vector2.zero;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (playerScript.isStart && !isNotWork)
        {
            ChangePoint();
            //if (MoveCtrl.isMove)
            //    MoveMap();
        }
	}

    public void SetMap() // stage에 맞는 맵을 생성한다.
    {
        Instantiate(stage.start, new Vector3(-stage.start.GetComponent<SpriteRenderer>().sprite.bounds.size.x, 1, 0), new Quaternion(0, 0, 0, 0));
        Instantiate(stage.destination, new Vector3(stage.distance, 1, 0), new Quaternion(0, 0, 0, 0));

        for (int i = 0; i < mapSprites.Length; i++)
            mapSprites[i] = Instantiate(stage.maps[i], new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
    }

    public void MoveMap() // 맵을 이루는 각 5개의 요소에 각각의 고유 이동속도로 이동시킨다.
    {
        float moveSpeed = -1.8f;

        for (int i = 0; i < mapVectors.Length; i++)
        {
            if (mapVectors[i] != Vector2.zero)
            {
                mapSprites[i].gameObject.transform.position += new Vector3(moveSpeed, 0, 0) * Time.deltaTime;
                copy_mapSprites[i].gameObject.transform.position += new Vector3(moveSpeed, 0, 0) * Time.deltaTime;
            }
                
            moveSpeed += 0.6f;
        }
    }

    public void ChangePoint() // 맵의 포지션을 매번 변경한다.
    {
        for (int i = 0; i < mapVectors.Length; i++)
        {
            if (mapVectors[i] != Vector2.zero)
            {
                if(MoveCtrl.moveVec.x >= 0f)
                {
                    if (!isSecondMap[i])
                    {
                        // 맵이 카메라 밖으로 나갈 경우
                        if (camera.transform.position.x - CameraCtrl.cameraRadius * 1.5f >= mapSprites[i].transform.position.x + mapVectors[i].x)
                        {
                            float distance = mapVectors[i].x * 4f; // 맵의 끝자락이 카메라의 첫자락에 오도록 연산해주는 값

                            mapSprites[i].transform.position = new Vector3(mapSprites[i].transform.position.x + distance, mapSprites[i].transform.position.y,
                                mapSprites[i].transform.position.z);

                            isSecondMap[i] = true;
                        }
                    }
                    else
                    {
                        if (camera.transform.position.x - CameraCtrl.cameraRadius * 1.5f >= copy_mapSprites[i].transform.position.x + mapVectors[i].x)
                        {
                            float distance = mapVectors[i].x * 4f; // 맵의 끝자락이 카메라의 첫자락에 오도록 연산해주는 값

                            copy_mapSprites[i].transform.position = new Vector3(copy_mapSprites[i].transform.position.x + distance, copy_mapSprites[i].transform.position.y,
                                copy_mapSprites[i].transform.position.z);

                            isSecondMap[i] = false;
                        }
                    }
                }
                else
                {
                    if (!isSecondMap[i])
                    {
                        if (camera.transform.position.x + CameraCtrl.cameraRadius * 1.5f <= copy_mapSprites[i].transform.position.x - mapVectors[i].x)
                        {
                            float distance = mapVectors[i].x * 4f; // 맵의 끝자락이 카메라의 첫자락에 오도록 연산해주는 값

                            copy_mapSprites[i].transform.position = new Vector3(copy_mapSprites[i].transform.position.x - distance, copy_mapSprites[i].transform.position.y,
                                copy_mapSprites[i].transform.position.z);

                            isSecondMap[i] = true;
                        }
                    }
                    else
                    {
                        if (camera.transform.position.x + CameraCtrl.cameraRadius * 1.5f <= mapSprites[i].transform.position.x - mapVectors[i].x)
                        {
                            float distance = mapVectors[i].x * 4f; // 맵의 끝자락이 카메라의 첫자락에 오도록 연산해주는 값

                            mapSprites[i].transform.position = new Vector3(mapSprites[i].transform.position.x - distance, mapSprites[i].transform.position.y,
                                mapSprites[i].transform.position.z);

                            isSecondMap[i] = false;
                        }
                    }
                }
            }
        }
    }
}

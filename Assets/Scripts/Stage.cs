using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage : MonoBehaviour
{
    static public string[] stageName, stageDangerInfo, stageDropItemInfo, stageZombieInfo;
    static public int stageLength, stageLevelLength;
    static public bool isSetting;

    public int stageNum;
    public int stageLevelNum;
    public int totalZombieNum;
    public int currentZombieNum;
    public int[] zombiesNum;
    // 0 = 일반 좀비, 1 = 러너 좀비,...
    public float distance; // 클리어 거리
    public bool isCreateBackZombieOn;
    public float createBackZombieTime;

    public GameObject start; // 출발지 이미지 객체
    public GameObject destination; // 목적지 이미지 객체
    public GameObject[] maps; // 배경 이미지 객체

    private void Start()
    {
        stageNum = SaveScript.saveData.currentStage;
        stageLevelNum = SaveScript.saveData.currentStage_level;
        maps = new GameObject[5];
        if (!isSetting)
            Setting();
        if (SceneManager.GetActiveScene().name == "GameScene")
            init();
    } 

    static public void Setting()
    {
        isSetting = true;
        stageLength = 5;
        stageLevelLength = 5;
        stageName = new string[stageLength * stageLevelLength];
        stageName[0] = "튜토리얼";
        stageName[1] = "좁은 오솔길";
        stageName[2] = "등산로";
        stageName[3] = "넓은 공원로";
        stageName[4] = "마을 입구";

        stageDangerInfo = new string[stageLength * stageLevelLength];
        stageDangerInfo[0] = "[ 위험도 : ★☆☆☆☆ /  Lv.1 이상 추천 ]";
        stageDangerInfo[1] = "[ 위험도 : ★☆☆☆☆ /  Lv.3 이상 추천 ]";
        stageDangerInfo[2] = "[ 위험도 : ★★☆☆☆ /  Lv.5 이상 추천 ]";
        stageDangerInfo[3] = "[ 위험도 : ★★☆☆☆ /  Lv.8 이상 추천 ]";
        stageDangerInfo[4] = "[ 위험도 : ★★★☆☆ /  Lv.10 이상 추천 ]";

        stageDropItemInfo = new string[stageLength * stageLevelLength];
        stageDropItemInfo[0] = "[ 휙득 가능 아이템 등급 : 노멀 ~ 레어 ]";
        stageDropItemInfo[1] = "[ 휙득 가능 아이템 등급 : 노멀 ~ 레어 ]";
        stageDropItemInfo[2] = "[ 휙득 가능 아이템 등급 : 노멀 ~ 레어 ]";
        stageDropItemInfo[3] = "[ 휙득 가능 아이템 등급 : 노멀 ~ 에픽 ]";
        stageDropItemInfo[4] = "[ 휙득 가능 아이템 등급 : 노멀 ~ 유니크 ]";

        stageZombieInfo = new string[stageLength * stageLevelLength];
        stageZombieInfo[0] = "[ 출현 좀비 : 느린 좀비 ]";
        stageZombieInfo[1] = "[ 출현 좀비 : 느린 좀비, 빠른 좀비 ]";
        stageZombieInfo[2] = "[ 출현 좀비 : 느린 좀비, 빠른 좀비, 거대 좀비 ]";
        stageZombieInfo[3] = "[ 출현 좀비 : 빠른 좀비, 거대 좀비, 칼날 좀비 ]";
        stageZombieInfo[4] = "[ 출현 좀비 : 거대 좀비, 칼날 좀비, 거미 좀비 ]";
    }

    private void init()
    {
        SetMaps(); // 맵 이미지 및 목적지 이미지 설정

        switch (stageNum) // 좀비 생성 설정
        {
            case 0:
                switch (stageLevelNum)
                {
                    case 0:
                        int[] ZombieNumData1 = { 10 };
                        SetValue(ZombieNumData1, 80f, 0f); // SetValue(좀비 수 배열, 거리, 뒤쪽 좀비 최대 생성 시간[0일시 false])
                        break;
                    case 1:
                        int[] ZombieNumData2 = { 15, 15, 5, 5, 5, 0 };
                        SetValue(ZombieNumData2, 100f, 5f);
                        break;
                    case 2:
                        int[] ZombieNumData3 = { 0, 0, 0, 0, 0, 1 };
                        SetValue(ZombieNumData3, 40f, 4f);
                        break;
                    case 3:
                        int[] ZombieNumData4 = { 0, 15, 8, 5, 0 };
                        SetValue(ZombieNumData4, 30f, 4f);
                        break;
                    case 4:
                        int[] ZombieNumData5 = { 0, 0, 10, 8, 5 };
                        SetValue(ZombieNumData5, 30f, 4f);
                        break;
                    case 5:
                        break;
                }
                break;
            case 1:
                switch (stageLevelNum)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                }
                break;
            case 2:
                switch (stageLevelNum)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                }
                break;
            case 3:
                switch (stageLevelNum)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                }
                break;
            case 4:
                switch (stageLevelNum)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                }
                break;
        }
    }

    private void SetValue(int[] _zombieArr, float _distance, float time)
    {
        distance = _distance;
        createBackZombieTime = time;
        zombiesNum = _zombieArr;

        for (int i = 0; i < zombiesNum.Length; i++)
            totalZombieNum += zombiesNum[i];
        currentZombieNum = totalZombieNum;

        if (time != 0f)
            isCreateBackZombieOn = true;
    }

    private void SetMaps() // 맵과 목적지 이미지를 불러와서 변수에 저장한다. (이미지는 MapManager에서 생성된다.)
    {
        if(stageNum == 0 && stageLevelNum == 0) // 튜토리얼
        {
            start = FindObjectOfType<MapScript>().GetComponentsInChildren<SpriteRenderer>()[0].gameObject;
            destination = FindObjectOfType<MapScript>().GetComponentsInChildren<SpriteRenderer>()[1].gameObject;

            for (int i = 0; i < 5; i++)
                maps[i] = FindObjectOfType<MapScript>().GetComponentsInChildren<SpriteRenderer>()[i + 2].gameObject;
        } else
        {
            start = FindObjectOfType<MapScript>().GetComponentsInChildren<SpriteRenderer>()[(stageNum + 1) * 7].gameObject;
            destination = FindObjectOfType<MapScript>().GetComponentsInChildren<SpriteRenderer>()[(stageNum + 1) * 7 + 1].gameObject;
            
            for (int i = 0; i < 5; i++)
                maps[i] = FindObjectOfType<MapScript>().GetComponentsInChildren<SpriteRenderer>()[(stageNum + 1) * 7 + 2 + i].gameObject;
        }

        
    }
}
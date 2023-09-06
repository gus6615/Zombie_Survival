using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyScript : MonoBehaviour
{
    static private DontDestroyScript instance;
    static private int num; // 초기 씬에서 이 스크립트를 보유하고 있는 오브젝트들의 수
    static private List<int> list;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyScript[] datas = FindObjectsOfType<DontDestroyScript>();
            num = datas.Length;
            for (int i = 0; i < num; i++)
                DontDestroyOnLoad(datas[i].gameObject);
        }
        else
        {
            DontDestroyScript[] datas = FindObjectsOfType<DontDestroyScript>();
            for (int i = num; i < datas.Length; i++)
                Destroy(datas[i].gameObject);
        }
    }
}

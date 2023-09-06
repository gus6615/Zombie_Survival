using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    static public AbilityButton instance;

    [SerializeField] private GameObject[] infoObject;
    private int upAndDown; // 0 = UP, 1 = DOWN
    private bool isOpenInfo;

    private void Start()
    {
        StartCoroutine("CoWaitForPosition");

        infoObject[1].GetComponentsInChildren<Text>()[0].text = infoObject[0].GetComponentsInChildren<Text>()[0].text;
        infoObject[1].GetComponentsInChildren<Text>()[1].text = infoObject[0].GetComponentsInChildren<Text>()[1].text;

        infoObject[0].SetActive(false);
        infoObject[1].SetActive(false);
    }

    private void Update()
    {
        if(instance != this)
        {
            isOpenInfo = false;
            infoObject[upAndDown].SetActive(isOpenInfo);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.currentSelectedGameObject != this.gameObject)
            {
                isOpenInfo = false;
                infoObject[upAndDown].SetActive(isOpenInfo);
            }
        }
    }

    public void ButtonOn()
    {
        instance = this;
        instance.isOpenInfo = !instance.isOpenInfo;
        instance.infoObject[upAndDown].SetActive(isOpenInfo);
    }

    IEnumerator CoWaitForPosition()
    {
        yield return new WaitForEndOfFrame();

        if (this.transform.position.y >= 150f)
            upAndDown = 1;
        else
            upAndDown = 0;
    }
}

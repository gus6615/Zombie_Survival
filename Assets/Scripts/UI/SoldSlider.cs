using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SoldSlider : MonoBehaviour
{
    private Slider slider;
    static public int soldNum;

    // Start is called before the first frame update
    void Start()
    {
        slider = this.GetComponent<Slider>();
    }

    private void Update()
    {
        slider.value = soldNum = (int)slider.value;
        Shop.sold_slider.value = soldNum;
        Shop.sold_Text.text = soldNum.ToString();
        Shop.sold_goldTexts[1].text = (SaveScript.saveData.gold + soldNum * SaveScript.etcs[Shop.etcCodes[Shop.order] - SaveScript.saveData.hasGuns.Count - SaveScript.saveData.hasArmors.Count - 1].price) + " 원";
    }
}

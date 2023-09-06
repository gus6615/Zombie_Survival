using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainText : MonoBehaviour
{

    [SerializeField] private Text startText;

    Color startTextColor;
    bool switchAlpha; // false는 알파 감소, true는 알파 증가

    // Start is called before the first frame update
    void Start()
    {
        startTextColor = startText.color;
        switchAlpha = false;
    }

    // Update is called once per frame
    void Update()
    {
        ShowStartText();
    }

    private void ShowStartText()
    {
        if (startTextColor.a >= 1f) 
            switchAlpha = false;


        else if (startTextColor.a <= 0f)
            switchAlpha = true;

        if (switchAlpha)
            startTextColor.a += 0.75f * Time.deltaTime;
        else
            startTextColor.a -= 0.75f * Time.deltaTime;

        startText.color = startTextColor;
    }
}
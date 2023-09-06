using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    private Image[] allImages;
    private Text[] allTexts;
    private Color imageTemp;
    private Color TextTemp;
    private float currentTime;
    private bool isStart;

    // Start is called before the first frame update
    void Start()
    {
        allImages = this.GetComponentsInChildren<Image>();
        allTexts = this.GetComponentsInChildren<Text>();
        imageTemp = new Color(1, 1, 1, 1);
        TextTemp = new Color(0, 0, 0, 1);

        for (int i = 0; i < allImages.Length; i++)
            allImages[i].color = imageTemp;

        for (int i = 0; i < allTexts.Length; i++)
            allTexts[i].color = TextTemp;

        StartCoroutine("Wait");
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            imageTemp.a -= Time.deltaTime * 0.5f;
            TextTemp.a -= Time.deltaTime * 0.5f;

            // 이미지 컬러
            for (int i = 0; i < allImages.Length; i++)
                allImages[i].color = imageTemp;

            // 텍스트 컬러
            for (int i = 0; i < allTexts.Length; i++)
                allTexts[i].color = TextTemp;

            if (TextTemp.a <= 0f)
                Destroy(this.gameObject);
        }   
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        isStart = true;
    }
}

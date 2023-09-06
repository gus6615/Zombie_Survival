using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectScript : MonoBehaviour
{
    static private EffectScript instance;

    static private List<Image> images;
    static private List<Text> texts;
    static private bool isFade;
    static private List<float> goalA,_time;
    static private float fadeScale; // 프레임마다 Fade가 되는 정도
    static private List<Color> imageColors, textColors; // 최초의 이미지 컬러
    static private List<int> imageCount, textCount; 
    static private List<float> currentTime;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        images = new List<Image>(); // 복수 변수
        texts = new List<Text>(); // 복수 변수
        goalA = new List<float>(); // 단일 변수
        _time = new List<float>(); // 단일 변수
        imageColors = new List<Color>(); // 복수 변수
        textColors = new List<Color>(); // 복수 변수
        currentTime = new List<float>(); // 단일 변수
        imageCount = new List<int>(); // 단일 변수
        textCount = new List<int>(); // 단일 변수
    }

    private void Update()
    {
        if (isFade)
            Fade();
    }

    public void Fade()
    {
        // Fade Time Curculate
        for (int i = 0; i < currentTime.Count; i++)
            currentTime[i] += Time.deltaTime;

        // Image and Text Fade Effect
        for (int i = 0; i < imageCount.Count; i++)
        {
            int n = 0;
            if (i == 0) n = 0;
            else
                for (int k = 0; k < i; k++)
                    n += imageCount[k];

            for (int j = n; j < n + imageCount[i]; j++)
            {
                fadeScale = (goalA[i] - imageColors[j].a) / _time[i];
                images[j].color = new Color(images[j].color.r, images[j].color.g, images[j].color.b, images[j].color.a + fadeScale * Time.deltaTime);
            }
        }

        for (int i = 0; i < textCount.Count; i++)
        {
            int n = 0;
            if (i == 0) n = 0;
            else
                for (int k = 0; k < i; k++)
                    n += textCount[k];

            for (int j = n; j < n + textCount[i]; j++)
            {
                fadeScale = (goalA[i] - textColors[j].a) / _time[i];
                texts[j].color = new Color(texts[j].color.r, texts[j].color.g, texts[j].color.b, texts[j].color.a + fadeScale * Time.deltaTime);
            }
        }

        // Time Check
        for (int i = 0; i < currentTime.Count; i++)
        {
            if (currentTime[i] >= _time[i])
            {
                // Image Clear
                int n = 0;
                if (i == 0) n = 0;
                else
                    for (int k = 0; k < i; k++)
                        n += imageCount[k];

                for (int k = 0; k < imageCount[i]; k++)
                {
                    imageColors.RemoveAt(n);
                    images.RemoveAt(n);
                }

                imageCount.RemoveAt(i);

                // Text Clear
                n = 0;
                if (i == 0) n = 0;
                else
                    for (int k = 0; k < i; k++)
                        n += textCount[k];

                for (int k = 0; k < textCount[i]; k++)
                {
                    textColors.RemoveAt(n);
                    texts.RemoveAt(n);
                }

                textCount.RemoveAt(i);

                // General Clear
                goalA.RemoveAt(i);
                _time.RemoveAt(i);
                currentTime.RemoveAt(i);
                i--;
            }
        }

        // End Fade
        if (imageCount.Count == 0 && textCount.Count == 0)
        {
            isFade = false;
        }
    }

    static public void SettingFade(GameObject data, float a, float time)
    {
        Image[] imageData = data.GetComponentsInChildren<Image>();
        Text[] textData = data.GetComponentsInChildren<Text>();

        if(imageData.Length != 0 || textData.Length != 0)
        {
            images.AddRange(imageData);
            texts.AddRange(textData);
            imageCount.Add(imageData.Length);
            textCount.Add(textData.Length);
            for (int i = 0; i < imageData.Length; i++)
                imageColors.Add(imageData[i].color);
            for (int i = 0; i < textData.Length; i++)
                textColors.Add(textData[i].color);

            currentTime.Add(0f);
            goalA.Add(a);
            _time.Add(time);
            isFade = true;
        }
    }
}

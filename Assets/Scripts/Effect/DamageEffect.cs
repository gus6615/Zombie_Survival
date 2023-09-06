using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffect : MonoBehaviour
{
    private Image image;
    private Text text;
    private Color temp;
    public float fadeSpeed;
    public bool isStart;
    private Vector3 startVec;

    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponentInChildren<Text>();
        startVec = Camera.main.ScreenToWorldPoint(this.transform.position);

        image = GetComponentInChildren<Image>();
        fadeSpeed = 3f;
        text.gameObject.SetActive(true);
        temp = text.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            startVec += Vector3.up * Time.deltaTime;
            this.transform.position = Camera.main.WorldToScreenPoint(startVec);

            if (text.color.a >= 0.1f)
            {
                temp.a -= Time.deltaTime * fadeSpeed;
                text.color = temp;
                if (image != null)
                    image.color = new Color(1, 1, 1, temp.a);
            }
            else
                Destroy(this.gameObject);
        }
    }
}

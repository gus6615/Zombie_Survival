using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongAttackCircle : MonoBehaviour
{
    public GameObject attackEffect;
    private SpriteRenderer sprite;

    public bool isBloodCtrlOn;
    public float createTime;
    public float duringTime;
    private int fadeOrder;
    private bool isDuring;

    // Start is called before the first frame update
    void Start()
    {
        sprite = this.GetComponent<SpriteRenderer>();
        sprite.color = new Color(1f, 1f, 1f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        switch (fadeOrder)
        {
            case 0:
                if(sprite.color.a < 0.35f)
                {
                    Color temp = sprite.color;
                    temp.a += Time.deltaTime / createTime;
                    sprite.color = temp;
                }
                else
                {
                    if (!isDuring)
                    {
                        sprite.color = new Color(1f, 1f, 1f, 0.35f);
                        StartCoroutine("During");
                    }
                }
                break;
            case 1:
                if (sprite.color.a > 0f)
                {
                    Color temp = sprite.color;
                    temp.a -= Time.deltaTime / createTime;
                    sprite.color = temp;
                }
                else
                {
                    sprite.color = new Color(1f, 1f, 1f, 0f);
                    attackEffect.SetActive(true);
                    Destroy(this.gameObject);
                }
                break;
        }
    }

    IEnumerator During()
    {
        isDuring = true;

        yield return new WaitForSeconds(duringTime);

        fadeOrder = 1;
    }
}

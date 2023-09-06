using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FarmingObject : MonoBehaviour
{
    [SerializeField] private GameObject itemDrop;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private Transform itemDropSpace;
    private new AudioSource audio;
    private Image image;

    public bool isInit;
    public float colorA;
    private float scale;
    private bool isTouch, isForward;
    private float moveSpeed;

    public int maxCount;
    public int timesAsMaxCount;
    public int currentCount;
    private bool isReward;

    // Start is called before the first frame update
    void Start()
    {
        isInit = true;
        colorA = 0f;
        scale = 1f;
        moveSpeed = 0.5f;
        audio = GetComponent<AudioSource>();
        audio.clip = audioClips[0];
        image = GetComponent<Image>();
        image.color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInit)
        {
            Farming.objectHpSlider.gameObject.SetActive(true);
            Farming.objectHpSlider.maxValue = maxCount;
            Farming.objectHpSlider.value = maxCount - currentCount;

            Init();
        }

        if (isReward)
            FadeOff();

        if (isTouch)
        {
            SetColorA();
            SetScale();
        }
    }

    public void Init()
    {
        if(colorA < 1f)
        {
            colorA += Time.deltaTime * moveSpeed * 2f;
            image.color = new Color(1, 1, 1, colorA);
        }
        else
        {
            colorA = 1f;
            image.color = new Color(1, 1, 1, colorA);
            isInit = false;
        }
    }

    public void Click()
    {
        if (!isInit && !isReward)
        {
            isTouch = true;
            isForward = true;
            scale = 1f;
            colorA = 1f;
            this.transform.localScale = Vector3.one * scale;
            audio.Play();
            currentCount += 1;
            Farming.objectHpSlider.value = maxCount - currentCount;

            // 보너스골드 휙득
            if (Percent.GetRandFlag(0.1f + SaveScript.saveData.Farming_GoldPlusPercentUpgrade * 0.1f * SaveScript.upgradeDatas[6]))
                GetGold();

            // 보너스아이템 휙득
            GetItem();

            // 보성 휙득
            if (currentCount >= maxCount)
                 GetReward();
        }
    }

    public void GetGold()
    {
        int gold = (int)(Random.Range(Mathf.Pow(2, Farming.index - 1) * 10 + 1, Mathf.Pow(2, Farming.index) * 10) * (1 + SaveScript.saveData.Farming_GoldPlusMountUpgrade * SaveScript.upgradeDatas[7]));
        GameObject temp = Instantiate(itemDrop, itemDropSpace);
        temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-300f, 300f), Random.Range(300f, 350f));

        Farming.SetInfoText("(BONUS) " + gold + "원 휙득", Color.yellow);
        SaveScript.saveData.gold += gold;
        Farming.goldText.text = SaveScript.saveData.gold + "원";
    }

    public void GetItem()
    {
        List<Item> items = new List<Item>();

        for (int i = 0; i < Farming.index + 1; i++)
        {
            float percent = etc.FarmingPercentAsQuality[i] + etc.FarmingPlusPercentAsQuality[i] * SaveScript.saveData.Farming_ItemPlusPercentUpgrade;
            if(Percent.GetRandFlag(percent))
            {
                int code = 11; // 최초의 etc 아이템 코드
                List<int> selectedCodes = new List<int>();

                for (int j = 0; j < etc._quallity.Length; j++)
                    if (etc._quallity[j] == i)
                        selectedCodes.Add(code + j);

                int randCode = selectedCodes[Random.Range(0, selectedCodes.Count)];
                items.Add(new etc(randCode));
            }
        }

        for (int i = 0; i < items.Count; i++)
        {
            for (int j = 0; j < 1 + SaveScript.saveData.Farming_ItemPlusMountUpgrade * SaveScript.upgradeDatas[9]; j++)
            {
                GameObject temp = Instantiate(itemDrop, itemDropSpace);
                temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-300f, 300f), Random.Range(300f, 350f));
                temp.GetComponentsInChildren<Image>()[0].sprite = items[i].image.sprite;
                SaveScript.saveData.hasEtcs[items[i].itemCode - SaveScript.weaponNum - SaveScript.armorNum - 1]++;
            }

            Farming.SetInfoText("(BONUS) '" + items[i].name + "' x" + (1 + SaveScript.saveData.Farming_ItemPlusMountUpgrade * SaveScript.upgradeDatas[9]) + " 휙득", Item.colors[(items[i] as etc).quallity]);
        }
    }

    public void FadeOff()
    {
        Color temp = GetComponent<Image>().color;

        if (temp.a >= 0.01f)
        {
            temp.a -= Time.deltaTime;
            GetComponent<Image>().color = temp;
        }
        else
        {
            temp.a = 0f;
            GetComponent<Image>().color = temp;
            isReward = false;
            currentCount = 0;
            this.gameObject.SetActive(false);
            Farming.isRecreate = true;
        }
    }

    public void SetScale()
    {
        if (isForward)
        {
            if (scale < 1.1f)
            {
                scale += Time.deltaTime * moveSpeed;
                this.transform.localScale = Vector3.one * scale;
            }
            else
            {
                scale = 1.1f;
                this.transform.localScale = Vector3.one * scale;
                isForward = false;
            }
        }
        else
        {
            if (scale > 1f)
            {
                scale -= Time.deltaTime * moveSpeed;
                this.transform.localScale = Vector3.one * scale;
            }
            else
            {
                scale = 1f;
                this.transform.localScale = Vector3.one * scale;
                isTouch = false;
            }
        }
    }

    public void SetColorA()
    {
        if (isForward)
        {
            if (colorA > 0.9f)
            {
                colorA -= Time.deltaTime * moveSpeed * 2f;
                image.color = new Color(1, 1, 1, colorA);
            }
            else
            {
                colorA = 0.9f;
                image.color = new Color(1, 1, 1, colorA);
                isForward = false;
            }
        }
        else
        {
            if (colorA < 1f)
            {
                colorA += Time.deltaTime * moveSpeed;
                image.color = new Color(1, 1, 1, colorA);
            }
            else
            {
                colorA = 1f;
                image.color = new Color(1, 1, 1, colorA);
                isTouch = false;
            }
        }
    }

    public void GetReward()
    {
        isReward = true;
        Farming.objectHpSlider.gameObject.SetActive(false);

        // 보상 초기화 및 설정
        List<Item> items = new List<Item>();
        List<int> nums = new List<int>();
        int gold = (int)Random.Range(Mathf.Pow(2, Farming.index + 1) * 10 * timesAsMaxCount, Mathf.Pow(2, Farming.index + 2) * 10 * timesAsMaxCount);

        // 보상 채택
        for (int i = 0; i < Farming.index + 1; i++)
        {
            int code = 11; // 최초의 etc 아이템 코드
            List<int> selectedCodes = new List<int>();

            for (int j = 0; j < etc._quallity.Length; j++)
                if (etc._quallity[j] == i)
                    selectedCodes.Add(code + j);

            int count = 1;
            while (selectedCodes.Count != 0 && Percent.GetRandFlag(etc.FarmingPercentAsQuality[i] * (6f - count) * (Farming.index + 1)))
            {
                int randCode = selectedCodes[Random.Range(0, selectedCodes.Count)];
                items.Add(new etc(randCode));
                switch (i)
                {
                    case 0:
                        nums.Add(Random.Range((Farming.index * 3 + 1) * timesAsMaxCount, (Farming.index + 1) * 3 * timesAsMaxCount + 1));
                        break;
                    case 1:
                        nums.Add(Random.Range(((Farming.index - 1) * 2 + 1) * timesAsMaxCount, (Farming.index * 2) * timesAsMaxCount + 1));
                        break;
                    case 2:
                        nums.Add(Random.Range(((Farming.index - 2) * 2 + 1) * timesAsMaxCount, (Farming.index - 1) * 2 * timesAsMaxCount + 1));
                        break;
                    case 3:
                        nums.Add(Random.Range(1, 2 + (Farming.index - 3) + 1));
                        break;
                    case 4:
                        nums.Add(1);
                        break;
                }
                selectedCodes.Remove(randCode);
                count++;
            }
        }

        // 골드 보상 수령
        GameObject goldTemp = Instantiate(itemDrop, itemDropSpace);
        goldTemp.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-300f, 300f), Random.Range(300f, 350f));
        SaveScript.saveData.gold += gold;
        Farming.goldText.text = SaveScript.saveData.gold + "원";
        Farming.SetInfoText(gold + "원 휙득", Color.yellow);

        // 아이템 보상 수령
        for (int i = 0; i < items.Count; i++)
        {
            GameObject temp = Instantiate(itemDrop, itemDropSpace);
            temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-300f, 300f), Random.Range(300f, 350f));
            temp.GetComponentsInChildren<Image>()[0].sprite = items[i].image.sprite;

            SaveScript.saveData.hasEtcs[items[i].itemCode - SaveScript.weaponNum - SaveScript.armorNum - 1] += nums[i];
            Farming.SetInfoText("'" + items[i].name + "' x" + nums[i] + " 휙득", Item.colors[(items[i] as etc).quallity]);
        }
    }
}

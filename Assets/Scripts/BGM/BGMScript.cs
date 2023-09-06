using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMScript : MonoBehaviour
{
    static private BGMScript data;
    static private AudioSource[] audios;

    static private int audioIndex;
    static private bool isFadeIn; // true면 볼륨이 점점 커지고 false면 점점 작아진다.
    static private bool isFadeOn;

    static AudioSource temp;
    static float Volume;

    void Start()
    {
        if (data == null)
        {
            DontDestroyOnLoad(this.gameObject);
            data = this;
        }
        else if (data != null)
        {
            Destroy(this.gameObject);
        }

        audios = GetComponentsInChildren<AudioSource>();
        audioIndex = 0;
        isFadeIn = false;
        isFadeOn = false;
    }

    void Update()
    {
        if (isFadeOn)
        {
            temp = audios[audioIndex];
            Volume = temp.volume;
            float temp_volume = Volume;

            if(Volume <= 0f)
                temp.Play();

            if (isFadeIn)
            {
                if (Volume <= 1f)
                {
                    Volume += Time.deltaTime;
                    temp.volume = Volume;
                }
            }
            else
            {
                if (Volume >= 0f)
                {
                    Volume -= Time.deltaTime;
                    temp.volume = Volume;
                }
            }

            if (Mathf.Abs(temp_volume - Volume) >= 1f)
                isFadeOn = false;
        }
    }

    static public void SetFade(bool n_isFadeIn)
    {
        isFadeIn = n_isFadeIn;
    }

    static public void SetAudioIndex(int n)
    {
        audioIndex = n;
    }


    static public void StartFade(int index, bool n_isFadeIn) // index번째에 해당하는 BGM의 소리를 bool에 따라 커지거나 작아지게 한다. ( true면 커지고 false이면 작아진다. )
    {
        SetAudioIndex(index);
        SetFade(n_isFadeIn);
        isFadeOn = true;
    }
}

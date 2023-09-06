using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCtrl : MonoBehaviour {

    static private SoundCtrl instance;

    static public AudioSource[] gunShoutSounds, bioGunShoutSounds;
    static public AudioSource[] gunReloadSounds;
    static public AudioSource[] zombieSounds;

	// Use this for initialization
	void Awake () {
		if(instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;

            Transform[] temp = this.gameObject.GetComponentsInChildren<Transform>();

            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].gameObject.GetComponentInParent<Transform>().gameObject.name == "GunShoutSoundsObject")
                    gunShoutSounds = temp[i].GetComponentsInChildren<AudioSource>();
                else if (temp[i].gameObject.GetComponentInParent<Transform>().gameObject.name == "GunReloadSoundsObject")
                    gunReloadSounds = temp[i].GetComponentsInChildren<AudioSource>();
                else if (temp[i].gameObject.GetComponentInParent<Transform>().gameObject.name == "ZombieSoundsObject")
                    zombieSounds = temp[i].GetComponentsInChildren<AudioSource>();
                else if (temp[i].gameObject.GetComponentInParent<Transform>().gameObject.name == "BioGunShoutSoundsObject")
                    bioGunShoutSounds = temp[i].GetComponentsInChildren<AudioSource>();
            }
        } else if(instance != null)
        {
            Destroy(this.gameObject);
        }
    }
}

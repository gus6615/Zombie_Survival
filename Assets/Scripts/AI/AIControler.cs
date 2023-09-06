using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIControler : MonoBehaviour {

    [SerializeField] private GameObject StoryAI;
    [SerializeField] private GameObject AI;
    [SerializeField] private GameObject SpecialAI;
    static public AI[] StoryAIs, AIs, SpecialAIs;

	// Use this for initialization
	void Start () {
        StoryAIs = StoryAI.GetComponentsInChildren<AI>();
        AIs = AI.GetComponentsInChildren<AI>();
        SpecialAIs = SpecialAI.GetComponentsInChildren<AI>();

        CheckAIs();
	}

    public void CheckAIs()
    {
        for (int i = 0; i < StoryAIs.Length; i++)
        {
            if (SaveScript.saveData.hasStoryAI[i])
                StoryAIs[i].gameObject.SetActive(true);
            else
                StoryAIs[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < AIs.Length; i++)
        {
            if (SaveScript.saveData.hasAI[i])
                AIs[i].gameObject.SetActive(true);
            else
                AIs[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < SpecialAIs.Length; i++)
        {
            if (SaveScript.saveData.hasSpecialAI[i])
                SpecialAIs[i].gameObject.SetActive(true);
            else
                SpecialAIs[i].gameObject.SetActive(false);
        }
    }

    static public void SpecialAIWork()
    {
        for (int i = 0; i < StoryAIs.Length; i++)
        {
            StoryAIs[i].animator.speed = 0;
        }

        for (int i = 0; i < AIs.Length; i++)
        {
            AIs[i].animator.speed = 0;
        }
    }

    static public void SpecialAINotWork()
    {
        for (int i = 0; i < StoryAIs.Length; i++)
        {
            StoryAIs[i].animator.speed = 1;
        }

        for (int i = 0; i < AIs.Length; i++)
        {
            AIs[i].animator.speed = 1;
        }
    }
}

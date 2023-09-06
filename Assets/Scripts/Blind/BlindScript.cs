using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BlindScript : MonoBehaviour
{
    static private BlindScript data;
    static private Animator animator;

    static private StageController stageController;

    static public Image blindImage;
    static public int currentSceneIndex = 0;
    static public bool isChangeScene;
    static public bool isEndChange;

    /***********************************
     * 0 = Main
     * 1 = Story
     * 2 = Shelter
     * 3 = Game
     * 4 = Profile
     * 5 = Shop
     * 6 = Upgrade
     * 7 = Farming
     * 8 = Quast
     **********************************/

    // Start is called before the first frame update
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

        animator = GetComponentInChildren<Animator>();
        BGMScript.StartFade(currentSceneIndex, true);
        isChangeScene = false;
        blindImage = GetComponentInChildren<Image>();
        blindImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isChangeScene)
        {
            if (currentSceneIndex == 0) // MainScene's Blind
            {
                if (Input.anyKeyDown)
                {
                    blindImage.gameObject.SetActive(true);
                    isChangeScene = true;
                    BGMScript.StartFade(currentSceneIndex, false);
                    FadeIn();
                    currentSceneIndex = 1;
                    StartCoroutine(switchScene(currentSceneIndex, currentSceneIndex, 1f)); // 스토리씬으로 이동
                }
            }
            else if (currentSceneIndex == 1) // StoryScene's Blind
            {
                switch (SaveScript.saveData.storyIndex)
                {
                    case 0:
                        if (StoryScript.isNext)
                        {
                            blindImage.gameObject.SetActive(true);
                            isChangeScene = true;
                            BGMScript.StartFade(currentSceneIndex, false);
                            FadeIn();
                            currentSceneIndex = 2;
                            StartCoroutine(switchScene(currentSceneIndex, currentSceneIndex, 1f)); // 쉘터씬으로 이동
                        }
                        break;
                }
            }
            else if(currentSceneIndex == 2) // Shelter's Blind
            {
                if (ShelterUICtrl.isGotoGameScene)
                {
                    blindImage.gameObject.SetActive(true);
                    isChangeScene = true;
                    BGMScript.StartFade(currentSceneIndex, false);
                    FadeIn();
                    currentSceneIndex = 3;
                    StartCoroutine(switchScene(currentSceneIndex, currentSceneIndex, 1f)); // 게임씬으로 이동
                    ShelterUICtrl.isGotoGameScene = false;
                }

                if (ShelterUICtrl.isGotoProfileScene) // Profile
                {
                    blindImage.gameObject.SetActive(true);
                    isChangeScene = true;
                    FadeIn();
                    currentSceneIndex = 4;
                    StartCoroutine(switchScene(currentSceneIndex, 1f));
                    ShelterUICtrl.isGotoProfileScene = false;
                }

                if (ShelterUICtrl.isGotoShopScene) // Shop
                {
                    blindImage.gameObject.SetActive(true);
                    isChangeScene = true;
                    FadeIn();
                    currentSceneIndex = 5;
                    StartCoroutine(switchScene(currentSceneIndex, 1f));
                    ShelterUICtrl.isGotoShopScene = false;
                }

                if (ShelterUICtrl.isGotoUpgradeScene) // Upgrade
                {
                    blindImage.gameObject.SetActive(true);
                    isChangeScene = true;
                    FadeIn();
                    currentSceneIndex = 6;
                    StartCoroutine(switchScene(currentSceneIndex, 1f));
                    ShelterUICtrl.isGotoUpgradeScene = false;
                }

                if (ShelterUICtrl.isGotoFarmingScene) // Farming
                {
                    blindImage.gameObject.SetActive(true);
                    isChangeScene = true;
                    FadeIn();
                    currentSceneIndex = 7;
                    StartCoroutine(switchScene(currentSceneIndex, 1f));
                    ShelterUICtrl.isGotoFarmingScene = false;
                }

                if (ShelterUICtrl.isGotoQuastScene) // Quast
                {
                    blindImage.gameObject.SetActive(true);
                    isChangeScene = true;
                    FadeIn();
                    currentSceneIndex = 8;
                    StartCoroutine(switchScene(currentSceneIndex, 1f));
                    ShelterUICtrl.isGotoQuastScene = false;
                }
            }
            else if(currentSceneIndex == 3) // Game's Blind
            {
                stageController = FindObjectOfType<StageController>();
                if (stageController.isGoToShelter)
                {
                    if (SaveScript.saveData.isTutorial)
                        SaveScript.saveData.isTutorial = false;
                    blindImage.gameObject.SetActive(true);
                    isChangeScene = true;
                    BGMScript.StartFade(currentSceneIndex, false);
                    FadeIn();
                    currentSceneIndex = 2;
                    StartCoroutine(switchScene(currentSceneIndex, currentSceneIndex, 1f)); // 로비로 이동
                }
            }
            else if (currentSceneIndex == 4) // Profile's Blind
            {
                if (Profile.isBack)
                {
                    blindImage.gameObject.SetActive(true);
                    isChangeScene = true;
                    FadeIn();
                    currentSceneIndex = 2;
                    StartCoroutine(switchScene(currentSceneIndex, 1f)); // 로비로 이동
                    Profile.isBack = false;
                }
            }
            else if (currentSceneIndex == 5) // Shop's Blind
            {
                if (Shop.isBack)
                {
                    blindImage.gameObject.SetActive(true);
                    isChangeScene = true;
                    FadeIn();
                    currentSceneIndex = 2;
                    StartCoroutine(switchScene(currentSceneIndex, 1f)); // 로비로 이동
                    Shop.isBack = false;
                }
            }
            else if (currentSceneIndex == 6) // Upgrade's Blind
            {
                if (Upgrade.isBack)
                {
                    blindImage.gameObject.SetActive(true);
                    isChangeScene = true;
                    FadeIn();
                    currentSceneIndex = 2;
                    StartCoroutine(switchScene(currentSceneIndex, 1f)); // 로비로 이동
                    Upgrade.isBack = false;
                }
            }
            else if (currentSceneIndex == 7) // Farming's Blind
            {
                if (Farming.isBack)
                {
                    blindImage.gameObject.SetActive(true);
                    isChangeScene = true;
                    FadeIn();
                    currentSceneIndex = 2;
                    StartCoroutine(switchScene(currentSceneIndex, 1f)); // 로비로 이동
                    Farming.isBack = false;
                }
            }
            else if (currentSceneIndex == 8) // Quast's Blind
            {

            }
        }
    }

    static public void FadeIn()
    {
        animator.SetBool("isBlind", true);
        animator.SetBool("isFade", false);
    }

    static public void FadeOut()
    {
        animator.SetBool("isBlind", false);
        animator.SetBool("isFade", true);
    }

    static public void FadeEnd()
    {
        animator.SetBool("isBlind", false);
        animator.SetBool("isFade", false);
    }

    static public IEnumerator switchScene(int Sceneindex, int BGMindex, float time)
    {
        isEndChange = false;
        animator.speed = 1f / time;
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(Sceneindex);
        FadeOut();
        BGMScript.StartFade(BGMindex, true);

        SaveScript.SaveData();

        yield return new WaitForSeconds(time);
        FadeEnd();
        isChangeScene = false;
        isEndChange = true;
        blindImage.gameObject.SetActive(false);
    }

    static public IEnumerator switchScene(int Sceneindex, float time)
    {
        isEndChange = false;
        animator.speed = 1f / time;
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(Sceneindex);
        FadeOut();

        SaveScript.SaveData();

        yield return new WaitForSeconds(time);
        FadeEnd();
        isChangeScene = false;
        isEndChange = true;
        blindImage.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWeaponButton : MonoBehaviour
{
    private PrintUI printUI;
    [SerializeField] private GameObject playerWeaponObject;
    private ImageOrder[] playerWeapons;
    private ItemOrder[] bioWeapons;

    static public bool isChangeWeapon; // 리로드하는 경우와 무기 교체가 겹치는 현상을 조절한다.

    // Start is called before the first frame update
    void Start()
    {
        printUI = FindObjectOfType<PrintUI>();
        playerWeapons = playerWeaponObject.GetComponentsInChildren<ImageOrder>();
        bioWeapons = playerWeaponObject.GetComponentsInChildren<ItemOrder>();

        for (int j = 0; j < playerWeapons.Length; j++)
            playerWeapons[j].gameObject.SetActive(false);
        playerWeapons[SaveScript.saveData.equipGun].gameObject.SetActive(true);

        for (int j = 0; j < bioWeapons.Length; j++)
            bioWeapons[j].gameObject.SetActive(false);
    }

    public void ChangeWeapon()
    {
        isChangeWeapon = true;
        int temp = SaveScript.saveData.equipGun;

        for (int i = 0; i < SaveScript.weaponNum; i++)
        {
            if (++temp >= SaveScript.weaponNum)
                temp = 0;

            if (SaveScript.saveData.hasGuns[temp])
            {
                if (SaveScript.saveData.hasGuns[temp])
                {
                    SaveScript.saveData.equipGun = temp;

                    for (int j = 0; j < playerWeapons.Length; j++)
                        playerWeapons[j].gameObject.SetActive(false);
                    playerWeapons[temp].gameObject.SetActive(true);

                    printUI.gunImage.sprite = SaveScript.guns[temp].image.sprite; // 총 이미지 변경
                    if (temp == 0)
                        printUI.bulletText.text = SaveScript.guns[temp].currentBulletNum + " / ∞";
                    else
                        printUI.bulletText.text = SaveScript.guns[temp].currentBulletNum + " / " + SaveScript.saveData.hasGunsBullets[temp];
                    printUI.bulletSlider.maxValue = SaveScript.guns[temp].bulletNum;
                    printUI.bulletSlider.value = SaveScript.guns[temp].currentBulletNum;
                    printUI.reloadingText.gameObject.SetActive(false);
                    ShoutButtonCtrl.SetShotInfo();
                    CameraCtrl.ChangeCameraSize(SaveScript.guns[temp].shotDis);

                    break;
                }
            }
        }
    }

    public void SettingBioWeapon(int data)
    {
        isChangeWeapon = true;

        for (int j = 0; j < playerWeapons.Length; j++)
            playerWeapons[j].gameObject.SetActive(false);

        for (int j = 0; j < bioWeapons.Length; j++)
            bioWeapons[j].gameObject.SetActive(false);
        bioWeapons[data].gameObject.SetActive(true);

        printUI.gunImage.sprite = SaveScript.bioGuns[data].image.sprite; // 총 이미지 변경

        printUI.bulletText.text = SaveScript.bioGuns[data].currentBulletNum.ToString();
        printUI.bulletSlider.maxValue = SaveScript.bioGuns[data].bulletNum;
        printUI.bulletSlider.value = SaveScript.bioGuns[data].currentBulletNum;
        printUI.reloadingText.gameObject.SetActive(false);
        ShoutButtonCtrl.SetShotInfo();
        CameraCtrl.ChangeCameraSize(SaveScript.bioGuns[data].shotDis);
    }

    public void EndBioWeapon()
    {
        isChangeWeapon = true;

        for (int j = 0; j < playerWeapons.Length; j++)
            playerWeapons[j].gameObject.SetActive(false);
        playerWeapons[SaveScript.saveData.equipGun].gameObject.SetActive(true);

        for (int j = 0; j < bioWeapons.Length; j++)
            bioWeapons[j].gameObject.SetActive(false);

        printUI.gunImage.sprite = SaveScript.guns[SaveScript.saveData.equipGun].image.sprite; // 총 이미지 변경

        if (SaveScript.saveData.equipGun == 0)
            printUI.bulletText.text = SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum + " / ∞";
        else
            printUI.bulletText.text = SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum + " / " + SaveScript.saveData.hasGunsBullets[SaveScript.saveData.equipGun];
        printUI.bulletSlider.maxValue = SaveScript.guns[SaveScript.saveData.equipGun].bulletNum;
        printUI.bulletSlider.value = SaveScript.guns[SaveScript.saveData.equipGun].currentBulletNum;
        printUI.reloadingText.gameObject.SetActive(false);
        ShoutButtonCtrl.SetShotInfo();
        CameraCtrl.ChangeCameraSize(SaveScript.guns[SaveScript.saveData.equipGun].shotDis);
    }
}

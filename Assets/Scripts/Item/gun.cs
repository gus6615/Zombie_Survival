using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun : Item {

    private int[] _types, _bioTypes;
    private float[] _damages, _bioDamages;
    private int[] _bulletNums, _bioBulletNums;
    private float[] _shootDelayTimes, _bioShootDelayTimes;
    private float[] _reloadingTimes;
    private float[] _bulletShellDis, _bioBulletShellDis;
    private float[] _shotDis, _bioShotDis;
    private int[] _prices, _ironBarNums;
    private bool isSavedData;

    public bool isHas; // 가지고 있는가?
    public int type; // 1은 단발(권총 및 소총), 2는 다발(샷건), 3은 관통(저격소총)
    public float damage; // 데미지
    public int bulletNum; // 최대 장탄 수
    public int currentBulletNum; // 남은 총알 수
    public float shootDelayTime; // 연사력, 작을수록 연사력이 좋다.
    public float reloadingTime; // 리로드 시간
    public float bulletShellDis; // 탄피 거리
    public float shotDis; // 카메라 크기 (유효거리) 
    public int price; // 구매 가격
    public int ironBarNum; // 구매 철괴수

    public AudioSource shoutSound;
    public AudioSource reloadSound;

    public gun(int _itemCode)
    {
        if (!isInit)
            init();

        if (!isSavedData)
            SavedData();

        itemCode = _itemCode;
        SetItem();
        SetValue();
    }

    public void SetValue()
    {
        int weaponItemCode = Mathf.Abs(itemCode) - 1;

        if (itemCode > 0)
        {
            type = _types[weaponItemCode];
            damage = _damages[weaponItemCode];
            currentBulletNum = bulletNum = _bulletNums[weaponItemCode];
            shootDelayTime = _shootDelayTimes[weaponItemCode];
            reloadingTime = _reloadingTimes[weaponItemCode];
            bulletShellDis = _bulletShellDis[weaponItemCode];
            shotDis = _shotDis[weaponItemCode];
            ironBarNum = _ironBarNums[weaponItemCode];
            price = _prices[weaponItemCode];

            reloadSound = SoundCtrl.gunReloadSounds[weaponItemCode];
            shoutSound = SoundCtrl.gunShoutSounds[weaponItemCode];
        }
        else
        {
            type = _bioTypes[weaponItemCode];
            damage = _bioDamages[weaponItemCode];
            currentBulletNum = bulletNum = _bioBulletNums[weaponItemCode];
            shootDelayTime = _bioShootDelayTimes[weaponItemCode];
            bulletShellDis = _bioBulletShellDis[weaponItemCode];
            shotDis = _bioShotDis[weaponItemCode];

            shoutSound = SoundCtrl.bioGunShoutSounds[weaponItemCode];
        }
    }

    public void SavedData()
    {
        isSavedData = true;

        _types = new int[SaveScript.weaponNum];
        _types[0] = 1;
        _types[1] = 1;
        _types[2] = 2;
        _types[3] = 1;
        _types[4] = 3;

        _damages = new float[SaveScript.weaponNum];
        _damages[0] = 12f;
        _damages[1] = 21f;
        _damages[2] = 25f;
        _damages[3] = 58f;
        _damages[4] = 200f;

        _bulletNums = new int[SaveScript.weaponNum];
        _bulletNums[0] = 7;
        _bulletNums[1] = 25;
        _bulletNums[2] = 9;
        _bulletNums[3] = 30;
        _bulletNums[4] = 10;

        _shootDelayTimes = new float[SaveScript.weaponNum];
        _shootDelayTimes[0] = 0.3f;
        _shootDelayTimes[1] = 0.1f;
        _shootDelayTimes[2] = 0.5f;
        _shootDelayTimes[3] = 0.15f;
        _shootDelayTimes[4] = 0.5f;

        _reloadingTimes = new float[SaveScript.weaponNum];
        _reloadingTimes[0] = 1.5f;
        _reloadingTimes[1] = 2.0f;
        _reloadingTimes[2] = 2.5f;
        _reloadingTimes[3] = 2.5f;
        _reloadingTimes[4] = 3f;

        _bulletShellDis = new float[SaveScript.weaponNum];
        _bulletShellDis[0] = 0.5f;
        _bulletShellDis[1] = 1f;
        _bulletShellDis[2] = 1.3f;
        _bulletShellDis[3] = 2.2f;
        _bulletShellDis[4] = 2.7f;

        _shotDis = new float[SaveScript.weaponNum];
        _shotDis[0] = 10f;
        _shotDis[1] = 12f;
        _shotDis[2] = 8f;
        _shotDis[3] = 13f;
        _shotDis[4] = 14f;

        _prices = new int[SaveScript.weaponNum];
        _prices[0] = 500;
        _prices[1] = 2500;
        _prices[2] = 6000;
        _prices[3] = 15000;
        _prices[4] = 25000;

        _ironBarNums = new int[SaveScript.weaponNum];
        _ironBarNums[0] = 5;
        _ironBarNums[1] = 15;
        _ironBarNums[2] = 40;
        _ironBarNums[3] = 80;
        _ironBarNums[4] = 150;

        _bioTypes = new int[SaveScript.bioWeaponNum];
        _bioTypes[0] = 1;

        _bioDamages = new float[SaveScript.bioWeaponNum];
        _bioDamages[0] = 40f;

        _bioBulletNums = new int[SaveScript.bioWeaponNum];
        _bioBulletNums[0] = 30;

        _bioShootDelayTimes = new float[SaveScript.bioWeaponNum];
        _bioShootDelayTimes[0] = 0.2f;

        _bioBulletShellDis = new float[SaveScript.bioWeaponNum];
        _bioBulletShellDis[0] = 3f;

        _bioShotDis = new float[SaveScript.bioWeaponNum];
        _bioShotDis[0] = 9f;
    }
}

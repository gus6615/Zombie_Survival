using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    public new string name;
    public int type; // -1 = SpecialAI ,0 = ShotAI, 1 = AssistAI
    public Ability[] abilities;
    public int price;
    public int redJam;
    public int blueJam;
    public int level;
    public int workPorce;
    public bool isFarming;
    public string weaponName;
    public Color color;
    public bool isBloodCtrlOn;

    public Animator animator;
    protected new AudioSource audio;
    protected PlayerScript playerScript;
    protected GameObject player;
    protected new Camera camera;

    public SpriteRenderer shop_image;
    public SpriteRenderer weapon_image;
}

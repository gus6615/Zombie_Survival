using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialAI : AI
{
    public Canvas canvas; // 스킬 캔버스
    public GameObject damageObject;
    public Vector3 savedPos;

    public bool isCanUse; // 현재 스킬이 사용 가능한가
    public bool isWork; // 스킬 사용 여부 확인
    public bool isStartAct; // 최초의 동작 시전 확인

    public string skillInfo; // 스킬 설명
    public string typeInfo; // 용병의 타입 (공격 분야, 방어 분야, 보조 분야 등)
    public float damage;
    public float coolTime; // 쿨타임
    public float currentCoolTime;
    public float fallTime, skillTime; // 공격 준비 시간, 공격 행동 시간
    public float mediatedDisX, mediatedDisY; // 천장에서 떨어지는 거리의 축적
    public float distance; // 사거리
}

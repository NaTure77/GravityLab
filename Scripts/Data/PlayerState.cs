using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* 플레이어의 상태 정보를 담는 클래스.
 * StageController와 함께 죽지 않도록 설정한다. 단,
 * Main 씬으로 가면 죽는다.
 * */
public class PlayerState : MonoBehaviour{
    public static float HP = 100;
    public static int Coin = 0;
    public static float Power = 10;
    public static float moveSpeed = 13;//13.0f;
    public static float runSpeed = 30f;
    public static float jumpPower = 70.0f;
    public Rigidbody rgb;
    void Awake()
    {
    }

   


}

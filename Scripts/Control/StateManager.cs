using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {

    public static bool isPaused = false;// UIManager
    public static bool InventoryEnabled = false;//StageUI
    public static bool useSmallUI = false;//SmallUITool
    public static bool isFlying = false;
    public static bool isGroundChanging = false;

    public static bool isGrounded = true;
    public static bool isJumping = false;
    public static class keySet
    {
        public static bool front;
        public static bool left;
        public static bool back;
        public static bool right;
        public static bool attack;
        public static bool moveKey;
        public static bool aim;
        public static bool dash;
        public static bool inventory;
        public static bool jump;
        public static float h;
        public static float v;
    }

    void resetAll()
    {
        isPaused = false;
        InventoryEnabled = false;
        useSmallUI = false;
        isFlying = false;
        isGroundChanging = false;
    }
    void Awake()
    {
        resetAll();
        StartCoroutine(CheckInput());
        Application.targetFrameRate = 60;
    }
    IEnumerator CheckInput()
    {
        while (true)
        {
            keySet.h = Input.GetAxis("Horizontal");
            keySet.v = Input.GetAxis("Vertical");
            keySet.front = Input.GetKey(KeyCode.W);
            keySet.left = Input.GetKey(KeyCode.A);
            keySet.back = Input.GetKey(KeyCode.S);
            keySet.right = Input.GetKey(KeyCode.D);
            keySet.moveKey = keySet.front || keySet.left || keySet.back || keySet.right;
            keySet.attack = Input.GetKey(KeyCode.Mouse0) && !Input.GetKeyUp(KeyCode.Mouse0)&& !(InventoryEnabled || isPaused || useSmallUI);
            keySet.aim = (Input.GetKey(KeyCode.Mouse1) || keySet.attack) &&!isPaused;
            keySet.jump = Input.GetKeyDown(KeyCode.Space);
            yield return null;
        }
    }
}

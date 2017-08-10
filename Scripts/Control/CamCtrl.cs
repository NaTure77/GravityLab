using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCtrl : MonoBehaviour {

    public static CamCtrl instance;
    public float sensitivityY = 8F;
    public float sensitivityX = 8F;
    public GameObject CrossHair;

    float y = 0f;
    float x = 0f;

    GameObject cam;
    Vector3 DefaultCamPos = new Vector3(0, 0, -6.5f);
    Vector3 AimCamPos = new Vector3(-1.3f, 0, -2f);
    Vector3 CamPos {
        get {
            if (StateManager.keySet.aim) return AimCamPos;
            else return DefaultCamPos;
        }
    }
    public Transform target;

    public float followSpeed = 20f;

    bool isWallCrashed = false;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        CrossHair = GameObject.Find("CrossHair");
        CrossHair.SetActive(false);
        cam = GameObject.Find("Main Camera");
        StartCoroutine(awareWorld());
        StartCoroutine(DoLogic());
        StartCoroutine(NotPassWall(cam.transform,GameObject.Find("RaycastPos").transform));
    }

    private IEnumerator DoLogic()
    {
        float deltaX = 0;
        float deltaY = 0;
        while (true)
        {
            deltaY = Input.GetAxisRaw("Mouse Y") * sensitivityY;
            deltaX = Input.GetAxisRaw("Mouse X") * sensitivityX;
            if (Time.deltaTime != 0)
            {
                if (!isWallCrashed)
                    SetCameraMode(CamPos,StateManager.keySet.aim);
                ChangeHeading(deltaX, -deltaY);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    void ChangeHeading(float aVal, float bVal)
    {
        if (StateManager.InventoryEnabled) return;
        y += bVal;
        x += aVal;
        if (y >= 80) y = 80;
        else if (y <= -60) y = -60;

        WrapAngle(ref y);
        WrapAngle(ref x);
        transform.localEulerAngles = (Vector3.right * y + Vector3.up * x);//new Vector3(y, x, 0);
    }
    public void WrapAngle(ref float angle)
    {
        if (angle < -180F)
            angle += 360F;
        if (angle > 180F)
            angle -= 360F;
    }
    void SetCameraMode(Vector3 pos, bool enableCrossHair)
    {
        CrossHair.SetActive(enableCrossHair);
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, CamPos, Time.deltaTime * 7);
    }
    IEnumerator awareWorld()
    {
        float enabledDir = 1000;
        Vector2 input = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
        RaycastHit hit;
        while (true)
        {
            Ray ray = Camera.main.ScreenPointToRay(input);
            if (Physics.Raycast(ray, out hit, enabledDir))
            {
                if (hit.collider.CompareTag("Panel"))
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    StateManager.useSmallUI = true;

                }
                else if (!(StateManager.isPaused))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    StateManager.useSmallUI = false;
                    Cursor.visible = false;
                }

                if (hit.collider.CompareTag("Ground") && !StateManager.isFlying && !hit.transform.gameObject.Equals(MoveController.instance.currentGruond))
                {
                    StageUI.instance.EnableTargetInfo(hit.collider.name);
                    StageUI.instance.ShowDistance((int)Vector3.Distance(MoveController.instance.player.transform.position, hit.point));
                    StageUI.instance.TargetInfo.transform.position = input + Vector2.up * Screen.height * 0.2f;
                    Debug.DrawRay(Vector3.zero, hit.transform.forward, Color.yellow);
                    if (Input.GetKey(KeyCode.Z))
                    {
                        StartCoroutine(GroundChanger.ChangeGroundOnJump(MoveController.instance.transform,hit.transform,hit.normal, hit.point,() => StartCoroutine(MoveController.instance.JumpLogic(4))));
                    }
                }
                else StageUI.instance.DisableTargetInfo();
            }
            else
            {
                StageUI.instance.DisableTargetInfo();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator NotPassWall(Transform cam, Transform RaycastPos)
    {
        float dist = cam.transform.localPosition.z * -1;
        float raySphereradious = 0.5f;
        RaycastHit hit;
        while (true)
        {
            RaycastPos.LookAt(cam);
            dist = cam.transform.localPosition.z * -1;
            Debug.DrawRay(RaycastPos.position, RaycastPos.forward * dist, Color.red, Time.deltaTime);
            if (Physics.SphereCast(RaycastPos.position, raySphereradious, RaycastPos.forward, out hit, dist))
            {
                isWallCrashed = true;
                cam.transform.position = RaycastPos.position + RaycastPos.forward * hit.distance;
            }
            else isWallCrashed = false;
            yield return null;
        }
    }
}

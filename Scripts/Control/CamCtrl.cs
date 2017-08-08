using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCtrl : MonoBehaviour {

    public static CamCtrl instance;
    public float sensitivityY = 8F;
    public float sensitivityX = 8F;
    public GameObject CrossHair;
    float y = 0F;
    float x = 0f;
    float deltaX = 0;
    float deltaY = 0;
    GameObject cam;
    Vector3 DefaultCamPos = new Vector3(0, 0, -6.5f);
    Vector3 AimCamPos = new Vector3(-1.3f, 0, -2f);
    Vector3 CamPos;
    public Transform RaycastPos;
    public Transform target;

    Transform attackArea;
    Vector3 originAttackArea;
    Vector3 aimAttackArea;
    public float followSpeed = 20f;

    bool isWallCrashed = false;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        //Time.timeScale = 0.3f;
        CamPos = DefaultCamPos;
        CrossHair = GameObject.Find("CrossHair");
        //CrossHair.SetActive(false);
        //target = GameObject.Find("CamTarget").GetComponent<Transform>();
        attackArea= GameObject.Find("AttackArea").GetComponent<Transform>();
        originAttackArea = attackArea.localPosition;
        aimAttackArea = originAttackArea + new Vector3(AimCamPos.x, 0, 0);
        cam = GameObject.Find("Main Camera");
        StartCoroutine(awareWorld());
        //StartCoroutine(DoLogic());
        StartCoroutine(NotPassWall());
    }
    void Update()
    {
         deltaY = Input.GetAxisRaw("Mouse Y") * sensitivityY;
         deltaX = Input.GetAxisRaw("Mouse X") * sensitivityX;
        if (Time.deltaTime != 0)
        {
            Follow();
            ChangeHeading(deltaX, -deltaY);

        }
        /*if (!StateManager.isPaused)
        {
            Follow();
            ChangeHeading(deltaX, -deltaY);

        }*/
    }
    /*void ChangeHeading(float aVal, float bVal)
    {
        x += aVal;
        if ((y < 90 || bVal < 0) && (y > -90 || bVal > 0))
            y += bVal;
        WrapAngle(ref x);
        WrapAngle(ref y);
        transform.localEulerAngles = new Vector3(y, x, 0);
    }*/
    IEnumerator DoLogic()
    {
        while(true)
        {
            if (Time.deltaTime != 0)
            {
                Follow();
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

        WrapAngle2(ref y);
        WrapAngle2(ref x);
        transform.localEulerAngles = (Vector3.right * y + Vector3.up * x);//new Vector3(y, x, 0);
    }

    public void resetHeading()
    {
        //y = transform.localEulerAngles.x;
        x = transform.localEulerAngles.y;
    }
    public void WrapAngle(ref float angle)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
    }
    public void WrapAngle2(ref float angle)
    {
        if (angle < -180F)
            angle += 360F;
        if (angle > 180F)
            angle -= 360F;
    }
    void Follow()
    {
        transform.position = Vector3.Slerp(transform.position, target.position, Time.smoothDeltaTime * followSpeed);
        //transform.position += (target.position - transform.position) / 16;
        //transform.position = target.position;
        //transform.position = new Vector3(transform.position.x, target.position.y + 3, transform.position.z);
        if (Input.GetKey(KeyCode.Mouse1))
        {
            CamPos = AimCamPos;
            CrossHair.SetActive(!StateManager.useSmallUI);
            attackArea.localPosition = aimAttackArea;
        }
        else
        {
            CamPos = DefaultCamPos;
            CrossHair.SetActive(false);
            attackArea.localPosition = originAttackArea;
        }
        if(!isWallCrashed)cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, CamPos, Time.deltaTime * 7);


    }
    IEnumerator awareWorld()
    {
        float enabledDir = 1000;
        while (true)
        {
            Vector2 input = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0f);
            Ray ray = Camera.main.ScreenPointToRay(input);
            RaycastHit hit;
            //Debug.Log(Cursor.visible);
            if (Physics.Raycast(ray, out hit, enabledDir))
            {
                if (!(hit.collider.CompareTag("Untagged") || hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Player") || hit.collider.CompareTag("Panel")))
                {
                    StageUI.instance.EnableTargetInfo(hit.collider.name);
                    StageUI.instance.TargetInfo.transform.position = Camera.main.WorldToScreenPoint(hit.collider.transform.position + new Vector3(0, 2, 0));
                }
                else
                {
                    StageUI.instance.DisableTargetInfo();
                }
                if (hit.collider.CompareTag("Panel"))
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    StateManager.useSmallUI = true;

                }
                else if (!(StateManager.InventoryEnabled || StateManager.isPaused))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    StateManager.useSmallUI = false;
                    Cursor.visible = false;
                }

                if (hit.collider.CompareTag("Ground") && !StateManager.isFlying)
                {

                    //if (!MoveController.instance.isJumping && !MoveController.instance.currentGruond.Equals(hit.collider.gameObject)) break;
                    StageUI.instance.EnableTargetInfo(hit.collider.name);
                    StageUI.instance.TargetInfo.transform.position = input + Vector2.up * Screen.height * 0.2f;
                    Debug.DrawRay(Vector3.zero, hit.transform.forward, Color.yellow);
                    if (Input.GetKey(KeyCode.Z))
                    {
                        //Debug.Log("Fly");
                        //Debug.Log("count");
                        //StartCoroutine(MoveController.instance.Jump(MoveController.instance.jumpPower));
                        //Debug.Log(Vector3.Distance(MoveController.instance.transform.position, hit.point));

                        StartCoroutine(GroundChanger.instance.ChangeGroundOnJump(hit.transform, hit.point));
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

    IEnumerator NotPassWall()
    {
        float dist = CamPos.z * -1;
        float raySphereradious = 0.5f;
        RaycastHit hit;
        while (true)
        {
            RaycastPos.LookAt(cam.transform);
            dist = cam.transform.localPosition.z * -1;
            Debug.DrawRay(RaycastPos.position, RaycastPos.forward * dist, Color.red, Time.deltaTime);
            if (Physics.SphereCast(RaycastPos.position, raySphereradious, RaycastPos.forward, out hit, dist))
            {
                isWallCrashed = true;

                cam.transform.position = RaycastPos.position + RaycastPos.forward * hit.distance;//hit.point;
            }
            else isWallCrashed = false;
            yield return null;
        }
    }
}

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
    public GameObject cam;
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

   // bool isWallCrashed = false;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        CrossHair = GameObject.Find("CrossHair");
        CrossHair.SetActive(false);
        StartCoroutine(DoLogic());
        StartCoroutine(NotPassWall(cam.transform, GameObject.Find("RaycastPos").transform));
    }

    private IEnumerator DoLogic()
    {
        float deltaX = 0;
        float deltaY = 0;
        while (true)
        {
            if(StateManager.isPaused)
            {
                yield return null;
                continue;
            }
            deltaY = Input.GetAxisRaw("Mouse Y") * sensitivityY;
            deltaX = Input.GetAxisRaw("Mouse X") * sensitivityX;
            if (Time.deltaTime != 0)
            {
                //if (!isWallCrashed)
                    SetCameraMode(CamPos,StateManager.keySet.aim && !StateManager.useSmallUI);

                ChangeHeading(deltaX, -deltaY);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    void ChangeHeading(float aVal, float bVal)
    {
        //if (StateManager.InventoryEnabled) return;
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
    

    IEnumerator NotPassWall(Transform cam, Transform RaycastPos)
    {
        float dist = cam.transform.localPosition.z * -1;
        float raySphereradious = 0.5f;
        float maxDistanceBetweenPlayer = 6.5f;
        RaycastHit hit;
        while (true)
        {
            RaycastPos.LookAt(cam);
            dist = cam.transform.localPosition.z * -1;

            if (Physics.SphereCast(RaycastPos.position, raySphereradious, RaycastPos.forward, out hit, dist)&&Vector3.Distance(RaycastPos.position, cam.transform.position)<= maxDistanceBetweenPlayer)
            {
                //isWallCrashed = true;
                cam.transform.position = RaycastPos.position + RaycastPos.forward * hit.distance;
            }
           // else isWallCrashed = false;
            yield return null;
        }
    }
}

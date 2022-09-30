using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour {

    public GameObject CamTarget;
    public GameObject cam;
    private void Awake()
    {
       // StartCoroutine(Follow());
    }
    private void LateUpdate()
    {
        if (StateManager.isStanding)
        {
            cam.transform.rotation = Quaternion.LerpUnclamped(cam.transform.rotation, CamTarget.transform.rotation, 0.2f);
        }
        //cam.transform.position = Vector3.Lerp(cam.transform.position,CamTarget.transform.position,0.5f);
        cam.transform.position = CamTarget.transform.position;
    }
    IEnumerator Follow()
    {
        while(true)
        {
            if (StateManager.isStanding)
            {
                cam.transform.rotation = Quaternion.LerpUnclamped(cam.transform.rotation, CamTarget.transform.rotation, 0.2f);
            }
            //cam.transform.position = Vector3.Lerp(cam.transform.position,CamTarget.transform.position,0.5f);
            cam.transform.position = CamTarget.transform.position;//Vector3.SmoothDamp(cam.transform.position, CamTarget.transform.position, ref currentVelocity, smoothTime * Time.smoothDeltaTime);
            yield return null;
        }
    }
}

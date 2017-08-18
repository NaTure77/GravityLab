using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour {

    public GameObject player;
    public GameObject cam;

    private void Start()
    {
        StartCoroutine(Follow());
    }
    IEnumerator Follow()
    {
        while(true)
        {
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, player.transform.rotation, 0.5f);
            cam.transform.position = Vector3.Lerp(cam.transform.position,player.transform.position,0.5f);
            yield return null;
        }
    }
}

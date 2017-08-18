using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

    public Quaternion[] rotateList;
    public float rotateTime = 10;
    public float delay = 3;
    // Use this for initialization
    public GameObject Map;
	void Start () {
        StartCoroutine(StartRotate(Map));
	}
	
    IEnumerator StartRotate(GameObject map)
    {
        yield return new WaitForSeconds(10);
        int listNum = 0;
        Quaternion fromRotation;
        Quaternion toRotation;
        float fromToRate = 0;
        while(listNum < rotateList.Length)
        {
            fromToRate = 0;
            fromRotation = map.transform.rotation;
            toRotation = map.transform.rotation * rotateList[listNum];
            while(fromToRate < 1)
            {
                map.transform.rotation = Quaternion.Slerp(fromRotation, toRotation, fromToRate);
                fromToRate += Time.deltaTime / rotateTime;
                yield return null;
            }
            map.transform.rotation = toRotation;
            yield return new WaitForSeconds(delay);
        }
    }
}

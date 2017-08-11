using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MakeSystem : MonoBehaviour {

    public Transform MakePlace;
    public void Making(GameObject obj)
    {
        GameObject clone = GameObject.Instantiate(obj,MakePlace.position,MakePlace.rotation);
        clone.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }
}

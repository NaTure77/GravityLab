using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class RotateAroundPlanet : MonoBehaviour {

    public GameObject planet;
    public float speedX = 10;
    public float speedY = 10;
    public Scrollbar H_Slider;
    public Scrollbar V_Slider;
    public Text H_Text;
    public Text V_Text;

    public void StartLogic()
    {
        StartCoroutine("RotateZAxis");
        StartCoroutine("RotateYAxis");
        StartCoroutine("GetValue");
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void StopLogic()
    {
        StopCoroutine("RotateZAxis");
        StopCoroutine("RotateYAxis");
        StopCoroutine("GetValue");
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void resetValue()
    {
        V_Slider.value = 0.5f;
        H_Slider.value = 0.5f;
        speedX = 0;
        speedY = 0;
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void ShowValue()
    {
        H_Text.text = (speedX/ 1).ToString();
        V_Text.text = (speedY / 1).ToString();
    }
    IEnumerator GetValue()
    {
        while(true)
        {
            speedX = (0.5f - V_Slider.value) * 10;
            speedY = (0.5f -  H_Slider.value) * 10;
            ShowValue();
            yield return null;
        }
    }
    IEnumerator RotateZAxis()
    {
        while(true)
        {
            this.transform.RotateAround(planet.transform.position, transform.forward, speedX);
            yield return null;
        }
    }

    IEnumerator RotateYAxis()
    {
        while (true)
        {
            this.transform.RotateAround(planet.transform.position, transform.right, speedY);
            yield return null;
        }
    }
}

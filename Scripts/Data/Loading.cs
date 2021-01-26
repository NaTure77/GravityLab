  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Loading : MonoBehaviour {

    public Slider slider;
    AsyncOperation async_operation;

    void Awake()
    {
        slider = GameObject.Find("Slider").GetComponent<Slider>();
    }
    void Start()
    {
        StartCoroutine(StartLoad(PlayerPrefs.GetString("Next Scene")));
    }
    public IEnumerator StartLoad(string strSceneName)
    {
        async_operation = SceneManager.LoadSceneAsync(strSceneName);
        async_operation.allowSceneActivation = false;
        while (async_operation.progress < 0.9f)
        {
            slider.value = async_operation.progress;

            yield return new WaitForEndOfFrame();
        }
        async_operation.allowSceneActivation = true;
    }
}
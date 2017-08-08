using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//스테이지들의 아버지. 스테이지들이 공통적으로 갖는 속성을 갖는다.
public class StageManager : MonoBehaviour {

    [System.NonSerialized]
    public float time = 0;

    void Awake()
    {
        LoadStage();
        Application.targetFrameRate = 60;
    }

    public void LoadStage()
    {
        LoadTempData();
    }
    public void CheckTime()
    {
        time += Time.deltaTime;
    }
    public static void SaveTempData(string nextSceneName)
    {
        PlayerPrefs.SetFloat("Current HP", PlayerState.HP);
        PlayerPrefs.SetInt("Current Coin", PlayerState.Coin);
        PlayerPrefs.SetString("Next Scene", nextSceneName);//로딩에 사용하기 위한 임시 저장.
    }

    //UIManager에서 메인메뉴 씬 넘어갈때 사용.
    public static void SaveTempData(float HP, int Coin, string nextSceneName)
    {
        PlayerPrefs.SetFloat("Current HP", HP);
        PlayerPrefs.SetInt("Current Coin", Coin);
        PlayerPrefs.SetString("Next Scene", nextSceneName);
    }
    public static void LoadTempData()
    {
        PlayerState.HP = PlayerPrefs.GetFloat("Current HP");
        PlayerState.Coin = PlayerPrefs.GetInt("Current Coin");
    }
    
    //하위 클래스에서 소규모 스테이지가 있을 경우 오버라이딩.
    //게임 내 포탈 등에 사용.
    public void NextStage(string sceneName)
    {
        SaveTempData(sceneName);
        SceneManager.LoadScene("Loading");
    }
    public void RestartStage()
    {
        LoadTempData();
        NextStage(SceneManager.GetActiveScene().name);
    }
    public static void GetDamaged(float x)
    {
        if (x < 0) return;
        PlayerState.HP -= x;
        //StageUI.UpdateHP();
    }
    public static void GetHP(float x)
    {
        if (x < 0) return;
        PlayerState.HP += x;
        //StageUI.UpdateHP();
    }
    public static void GetCoin(int x)
    {
        if (x < 0) return;
        PlayerState.Coin += x;
    }
}

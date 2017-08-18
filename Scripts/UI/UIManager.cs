using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//UI의 최상루트.
public class UIManager : MonoBehaviour {

    public static UIManager instance;
    public  GameObject pauseMenu;
    public  GameObject playUI;
    public  GameObject popUI;
    //public bool iSPaused = false;

    void Awake()
    {
        LoadUI();
        //Screen.SetResolution(1920,1080, true);

    }
    public void Start()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        if (popUI != null)
            popUI.SetActive(false);
        if (playUI != null)
            playUI.SetActive(true);
    }
    public void LoadUI()
    {
        instance = this;
        playUI = GameObject.Find("Play UI");
        pauseMenu = GameObject.Find("Pause Menu");
        popUI = GameObject.Find("Pop UI");
    }

    //메뉴 창에서 사용.
    public void SelectScene(string name)
    {
        Time.timeScale = 1;
        StateManager.isPaused = false;
        StageManager.SaveTempData(100, 0,name);
        SceneManager.LoadScene("Loading");
    }

    public void Pause()
    {
        StateManager.isPaused = true;
        //MousePointCtrl.active = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //Time.timeScale = 0;
        playUI.SetActive(false);
        pauseMenu.SetActive(true);
    }
    public void Resume()
    {
        StateManager.isPaused = false;
        //MousePointCtrl.active = true;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        playUI.SetActive(true);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        StateManager.isPaused = false;
        StageManager.LoadTempData();
        StageManager.SaveTempData(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("Loading");
    }

    public void End()
    {
        Application.Quit();
    }



}

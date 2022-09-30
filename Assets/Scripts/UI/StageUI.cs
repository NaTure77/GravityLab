using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//스테이지에 특화된 UI. Main과는 차별성을 갖는다.
public class StageUI :UIManager {
    //public static Text textTime;
    public float maxStage = 2;
    public static new StageUI instance;
    public GameObject InventoryUI;
    public GameObject ItemDock;


    public GameObject TargetInfo;
    public Text TargetName;

    public GameObject DistanceInfo;
    public Text DistanceText;

    public GameObject StageInfo;
    public GameObject triggerUI;

    void Awake()
    {
        LoadUI();
    }
    public new void Start()
    {
        base.Start();
        if (triggerUI != null)
            triggerUI.SetActive(false);
        if (TargetInfo != null)
            TargetInfo.SetActive(false);
        if (InventoryUI != null)
            InventoryUI.SetActive(false);
        if(StageInfo != null)
            StageInfo.SetActive(false);
        StartCoroutine(GetESC());
        StartCoroutine(GetKey());
    }
    new public void LoadUI()
    {
        base.LoadUI();
        instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameObject.Find("Resume").GetComponent<Button>().onClick.AddListener(Resume);
        GameObject.Find("Restart").GetComponent<Button>().onClick.AddListener(Restart);
        GameObject.Find("End").GetComponent<Button>().onClick.AddListener(End);
        GameObject.Find("Main").GetComponent<Button>().onClick.AddListener(goMainMenu);

        TargetInfo = GameObject.Find("TargetInfo");
        TargetName = TargetInfo.GetComponentInChildren<Text>();

        DistanceInfo = GameObject.Find("DistanceInfo");
        DistanceText = DistanceInfo.GetComponentInChildren<Text>();
        DistanceInfo.SetActive(false);

        StageInfo = GameObject.Find("StageInfo");

        InventoryUI = GameObject.Find("InventoryUI");
        ItemDock = GameObject.Find("ItemDock");

        triggerUI = GameObject.Find("Trigger UI");
    }

    IEnumerator GetESC()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!StateManager.isPaused) Pause();
                else Resume();
            }
            yield return null;
            //yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator GetKey()
    {      
        while(true)
        {
            //if (StateManager.isPaused)
            //{
            //    yield return null;
            //    continue;
            //}
            if(Input.GetKey(KeyCode.LeftControl)&& Input.GetKeyDown(KeyCode.T))
            {
                CommandCtrl.instance.ToggleCmd();
            }
            yield return null;
        }
    }
    public void UpdateTime(int t)
    {
        //textTime.text = "Time" + t.ToString();
    }


    public void goMainMenu()
    {
        SelectScene("Main");
    }
    public void EnableTargetInfo(string name)
    {
        TargetName.text = name;
        TargetInfo.SetActive(true);
    }

    public void ShowDistance(int distance)
    {
        TargetName.text += "\n" + (distance).ToString() + "m";
    }
    public void DisableTargetInfo()
    {
        TargetInfo.SetActive(false);
    }

    public void EnableDistanceInfo()
    {
        DistanceText.text = "착지";
        DistanceInfo.SetActive(true);
    }
    public void DisableDistanceInfo()
    {
        DistanceInfo.SetActive(false);
    }
    public void SetSuccessMessage()
    {
        DistanceText.text = "성공";
    }
    public void SetFailedMessage()
    {
        DistanceText.text = "실패";
    }
    public void ShowSecMessage()
    {
        StartCoroutine(_ShowSecMessage());
    }
    public IEnumerator _ShowSecMessage()
    {
        DistanceInfo.SetActive(true);
        yield return new WaitForSeconds(1);
        DistanceInfo.SetActive(false);
    }

    public IEnumerator ShowStageClear(int stageNum)
    {
        StageInfo.GetComponentInChildren<Text>().text = "Stage " + stageNum + " Clear";
        StageInfo.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        StageInfo.SetActive(false);
        yield return new WaitForSeconds(0.7f);
        if (stageNum < maxStage) StartCoroutine(ShowStageStart(stageNum + 1));// 추후 수정.
        else SelectScene("Main");
    }

    private IEnumerator ShowStageStart(int stageNum)
    {
        StageInfo.GetComponentInChildren<Text>().text = "Stage " + stageNum + " Start";
        StageInfo.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        StageInfo.SetActive(false);
    }

    public IEnumerator ShowTryAgain()
    {
        StageInfo.GetComponentInChildren<Text>().text = "Try Again!";
        StageInfo.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        StageInfo.SetActive(false);
    }
    /*public void ToggleInventory(bool B)
    {
        InventoryUI.SetActive(B);
        StateManager.InventoryEnabled = B;
        playUI.SetActive(!B);

        Cursor.visible = B;
        if (B)
        {
            //StartCoroutine(ItemShortCut.instance.UpdateCurrent());
            Cursor.lockState = CursorLockMode.None;
        }
        else Cursor.lockState = CursorLockMode.Locked;
    }*/

    public void ToggleTriggerUI(bool t)
    {
        triggerUI.SetActive(t);
    }

}

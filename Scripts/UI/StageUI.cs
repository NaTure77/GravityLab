using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//스테이지에 특화된 UI. Main과는 차별성을 갖는다.
public class StageUI :UIManager {
    //public static Text textTime;

    public static new StageUI instance;
    public GameObject InventoryUI;
    public GameObject ItemDock;


    public GameObject TargetInfo;
    public Text TargetName;
    //public bool InventoryEnabled = false;

    public GameObject triggerUI;
    //public static Text textHP;
    //public static Text textCoin;
    /*class keySet
    {
        public static bool Inventory;
        public static bool Pause;
    }*/
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

        InventoryUI = GameObject.Find("InventoryUI");
        ItemDock = GameObject.Find("ItemDock");

        triggerUI = GameObject.Find("Trigger UI");

        //textHP = GameObject.Find("textHP").GetComponent<Text>();
        //textCoin = GameObject.Find("textCoin").GetComponent<Text>();
        //UpdateHP();
        //UpdateCoin();
    }

    IEnumerator GetESC()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (StateManager.InventoryEnabled) ToggleInventory(false);
                else if (!StateManager.isPaused) Pause();
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
            if (StateManager.isPaused)
            {
                yield return null;
                continue;
            }
            if(Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventory(!StateManager.InventoryEnabled);
            }
            yield return null;
        }
    }
    public void UpdateTime(int t)
    {
        //textTime.text = "Time" + t.ToString();
    }
    /* public static void UpdateHP()
     {
         textHP.text = "HP" + PlayerState.HP.ToString();
     }
     public static void UpdateCoin()
     {
         textCoin.text = "Coin" + PlayerState.Coin.ToString();
     }*/

    public void goMainMenu()
    {
        SelectScene("Main");
    }
    public void EnableTargetInfo(string name)
    {
        TargetName.text = name;
        TargetInfo.SetActive(true);
    }
    public void DisableTargetInfo()
    {
        TargetInfo.SetActive(false);
    }

    public void ToggleInventory(bool B)
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
    }

    public void ToggleTriggerUI(bool t)
    {
        triggerUI.SetActive(t);
    }

}

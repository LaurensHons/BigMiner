using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject camera;
    private Vector3 cameraOriginPosition;
    private Vector3 cameraDifference;
    private bool Drag = false;
    private GameObject SubPanel = null;
    public bool EditMode => EditController.EditMode;

    private Vector3 dragOrigin;

    public GameObject BayGameObject;
    private Bay Bay;

    public ScannerController ScannerController;
    public SiloController SiloController;
    public MinerController MinerController;
    public EditController EditController;
    
    public GameObject MenuPanel;
    public GameObject ScannerMenu;
    public GameObject SiloMenu;
    public GameObject MinerMenu;
    public GameObject MinerToolMenu;
    public GameObject MinerUpgradeMenu;
    public GameObject BuildingsMenu;
    
    public Text ScreenRes;

    private GameObject activeMenu;
    private MinerStation activeMinerStation;

    private Action CloseButton;


    void Start()
    {
        int width = Display.main.systemWidth;
        int height = Display.main.systemHeight;

        ScreenRes.text = "Width: " + width + ", Height: " + height;

        float factor = height / (float) width;
        float normalfactor = 16 / (float) 9;
        Debug.Log("Normal " + normalfactor + ", factor " + factor);
        camera.GetComponent<Camera>().orthographicSize = factor / normalfactor * 5;
        Bay = BayGameObject.GetComponent<Bay>();

        MenuPanel.SetActive(false);
        ScannerMenu.SetActive(false);
        SiloMenu.SetActive(false);
        MinerMenu.SetActive(false);
        MinerToolMenu.SetActive(false);
        MinerUpgradeMenu.SetActive(false);


        Vector3 cameraPos = new Vector3((Bay.gridSize.x - 1) / 2f, (Bay.gridSize.y - 1) / 2f, -10);
        camera.transform.position = cameraPos;
    }

    public void moveCamera()
    {
        if (activeMenu != null || EditMode) return;
        if (Input.GetMouseButton(0))
        {
            cameraDifference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (Drag == false)
            {
                Drag = true;
                cameraOriginPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            Drag = false;
        }

        if (Drag == true)
        {
            Camera.main.transform.position =
                new Vector3((Bay.gridSize.x - 1) / 2f, cameraOriginPosition.y - cameraDifference.y, -1);
        }
    }

    private void setActivePanel(GameObject panel)
    {
        if (activeMenu != null)
        {
            activeMenu.SetActive(false);
        }
        else
            setActiveMenuPanel();

        activeMenu = panel;
        activeMenu.SetActive(true);
    }

    public void OpenScannerMenu()   //Activated by scanner button
    {
        setActivePanel(ScannerMenu);
    }

    public void OpenSiloMenu()
    {
        setActivePanel(SiloMenu);
        SiloController.setActive(true);
    }

    public void OpenMinerMenu(MinerStation minerStation, bool forced = false)   //Activated by tapping on miner station
    {
        if (activeMenu != null && !forced) return;
        MinerController.setMinerStation(minerStation);
        MinerController.setActive(true);
        setActivePanel(MinerMenu);
    }

    public void OpenToolMenu() //Activated by Toolbutton
    {
        MinerController.loadTools();
        setActivePanel(MinerToolMenu);
        SubPanel = MinerMenu;
    }

    public void OpenUpgradeMenu() //Activated by UpgradesButton
    {
        setActivePanel(MinerUpgradeMenu);
        MinerController.loadUpgrades();
        SubPanel = MinerMenu;
    }

    public void OpenBuildingsMenu()
    {
        setActivePanel(BuildingsMenu);
    }

    public void EditButton()   //Activated by Edit Button
    {
        EditController.EditButtonClick();
    }
    public void CommitButton()   //Activated by Commit Button
    {
        EditController.CommitButtonClick();
    }

    public void TrashButton()   //Activated by trash Button
    {
        Bay.clearBay();
    }
    

    private void setActiveMenuPanel()
    {
        MenuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        if (activeMenu == MinerMenu)
            MinerController.setActive(false);
        
        if (activeMenu == SiloMenu)
            SiloController.setActive(false);

        if (SubPanel != null)
        {
            setActivePanel(SubPanel);
            SubPanel = null;
        }
        else
        {
            activeMenu.SetActive(false);
            activeMenu = null;
            MenuPanel.SetActive(false);   
        }
    }

    private void Update()
    {
        moveCamera();
    }
    
    public void DragBattery()
    {
        MinerController.DragBattery();
    }


    public Bay getBay()
    {
        return Bay;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject camera;
    private Vector3 cameraOriginPosition;
    private Vector3 cameraDifference;
    private bool Drag = false;

    private Vector3 dragOrigin;
    
    public GameObject BayGameObject;
    private Bay Bay;

    public ScannerController ScannerController;
    public MinerController MinerController;
    
    public GameObject UICanvas;
    public GameObject MenuPanel;
    public GameObject ScannerMenu;
    public GameObject MinerMenu;
    public GameObject ToolMenu;

    private GameObject activeMenu;
    private MinerStation activeMinerStation;
    

    void Start()
    {
        Bay = BayGameObject.GetComponent<Bay>();
        
        MenuPanel.SetActive(false);
        ScannerMenu.SetActive(false);
        MinerMenu.SetActive(false);
        ToolMenu.SetActive(false);


        Vector3 cameraPos = new Vector3(Bay.gridSize/2 , 0, -10);
        camera.transform.position = cameraPos;
        Vector3 middleOfTheGrid = new Vector3(Bay.gridSize/2 , 0, 1);
        UICanvas.transform.position = middleOfTheGrid;
    }
    
    public void moveCamera()
    {
        if (activeMenu != null) return;
        if (Input.GetMouseButton (0)) {
            cameraDifference = (Camera.main.ScreenToWorldPoint (Input.mousePosition))- Camera.main.transform.position;
            if (Drag == false){
                Drag = true;
                cameraOriginPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
            }
        } else {
            Drag = false;
        }
        if (Drag == true){
            Camera.main.transform.position = new Vector3(Bay.gridSize/2, cameraOriginPosition.y - cameraDifference.y, -1);
        }
    }

    private void setActivePanel(GameObject panel)
    {
        if (activeMenu != null)
        {
            activeMenu.SetActive(false);
        } else
            setActiveMenuPanel();
        
        activeMenu = panel;
        activeMenu.SetActive(true);
    }

    public void OpenScannerMenu()
    {
        setActivePanel(ScannerMenu);
    }

    public void OpenMinerMenu(MinerStation minerStation, bool forced = false)
    {
        if (activeMenu != null && !forced) return;
        MinerController.setMinerStation(minerStation);
        MinerController.setActive(true);
        setActivePanel(MinerMenu);
    }

    public void OpenToolMenu()
    {
        MinerController.loadTools();
        setActivePanel(ToolMenu);
    }

    private void setActiveMenuPanel()
    {
        Vector2 pos = camera.transform.position;
        MenuPanel.transform.position = pos;
        MenuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        if (activeMenu == MinerMenu)
            MinerController.setActive(false);

        if (activeMenu == ToolMenu)
        {
            setActivePanel(MinerMenu);
        }
        
        activeMenu.SetActive(false);
        activeMenu = null;
        MenuPanel.SetActive(false);
        
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
    
    public GameObject getCanvas()
    {
        return UICanvas;
    }

    
}

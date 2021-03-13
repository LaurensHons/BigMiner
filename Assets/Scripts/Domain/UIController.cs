using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject camera;
    private Vector3 cameraOriginPosition;
    private Vector3 cameraDifference;
    private bool Drag = false;

    public bool hasMenuOpen = false;
    
    private Vector3 dragOrigin;
    
    public GameObject BayGameObject;
    private Bay Bay;

    public int amountOfBays = 1;

    public GameObject MinerStationPrefab;

    public GameObject UICanvas;

    private List<GameObject> MinerStations = new List<GameObject>();
    
    void Start()
    {
        Bay = BayGameObject.GetComponent<Bay>();

        Vector3 cameraPos = new Vector3(Bay.gridSize/2 , 0, -10);
        camera.transform.position = cameraPos;
        Vector3 middleOfTheGrid = new Vector3(Bay.gridSize/2 , 0, 1);
        UICanvas.transform.position = middleOfTheGrid;
        
        InstantiateMinerStations();
    }

    private void InstantiateMinerStations()
    {
        for (int i = 0; i < amountOfBays; i++)
        {
            Vector2 pos = new Vector2(i * 2f + .5f, -1f);
            MinerStation minerStation =
                Instantiate(MinerStationPrefab, pos, Quaternion.identity).GetComponent<MinerStation>();
            minerStation.setBay(Bay);
            MinerStations.Add(minerStation.gameObject);
        }
    }

    public void moveCamera()
    {
        if (hasMenuOpen) return;
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

    private void Update()
    {
        moveCamera();
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

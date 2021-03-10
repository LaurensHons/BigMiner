using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject BayGameObject;
    private Bay Bay;

    public int amountOfBays = 1;

    public GameObject MinerStationPrefab;

    private List<GameObject> MinerStations = new List<GameObject>();
    public GameObject ScannerButton;


    void Start()
    {
        Bay = BayGameObject.GetComponent<Bay>();

        InstantiateMinerStations();

        Vector2 pos = new Vector2(Bay.gridSize / 2, Bay.gridSize);
        ScannerButton.transform.position = pos;
        ScannerButton.transform.localScale = new Vector3(Bay.blockScale * Bay.gridSize, Bay.blockScale, Bay.blockScale);
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
    
    
    void Update()
    {
        
    }
}

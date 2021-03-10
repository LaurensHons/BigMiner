using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Camera;
    public GameObject BayGameObject;
    private Bay bay;
    void Start()
    {
        bay = BayGameObject.GetComponent<Bay>();
        Camera.transform.position = new Vector3(bay.GridSize/2 , 0, -1);   
    }

    
    void Update()
    {
        
    }
}

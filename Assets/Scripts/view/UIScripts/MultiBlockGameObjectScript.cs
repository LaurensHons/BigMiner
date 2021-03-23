using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiBlockGameObjectScript : MonoBehaviour
{
    private MultiBlock structure;

    private MinerStation minerStation;
    private UIController uiController;
    private Image objectImage;
    
    public bool isTemporaryStructure = false;
    public bool canPlace = true;
    
    
    void Start()
    {
        uiController = uiController = GameObject.FindWithTag("UIController").GetComponent<UIController>();
        objectImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (!isTemporaryStructure || objectImage == null)
            return;
        else
        {
            if (canPlace)
                objectImage.color = Color.green;
            else 
                objectImage.color = Color.red;
        }
    }

    public void setStructure(MultiBlock structure)
    {
        this.structure = structure;
    }
    
    public void OnMouseDrag()
    {
        if (uiController.EditMode)
        {
            var world_point_mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,Mathf.Abs( Camera.main.transform.position.z)));
                
            Vector3 pos = new Vector3((float) Math.Round(world_point_mousePos.x - structure.getDimensions().x/4) + (structure.getDimensions().x - 1) / 2, (float) Math.Round(world_point_mousePos.y - structure.getDimensions().y/4) + (structure.getDimensions().y - 1) / 2);
            structure.moveTemporaryStructure(pos);
        }
        else structure.onClick();
    }




}
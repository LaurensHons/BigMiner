
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveBuildingPanelScript : TabGroup
{
    public Processor Processor { get; private set; }
    public List<GameObject> inputItemGameObjects = new List<GameObject>();
    public List<GameObject> outputItemGameObjects = new List<GameObject>();
    
    public Text nameText;
    public Image BuildingImage;

    public void setActive(bool active, Processor processor)
    {
        if (active)
        {
            Processor = processor;
            nameText.text = processor.getName();
            BuildingImage.GetComponent<Image>().sprite = processor.BlockSprite;
            setItemRecipe();
            return;
        }


    }

    private void setItemRecipe()
    {
        Item[] inputItems = Processor.getActualInputItems();
        for (int i = 0; i < inputItemGameObjects.Count; i++)
        {
            if (i < inputItems.Length)
            {
                inputItemGameObjects[i].SetActive(true);

                inputItemGameObjects[i].GetComponentInChildren<Text>().text = inputItems[i].getAmount().ToString();
                inputItemGameObjects[i].GetComponentInChildren<Image>().sprite = inputItems[i].GetSprite();

            }
            else
            {
                inputItemGameObjects[i].SetActive(false);
            }
        }
        
        Item[] outputItem = Processor.getOutputItems();
        for (int i = 0; i < outputItemGameObjects.Count; i++)
        {
            if (1 < outputItem.Length)
            {
                outputItemGameObjects[i].SetActive(true);

                outputItemGameObjects[i].GetComponentInChildren<Text>().text = outputItem[i].getAmount().ToString();
                outputItemGameObjects[i].GetComponentInChildren<Image>().sprite = outputItem[i].GetSprite();
                
            }
            else
            {
                outputItemGameObjects[i].SetActive(false);
            }
        }
    }
}

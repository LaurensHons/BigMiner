
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingPanelScript : TabGroup
{
    public Text nameText;
    public Processor Processor { get; private set; }
    
    public List<GameObject> inputItemGameObjects;
    public List<GameObject> outputItemGameObjects;

    public void setActive(bool active, Processor processor)
    {
        if (active)
        {
            Processor = processor;
            nameText.text = processor.getName();
            //BuildingImage.GetComponent<Image>().sprite = processor.BlockSprite;
            setItemRecipes();
            return;
        }
    }

    private void setItemRecipes()
    {
        Item[] inputItems = Processor.getActualInputItems();
        
        for (int i = 0; i < inputItemGameObjects.Count; i++)
        {
            if (i < inputItems.Length)
            {
                GameObject inputGameObject = inputItemGameObjects[i];
                inputGameObject.SetActive(true);

                for (int j = 0; j < inputGameObject.transform.childCount; j++)
                {
                    GameObject itemComponent = inputGameObject.transform.GetChild(j).gameObject;

                    switch (itemComponent.name)
                    {
                        case "Image":
                        {
                            itemComponent.GetComponent<Image>().sprite = inputItems[i].GetSprite();
                            continue;
                        }
                        case "RecipeCounter":
                        {
                            itemComponent.GetComponent<Text>().text = inputItems[i].getAmount().ToString();
                            continue;
                        }
                        case "InInventoryCounter":
                        {
                            Item inventoryItem = Processor.getInputInventory().TryGetItem(inputItems[i]);
                            String outString = inventoryItem == null ? "0" : inventoryItem.getAmount().ToString();
                            int amountUnderway = getItemCountUnderway(inputItems[i]);
                            if (amountUnderway > 0) outString += " (" + amountUnderway + ")";
                            
                            itemComponent.GetComponent<Text>().text = outString;
                            continue;
                        }
                        case "InStorageCounter":
                        {
                            Item siloInventoryItem = Silo.Instance.Inventory.TryGetItem(inputItems[i]);
                            String outString = siloInventoryItem == null ? "0" : siloInventoryItem.getAmount().ToString();
                            itemComponent.GetComponent<Text>().text = outString;
                            continue;
                        }
                    }
                    
                }
            }
            else
            {
                inputItemGameObjects[i].SetActive(false);
            }
        }
        
        Item[] outputItems = Processor.getOutputItems();
        for (int i = 0; i < outputItemGameObjects.Count; i++)
        {
            if (i < outputItems.Length)
            {
                GameObject outputGameObject = outputItemGameObjects[i];
                outputGameObject.SetActive(true);

                for (int j = 0; j < outputGameObject.transform.childCount; j++)
                {
                    GameObject itemComponent = outputGameObject.transform.GetChild(j).gameObject;

                    switch (itemComponent.name)
                    {
                        
                        case "OutputInventoryCounter":
                        {
                            Item inventoryItem = Processor.getInputInventory().TryGetItem(outputItems[i]);
                            String outString = inventoryItem == null ? "0" : inventoryItem.getAmount().ToString();
                            itemComponent.GetComponent<Text>().text = outString;
                            continue;
                        }
                        
                    }
                }
            }
            else
            {
                outputItemGameObjects[i].SetActive(false);
            }
        }
    }

    public void addItemOne()
    {
        Processor.addInventoryCall(Processor.getActualInputItems()[0], 1);
        setItemRecipes();
    }

    private int getItemCountUnderway(Item item)
    {
        List<JobCall> jobCalls = JobController.Instance.getItemsUnderway(Processor);
        foreach (var jobCall in jobCalls)
        {
            if (jobCall.itemToBeDelivered.ToString().Equals(item.ToString()))
                return item.getAmount();
        }

        return 0;
    }
    
}


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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
            this.Processor = processor;
            nameText.text = processor.getName();
            AsyncOperationHandle<Sprite> spriteHandle = Addressables.LoadAssetAsync<Sprite>(processor.getSpritePath());
            spriteHandle.Completed += obj =>
            {
                BuildingImage.GetComponent<Image>().sprite = obj.Result;
            };

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
                AsyncOperationHandle<Sprite> inputItemImageHandler = Addressables.LoadAssetAsync<Sprite>(inputItems[i].getSpritePath());
                var i1 = i;
                inputItemImageHandler.Completed += obj =>
                {
                    inputItemGameObjects[i1].GetComponentInChildren<Image>().sprite = obj.Result;
                };
                
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
                AsyncOperationHandle<Sprite> outputItemImageHandler = Addressables.LoadAssetAsync<Sprite>(outputItem[i].getSpritePath());
                var i1 = i;
                outputItemImageHandler.Completed += obj =>
                {
                    outputItemGameObjects[i1].GetComponentInChildren<Image>().sprite = obj.Result;
                };
                
            }
            else
            {
                outputItemGameObjects[i].SetActive(false);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.PlayerLoop;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class SiloItemPrefabScript : MonoBehaviour
{
    public Image Image;
    public Text NameText;
    public Text AmountText;
    private Item item;
    
    public void setItem(Item item)
    {
        if (this.item != null)
            this.item.ItemUpdate -= updateAmount;
        this.item = item;
        item.ItemUpdate += updateAmount;
        AsyncOperationHandle<Sprite> spriteHandle = Addressables.LoadAssetAsync<Sprite>(item.getSpritePath());
        spriteHandle.Completed += obj =>
        {
            Image.GetComponent<Image>().sprite = obj.Result;
        };

        NameText.text = item.getName();
        updateAmount();
    }
    private void updateAmount()
    {
        AmountText.text = item.getAmount().ToString();
    }

    public void OnDestroy()
    {
        item.ItemUpdate -= updateAmount;
    }
}

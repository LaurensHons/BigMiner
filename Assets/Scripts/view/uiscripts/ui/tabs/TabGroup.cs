
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public String tabActiveAddressableString;
    private Sprite tabActive;
    public String tabClosedAddressableString;
    private Sprite tabClosed;
    private TabButton activeButton;
    public List<GameObject> menuPanels;

    private void Start()
    {
        AsyncOperationHandle<Sprite> tabActiveSpriteHandler = Addressables.LoadAssetAsync<Sprite>(tabActiveAddressableString);
        tabActiveSpriteHandler.Completed += obj =>
        {
            tabActive = obj.Result;
        };
        AsyncOperationHandle<Sprite> tabClosedSpriteHandler = Addressables.LoadAssetAsync<Sprite>(tabClosedAddressableString);
        tabClosedSpriteHandler.Completed += obj =>
        {
            tabClosed = obj.Result;
        };
    }

    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
            button.backGround.sprite = tabActive;
        }
        tabButtons.Add(button);
        
    }

    public void onTabSelected(TabButton button)
    {
        if (activeButton != null)
            activeButton.backGround.sprite = tabClosed!;
        activeButton = button;
        button.backGround.sprite = tabActive!;
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < menuPanels.Count; i++)
        {
            if (i == index)
            {
                menuPanels[i].SetActive(true);
            }
            else
            {
                menuPanels[i].SetActive(false);
            }
        }
    }
}

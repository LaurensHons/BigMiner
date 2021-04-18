
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public Sprite tabActive;
    public Sprite tabClosed;
    private TabButton activeButton;
    public List<GameObject> menuPanels;
    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
            tabButtons = new List<TabButton>();
        
        tabButtons.Add(button);
        tabButtons[0].backGround.sprite = tabActive;
        activeButton = tabButtons[0];
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

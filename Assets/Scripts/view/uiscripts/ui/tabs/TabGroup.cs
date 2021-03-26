
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public Sprite tabActive;
    public TabButton selectedTab;
    public List<GameObject> menuPanels;
    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
            tabButtons = new List<TabButton>();
        tabButtons.Add(button);
    }

    public void onTabSelected(TabButton button)
    {
        selectedTab = button;
        ResetTabs();
        button.backGround.color = Color.grey;
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

    public void ResetTabs()
    {
        foreach (var tabButton in tabButtons)
        {
            tabButton.backGround.color = Color.white;
        }
    }
}

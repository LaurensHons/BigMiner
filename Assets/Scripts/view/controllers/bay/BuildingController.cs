
using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildingController : MonoBehaviour, IMenuController
{
    public GameObject BuildingMenu;
    public Text nameText;
    public Image BuildingImage;
    public GameObject GeneralInfoTab;

    private void Start()
    {
        BuildingMenu.SetActive(false);
    }

    public void setActive(bool active)
    {
        BuildingMenu.SetActive(active);
    }
}

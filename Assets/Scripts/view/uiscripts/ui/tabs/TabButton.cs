
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabButton : MonoBehaviour, IPointerClickHandler
{
    public TabGroup TabGroup;
    public Image backGround;


    private void Start()
    {
        backGround = GetComponent<Image>();
        TabGroup.Subscribe(this);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        TabGroup.onTabSelected(this);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextPrefab : MonoBehaviour
{
    public GameObject AmountObject;
    public GameObject ImageObject;
    private List<Item> items;
    private void Start() { Silo.Instance.Inventory.InventoryChanged += update; }

    public void Load(List<Item> itemcost)
    {
        items = new List<Item>(itemcost);
        for (int i = 0; i < 3; i++)
        {
            if (i >= items.Count)
            {
                AmountObject.transform.GetChild(i).gameObject.SetActive(false);
                ImageObject.transform.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                setText(i);
                setImage(i);
            }
        }
    }

    private void update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i >= items.Count)
            {
                AmountObject.transform.GetChild(i).gameObject.SetActive(false);
                ImageObject.transform.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
               setText(i);
            }
        }
    }

    private void setText(int i)
    {
        GameObject amountgo = AmountObject.transform.GetChild(i).gameObject;
        amountgo.SetActive(true);
        Text amountText = amountgo.GetComponent<Text>();
        amountText.text = items[i].getAmount().ToString();
        Item itemInInv = Silo.Instance.Inventory.TryGetItem(items[i]); 
        if (itemInInv == null || itemInInv.getAmount() < items[i].getAmount()) amountText.color = Color.red;
        else amountText.color = Color.green;
    }

    private void setImage(int i)
    {
        GameObject imagego = ImageObject.transform.GetChild(i).gameObject;
        imagego.SetActive(true);
        imagego.GetComponent<Image>().sprite = items[i].GetSprite();
    }
}

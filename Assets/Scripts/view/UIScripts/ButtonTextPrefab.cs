using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextPrefab : MonoBehaviour
{
    public GameObject AmountObject;
    public GameObject ImageObject;
    public void Load(List<Item> items)
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
                GameObject amountgo = AmountObject.transform.GetChild(i).gameObject;
                amountgo.SetActive(true);
                amountgo.GetComponent<Text>().text = items[i].getAmount().ToString();
                
                GameObject imagego = ImageObject.transform.GetChild(i).gameObject;
                imagego.SetActive(true);
                imagego.GetComponent<Image>().sprite = items[i].getSprite();
            }
        }
    }
}

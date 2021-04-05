
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditController : MonoBehaviour
{
    public Button EditButton;
    public Button CommitButton;

    public bool EditMode = false;

    private static List<MultiBlock> temporaryPlacements = new List<MultiBlock>();

    public void EditButtonClick()
    {
        if (!EditMode)
        {
            EditMode = true;
            EditButton.GetComponentInChildren<Text>().text = "X";
            CommitButton.gameObject.SetActive(true);
        }
        else
        {
            CommitButton.gameObject.SetActive(false);
            EditMode = false;
            EditButton.GetComponentInChildren<Text>().text = "Edit";
            foreach (var multiBlock in temporaryPlacements)
            {
                multiBlock.cancelTemporaryPlacement();
            }
        }
    }

    private void Update()
    {
        if (EditMode)
        {
            foreach (var multiBlock in temporaryPlacements)
            {
                if (!multiBlock.CanPlaceTemporary)
                {
                    CommitButton.gameObject.SetActive(false);
                    break;
                }
            }
            CommitButton.gameObject.SetActive(true);
        }
    }

    public static void addTemporaryPlacement(MultiBlock structure)
    {
        temporaryPlacements.Add(structure);
    }
    
    public void CommitButtonClick()
    {
        if (EditMode)
        {
            foreach (var multiBlock in temporaryPlacements)
            {
                if (!multiBlock.CanPlaceTemporary) return;
            }

            Bay bay = GameObject.FindWithTag("Bay").GetComponent<Bay>();
            foreach (var multiBlock in temporaryPlacements)
            {
                multiBlock.commitTemporaryStructure();
                bay.updateStructure(multiBlock);
            }
            CommitButton.gameObject.SetActive(false);
            EditMode = false;
            EditButton.GetComponentInChildren<Text>().text = "Edit";
        }
        else Debug.LogError("Committing while not in edit mode");
    }
}

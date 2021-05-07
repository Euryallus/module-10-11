using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChestPanel : UIPanel
{
    public TextMeshProUGUI          chestNameText;
    public List<ContainerSlotUI>    slotsUI;

    protected override void Start()
    {
        base.Start();

        //Hide the UI panel by default
        Hide();
    }

    public void Show(bool linkedChest)
    {
        if (linkedChest)
        {
            chestNameText.text = "Linked Chest";
        }
        else
        {
            chestNameText.text = "Chest";
        }

        base.Show();
    }

    public override void Show()
    {
        Debug.LogError("Should not use default Show function for ChestPanel - use overload that takes a linkedChest bool instead");
    }

    private void Update()
    {
        CheckForShowHideInput();
    }

    private void CheckForShowHideInput()
    {
        //Block keyboard input if an input field is selected
        if (!InputFieldSelection.AnyFieldSelected)
        {
            if (showing && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.I)))
            {
                Hide();
            }
        }
    }
}

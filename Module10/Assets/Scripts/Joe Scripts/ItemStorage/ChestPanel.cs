using System.Collections.Generic;
using UnityEngine;

public class ChestPanel : UIPanel
{
    public List<ContainerSlotUI> slotsUI;

    protected override void Start()
    {
        base.Start();

        //Hide the UI panel by default
        Hide();
    }

    private void Update()
    {
        CheckForShowHideInput();
    }

    private void CheckForShowHideInput()
    {
        //Block keyboard input if an input field is selected
        if (!CustomInputField.AnyFieldSelected)
        {
            if (showing && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.I)))
            {
                Hide();
            }
        }
    }
}

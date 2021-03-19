using UnityEngine;

[RequireComponent(typeof(Outline))]
public class InteractableWithOutline : InteractableObject
{
    private Outline outline;

    protected override void Start()
    {
        base.Start();

        outline = GetComponent<Outline>();

        outline.enabled = false;
    }

    public override void Interact()
    {
    }

    public override void StartHover()
    {
        outline.enabled = true;
    }

    public override void EndHover()
    {
        outline.enabled = false;
    }
}

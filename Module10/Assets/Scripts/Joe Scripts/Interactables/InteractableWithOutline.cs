using UnityEngine;

[RequireComponent(typeof(Outline))]
public class InteractableWithOutline : InteractableObject
{
    private Outline outline;

    [SerializeField] protected string interactionSound = "buttonClickMain1";

    protected override void Start()
    {
        base.Start();

        outline = GetComponent<Outline>();

        outline.enabled = false;
    }

    public override void Interact()
    {
        base.Interact();

        if (!string.IsNullOrEmpty(interactionSound))
        {
            AudioManager.Instance.PlaySoundEffect2D(interactionSound);
        }
    }

    public override void StartHover()
    {
        base.StartHover();

        outline.enabled = true;
    }

    public override void EndHover()
    {
        base.EndHover();

        outline.enabled = false;
    }
}

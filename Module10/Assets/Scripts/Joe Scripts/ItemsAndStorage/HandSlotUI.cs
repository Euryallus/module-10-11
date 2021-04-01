using UnityEngine.EventSystems;

public class HandSlotUI : ContainerSlotUI
{
    private void Awake()
    {
        LinkToContainerSlot(new ContainerSlot(0, null));
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        //No pointer down behaviour for hand slots
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonPressInputType
{
    DepressOnHover,
    DepressOnClick
}

[RequireComponent(typeof(Image))]
public class PressEffectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Note: Pivots must be set to (0.5, 0.5)")]

    [SerializeField] private Button                 button;
    [SerializeField] private GameObject             buttonTopGameObject;
    [SerializeField] private float                  pressSpeed = 120f;
    [SerializeField] private ButtonPressInputType   inputType = ButtonPressInputType.DepressOnClick;
    [SerializeField] private Color                  buttonColour = Color.grey;
    [SerializeField] private Color                  buttonShadowTint = new Color(0.8f, 0.8f, 0.8f);

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool    interactable;

    private void Start()
    {
        startPos = buttonTopGameObject.transform.localPosition;
        targetPos = startPos;
    }

    private void Update()
    {
        buttonTopGameObject.transform.localPosition = Vector3.MoveTowards(buttonTopGameObject.transform.localPosition, targetPos, Time.unscaledDeltaTime * pressSpeed);
    }

    private void OnValidate()
    {
        #if (UNITY_EDITOR)
            SetButtonColour(buttonColour);
        #endif
    }

    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;
        this.interactable = interactable;
    }

    public void SetButtonColour(Color colour)
    {
        buttonColour = colour;

        GetComponent<Image>().color = buttonColour * buttonShadowTint;
        if (buttonTopGameObject != null)
        {
            buttonTopGameObject.GetComponent<Image>().color = buttonColour;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(inputType == ButtonPressInputType.DepressOnHover && interactable)
        {
            Press();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inputType == ButtonPressInputType.DepressOnHover)
        {
            Depress();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(inputType == ButtonPressInputType.DepressOnClick && interactable)
        {
            Press();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (inputType == ButtonPressInputType.DepressOnClick)
        {
            Depress();
        }
    }

    private void Press()
    {
        targetPos = Vector3.zero;
    }

    private void Depress()
    {
        targetPos = startPos;
    }
}

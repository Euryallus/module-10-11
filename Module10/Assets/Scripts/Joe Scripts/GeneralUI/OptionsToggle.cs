using System;
using UnityEngine;

public class OptionsToggle : MonoBehaviour
{
    [SerializeField] private GameObject toggleIcon;

    public bool Selected { get { return selected; } }

    public event Action<bool> ToggleEvent;

    private bool selected;

    private void Awake()
    {
        SetSelected(false);
    }

    public void Toggle()
    {
        SetSelected(!selected);

        ToggleEvent.Invoke(selected);
    }

    public void SetSelected(bool selected)
    {
        this.selected = selected;

        if (selected)
        {
            toggleIcon.SetActive(true);
        }
        else
        {
            toggleIcon.SetActive(false);
        }
    }
}

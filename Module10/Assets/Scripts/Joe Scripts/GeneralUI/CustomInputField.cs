using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class CustomInputField : MonoBehaviour
{
    public static bool AnyFieldSelected;

    private TMP_InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();

        inputField.onSelect     .AddListener(Select);
        inputField.onDeselect   .AddListener(Deselect);
        inputField.onEndEdit    .AddListener(Deselect);
    }

    public void Select(string s)
    {
        AnyFieldSelected = true;
    }

    public void Deselect(string s)
    {
        AnyFieldSelected = false;
    }

}

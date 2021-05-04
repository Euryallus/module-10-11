using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalTrigger : MonoBehaviour
{
    [SerializeField]
    private string triggerId;

    private List<IExternalTriggerListener> listeners = new List<IExternalTriggerListener>();

    public void AddListener(IExternalTriggerListener listener)
    {
        listeners.Add(listener);
    }

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].OnExternalTriggerEnter(triggerId, other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].OnExternalTriggerStay(triggerId, other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].OnExternalTriggerExit(triggerId, other);
        }
    }
}

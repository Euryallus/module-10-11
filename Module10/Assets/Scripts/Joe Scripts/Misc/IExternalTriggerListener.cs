using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExternalTriggerListener
{
    public void OnExternalTriggerEnter(string triggerId, Collider other);
    public void OnExternalTriggerStay(string triggerId, Collider other);
    public void OnExternalTriggerExit(string triggerId, Collider other);
}

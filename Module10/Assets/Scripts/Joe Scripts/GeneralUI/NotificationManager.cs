using UnityEngine;
using TMPro;
using System.Collections.Generic;

public enum NotificationTextType
{
    PlayerTooFull,

    SaveSuccess,
    SaveError
}

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private GameObject prefabNotificationPanel;
    [SerializeField] private Transform  notificationParentTransform;

    public static NotificationManager Instance;

    private readonly Dictionary<NotificationTextType, string> notificationTextDict = new Dictionary<NotificationTextType, string>()
    {
        { NotificationTextType.PlayerTooFull,   "You're too full to eat that!" },
        { NotificationTextType.SaveSuccess,     "Progress saved successfully!" },
        { NotificationTextType.SaveError,       "Error: Progress could not be saved." }
    };

    private void Awake()
    {
        //Ensure that an instance of the class does not already exist
        if (Instance == null)
        {
            //Set this class as the instance and ensure that it stays when changing scenes
            Instance = this;
        }
        //If there is an existing instance that is not this, destroy the GameObject this script is connected to
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void ShowNotification(NotificationTextType textType)
    {
        GameObject notification = Instantiate(prefabNotificationPanel, notificationParentTransform);

        if (notificationTextDict.ContainsKey(textType))
        {
            notification.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = notificationTextDict[textType];
        }
        else
        {
            Debug.LogError("Notification text type not added to dictionary: " + textType);
        }
    }
}

using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public enum NotificationMessageType
{
    PlayerTooFull,

    ItemRequiredForDoor,
    DoorUnlocked,
    CantOpenDoorManually,

    ItemCannotBePlaced,

    CantAffordItem,

    SaveSuccess,
    AutoSaveSuccess,
    SaveError
}

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private GameObject prefabNotificationPanel;

    public static NotificationManager Instance;

    private Transform notificationParentTransform;

    private GameObject                  activeNotificationGameObj;
    private QueuedNotification          activeNotification;
    private Queue<QueuedNotification>   queuedNotifications = new Queue<QueuedNotification>();

    private readonly Dictionary<NotificationMessageType, string> notificationTextDict = new Dictionary<NotificationMessageType, string>()
    {
        { NotificationMessageType.PlayerTooFull,       "You're too full to eat that!" },

        { NotificationMessageType.ItemRequiredForDoor, "* is required to unlock this door." },
        { NotificationMessageType.DoorUnlocked,        "The door was unlocked with *" },
        { NotificationMessageType.CantOpenDoorManually,"This door cannot be opened or closed manually." },

        { NotificationMessageType.ItemCannotBePlaced,  "The held item cannot be placed there." },

        { NotificationMessageType.CantAffordItem,      "You cannot purchase this item - * * required." },

        { NotificationMessageType.SaveSuccess,         "Progress saved successfully.\nSpawn point set." },
        { NotificationMessageType.AutoSaveSuccess,     "Your progress has been auto-saved.\nSpawn point set." },
        { NotificationMessageType.SaveError,           "Error: Progress could not be saved." }
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

    private void Start()
    {
        notificationParentTransform = GameObject.FindGameObjectWithTag("JoeCanvas").transform.Find("NotificationsParent");
    }

    private void Update()
    {
        if(activeNotificationGameObj == null)
        {
            activeNotification = null;

            if(queuedNotifications.Count > 0)
            {
                ShowNotification(queuedNotifications.Dequeue());
            }
        }
    }

    public void AddNotificationToQueue(NotificationMessageType messageType, string[] parameters = null, string soundName = "notification1")
    {
        QueuedNotification notificationToAdd = new QueuedNotification()
        {
            MessageType = messageType,
            Parameters = parameters,
            SoundName = soundName
        };

        if(activeNotification != null && NotificationsAreTheSame(notificationToAdd, activeNotification))
        {
            //The notification being shown matches the one being added, no need to add it
            return;
        }

        for (int i = 0; i < queuedNotifications.Count; i++)
        {
            if(NotificationsAreTheSame(notificationToAdd, queuedNotifications.ElementAt(i)))
            {
                //A matching notification is already in the queue, no need to add this one
                return;
            }
        }

        queuedNotifications.Enqueue(notificationToAdd);
    }

    private void ShowNotification(QueuedNotification notification)
    {
        if (notificationTextDict.ContainsKey(notification.MessageType))
        {
            GameObject notificationGameObj = Instantiate(prefabNotificationPanel, notificationParentTransform);

            string textToShow = notificationTextDict[notification.MessageType];

            if (notification.Parameters != null)
            {
                for (int i = 0; i < notification.Parameters.Length; i++)
                {
                    int replaceIndex = textToShow.IndexOf("*");

                    textToShow = textToShow.Remove(replaceIndex, 1);
                    textToShow = textToShow.Insert(replaceIndex, notification.Parameters[i]);
                }
            }

            notificationGameObj.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = textToShow;

            if (!string.IsNullOrEmpty(notification.SoundName))
            {
                AudioManager.Instance.PlaySoundEffect2D(notification.SoundName);
            }

            activeNotificationGameObj   = notificationGameObj;
            activeNotification          = notification;
        }
        else
        {
            Debug.LogError("Notification text type not added to dictionary: " + notification.MessageType);
        }
    }

    private bool NotificationsAreTheSame(QueuedNotification notification1, QueuedNotification notification2)
    {
        if(notification1.MessageType == notification2.MessageType)
        {
            if(notification1.Parameters != null && notification2.Parameters != null)
            {
                int parameterCount = Mathf.Min(notification1.Parameters.Length, notification2.Parameters.Length);

                for (int i = 0; i < parameterCount; i++)
                {
                    if(notification1.Parameters[i] != notification2.Parameters[i])
                    {
                        //Different parameters, notifications are not the same
                        return false;
                    }
                }

                //Same parameters and message
                return true;
            }
            else
            {
                //Same message, both have no parameters
                return true;
            }
        }
        
        //Different messages
        return false;
    }
}

public class QueuedNotification
{
    public NotificationMessageType  MessageType;
    public string[]                 Parameters;
    public string                   SoundName;
}

using System;
using UnityEngine;

[Serializable]
public enum NotificationTypeEnum { Punish, Knockdown, CrossUp, GuardBreak, Counter, Reversal };

public class NotificationTypes : MonoBehaviour
{
    [SerializeField] private NotificationTypeEnum _notificationTypeEnum = default;

    public NotificationTypeEnum NotificationTypeEnum { get { return _notificationTypeEnum; } private set { } }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Message
{
    [Tooltip("The message to be displayed")]
    public string message;
    [Tooltip("If greater than 0 (zero), the message will be displayed in the top of the screen, otherwise it will be displayed at the bottom.")]
    public int position;
    public int callbackIndex;
    public MessageCallback callback;

    public Message (string message, int position) : this(message, position, 0, null) { }

    public Message (string message, int position, int callbackIndex, MessageCallback callback) {
        this.message = message;
        this.position = position;
        this.callbackIndex = callbackIndex;
        this.callback = callback;
    }

    public void Invoke () {
        callback?.Invoke(callbackIndex);
    }
}

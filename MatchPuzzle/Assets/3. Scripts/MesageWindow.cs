using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(RectXformMover))]
public class MesageWindow : MonoBehaviour
{
    public Image messageIcon;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI buttonText;

    public void ShowMessage(Sprite sprite = null, string massage = "", string buttonMsg = "start")
    {
        if (messageIcon != null) messageIcon.sprite = sprite;

        if (messageText != null) messageText.text = massage;

        if (buttonMsg != null) buttonText.text = buttonMsg;
    }
}

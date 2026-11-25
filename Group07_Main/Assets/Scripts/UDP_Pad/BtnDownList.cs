using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class BtnDownList : MonoBehaviour
{
    public static BtnDownList _instance;
    public List<Button> actionBtns;
    public List<Button> sceneBtns;
    public Slider slider;

    void Awake()
    {
        _instance = this;
    }

    public void OnReceivedDataReader(string msg)
    {
        if (string.IsNullOrWhiteSpace(msg)) return;
        msg = msg.Trim();
        if (msg.StartsWith("actionBtn_") && int.TryParse(msg.Substring(10), out int actionIndex))
        {
            if (actionIndex == 5) actionIndex = 0;
            else if (actionIndex == 6) actionIndex = 1;
            InvokeButton(actionBtns, actionIndex);
        }
        else if (msg.StartsWith("sceneBtn_") && int.TryParse(msg.Substring(9), out int sceneIndex))
        {
            InvokeButton(sceneBtns, sceneIndex);
        }
        else if (msg.StartsWith("Should_") && float.TryParse(msg.Substring(7), NumberStyles.Float,
                     CultureInfo.InvariantCulture, out float armLength)
                     && !float.IsNaN(armLength) && !float.IsInfinity(armLength))
        {
            slider.value = armLength;
        }
    }

    void InvokeButton(List<Button> buttons, int index)
    {
        if (index < 0 || index >= buttons.Count) return;
        Button button = buttons[index];
        if (button != null && button.isActiveAndEnabled && button.interactable)
        {
            button.onClick.Invoke();
        }
    }
}

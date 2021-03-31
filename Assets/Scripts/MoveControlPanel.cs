using System;
using UnityEngine;
using UnityEngine.UI;

public class MoveControlPanel : MonoBehaviour
{
    [SerializeField] private MoveButton frontButton;
    [SerializeField] private MoveButton backButton;
    [SerializeField] private MoveButton leftButton;
    [SerializeField] private MoveButton rightButton;
    [SerializeField] private Slider speedSlider;

    public Action<int, int> OnValueChanged { get; set; }
    private static readonly int turnSpeed = 10;

    private void Start()
    {
        frontButton.OnValueChanged = OnSpeedChanged;
        backButton.OnValueChanged = OnSpeedChanged;
        leftButton.OnValueChanged = OnSpeedChanged;
        rightButton.OnValueChanged = OnSpeedChanged;

        speedSlider.onValueChanged.AddListener((_) => { OnSpeedChanged(); });
        speedSlider.minValue = 8 + turnSpeed;
        speedSlider.maxValue = 115 - turnSpeed;
        speedSlider.normalizedValue = 0.5f;
    }

    private void OnSpeedChanged()
    {
        var frontSpeed = frontButton.IsPressed ? (int)speedSlider.value : 0;
        var backSpeed = backButton.IsPressed ? (int)speedSlider.value : 0;
        var leftTurnSpeed = leftButton.IsPressed ? turnSpeed : 0;
        var rightTurnSpeed = rightButton.IsPressed ? turnSpeed : 0;
        var leftMotorSpeed = frontSpeed - backSpeed + rightTurnSpeed - leftTurnSpeed;
        var rightMotorSpeed = frontSpeed - backSpeed + leftTurnSpeed - rightTurnSpeed;
        OnValueChanged?.Invoke(leftMotorSpeed, rightMotorSpeed);
    }
}

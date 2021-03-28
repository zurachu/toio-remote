using System;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public bool IsPressed { get; private set; }
    public Action OnValueChanged;

    public void OnPointerDown()
    {
        IsPressed = true;
        OnValueChanged?.Invoke();
    }

    public void OnPointerUp()
    {
        IsPressed = false;
        OnValueChanged?.Invoke();
    }
}

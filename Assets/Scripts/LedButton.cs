using System;
using UnityEngine;
using UnityEngine.UI;

public class LedButton : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Color color;

    public Action<Color> OnClicked;

    private void Start()
    {
        UIUtility.TrySetColor(image, color);
    }

    public void OnClickButton()
    {
        OnClicked?.Invoke(color);
    }
}

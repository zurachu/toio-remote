using System;
using System.Collections.Generic;
using UnityEngine;

public class LedControlPanel : MonoBehaviour
{
    [SerializeField] private List<LedButton> ledButtons;

    public Action<Color> OnTurnedOn { get; set; }
    public Action OnTurnedOff { get; set; }

    private void Start()
    {
        foreach (var ledButton in ledButtons)
        {
            ledButton.OnClicked = (_color) => { OnTurnedOn?.Invoke(_color); };
        }
    }

    public void OnClickTurnOff()
    {
        OnTurnedOff?.Invoke();
    }
}

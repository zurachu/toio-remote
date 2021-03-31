using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundControlPanel : MonoBehaviour
{
    [SerializeField] private List<EventTrigger> eventTriggers;

    public Action<int> OnPlaySound { get; set; }

    private void Start()
    {
        foreach (var (eventTrigger, index) in eventTriggers.WithIndex())
        {
            var soundId = index + 1;
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((_) => { OnPlaySound?.Invoke(soundId); });
            eventTrigger.triggers.Add(entry);
        }
    }
}

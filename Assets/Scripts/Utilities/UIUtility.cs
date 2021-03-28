using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtility
{
    public static void TrySetActive(GameObject gameObject, bool value)
    {
        if (gameObject != null)
        {
            gameObject.SetActive(value);
        }
    }

    public static void TrySetActive(MonoBehaviour monoBehaviour, bool value)
    {
        if (monoBehaviour != null)
        {
            TrySetActive(monoBehaviour.gameObject, value);
        }
    }

    public static void TrySetActive(IEnumerable<GameObject> gameObjects, bool value)
    {
        foreach (var gameObject in gameObjects)
        {
            TrySetActive(gameObject, value);
        }
    }

    public static void TrySetActive(IEnumerable<MonoBehaviour> monoBehaviours, bool value)
    {
        foreach (var monoBehaviour in monoBehaviours)
        {
            TrySetActive(monoBehaviour, value);
        }
    }

    public static void TrySetText(Text text, string message)
    {
        if (text != null)
        {
            text.text = message;
        }
    }

    public static void TrySetColor(Graphic graphic, Color color)
    {
        if (graphic != null && color != null)
        {
            graphic.color = color;
        }
    }
}

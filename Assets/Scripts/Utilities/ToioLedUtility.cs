using UnityEngine;
using toio;

public static class ToioLedUtility
{
    public static void TurnLedOn(Cube cube, Color color, int durationMs, Cube.ORDER_TYPE order = Cube.ORDER_TYPE.Strong)
    {
        cube.TurnLedOn(ColorByteValue(color.r), ColorByteValue(color.g), ColorByteValue(color.b), durationMs, order);
    }

    public static Cube.LightOperation LightOperationOf(Color color, int durationMs)
    {
        return new Cube.LightOperation(durationMs, ColorByteValue(color.r), ColorByteValue(color.g), ColorByteValue(color.b));
    }

    public static byte ColorByteValue(float value)
    {
        return (byte)Mathf.Clamp(value * 255, 0, 255);
    }
}

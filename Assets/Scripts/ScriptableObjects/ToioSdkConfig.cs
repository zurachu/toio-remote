using UnityEngine;
using toio;

[CreateAssetMenu]
public class ToioSdkConfig : ScriptableObject
{
    private static ToioSdkConfig instance;

    public ConnectType ConnectType;

    public bool IsSimulator => ActualConnectType == ConnectType.Simulator;

    private ConnectType ActualConnectType => ConnectType == ConnectType.Auto
        ? CubeScanner.actualTypeOfAuto
        : ConnectType;

    public static ToioSdkConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<ToioSdkConfig>("ScriptableObjects/ToioSdkConfig");
            }

            return instance;
        }
    }
}

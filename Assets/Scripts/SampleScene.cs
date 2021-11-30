using Cysharp.Threading.Tasks;
using toio;
using UnityEngine;
using UnityEngine.UI;

public class SampleScene : MonoBehaviour
{
    [SerializeField] private MoveControlPanel moveControlPanel;
    [SerializeField] private LedControlPanel ledControlPanel;
    [SerializeField] private SoundControlPanel soundControlPanel;
    [SerializeField] private GameObject connectingObject;
    [SerializeField] private TextAsset sdkVersion;

    [SerializeField] private Text sdkVersionText;
    [SerializeField] private Text versionText;
    [SerializeField] private Text idText;
    [SerializeField] private Text addrText;
    [SerializeField] private Text localNameText;
    [SerializeField] private Text batteryText;
    [SerializeField] private Text buttonText;
    [SerializeField] private Text slopeText;
    [SerializeField] private Text collisionText;
    [SerializeField] private Text positionIdXText;
    [SerializeField] private Text positionIdYText;
    [SerializeField] private Text angleText;
    [SerializeField] private Text standardIdText;
    [SerializeField] private Text doubleTapText;
    [SerializeField] private Text poseText;
    [SerializeField] private Text shakeText;
    [SerializeField] private Text leftMotorSpeedText;
    [SerializeField] private Text rightMotorSpeedText;

    private static readonly string callbackKey = nameof(SampleScene);

    private Cube cube;

    private async void Start()
    {
        Input.backButtonLeavesApp = true;

        UIUtility.TrySetActive(connectingObject, true);

        moveControlPanel.OnValueChanged = OnSpeedChanged;
        ledControlPanel.OnTurnedOn = OnTurnLedOn;
        ledControlPanel.OnTurnedOff = OnTurnLedOff;
        soundControlPanel.OnPlaySound = OnPlaySound;

        cube = await ToioCubeManagerService.Instance.CubeManager.SingleConnect();
        await cube.ConfigMotorRead(true);

        AddCallback(cube);
        InitializeStatus(cube);

        UIUtility.TrySetActive(connectingObject, false);
    }

    private void Update()
    {
        if (cube == null)
        {
            return;
        }

        UIUtility.TrySetText(batteryText, $"でんち: {cube.battery}");
        TrySetColor(collisionText, cube.isCollisionDetected);
        TrySetColor(doubleTapText, cube.isDoubleTap);
    }

    private void AddCallback(Cube c)
    {
        c.buttonCallback.AddListener(callbackKey, OnPressButton);
        c.slopeCallback.AddListener(callbackKey, OnSlope);
        c.collisionCallback.AddListener(callbackKey, DelayedRequestSensor);
        c.idCallback.AddListener(callbackKey, OnUpdatedId);
        c.standardIdCallback.AddListener(callbackKey, OnUpdatedStandardId);
        c.idMissedCallback.AddListener(callbackKey, OnMissedId);
        c.standardIdMissedCallback.AddListener(callbackKey, OnMissedStandardId);
        c.doubleTapCallback.AddListener(callbackKey, DelayedRequestSensor);
        c.poseCallback.AddListener(callbackKey, OnPose);
        c.shakeCallback.AddListener(callbackKey, OnShake);
        c.motorSpeedCallback.AddListener(callbackKey, OnMotorSpeed);
    }

    private void InitializeStatus(Cube c)
    {
        UIUtility.TrySetText(sdkVersionText, sdkVersion.text);
        UIUtility.TrySetText(versionText, $"ファームウェアバージョン: {c.version}");
        UIUtility.TrySetText(idText, $"しきべつID: {c.id}");
        UIUtility.TrySetText(addrText, $"アドレス: {c.addr}");
        UIUtility.TrySetText(localNameText, $"ローカルネーム: {c.localName}");

        OnPressButton(c);
        OnSlope(c);
        OnMissedId(c);
        OnMissedStandardId(c);
        OnPose(c);
        OnShake(c);
        OnMotorSpeed(c);
    }

    private void TrySetColor(Graphic graphic, bool value)
    {
        UIUtility.TrySetColor(graphic, value ? Color.red : Color.gray);
    }

    public void OnSpeedChanged(int left, int right)
    {
        if (cube == null)
        {
            return;
        }

        cube.Move(left, right, 0);
    }

    private void OnTurnLedOn(Color color)
    {
        if (cube == null)
        {
            return;
        }

        ToioLedUtility.TurnLedOn(cube, color, 0);
    }

    private void OnTurnLedOff()
    {
        if (cube == null)
        {
            return;
        }

        cube.TurnLedOff();
    }

    private void OnPlaySound(int soundId)
    {
        if (cube == null)
        {
            return;
        }

        cube.PlayPresetSound(soundId);
    }

    private void OnPressButton(Cube c)
    {
        TrySetColor(buttonText, c.isPressed);
    }

    private void OnSlope(Cube c)
    {
        TrySetColor(slopeText, c.isSloped);
    }

    private async void DelayedRequestSensor(Cube c)
    {
        await UniTask.Delay(500);
        c.RequestMotionSensor();
    }

    private void OnUpdatedId(Cube c)
    {
        UIUtility.TrySetColor(positionIdXText, Color.black);
        UIUtility.TrySetColor(positionIdYText, Color.black);
        UIUtility.TrySetText(positionIdXText, $"X: {c.pos.x}");
        UIUtility.TrySetText(positionIdYText, $"Y: {c.pos.y}");
        OnUpdatedAngle(c);
    }

    private void OnUpdatedStandardId(Cube c)
    {
        UIUtility.TrySetColor(standardIdText, Color.black);
        UIUtility.TrySetText(standardIdText, $"Standard ID: {c.standardId}");
        OnUpdatedAngle(c);
    }

    private void OnUpdatedAngle(Cube c)
    {
        UIUtility.TrySetColor(angleText, Color.black);
        UIUtility.TrySetText(angleText, $"かくど: {c.angle}");
    }

    private void OnMissedId(Cube c)
    {
        UIUtility.TrySetColor(positionIdXText, Color.gray);
        UIUtility.TrySetColor(positionIdYText, Color.gray);
        UIUtility.TrySetColor(angleText, Color.gray);
    }

    private void OnMissedStandardId(Cube c)
    {
        UIUtility.TrySetColor(standardIdText, Color.gray);
        UIUtility.TrySetColor(angleText, Color.gray);
    }

    private void OnPose(Cube c)
    {
        switch (c.pose)
        {
            case Cube.PoseType.Up:
            default:
                UIUtility.TrySetText(poseText, "しせい: うえ");
                break;
            case Cube.PoseType.Down:
                UIUtility.TrySetText(poseText, "しせい: した");
                break;
            case Cube.PoseType.Front:
                UIUtility.TrySetText(poseText, "しせい: まえ");
                break;
            case Cube.PoseType.Back:
                UIUtility.TrySetText(poseText, "しせい: うしろ");
                break;
            case Cube.PoseType.Right:
                UIUtility.TrySetText(poseText, "しせい: みぎ");
                break;
            case Cube.PoseType.Left:
                UIUtility.TrySetText(poseText, "しせい: ひだり");
                break;
        }
    }

    private void OnShake(Cube c)
    {
        UIUtility.TrySetText(shakeText, $"シェイク: {c.shakeLevel}");
    }

    private void OnMotorSpeed(Cube c)
    {
        UIUtility.TrySetText(leftMotorSpeedText, $"ひだり: {c.leftSpeed}");
        UIUtility.TrySetText(rightMotorSpeedText, $"みぎ　: {c.rightSpeed}");
    }
}

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
    [SerializeField] private Text magnetButtonText;
    [SerializeField] private Text magnetStateText;
    [SerializeField] private GameObject magneticForceRoot;
    [SerializeField] private Text magneticForceXText;
    [SerializeField] private Text magneticForceYText;
    [SerializeField] private Text magneticForceZText;
    [SerializeField] private Text attitudeButtonText;
    [SerializeField] private GameObject attitudeEulerRoot;
    [SerializeField] private Text attitudeEulerXText;
    [SerializeField] private Text attitudeEulerYText;
    [SerializeField] private Text attitudeEulerZText;
    [SerializeField] private GameObject attitudeQuaternionRoot;
    [SerializeField] private Text attitudeQuaternionXText;
    [SerializeField] private Text attitudeQuaternionYText;
    [SerializeField] private Text attitudeQuaternionZText;
    [SerializeField] private Text attitudeQuaternionWText;

    private static readonly string callbackKey = nameof(SampleScene);

    private Cube cube;
    private int currentMagneticMode;
    private int currentAttitudeFormat;

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
        c.magnetStateCallback.AddListener(callbackKey, OnMagnetState);
        c.magneticForceCallback.AddListener(callbackKey, OnMagneticForce);
        c.attitudeCallback.AddListener(callbackKey, OnAttitude);
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
        OnChangedMagneticMode(0);
        OnMagnetState(c);
        OnMagneticForce(c);
        OnChangedAttitudeFormat(0);
        OnAttitude(c);
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

    public async void OnClickChangeMagneticMode()
    {
        var nextMagneticMode = currentMagneticMode + 1;
        if (nextMagneticMode > 2)
        {
            nextMagneticMode = 0;
        }

        await cube.ConfigMagneticSensor((Cube.MagneticMode)nextMagneticMode, 20, Cube.MagneticNotificationType.OnChanged);
        OnChangedMagneticMode(nextMagneticMode);
    }

    public async void OnClickChangeAttitudeFormat()
    {
        var nextAttitudeFormat = currentAttitudeFormat + 1;
        if (nextAttitudeFormat > 2)
        {
            nextAttitudeFormat = 1;
        }

        await cube.ConfigAttitudeSensor((Cube.AttitudeFormat)nextAttitudeFormat, 10, Cube.AttitudeNotificationType.OnChanged);
        OnChangedAttitudeFormat(nextAttitudeFormat);
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

    private void OnChangedMagneticMode(int magneticMode)
    {
        currentMagneticMode = magneticMode;
        UIUtility.TrySetActive(magnetStateText, false);
        UIUtility.TrySetActive(magneticForceRoot, false);
        switch ((Cube.MagneticMode)currentMagneticMode)
        {
            case Cube.MagneticMode.MagnetState:
                UIUtility.TrySetText(magnetButtonText, "じしゃくのじょうたい");
                UIUtility.TrySetActive(magnetStateText, true);
                break;
            case Cube.MagneticMode.MagneticForce:
                UIUtility.TrySetText(magnetButtonText, "じりょくのつよさ");
                UIUtility.TrySetActive(magneticForceRoot, true);
                break;
            default:
                UIUtility.TrySetText(magnetButtonText, "じしゃく");
                break;
        }
    }

    private void OnMagnetState(Cube c)
    {
        switch (c.magnetState)
        {
            case Cube.MagnetState.None:
            default:
                UIUtility.TrySetText(magnetStateText, "じょうたい: なし");
                break;
            case Cube.MagnetState.S_Center:
                UIUtility.TrySetText(magnetStateText, "じょうたい: Sちゅうおう");
                break;
            case Cube.MagnetState.N_Center:
                UIUtility.TrySetText(magnetStateText, "じょうたい: Nちゅうおう");
                break;
            case Cube.MagnetState.S_Right:
                UIUtility.TrySetText(magnetStateText, "じょうたい: Sみぎ");
                break;
            case Cube.MagnetState.N_Right:
                UIUtility.TrySetText(magnetStateText, "じょうたい: Nみぎ");
                break;
            case Cube.MagnetState.S_Left:
                UIUtility.TrySetText(magnetStateText, "じょうたい: Sひだり");
                break;
            case Cube.MagnetState.N_Left:
                UIUtility.TrySetText(magnetStateText, "じょうたい: Nひだり");
                break;
        }
    }

    private void OnMagneticForce(Cube c)
    {
        UIUtility.TrySetText(magneticForceXText, $"X: {c.magneticForce.x}");
        UIUtility.TrySetText(magneticForceYText, $"Y: {c.magneticForce.y}");
        UIUtility.TrySetText(magneticForceZText, $"Z: {c.magneticForce.z}");
    }

    private void OnChangedAttitudeFormat(int attitudeFormat)
    {
        currentAttitudeFormat = attitudeFormat;
        UIUtility.TrySetActive(attitudeEulerRoot, false);
        UIUtility.TrySetActive(attitudeQuaternionRoot, false);
        switch ((Cube.AttitudeFormat)currentAttitudeFormat)
        {
            case Cube.AttitudeFormat.Eulers:
                UIUtility.TrySetText(attitudeButtonText, "オイラー");
                UIUtility.TrySetActive(attitudeEulerRoot, true);
                break;
            case Cube.AttitudeFormat.Quaternion:
                UIUtility.TrySetText(attitudeButtonText, "クォータニオン");
                UIUtility.TrySetActive(attitudeQuaternionRoot, true);
                break;
            default:
                UIUtility.TrySetText(attitudeButtonText, "しせいかく");
                break;
        }
    }

    private void OnAttitude(Cube c)
    {
        UIUtility.TrySetText(attitudeEulerXText, $"X: {c.eulers.x}");
        UIUtility.TrySetText(attitudeEulerYText, $"Y: {c.eulers.y}");
        UIUtility.TrySetText(attitudeEulerZText, $"Z: {c.eulers.z}");
        UIUtility.TrySetText(attitudeQuaternionXText, $"X: {c.quaternion.x}");
        UIUtility.TrySetText(attitudeQuaternionYText, $"Y: {c.quaternion.y}");
        UIUtility.TrySetText(attitudeQuaternionZText, $"Z: {c.quaternion.z}");
        UIUtility.TrySetText(attitudeQuaternionWText, $"W: {c.quaternion.w}");
    }
}

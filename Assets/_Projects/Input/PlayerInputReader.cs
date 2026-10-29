using MyUtils;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Projects.Input
{
    // プレイヤーの入力の読み取りを行うクラス。
    public class PlayerInputReader : AbstractUnitControllable,
        InputSystem_Actions.IPlayerActions
    {
        // カメラ操作がマウスかコントローラか判別するためのプロパティ
        [SerializeField, ReadOnly] private SerializableReactiveProperty<string> _lookDeviceName = new();
        public override string LookDeviceName => _lookDeviceName.Value;

        private InputSystem_Actions _actions;
        public InputSystem_Actions.PlayerActions Player { get; private set; }

        protected override void Awake()
        {
            base.Awake(); // AbstractUnitControllableのAwake()を呼び出す
            _actions = new InputSystem_Actions(); // 入力システムのインスタンス生成
            Player = _actions.Player; // プレイヤーのアクションマップを取得
            Player.AddCallbacks(this); // コールバックを登録（下で定義しているOnMoveなどのメソッドが呼ばれるようになる）

            //InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;
            // _updateObservable.OnUpdate(EUpdatePhase.Input)
            //     .Subscribe(_ => OnPhaseUpdate())
            //     .AddTo(this);
        }

        private void OnEnable() => _actions.Enable(); // オブジェクトが有効なら入力システムを有効化

        private void OnDisable() => _actions.Disable(); // オブジェクトが無効なら入力システムを無効化

        private void OnDestroy()
        {
            // このオブジェクトが破棄されたら、登録したコールバックを削除して、入力システムのインスタンスを破棄する
            Player.RemoveCallbacks(this);
            _actions.Dispose();
        }
        // private static void OnPhaseUpdate() => InputSystem.Update();

        // 入力値をVector2として読み取るためにReadValue<Vector2>()を使用
        public void OnMove(InputAction.CallbackContext context) => _move.Value = context.ReadValue<Vector2>();

        public void OnLook(InputAction.CallbackContext context)
        {
            _look.Value = context.ReadValue<Vector2>();
            // 入力デバイスの名前を取得して、_lookDeviceNameに格納する。もしデバイスがnullの場合は空文字列を格納する。
            _lookDeviceName.Value = context.control?.device?.displayName ?? string.Empty;
        }

        // 入力値をButton（bool）として読み取るためにReadValueAsButton()を使用
        public void OnAttack(InputAction.CallbackContext context) => _fire.Value = context.ReadValueAsButton();
        public void OnFly(InputAction.CallbackContext context) => _fly.Value = context.ReadValueAsButton();
        public void OnBoost(InputAction.CallbackContext context) => _boost.Value = context.ReadValueAsButton();
        public void OnLockOn(InputAction.CallbackContext context) => _lockOn.Value = context.ReadValueAsButton();
    }
}
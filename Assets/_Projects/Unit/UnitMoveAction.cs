using System.Threading;
using Cysharp.Threading.Tasks;
using MyUtils;
using MyUtils.VContainerExtensions;
using R3;
using R3.Triggers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Projects.Unit
{
    // ユニットの移動アクションを監視するためのインターフェイス
    public interface IMoveObservable : IUnitActionObservable
    {
    }

    // ユニットの移動アクションを命令するためのインターフェイス
    public interface IMoveActionHandler : IUnitActionHandler<Vector2>
    {
    }

    public class UnitMoveAction : AbstractUnitAction, // 継承
        IScopeRegisterable,
        IMoveObservable, // インターフェイスの実装①
        IMoveActionHandler // インターフェイスの実装②
    {
        [Header("References")] // Inspectorでの表示用
        public Transform Target; // 動かす対象

        [Header("Settings")]
        public float MovePower = 70f; // 移動力
        public float PowerDownTime = 10f; // 移動力の減衰時間
        public float StoppingPower = 2f; // 自動停止の強さ

        // 移動方向（閲覧用）
        [field: SerializeField, ReadOnly] public Vector3 MoveDirection { get; private set; }

        private float _currentPower; // 現在の移動力
        [Inject] private Rigidbody _rigidbody; // ScopeRootから注入されたRigidbodyを取得
        [Inject] private GroundDetection _groundDetection;
        // [Inject] private IBoostActionObservable _playerBoost;

        protected override void Awake()
        {
            base.Awake();
            _currentPower = MovePower;
        }

        private void Start()
        {
            // 物理冤罪の更新タイミングで移動処理を行う
            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    var moveDirection = GetGroundAlignedMoveDirection();
                    ApplyMove(moveDirection); // 移動
                    ApplyAutoStop(); // 自動停止
                })
                .AddTo(this);
        }

        // 移動キー（入力値）が変化したときに呼ばれるメソッド
        public UniTask OnValueChanged(Vector2 value, CancellationToken ct = default)
        {
            _isAction.Value = value.sqrMagnitude > 0f; // 入力値が0出ないかチェック
            var clampMagnitude = Vector2.ClampMagnitude(value, 1); // 入力値の大きさを1に制限
            // 入力値を3Dの移動方向に変換（y軸は0にする）
            MoveDirection = new Vector3(clampMagnitude.x, 0f, clampMagnitude.y);
            return UniTask.CompletedTask;
        }

        public void CancelAction()
        {
            _isAction.Value = false;
            MoveDirection = Vector3.zero;
        }

        // 移動方向を地面に沿った方向に変換するメソッド
        private Vector3 GetGroundAlignedMoveDirection()
        {
            // 入力値の方向をキャラクターの向きに合わせる
            var moveDirection = Target.TransformDirection(MoveDirection);

            // 地面に設置しているときは移動方向を地面の法線に沿って調整する
            if (_groundDetection.IsHit.CurrentValue)
            {
                var groundNormal = _groundDetection.HitObject.CurrentValue.normal;
                moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized;
            }

            return moveDirection;
        }

        private void ApplyMove(Vector3 moveDirection)
        {
            // ブースト中はPlayerBoost.BoostPowerを目標に、それ以外はMovePowerを目標に近づける
            // var targetPower = _playerBoost.IsAction.CurrentValue ? _playerBoost.BoostPower : MovePower;
            // _currentPower = Mathf.Lerp(_currentPower, targetPower, PowerDownTime * Time.fixedDeltaTime);

            // オブジェクトの速度に移動方向と移動力を加算する
            _rigidbody.linearVelocity += moveDirection * (_currentPower * Time.fixedDeltaTime);
            Debug.Log(moveDirection * (_currentPower * Time.fixedDeltaTime));
            Debug.Log(_rigidbody.linearVelocity);
        }

        private void ApplyAutoStop()
        {
            // 自動で止まる
            _rigidbody.linearVelocity =
                Vector3.Lerp(
                    _rigidbody.linearVelocity, // 現在の速度
                    Vector3.zero, // 目標値（停止）
                    StoppingPower * Time.fixedDeltaTime // 補間の強さ（スピード）
                );
        }

        // このクラスに実装されている二つのインターフェイスをVContainerに登録するためのメソッド
        public void OnRegister(IContainerBuilder builder) =>
            builder.RegisterComponent(this).As<IMoveObservable, IMoveActionHandler>();
    }
}
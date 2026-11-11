using R3;
using R3.Triggers;
using UnityEngine;
using VContainer;

namespace _Projects.Unit
{
    public class UnitAnimation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Light _boostLight;

        [Header("Settings")]
        [SerializeField] private float _dampTime = 0.2f; // 値をゆっくり反映するための減衰時間

        [Header("Light Settings")]
        [SerializeField] private float _tweenSpeed = 100f; // ライトの強さを変化させる速度
        [SerializeField] private float _defaultLightIntensity = 10f; // ライトの強さの初期値
        [SerializeField] private float _boostLightIntensity = 200f; // ライトの強さ

        private static readonly int _boostHash = Animator.StringToHash("Boost");
        private static readonly int _groundHash = Animator.StringToHash("Ground");
        private static readonly int _moveXHash = Animator.StringToHash("MoveX");
        private static readonly int _moveZHash = Animator.StringToHash("MoveZ");
        private static readonly int _jumpOnHash = Animator.StringToHash("JumpOn");
        private static readonly int _jumpOffHash = Animator.StringToHash("JumpOff");

        [Inject] private UnitMoveAction _unitMove;
        [Inject] private GroundDetection _groundDetection;
        [Inject] private IBoostActionObservable _boost;

        private void Start()
        {
            // 地面の接地状態が変化したときに、ジャンプのアニメーションをトリガーする
            _groundDetection.IsHit.Subscribe(isGround => _animator.SetTrigger(isGround ? _jumpOffHash : _jumpOnHash))
                .AddTo(this);

            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    var deltaTime = Time.deltaTime;
                    var isBoost = _boost.IsAction.CurrentValue;

                    // ブースト時のライトの光量をゆっくり変化させる
                    UpdateBoostLight(isBoost, deltaTime);
                    UpdateAnimatorParameters(isBoost, _unitMove.MoveDirection, deltaTime);
                })
                .AddTo(this);
        }

        private void UpdateBoostLight(bool isBoost, float deltaTime)
        {
            // ブースト時のライトの光量をゆっくり変化させる
            var targetIntensity = isBoost ? _boostLightIntensity : _defaultLightIntensity;
            _boostLight.intensity = Mathf.Lerp(_boostLight.intensity, targetIntensity, deltaTime * _tweenSpeed);
        }

        private void UpdateAnimatorParameters(bool isBoost, Vector3 moveDirection, float deltaTime)
        {
            // 各アニメーションパラメータを更新する
            _animator.SetBool(_boostHash, isBoost);
            _animator.SetBool(_groundHash, _groundDetection.IsHit.CurrentValue);
            _animator.SetFloat(_moveXHash, moveDirection.x, _dampTime, deltaTime); // 減衰時間を指定して値をゆっくり反映
            _animator.SetFloat(_moveZHash, moveDirection.z, _dampTime, deltaTime); // 減衰時間を指定して値をゆっくり反映
        }
    }
}
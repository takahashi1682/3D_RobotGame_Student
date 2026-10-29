using MyUtils.VContainerExtensions;
using R3;
using R3.Triggers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Projects.Unit
{
    /// <summary>
    /// 重力の適用を担当する。PlayerFly(IFlyActionObservable)が飛行中の間は重力をリセットし、
    /// 飛行しておらず、かつ接地していない間だけ重力を加算する。
    /// </summary>
    public class UnitGravity : MonoBehaviour,
        IScopeRegisterable
    {
        [Header("Settings")]
        public float Gravity = -2.25f; // 落下速度
        public Vector3 GravityDirection = Vector3.down; // 落下方向

        private float _currentGravity;

        [Inject] private Rigidbody _rigidbody;
        [Inject] private GroundDetection _groundDetection;
        [Inject] private IFlyActionObservable _flyActionObservable;

        public void OnRegister(IContainerBuilder builder)
        {
            builder.RegisterComponent(this); // コンテナにこのクラスを登録
        }

        private void Start()
        {
            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    // 飛行中または接地中は重力をリセットし、そうでない場合は重力を適用する
                    if (_flyActionObservable.IsAction.CurrentValue ||
                        _groundDetection.IsHit.CurrentValue)
                    {
                        // 飛行中または接地中は重力をリセットする
                        _currentGravity = 0f;
                    }
                    else
                    {
                        ApplyGravity(); // 重力を適用する
                    }
                })
                .AddTo(this);
        }

        /// <summary>
        /// 重力の処理。重力方向に力を加える。
        /// </summary>
        private void ApplyGravity()
        {
            _currentGravity -= Gravity * Time.fixedDeltaTime;
            _rigidbody.linearVelocity += GravityDirection * _currentGravity;
        }
    }
}
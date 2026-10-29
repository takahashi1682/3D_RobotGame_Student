using System.Threading;
using Cysharp.Threading.Tasks;
using MyUtils.VContainerExtensions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Projects.Unit
{
    public interface IFlyActionHandler : IUnitActionHandler<bool>
    {
    }

    public interface IFlyActionObservable : IUnitActionObservable
    {
    }

    public class UnitFlyAction : AbstractUnitAction, // class継承
        IScopeRegisterable, // interface実装
        IFlyActionHandler, // interface実装
        IFlyActionObservable // interface実装
    {
        [Header("Settings")]
        public float FlyForce = 10f; // 飛行中の上方向の力
        public int FlyingEnergy = 1; // 飛行中に消費するエネルギー量

        [Inject] private Rigidbody _rigidbody; // コンテナからInject(注入)してもらう
        // [Inject] private Energy _energy;

        public void OnRegister(IContainerBuilder builder)
        {
            // このクラスに実装されている二つのinterfaceをコンテナに登録する
            builder.RegisterComponent(this).As<IFlyActionHandler, IFlyActionObservable>();
        }

        public UniTask OnValueChanged(bool value, CancellationToken ct)
        {
            _isAction.Value = value; // ボタンの入力情報を反映
            return UniTask.CompletedTask;
        }

        private void FixedUpdate()
        {
            if (!IsAction.CurrentValue) return; // アクション中でなければ何もしない

            // if (_energy.CurrentValue <= 0)
            // {
            //     CancelAction();
            //     return;
            // }

            ApplyFly();
            // _energy.Sub(FlyingEnergy);
        }

        /// <summary>
        /// 飛行中の処理。
        /// </summary>
        private void ApplyFly()
        {
            // 上方向に力を加える
            _rigidbody.linearVelocity += Vector3.up * (FlyForce * Time.fixedDeltaTime);
        }

        public void CancelAction()
        {
            _isAction.Value = false;
        }
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using MyUtils.VContainerExtensions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Projects.Unit
{
    public interface IBoostActionHandler : IUnitActionHandler<bool>
    {
    }

    public interface IBoostActionObservable : IUnitActionObservable
    {
        float BoostPower { get; } // ブーストの力
        int BoostEnergy { get; } // ブーストに必要なエネルギー量
    }

    public class UnitBoostAction : AbstractUnitAction,
        IScopeRegisterable,
        IBoostActionHandler,
        IBoostActionObservable
    {
        [Header("Settings")]
        public float BoostPower = 500f; // ブーストの推力
        public int BoostEnergy = 300; // ブーストに必要なエネルギー量
        public int BoostDuration = 300; // ブーストの持続時間
        public int BoostInterval = 300; // ブースト後のインターバル時間

        [Inject] private Energy _energy; // コンテナからInject(注入)してもらう

        float IBoostActionObservable.BoostPower => BoostPower;
        int IBoostActionObservable.BoostEnergy => BoostEnergy;

        public void OnRegister(IContainerBuilder builder)
        {
            builder.RegisterComponent(this).As<IBoostActionHandler, IBoostActionObservable>();
        }

        public async UniTask OnValueChanged(bool value, CancellationToken ct)
        {
            if (!value) return; // キーを押したときだけ処理する
            if (IsAction.CurrentValue) return; // すでにブースト中なら処理しない
            _isAction.Value = true; // ブースト中にする

            _energy.Sub(BoostEnergy); // ブースト時にEnergyを減算

            await UniTask.Delay(BoostDuration, cancellationToken: ct);
            _isAction.Value = false;
            await UniTask.Delay(BoostInterval, cancellationToken: ct);
        }

        public void CancelAction()
        {
            _isAction.Value = false;
        }
    }
}
using MyUtils.Parameter;
using MyUtils.VContainerExtensions;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Projects.Unit
{
    public class EnergyUpdater :
        IntParameterUpdater, // IntParameterUpdaterを継承することで、IntParameterの値を更新するクラスになる
        IScopeRegisterable
    {
        [Inject] private GroundDetection _groundDetection;

        public void OnRegister(IContainerBuilder builder)
        {
            builder.RegisterComponent(this);
        }

        private void Awake()
        {
            // 地面に設置している時のみ有効にする
            _groundDetection.IsHit
                .Subscribe(x => IsEnable = x)
                .AddTo(this);
        }
    }
}
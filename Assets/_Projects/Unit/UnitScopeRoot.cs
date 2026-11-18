using System;
using MyUtils.Parameter.Basic;
using MyUtils.VContainerExtensions;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Projects.Unit
{
    public class UnitScopeRoot : AbstractScopeRoot
    {
        // このユニットが動けるかの判定（ゲーム終了したり死んだらFalseに変わる）
        [SerializeField] private SerializableReactiveProperty<bool> _running = new(true);
        public ReadOnlyReactiveProperty<bool> Running => _running;

        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private GroundDetection _groundDetection;

        [field: SerializeField] public UnitSetting Setting { get; private set; }
        [field: SerializeField] public Health Health { get; private set; }
        [field: SerializeField] public Energy Energy { get; private set; }
        [Inject] private IUnitManager _unitManager;

        private void Awake()
        {
            _running.AddTo(this);
            // 設定値を反映
            Health.SetMax(Setting.MaxHealth); // 最大値を設定
            Health.SetFull(); // 現在値をFullにする
            Energy.SetMax(Setting.MaxEnergy);
            Energy.SetFull();
        }

        private void Start()
        {
            // 体力が0になったらUnitManagerから削除する
            Health.IsEmpty.Subscribe(x =>
            {
                if (x) _unitManager.RemoveUnit(this);
                else _unitManager.RegisterUnit(this);
            }).AddTo(this);
        }

        // このスコープでコンテナが作られる時に呼ばれるメソッド
        protected override void ConfigureScope(IContainerBuilder builder)
        {
            base.ConfigureScope(builder);
            // このオブジェクトにアタッチされているスクリプトにRigidbodyを注入するための登録
            builder.RegisterComponent(_rigidbody);
            // コンテナにGroundDetectionを登録する
            builder.RegisterComponent(_groundDetection);
            // コンテナにHealthを登録する
            builder.RegisterComponent(Health);
            // コンテナにEnergyを登録する
            builder.RegisterComponent(Energy);

            builder.RegisterComponent(Setting);
        }
    }
}
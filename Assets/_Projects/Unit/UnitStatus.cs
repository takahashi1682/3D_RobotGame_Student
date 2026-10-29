using System;
using MyUtils.Parameter;
using MyUtils.VContainerExtensions;
using R3;
using R3.Triggers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Projects.Unit
{
    namespace _Projects.Unit
    {
        // ユニットの状態フラグ
        public enum EUnitState
        {
            Move,
            Look,
            LockOn,
            Fire,
            Boost,
            Grounded, // 地上にいるか
            EnergyEmpty, // Energy切れ
            Dead, // 死亡
        }

        // ユニットが操作可能かどうかのフラグ
        public enum ECanFlags
        {
            Move,
            Look,
            LockOn,
            Fire,
            Boost,
            Fly,
        }

        public class UnitStatus : MonoBehaviour, IScopeRegisterable
        {
            // ユニットの状態を管理するフラグ
            [SerializeField] private FlagsParameter<EUnitState> _stateFlags = new();

            // ユニットが操作可能かどうかのフラグを管理する
            [SerializeField] private FlagsParameter<ECanFlags> _canFlags = new();

            // 外部から状態フラグを取得するためのプロパティ
            public bool HasFlag(EUnitState flag) => _stateFlags.HasFlag(flag);

            // 外部から操作可能フラグを取得するためのプロパティ
            public bool CanMove => _canFlags.HasFlag(ECanFlags.Move);
            public bool CanLook => _canFlags.HasFlag(ECanFlags.Look);
            public bool CanLockOn => _canFlags.HasFlag(ECanFlags.LockOn);
            public bool CanFire => _canFlags.HasFlag(ECanFlags.Fire);
            public bool CanBoost => _canFlags.HasFlag(ECanFlags.Boost);
            public bool CanFly => _canFlags.HasFlag(ECanFlags.Fly);

            private void Update()
            {
                // 毎フレーム各値を更新する

                // Moveできる条件
                var canMove = true;
                canMove &= !HasFlag(EUnitState.Dead);
                _canFlags.SetFlag(ECanFlags.Move, canMove);

                // Lookできる条件
                var canLook = true;
                canLook &= !HasFlag(EUnitState.Dead);
                canLook &= !HasFlag(EUnitState.LockOn);
                _canFlags.SetFlag(ECanFlags.Look, canLook);

                // Boostできる条件
                var canBoost = true;
                canBoost &= HasFlag(EUnitState.Move);
                canBoost &= !HasFlag(EUnitState.Boost);
                canBoost &= !HasFlag(EUnitState.Dead);
                // canBoost &= _energy.CurrentValue > _boost.BoostEnergy; あとで書く
                _canFlags.SetFlag(ECanFlags.Boost, canBoost);

                // Fireできる条件
                var canFire = true;
                canFire &= !HasFlag(EUnitState.Dead);
                _canFlags.SetFlag(ECanFlags.Fire, canFire);

                // Flyできる条件
                var canFly = true;
                canFly &= !HasFlag(EUnitState.EnergyEmpty);
                canFly &= !HasFlag(EUnitState.Dead);
                _canFlags.SetFlag(ECanFlags.Fly, canFly);

                // LockOnできる条件
                var canLockOn = true;
                canLockOn &= !HasFlag(EUnitState.Dead);
                _canFlags.SetFlag(ECanFlags.LockOn, canLockOn);
            }

            // このクラス自身をVContainerに登録するためのメソッド
            public void OnRegister(IContainerBuilder builder) => builder.RegisterComponent(this);
        }
    }
}
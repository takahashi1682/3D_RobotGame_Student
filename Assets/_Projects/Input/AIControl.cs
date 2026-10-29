using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Projects.Input
{
    public class AIControl : AbstractUnitControllable
    {
        public override string LookDeviceName => string.Empty;

        [Header("Settings")]
        public float ThinkingInterval = 1f; // AIの思考間隔（秒）
        public float FlyRate = 0.2f; // 飛行を試みる確率（0〜1）
        public float BoostRate = 0.5f; // ブーストを試みる確率（0〜1）
        public float FireRate = 0.9f; // 攻撃を試みる確率（0〜1）

        private void Start()
        {
            Observable.Interval(TimeSpan.FromSeconds(ThinkingInterval))
                .Subscribe(_ =>
                {
                    // ランダムな方向に移動する
                    _move.Value = new Vector2(
                            Random.Range(-1f, 1f), // X軸方向のランダムな値(-1~1)
                            Random.Range(-1f, 1f)) // Y軸方向のランダムな値(-1~1)
                        .normalized; // 正規化して方向ベクトルにする（例：(1,1) -> (0.707, 0.707)）

                    // 1秒間隔でランダムな行動を試みる
                    TryPress(_fly, FlyRate, Random.Range(1f, 2f)); // 1秒~2秒の間ボタンを押す
                    TryPress(_fire, FireRate, Random.Range(1f, 2f));　// 1秒~2秒の間ボタンを押す
                })
                .AddTo(this);
        }

        // rateの確率でactionをduration秒だけtrueにする(ボタンの単発押下を模す)。
        private void TryPress(ReactiveProperty<bool> action, float rate, float duration)
        {
            if (Random.value < rate) VirtualPress(action, duration, destroyCancellationToken).Forget();
        }

        // 一定時間だけボタンを押す動作を模倣する非同期メソッド。actionをtrueにして、duration秒待ってからfalseに戻す。
        private static async UniTask VirtualPress(ReactiveProperty<bool> action, float duration, CancellationToken ct)
        {
            action.Value = true; // ボタンを押す
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct); // duration秒待つ
            action.Value = false; // ボタンを離す
        }

        // [Inject] private UnitScopeRoot _unitScopeRoot;
        // [Inject] private UnitManager _unitManager;
        // [Inject] private IUnitTrackingObservable _trackingObservable;
        // [Inject] private IUnitTrackingHandler _trackingHandler;
        // [Inject] private UnitStatus _unitStatus;
        // [Inject] private UnitSetting _unitSetting;
        // [Inject] private UpdateScheduler _updateObservable;
        // private float _lastThinkTime;
        //
        // public void OnRegister(IContainerBuilder builder)
        // {
        //     builder.RegisterComponent(this).As<IUnitControllable>();
        // }
        //
        // private void Start()
        // {
        //     _move.AddTo(this);
        //     _look.AddTo(this);
        //     _fly.AddTo(this);
        //     _boost.AddTo(this);
        //     _fire.AddTo(this);
        //     _lockOn.AddTo(this);
        //     _lastThinkTime = Time.time;
        //
        //     _updateObservable.OnUpdate(EUpdatePhase.Default)
        //         .Subscribe(_ => OnPhaseUpdate())
        //         .AddTo(this);
        // }
        //
        // public void OnPhaseUpdate()
        // {
        //     if (Time.time - _lastThinkTime >= ThinkingInterval)
        //     {
        //         _lastThinkTime = Time.time;
        //         Think();
        //     }
        // }
        //
        // /// <summary>
        // /// 一定間隔で呼ばれるAIの思考処理。停止中はすべての操作を解除し、
        // /// 稼働中はロックオンの有無で行動を切り替える。
        // /// </summary>
        // private void Think()
        // {
        //     if (!_unitScopeRoot.Running.CurrentValue)
        //     {
        //         StopAllActions();
        //         return;
        //     }
        //
        //     if (_unitStatus.HasFlag(EUnitState.LockOn))
        //     {
        //         DecideLockedOnBehavior();
        //     }
        //     else
        //     {
        //         DecideSearchBehavior();
        //     }
        // }
        //
        // /// <summary>
        // /// ロックオン中の行動。ランダムに動き回りつつ、状況に応じて飛行・ブースト・攻撃を試みる。
        // /// </summary>
        // private void DecideLockedOnBehavior()
        // {
        //     _move.Value = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        //
        //     // 飛行を試みる
        //     if (_unitStatus.HasFlag(EUnitState.Grounded))
        //         TryPress(_fly, FlyRate, Random.Range(1f, 2f));
        //
        //     // ブーストを試みる(ロックオンの有無に関係なく共通)
        //     TryPress(_boost, BoostRate, 0.1f);
        //
        //     // ターゲット見える場合は攻撃を試みる
        //     if (_trackingObservable.IsLookingAtTarget)
        //     {
        //         if (!_fire.Value) TryPress(_fire, FireRate, Random.Range(2f, 3f));
        //     }
        // }
        //
        // /// <summary>
        // /// ロックオン前の行動。ターゲットを探し、見つかっていれば前進しつつロックオンを試みる。
        // /// </summary>
        // private void DecideSearchBehavior()
        // {
        //     if (_trackingObservable.Target.CurrentValue == null)
        //     {
        //         // 最も近い敵ユニットを検索する
        //         var target = _unitManager.FindClosestTargetUnit(
        //             _unitSetting.Army,
        //             _unitSetting.UnitPivot.position,
        //             float.MaxValue); // 範囲は無制限で検索する
        //
        //         if (target != null)
        //         {
        //             _trackingHandler.SetTarget(target, float.MaxValue);
        //         }
        //     }
        //     else
        //     {
        //         // 前進する
        //         _move.Value = new Vector2(0, 1);
        //
        //         // ブーストを試みる(ロックオンの有無に関係なく共通)
        //         TryPress(_boost, BoostRate, 0.1f);
        //
        //         // ロックオンを試みる
        //         TryPress(_lockOn, 1f, 0.1f);
        //     }
        // }
        //
        // /// <summary>
        // /// ユニットが停止中の場合、すべての操作を解除する。
        // /// </summary>
        // private void StopAllActions()
        // {
        //     _move.Value = Vector2.zero;
        //     _look.Value = Vector2.zero;
        //     _fly.Value = false;
        //     _boost.Value = false;
        //     _fire.Value = false;
        //     _lockOn.Value = false;
        // }
        //
        // /// <summary>
        // /// rateの確率でactionをduration秒だけtrueにする(ボタンの単発押下を模す)。
        // /// </summary>
        // private void TryPress(ReactiveProperty<bool> action, float rate, float duration)
        // {
        //     if (Random.value < rate)
        //     {
        //         VirtualPress(action, duration, destroyCancellationToken).Forget();
        //     }
        // }
        //
        // private static async UniTask VirtualPress(ReactiveProperty<bool> action, float duration, CancellationToken ct)
        // {
        //     action.Value = true;
        //     await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct);
        //     action.Value = false;
        // }
    }
}
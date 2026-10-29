using System.Threading;
using _Projects.Input;
using _Projects.Unit._Projects.Unit;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;

namespace _Projects.Unit
{
    // アクションの状態を監視するためのインターフェイス
    public interface IUnitActionObservable
    {
        ReadOnlyReactiveProperty<bool> IsAction { get; } // アクション中かどうか
    }

    // アクションの操作を命令するためのインターフェイス
    public interface IUnitActionHandler<in T>
    {
        UniTask OnValueChanged(T value, CancellationToken ct); // アクションの実行（引数は実行値）
        void CancelAction(); // アクションのキャンセル
    }

    // 入力された時のアクションの呼び出し処理
    public class UnitActionController : MonoBehaviour
    {
        // PlayerまたはAIの入力情報
        [Inject] protected IUnitControllable _control;
        // 現在のステータス
        [Inject] protected UnitStatus _status;
        // 各アクションの操作用インターフ     [Inject] protected UnitStatus _playerStatus;ェイス
        [Inject] protected IMoveActionHandler _moveHandler;
        [Inject] protected IFlyActionHandler _flyHandler;

        protected virtual void Start()
        {
            _control.Move
                .Where(_ => _status.CanMove) // 現在移動可能な状態かどうかを確認
                .SubscribeAwait(async (value, cts) =>
                    await _moveHandler.OnValueChanged(value, cts), AwaitOperation.Drop) // アクションの実行（値を渡す）
                .AddTo(this);

            _control.Fly
                .Where(_ => _status.CanFly) // 現在飛行可能な状態かどうかを確認
                .SubscribeAwait(async (value, cts) =>
                    await _flyHandler.OnValueChanged(value, cts), AwaitOperation.Drop) // アクションの実行（値を渡す）
                .AddTo(this);
        }
    }
}
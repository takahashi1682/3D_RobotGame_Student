using R3;
using UnityEngine;
using VContainer;

namespace _Projects.Unit
{
    // プレイヤーのアクションを管理するクラス
    public class PlayerActionController : UnitActionController
    {
        private ILookActionHandler _lookHandler;
        [Inject] private IObjectResolver _lookResolver; // コンテナー

        protected override void Start()
        {
            base.Start();
            _lookResolver.TryResolve(out _lookHandler);
            _control.Look
                .SubscribeAwait(async (x, cts) =>
                    {
                        // _unitScopeRoot.Running.CurrentValue && は後で入力
                        if (_status.CanLook)
                        {
                            await _lookHandler.OnValueChanged(x, cts);
                        }
                        else
                        {
                            await _lookHandler.OnValueChanged(Vector2.zero, cts);
                        }
                    }, AwaitOperation.Drop
                ).AddTo(this);
        }
    }
}
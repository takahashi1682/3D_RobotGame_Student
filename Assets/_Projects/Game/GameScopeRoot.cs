using _Projects.Unit;
using MyUtils.VContainerExtensions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Projects.Game
{
    // ゲームが始まったら最初に起動するスコープルート。ゲーム全体で共有する依存関係を登録する。
    public class GameScopeRoot : AbstractScopeRoot
    {
        [SerializeField] private Camera _mainCamera;

        protected override void ConfigureScope(IContainerBuilder builder)
        {
            base.ConfigureScope(builder);
            // ゲーム全体で共有する依存関係を登録する
            builder.RegisterComponent(_mainCamera);
            // まだインスタンスがない場合は、ここで生成される
            builder.RegisterEntryPoint<UpdateScheduler>();
            builder.RegisterEntryPoint<UnitManager>();
        }
    }
}
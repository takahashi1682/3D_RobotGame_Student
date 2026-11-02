using MyUtils.VContainerExtensions;
using VContainer;
using VContainer.Unity;

namespace _Projects.Game
{
    // ゲームが始まったら最初に起動するスコープルート。ゲーム全体で共有する依存関係を登録する。
    public class GameScopeRoot : AbstractScopeRoot
    {
        protected override void ConfigureScope(IContainerBuilder builder)
        {
            base.ConfigureScope(builder);
            // ゲーム全体で共有する依存関係を登録する
            
            // まだインスタンスがない場合は、ここで生成される
            builder.RegisterEntryPoint<UpdateScheduler>();
        }
    }
}
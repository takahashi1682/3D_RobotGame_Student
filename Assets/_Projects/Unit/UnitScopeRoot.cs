using MyUtils.VContainerExtensions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Projects.Unit
{
    public class UnitScopeRoot : AbstractScopeRoot
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private GroundDetection _groundDetection;

        // このスコープでコンテナが作られる時に呼ばれるメソッド
        protected override void ConfigureScope(IContainerBuilder builder)
        {
            base.ConfigureScope(builder);
            // このオブジェクトにアタッチされているスクリプトにRigidbodyを注入するための登録
            builder.RegisterComponent(_rigidbody);
            // コンテナにGroundDetectionを登録する
            builder.RegisterComponent(_groundDetection);
        }
    }
}
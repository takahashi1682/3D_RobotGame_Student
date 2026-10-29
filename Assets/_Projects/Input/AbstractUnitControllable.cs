using MyUtils;
using MyUtils.VContainerExtensions;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Projects.Input
{
    // ユニットを操作するためのルールの抽象クラス（クラスのテンプレート）
    public abstract class AbstractUnitControllable : MonoBehaviour,
        IScopeRegisterable,
        IUnitControllable
    {
        [SerializeField, ReadOnly] protected SerializableReactiveProperty<Vector2> _move = new();
        [SerializeField, ReadOnly] protected SerializableReactiveProperty<Vector2> _look = new();
        [SerializeField, ReadOnly] protected SerializableReactiveProperty<bool> _fire = new();
        [SerializeField, ReadOnly] protected SerializableReactiveProperty<bool> _fly = new();
        [SerializeField, ReadOnly] protected SerializableReactiveProperty<bool> _boost = new();
        [SerializeField, ReadOnly] protected SerializableReactiveProperty<bool> _lockOn = new();

        // インターフェイスのプロパティ
        public Observable<Vector2> Move => _move;
        public Observable<Vector2> Look => _look;
        public abstract string LookDeviceName { get; }
        public Observable<bool> Fire => _fire;
        public Observable<bool> Fly => _fly;
        public Observable<bool> Boost => _boost;
        public Observable<bool> LockOn => _lockOn;

        // このクラスに実装されているインターフェイスをVContainerに登録するためのメソッド
        public void OnRegister(IContainerBuilder builder) => builder.RegisterComponent(this).As<IUnitControllable>();

        protected virtual void Awake()
        {
            _move.AddTo(this);
            _look.AddTo(this);
            _fire.AddTo(this);
            _fly.AddTo(this);
            _boost.AddTo(this);
            _lockOn.AddTo(this);
        }
    }
}
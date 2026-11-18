using VContainer;

namespace _Projects.Battle
{
    // ダメージを受ける側のルール（すべてのダメージを受けるオブジェクトが持つ情報）
    public interface IDamageable
    {
        public IObjectResolver Owner { get; } // ダメージを受ける本人
        public bool TakeDamage(IDamageSource source); // ダメージを受ける処理
    }
}
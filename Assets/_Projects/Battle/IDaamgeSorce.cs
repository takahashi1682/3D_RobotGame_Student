using VContainer;

namespace _Projects.Battle
{
    // ダメージ情報のルール（すべてのダメージが持つ情報）
    public interface IDamageSource
    {
        public IObjectResolver Owner { get; } // 攻撃した本人
        public int Damage { get; set; } // ダメージ量
    }
}
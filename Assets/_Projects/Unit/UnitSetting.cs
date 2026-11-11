using UnityEngine;

namespace _Projects.Unit
{
    [System.Serializable]
    public class UnitSetting // Unityの設定をまとめたクラス
    {
        [Header("Stats")]
        [field: SerializeField] public int MaxHealth { get; private set; } = 5000;
        [field: SerializeField] public int MaxEnergy { get; private set; } = 1000;

        [Header("References")]
        [field: SerializeField] public Transform UnitPivot { get; private set; } // キャラの中心
        [field: SerializeField] public Transform FirePoint { get; private set; } // 弾を撃つ位置

        [Header("Combat")]
        [field: SerializeField] public float RotationSpeed { get; private set; } = 400f; // 旋回速度
        [field: SerializeField] public float MaxLockOnDistance { get; private set; } = 500f; // ロックオン可能距離
        [field: SerializeField] public ArmyType Army { get; private set; } = ArmyType.Player; // 所属陣営

        [Tooltip("Targetとの間を遮る障害物とみなすレイヤー(視認判定に使用)")]
        [field: SerializeField] public LayerMask ObstacleLayerMask { get; private set; }

        /// <summary>
        /// 距離判定・ロックオンなどで使う、このUnitの基準座標。
        /// </summary>
        public Vector3 Pivot => UnitPivot.position;
    }
}
using MyUtils.VContainerExtensions;
using R3;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace _Projects.Game
{
    public class CinemachineManualUpdater : MonoBehaviour
    {
        [SerializeField] private CinemachineBrain _brain;
        [Inject] private IUpdateObservable _updateObservable;

        private void Start() =>
            _updateObservable.OnUpdate(EUpdatePhase.CameraApply)
                .Subscribe(_ => _brain.ManualUpdate())
                .AddTo(this);
    }
}
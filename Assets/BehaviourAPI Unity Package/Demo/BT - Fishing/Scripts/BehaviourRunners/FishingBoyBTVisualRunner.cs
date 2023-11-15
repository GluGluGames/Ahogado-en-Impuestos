using BehaviourAPI.Core;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class FishingBoyBTVisualRunner : EditorBehaviourRunner
    {
        [SerializeField] GameObject _fishPrefab, _bootPrefab;
        [SerializeField] Transform _fishDropTarget, _bootDropTarget, _baitTarget;
        [SerializeField] FishingRod _rod;
        bool _fishCatched;
        GameObject _currentCapture;

        float _time;
        float totalTimeToDrop = 2f;

        public void DropCaptureInWater() => DropCapture(_bootPrefab, _bootDropTarget, true);

        public void StoreCaptureInBasket() => DropCapture(_fishPrefab, _fishDropTarget, false);

        public void StartCatch()
        {
            _rod.PickUp();

            var catchId = Random.Range(0, 2);
            _fishCatched = catchId == 0;

            var prefab = catchId == 0 ? _fishPrefab : _bootPrefab;

            _currentCapture = Instantiate(prefab, _baitTarget);
            _currentCapture.GetComponent<Rigidbody>().useGravity = false;
        }

        public void StartThrow() => _rod.Throw();

        public bool IsFishCatched() => _fishCatched;

        public Status CompleteOnSuccess()
        {
            if (_time < totalTimeToDrop)
            {
                _time += Time.deltaTime;
                return Status.Running;
            }
            _time = 0f;
            return Status.Success;
        }

        public Status RodThrownStatus() => _rod.IsThrown() ? Status.Success : Status.Running;

        public Status RodPickedStatus() => _rod.IsPickedUp() ? Status.Success : Status.Running;

        void DropCapture(GameObject capturePrefab, Transform target, bool destroyAfter)
        {
            Destroy(_currentCapture);
            var drop = Instantiate(capturePrefab, target.position, target.rotation);
            drop.transform.localScale = target.localScale;
            if (destroyAfter) Destroy(drop, 2f);
            _time = 0f;
        }
    }

}
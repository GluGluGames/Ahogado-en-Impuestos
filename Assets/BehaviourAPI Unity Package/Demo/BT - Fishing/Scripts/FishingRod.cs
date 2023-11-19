using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class FishingRod : MonoBehaviour
    {
        Animator _animator;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Throw() => _animator.SetTrigger("throw");

        public void PickUp() => _animator.SetTrigger("pickup");

        public bool IsThrown() => _animator.GetCurrentAnimatorStateInfo(0).IsName("Throwing");

        public bool IsPickedUp() => _animator.GetCurrentAnimatorStateInfo(0).IsName("Picking up");
    }

}
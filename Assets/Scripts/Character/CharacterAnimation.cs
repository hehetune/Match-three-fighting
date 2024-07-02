using UnityEngine;

namespace Character
{
    public class CharacterAnimator : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int AttackAnimKey = Animator.StringToHash("Attack");
        private static readonly int DamageAnimKey = Animator.StringToHash("Damage");
        private static readonly int DeathAnimKey = Animator.StringToHash("Death");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void PlayAttackAnimation()
        {
            _animator.SetTrigger(AttackAnimKey);
        }

        public void PlayDamageAnimation()
        {
            _animator.SetTrigger(DamageAnimKey);
        }

        public void PlayDeathAnimation()
        {
            _animator.SetTrigger(DeathAnimKey);
        }

        // Add other animation methods as needed
    }
}
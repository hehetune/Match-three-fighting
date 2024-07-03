using UnityEngine;

namespace Character
{
    public class CharacterAnimator : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int IdleAnimKey = Animator.StringToHash("Idle");
        private static readonly int SlideAnimKey = Animator.StringToHash("Slide");
        private static readonly int NormalAttackAnimKey = Animator.StringToHash("NormalAttack");
        private static readonly int HurtAnimKey = Animator.StringToHash("Hurt");
        private static readonly int DieAnimKey = Animator.StringToHash("Die");

        public float normalAttackDelay;
        public float normalAttackDuration;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void PlayIdleAnimation()
        {
            _animator.SetTrigger(IdleAnimKey);
        }
        
        public void PlaySlideAnimation()
        {
            _animator.SetTrigger(SlideAnimKey);
        }

        public void PlayNormalAttackAnimation()
        {
            _animator.SetTrigger(NormalAttackAnimKey);
        }

        public void PlayHurtAnimation()
        {
            _animator.SetTrigger(HurtAnimKey);
        }

        public void PlayDieAnimation()
        {
            _animator.SetTrigger(DieAnimKey);
        }

    }
}
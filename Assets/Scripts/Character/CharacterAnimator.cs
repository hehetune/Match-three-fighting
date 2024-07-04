using System;
using JetBrains.Annotations;
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
        
        [SerializeField] private CustomAnimation SlideAnimation;
        [SerializeField] private CustomAnimation NormalAttackAnimation;
        [SerializeField] private CustomAnimation HurtAnimation;
        [SerializeField] private CustomAnimation DieAnimation;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void PlayIdleAnimation()
        {
            _animator.SetTrigger(IdleAnimKey);
        }
        
        public void PlaySlideAnimation(Action onTrigger,Action onComplete)
        {
            SlideAnimation.PlayAnimation(() => _animator.SetTrigger(SlideAnimKey), onTrigger, onComplete);
        }

        public void PlayNormalAttackAnimation(Action onTrigger,Action onComplete)
        {
            NormalAttackAnimation.PlayAnimation(() => _animator.SetTrigger(NormalAttackAnimKey), onTrigger, onComplete);
        }

        public void PlayHurtAnimation(Action onTrigger,Action onComplete)
        {
            NormalAttackAnimation.PlayAnimation(() => _animator.SetTrigger(HurtAnimKey), onTrigger, onComplete);
        }

        public void PlayDieAnimation(Action onTrigger,Action onComplete)
        {
            NormalAttackAnimation.PlayAnimation(() => _animator.SetTrigger(DieAnimKey), onTrigger, onComplete);
        }

    }
}
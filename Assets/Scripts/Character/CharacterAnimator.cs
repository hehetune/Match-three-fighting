using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Character
{
    public class CharacterAnimator : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int IdleAnimKey = Animator.StringToHash("Idle");
        // private static readonly int SlideAnimKey = Animator.StringToHash("Slide");
        // private static readonly int NormalAttackAnimKey = Animator.StringToHash("NormalAttack");
        // private static readonly int HurtAnimKey = Animator.StringToHash("Hurt");
        // private static readonly int DieAnimKey = Animator.StringToHash("Die");
        
        [SerializeField] private CustomAnimation SlideAnimation;
        [SerializeField] private CustomAnimation NormalAttackAnimation;
        [SerializeField] private CustomAnimation HurtAnimation;
        [SerializeField] private CustomAnimation DieAnimation;
        [SerializeField] private CustomAnimation HealAnimation;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetFaceDirection(bool facingRight)
        {
            _animator.SetFloat("FacingRight", facingRight ? 1 : 0);
        }

        public void PlayIdleAnimation()
        {
            _animator.SetTrigger(IdleAnimKey);
        }
        
        public void PlaySlideAnimation([CanBeNull] Action onTrigger,[CanBeNull] Action onComplete)
        {
            SlideAnimation.TriggerAnimation(_animator, onTrigger, onComplete);
        }

        public void PlayNormalAttackAnimation([CanBeNull] Action onTrigger,[CanBeNull] Action onComplete)
        {
            NormalAttackAnimation.TriggerAnimation(_animator, onTrigger, onComplete);
        }

        public void PlayHurtAnimation([CanBeNull] Action onTrigger,[CanBeNull] Action onComplete)
        {
            HurtAnimation.TriggerAnimation(_animator, onTrigger, onComplete);
        }

        public void PlayDieAnimation([CanBeNull] Action onTrigger,[CanBeNull] Action onComplete)
        {
            DieAnimation.TriggerAnimation(_animator, onTrigger, onComplete);
        }
        
        public void PlayHealAnimation([CanBeNull] Action onTrigger,[CanBeNull] Action onComplete)
        {
            HealAnimation.TriggerAnimation(_animator, onTrigger, onComplete);
        }

    }
}
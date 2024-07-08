using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Character
{
    [Serializable]
    public class CustomAnimation
    {
        public string name;
        public float triggerDelay = 0;
        public AnimationClip clip;

        public void TriggerAnimation(Animator animator, [CanBeNull] Action onTrigger, [CanBeNull] Action onComplete)
        {
            CoroutineRunner.Instance.RunCoroutine(TriggerCoroutine(animator, onTrigger, onComplete));
        }

        private IEnumerator TriggerCoroutine(Animator animator, [CanBeNull] Action onTrigger, [CanBeNull] Action onComplete)
        {
            animator.ResetTrigger(name);
            animator.SetTrigger(name);
            yield return triggerDelay.Wait();
            onTrigger?.Invoke();
            yield return (clip.length - triggerDelay).Wait();
            onComplete?.Invoke();
        }

        public float GetAnimationDuration() => clip.length;
    }
}
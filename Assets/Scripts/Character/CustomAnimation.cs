using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Character
{
    [Serializable]
    public class CustomAnimation
    {
        public float triggerDelay = 0;
        public AnimationClip clip;

        public void PlayAnimation(Action playAnim, [CanBeNull] Action onTrigger, [CanBeNull] Action onComplete)
        {
            CoroutineRunner.Instance.RunCoroutine(PlayCoroutine(playAnim, onTrigger, onComplete));
        }

        private IEnumerator PlayCoroutine(Action playAnim, [CanBeNull] Action onTrigger, [CanBeNull] Action onComplete)
        {
            playAnim?.Invoke();
            yield return triggerDelay.Wait();
            onTrigger?.Invoke();
            yield return (clip.length - triggerDelay).Wait();
            onComplete?.Invoke();
        }

        public float GetAnimationDuration() => clip.length;
    }
}
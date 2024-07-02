using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public abstract class Character : MonoBehaviour
    {
        public int Health { get; protected set; }
        public int AttackPower { get; protected set; }
        protected CharacterAnimator CharacterAnimator;

        public abstract void TakeTurn();
        public abstract void TakeDamage(int amount);
        public abstract void Attack(Character target);

        public void CalculateMatchResult(List<MatchResult> results)
        {
            foreach (var result in results)
            {
                
            }
        }

        protected void Die()
        {
            // Handle death logic
            CharacterAnimator.PlayDeathAnimation();
            Debug.Log($"{name} has died.");
        }
    }
}
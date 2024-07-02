using UnityEngine;

namespace Character
{
    public class Player : Character
    {
        private void Start()
        {
            Health = 100; // Example value
            AttackPower = 20; // Example value
        }

        public override void TakeTurn()
        {
            Debug.Log("Player's turn");
        }

        public override void TakeDamage(int amount)
        {
            Health -= amount;
            CharacterAnimator.PlayDamageAnimation();
            if (Health <= 0)
            {
                Die();
            }
        }

        public override void Attack(Character target)
        {
            CharacterAnimator.PlayAttackAnimation();
            target.TakeDamage(AttackPower);
        }
    }

    
}
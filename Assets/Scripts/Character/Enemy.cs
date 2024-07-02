using UnityEngine;

namespace Character
{
    public class Enemy : Character
    {
        private void Start()
        {
            Health = 80; // Example value
            AttackPower = 15; // Example value
        }

        public override void TakeTurn()
        {
            Debug.Log("Enemy's turn");

            // Example AI: Simple attack on player
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                Attack(player);
            }
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
using System;

namespace Character
{
    public class CharacterHealth
    {
        public delegate void OnStatusChange(float value); 
        public event OnStatusChange OnHealthChanged;
        public event EventHandler OnDead;

        private int healthMax;
        private int health;

        public CharacterHealth(int healthMax) {
            this.healthMax = healthMax;
            health = healthMax;
        }

        public void SetHealthAmount(int health) {
            this.health = health;
            if (OnHealthChanged != null) OnHealthChanged(GetHealthPercent());
        }

        public float GetHealthPercent() {
            return (float)health / healthMax;
        }

        public int GetHealthAmount() {
            return health;
        }

        public void Damage(int amount) {
            health -= amount;
            if (health < 0) {
                health = 0;
            }
            if (OnHealthChanged != null) OnHealthChanged(GetHealthPercent());

            if (health <= 0) {
                Die();
            }
        }

        public void Die() {
            if (OnDead != null) OnDead(this, EventArgs.Empty);
        }

        public bool IsDead() {
            return health <= 0;
        }

        public void Heal(int amount) {
            health += amount;
            if (health > healthMax) {
                health = healthMax;
            }
            if (OnHealthChanged != null) OnHealthChanged(GetHealthPercent());
        }
    }
}
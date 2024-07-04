using System;

namespace Character
{
    public class CharacterHealth : CharacterStatus
    {
        public event EventHandler OnDead;
        
        public CharacterHealth(int valueMax) : base(valueMax)
        {
        }

        public void Damage(int amount) {
            base.Subtract(amount);

            if (this.value <= 0) {
                Die();
            }
        }

        public void Die() {
            if (OnDead != null) OnDead(this, EventArgs.Empty);
        }

        public bool IsDead() {
            return value <= 0;
        }

        public void Heal(int amount) {
            base.Add(amount);
        }
    }
}
using System;

namespace Character
{
    public class CharacterEnergy : CharacterStatus
    {
        public event EventHandler OnDead;
        
        public CharacterEnergy(int valueMax) : base(valueMax)
        {
        }

        public override void Subtract(int amount) {
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
    }
}
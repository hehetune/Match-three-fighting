using System;

namespace Character
{
    public class CharacterStatus
    {
        public delegate void OnValueChange(float value); 
        public event OnValueChange OnValueChanged;

        protected int valueMax;
        protected int value;

        public CharacterStatus(int valueMax) {
            this.valueMax = valueMax;
            value = valueMax;
        }

        public virtual void SetValueAmount(int value) {
            this.value = value;
            if (OnValueChanged != null) OnValueChanged(GetValuePercent());
        }

        public virtual float GetValuePercent() {
            return (float)value / valueMax;
        }

        public virtual int GetValueAmount() {
            return value;
        }

        public virtual void Subtract(int amount) {
            value -= amount;
            if (value < 0) {
                value = 0;
            }
            if (OnValueChanged != null) OnValueChanged(GetValuePercent());
        }

        public virtual void Add(int amount) {
            value += amount;
            if (value > valueMax) {
                value = valueMax;
            }
            if (OnValueChanged != null) OnValueChanged(GetValuePercent());
        }
        
        protected void TriggerOnValueChanged() {
            if (OnValueChanged != null) OnValueChanged(GetValuePercent());
        }
    }
}
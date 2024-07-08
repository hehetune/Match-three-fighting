using System;
using UnityEngine;

namespace Character
{
    public class CharacterStatus
    {
        public delegate void OnValueChange(float value); 
        public event OnValueChange OnValueChanged;

        protected int valueMax;
        protected int value;

        protected bool defaultMax;

        public CharacterStatus(int valueMax, bool defaultMax = true)
        {
            this.defaultMax = defaultMax; 
            this.valueMax = valueMax;
            value = defaultMax ? valueMax : 0;
        }

        public virtual float GetValuePercent() {
            return (float)value / valueMax;
        }

        public virtual int GetValueAmount() {
            return value;
        }

        public virtual void Reset()
        {
            value = defaultMax ? valueMax : 0;
            OnValueChanged?.Invoke(GetValuePercent());
        }

        public virtual void Subtract(int amount) {
            value -= amount;
            if (value < 0) {
                value = 0;
            }
            OnValueChanged?.Invoke(GetValuePercent());
        }

        public virtual void Add(int amount) {
            value += amount;
            
            if (value > valueMax) {
                value = valueMax;
            }
            OnValueChanged?.Invoke(GetValuePercent());
        }
        
        protected void TriggerOnValueChanged() {
            OnValueChanged?.Invoke(GetValuePercent());
        }
    }
}
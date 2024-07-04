using System;

namespace Character
{
    public class CharacterMana : CharacterStatus
    {
        public bool CanUseSkill1 => value / valueMax >= 1 / 3;
        public bool CanUseSkill2 => value / valueMax >= 2 / 3;
        public bool CanUseSkill3 => value / valueMax == 1;

        public CharacterMana(int valueMax) : base(valueMax)
        {
        }

        public bool Consume(SkillType skillType)
        {
            int amount = 0;
            if (skillType == SkillType.Skill1) amount = valueMax / 3;
            if (skillType == SkillType.Skill2) amount = valueMax * 2 / 3;
            if (skillType == SkillType.Skill3) amount = valueMax;
            if (amount > value) return false;
            value -= amount;
            TriggerOnValueChanged();
            return true;
        }
    }
}
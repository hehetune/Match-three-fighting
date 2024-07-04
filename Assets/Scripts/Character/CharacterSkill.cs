using UnityEngine;

namespace Character
{
    public enum SkillName
    {
        Skill1,
        Skill2,
        Skill3,
    }

    public enum SkillType
    {
        Skill1 = 1,
        Skill2 = 2,
        Skill3 = 3,
    }

    [CreateAssetMenu(fileName = "New Character Skill", menuName = "Character/Skill")]
    public class CharacterSkill : ScriptableObject
    {
        public SkillName skillName;
        public SkillType skillType;

        public int damage;

        public void Execute()
        {
            Debug.Log("Execute skill " + skillName);
        }
    }
}
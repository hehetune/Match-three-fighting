using UnityEngine;

public class ResourceUtil : MonoBehaviour
{
    public static ResourceUtil Ins { get; private set; }
    
    public Sprite SwordSprite;
    public Sprite HpSprite;
    public Sprite ManaSprite;
    public Sprite EnergySprite;
    public Sprite GoldSprite;
    public Sprite ExpSprite;
    
    private void Awake()
    {
        if (Ins == null)
        {
            Ins = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public Sprite GetSpriteByDotType(DotType dotType)
    {
        switch (dotType)
        {
            case DotType.Sword: return SwordSprite;
            case DotType.Hp: return HpSprite;
            case DotType.Mana: return ManaSprite;
            case DotType.Energy: return EnergySprite;
            case DotType.Gold: return GoldSprite;
            case DotType.Exp: return ExpSprite;
            default: return SwordSprite;
        }
    }
}
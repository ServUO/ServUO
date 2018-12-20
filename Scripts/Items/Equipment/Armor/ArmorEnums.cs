using System;

namespace Server.Items
{
    public enum ArmorDurabilityLevel
    {
        Regular,
        Durable,
        Substantial,
        Massive,
        Fortified,
        Indestructible
    }

    public enum ArmorProtectionLevel
    {
        Regular,
        Defense,
        Guarding,
        Hardening,
        Fortification,
        Invulnerability,
    }

    public enum ArmorBodyType
    {
        Gorget,
        Gloves,
        Helmet,
        Arms,
        Legs, 
        Chest,
        Shield
    }

    public enum ArmorMaterialType
    {
        Cloth,
        Leather,
        Studded,
        Bone,
        Spined,
        Horned,
        Barbed,
        Ringmail,
        Chainmail,
        Plate,
        Dragon,
        Wood,
        Stone,
    }

    public enum ArmorMeditationAllowance
    {
        All,
        Half,
        None
    }
}
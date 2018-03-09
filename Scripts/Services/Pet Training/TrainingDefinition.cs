using System;
using Server;
using Server.Gumps;
using Server.Items;

namespace Server.Mobiles
{
    public class TrainingDefinition
    {
        public Type CreatureType { get; private set; }
        public Class Class { get; private set; }
        public MagicalAbility MagicalAbilities { get; private set; }
        public SpecialAbility[] SpecialAbilities { get; private set; }
        public WeaponAbility[] WeaponAbilities { get; private set; }
        public AreaEffect[] AreaEffects { get; private set; }

        public TrainingDefinition(
            Type type,
            Class classificaion,
            MagicalAbility magicalAbility,
            SpecialAbility[] specialAbility,
            WeaponAbility[] weaponAbility, 
            AreaEffect[] areaEffect)
        {
            CreatureType = type;
            Class = Class;
            MagicalAbilities = magicalAbility;
            SpecialAbilities = specialAbility;
            WeaponAbilities = weaponAbility;
            AreaEffects = areaEffect;
        }
    }
}

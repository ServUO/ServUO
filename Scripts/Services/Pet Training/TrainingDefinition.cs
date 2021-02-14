using Server.Items;
using System;
using System.Linq;

namespace Server.Mobiles
{
    public class TrainingDefinition
    {
        public Type CreatureType { get; }
        public Class Class { get; }
        public MagicalAbility MagicalAbilities { get; }
        public SpecialAbility[] SpecialAbilities { get; }
        public WeaponAbility[] WeaponAbilities { get; }
        public AreaEffect[] AreaEffects { get; }

        public int ControlSlotsMin { get; }
        public int ControlSlotsMax { get; }

        public TrainingDefinition(
            Type type,
            Class classificaion,
            MagicalAbility magicalAbility,
            SpecialAbility[] specialAbility,
            WeaponAbility[] weaponAbility,
            AreaEffect[] areaEffect,
            int controlmin,
            int controlmax)
        {
            CreatureType = type;
            Class = classificaion;
            MagicalAbilities = magicalAbility;
            SpecialAbilities = specialAbility;
            WeaponAbilities = weaponAbility;
            AreaEffects = areaEffect;

            ControlSlotsMin = controlmin;
            ControlSlotsMax = controlmax;
        }

        public bool HasSpecialAbility(SpecialAbility ability)
        {
            return SpecialAbilities != null && SpecialAbilities.Any(a => a == ability);
        }

        public bool HasAreaEffect(AreaEffect ability)
        {
            return AreaEffects != null && AreaEffects.Any(a => a == ability);
        }
    }
}

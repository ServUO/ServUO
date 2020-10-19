using Server.Items;
using System;

namespace Server.Mobiles
{
    public class RisingColossus : BaseCreature
    {
        private int m_DispelDifficulty;
        public override double DispelDifficulty => m_DispelDifficulty;
        public override double DispelFocus => 45.0;

        [Constructable]
        public RisingColossus(Mobile m, double baseskill, double boostskill)
            : base(AIType.AI_Mystic, FightMode.Closest, 10, 1, 0.4, 0.5)
        {
            int level = (int)(baseskill + boostskill);
            int statbonus = (int)((baseskill - 83) / 1.3 + ((boostskill - 30) / 1.3) + 6);
            int hitsbonus = (int)((baseskill - 83) * 1.14 + ((boostskill - 30) * 1.03) + 20);
            double skillvalue = boostskill != 0 ? ((baseskill + boostskill) / 2) : ((baseskill + 20) / 2);

            Name = "a rising colossus";
            Body = 829;

            SetHits(315 + hitsbonus);

            SetStr(677 + statbonus);
            SetDex(107 + statbonus);
            SetInt(127 + statbonus);

            SetDamage(level / 12, level / 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 70);
            SetResistance(ResistanceType.Fire, 50, 55);
            SetResistance(ResistanceType.Cold, 50, 55);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 65, 70);

            SetSkill(SkillName.MagicResist, skillvalue);
            SetSkill(SkillName.Tactics, skillvalue);
            SetSkill(SkillName.Wrestling, skillvalue);
            SetSkill(SkillName.Anatomy, skillvalue);
            SetSkill(SkillName.Mysticism, skillvalue);
            SetSkill(SkillName.DetectHidden, 70.0);
            SetSkill(SkillName.EvalInt, skillvalue);
            SetSkill(SkillName.Mysticism, m.Skills[SkillName.Mysticism].Value);
            SetSkill(SkillName.Focus, m.Skills[SkillName.Focus].Value);

            ControlSlots = 5;

            m_DispelDifficulty = 91 + (int)((baseskill * 83) / 5.2);

            SetWeaponAbility(WeaponAbility.ArmorIgnore);
            SetWeaponAbility(WeaponAbility.CrushingBlow);
        }

        public override double GetFightModeRanking(Mobile m, FightMode acqType, bool bPlayerOnly)
        {
            return (m.Int + m.Skills[SkillName.Magery].Value) / Math.Max(GetDistanceToSqrt(m), 1.0);
        }

        public override bool AlwaysMurderer => true;

        public override bool BleedImmune => true;

        public override Poison PoisonImmune => Poison.Lethal;

        public RisingColossus(Serial serial) : base(serial)
        {
        }

        public override int GetAttackSound()
        {
            return 0x627;
        }

        public override int GetHurtSound()
        {
            return 0x629;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_DispelDifficulty);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            m_DispelDifficulty = reader.ReadInt();
        }
    }
}

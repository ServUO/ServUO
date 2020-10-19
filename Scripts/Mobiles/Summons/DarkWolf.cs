using Server.Spells.Necromancy;
using System;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("a dark wolf corpse")]
    public class DarkWolfFamiliar : BaseFamiliar
    {
        private DateTime m_NextRestore;
        public DarkWolfFamiliar()
        {
            Name = "a dark wolf";
            Body = 99;
            Hue = 0x901;
            BaseSoundID = 0xE5;

            SetStr(100);
            SetDex(90);
            SetInt(90);

            SetHits(60);
            SetStam(90);
            SetMana(0);

            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 25, 40);
            SetResistance(ResistanceType.Cold, 25, 40);
            SetResistance(ResistanceType.Poison, 25, 40);
            SetResistance(ResistanceType.Energy, 25, 40);

            SetSkill(SkillName.Wrestling, 85.1, 90.0);
            SetSkill(SkillName.Tactics, 50.0);

            ControlSlots = 1;
        }

        public DarkWolfFamiliar(Serial serial)
            : base(serial)
        {
        }

        public static readonly Type[] ControlTypes =
        {
            typeof(DireWolf), typeof(GreyWolf), typeof(TimberWolf), typeof(WhiteWolf), typeof(BakeKitsune)
        };

        public static bool CheckMastery(Mobile tamer, BaseCreature creature)
        {
            BaseCreature familiar = (BaseCreature)SummonFamiliarSpell.Table[tamer];

            if (familiar != null && !familiar.Deleted && familiar is DarkWolfFamiliar && ControlTypes.Any(t => t == creature.GetType()))
            {
                return true;
            }

            return false;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (DateTime.UtcNow < m_NextRestore)
                return;

            m_NextRestore = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);

            Mobile caster = ControlMaster;

            if (caster == null)
                caster = SummonMaster;

            if (caster != null)
                ++caster.Stam;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}

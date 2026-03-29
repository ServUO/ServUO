using Server;
using System;

namespace Server.Items
{
    public class RingOfTheSoulbinder : SilverRing
    {
        public override int LabelNumber { get { return 1116620; } }

        [Constructable]
        public RingOfTheSoulbinder()
        {
            Hue = 288;
            Attributes.RegenMana = 2;
            Attributes.DefendChance = 15;
            Attributes.CastSpeed = 1;
            Attributes.CastRecovery = 3;
            Attributes.SpellDamage = 50;
            Attributes.LowerRegCost = 10;
            Attributes.EnhancePotions = 35;
            Attributes.BonusMana = 8;

            switch (Utility.Random(5))
            {
                case 0: SkillBonuses.SetValues(0, SkillName.MagicResist, 20.0); break;
                case 1: SkillBonuses.SetValues(0, SkillName.EvalInt, 20.0); break;
                case 2: SkillBonuses.SetValues(0, SkillName.SpiritSpeak, 20.0); break;
                case 3: SkillBonuses.SetValues(0, SkillName.Focus, 20.0); break;
                case 4: SkillBonuses.SetValues(0, SkillName.AnimalLore, 20.0); break;
            }
        }

        public RingOfTheSoulbinder(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
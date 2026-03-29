using Server;
using System;

namespace Server.Items
{
    public class EnchantedCoralBracelet : SilverBracelet
    {
        public override int LabelNumber { get { return 1116624; } }

        [Constructable]
        public EnchantedCoralBracelet()
        {
            Hue = 1548;
            Attributes.BonusHits = 5;
            Attributes.RegenMana = 3;
            Attributes.AttackChance = 15;
            Attributes.DefendChance = 15;
            Attributes.CastSpeed = 1;
            Attributes.CastRecovery = 3;
            Attributes.SpellDamage = 30;
            Attributes.BonusStam = 8;
            Attributes.BonusMana = 8;
            Attributes.EnhancePotions = 25;

            switch (Utility.Random(8))
            {
                case 0: SkillBonuses.SetValues(0, SkillName.Magery, 10.0); break;
                case 1: SkillBonuses.SetValues(0, SkillName.Mysticism, 10.0); break;
                case 2: SkillBonuses.SetValues(0, SkillName.Chivalry, 10.0); break;
                case 3: SkillBonuses.SetValues(0, SkillName.Bushido, 10.0); break;
                case 4: SkillBonuses.SetValues(0, SkillName.Ninjitsu, 10.0); break;
                case 5: SkillBonuses.SetValues(0, SkillName.Necromancy, 10.0); break;
                case 6: SkillBonuses.SetValues(0, SkillName.Spellweaving, 10.0); break;
                case 7: SkillBonuses.SetValues(0, SkillName.AnimalTaming, 10.0); break;
            }
        }

        public EnchantedCoralBracelet(Serial serial)
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
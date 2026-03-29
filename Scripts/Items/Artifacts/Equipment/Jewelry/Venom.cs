using System;

namespace Server.Items
{
    public class Venom : GoldBracelet
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1114783; } } // Venom
		
        [Constructable]
        public Venom()
        {
            Hue = 1371;
            Attributes.CastRecovery = 1;
            Attributes.CastSpeed = 2;
            Attributes.SpellDamage = 10;
            Attributes.EnhancePotions = 35;
            Attributes.DefendChance = 25;
            Attributes.AttackChance = 15;
            Resistances.Poison = 20;

            switch (Utility.Random(7))
            {
                case 0: SkillBonuses.SetValues(0, SkillName.Magery, 20.0); break;
                case 1: SkillBonuses.SetValues(0, SkillName.Chivalry, 20.0); break;
                case 2: SkillBonuses.SetValues(0, SkillName.Spellweaving, 20.0); break;
                case 3: SkillBonuses.SetValues(0, SkillName.Necromancy, 20.0); break;
                case 4: SkillBonuses.SetValues(0, SkillName.Mysticism, 20.0); break;
                case 5: SkillBonuses.SetValues(0, SkillName.Bushido, 20.0); break;
                case 6: SkillBonuses.SetValues(0, SkillName.Ninjitsu, 20.0); break;
            }
        }

        public Venom(Serial serial)
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
using System;

namespace Server.Items
{
    public class DreadPirateHat : TricorneHat
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1063467; } }
        public override int BaseColdResistance { get { return 14; } }
        public override int BasePoisonResistance { get { return 10; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
		
        [Constructable]
        public DreadPirateHat()
        {
            Hue = 0x497;
            SkillBonuses.SetValues(0, Utility.RandomCombatSkill(), 10.0);
            Attributes.BonusDex = 8;
            Attributes.AttackChance = 10;
            Attributes.NightSight = 1;
        }

        public DreadPirateHat(Serial serial)
            : base(serial)
        {
        }       
		
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)3);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
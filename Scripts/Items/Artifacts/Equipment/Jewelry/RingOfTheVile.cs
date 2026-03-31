using System;

namespace Server.Items
{
    public class RingOfTheVile : GoldRing
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RingOfTheVile()
        {
            Hue = 0x4F7;
            Attributes.BonusDex = 8;
            Attributes.RegenStam = 6;
            Attributes.AttackChance = 15;
            Attributes.DefendChance = 15;
            Attributes.EnhancePotions = 25;
            Attributes.LowerManaCost = 8;
            Resistances.Poison = 20;
            SkillBonuses.SetValues(0, SkillName.Healing, 10.0);
            SkillBonuses.SetValues(1, SkillName.Anatomy, 10.0);
        }

        public RingOfTheVile(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061102;
            }
        }// Ring of the Vile
        public override int ArtifactRarity
        {
            get
            {
                return 11;
            }
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
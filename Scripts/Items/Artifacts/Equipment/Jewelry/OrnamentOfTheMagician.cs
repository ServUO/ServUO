using System;

namespace Server.Items
{
    public class OrnamentOfTheMagician : GoldBracelet
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public OrnamentOfTheMagician()
        {
            Hue = 0x554;
            Attributes.CastRecovery = 3;
            Attributes.CastSpeed = 2;
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 20;
            Attributes.BonusMana = 10;
            Attributes.BonusStam = 10;
            Attributes.RegenHits = 2;
            Attributes.RegenMana = 2;
            Attributes.RegenStam = 2;
            Attributes.EnhancePotions = 15;
            Attributes.DefendChance = 15;
            SkillBonuses.SetValues(0, SkillName.MagicResist, 10.0);
            Resistances.Energy = 15;
        }

        public OrnamentOfTheMagician(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061105;
            }
        }// Ornament of the Magician
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
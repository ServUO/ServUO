using System;

namespace Server.Items
{
    public class AcidProofRobe : Robe
	{
		public override int LabelNumber { get { return 1095236; } }// Acid-Proof Robe [Replica]
        public override int BaseFireResistance { get { return 4; } }
        public override int BasePoisonResistance { get { return 20; } }
        public override int InitMinHits { get { return 150; } }
        public override int InitMaxHits { get { return 150; } }
        public override bool CanFortify { get {  return false; } }
		public override bool IsArtifact { get { return true; } }

        [Constructable]
        public AcidProofRobe()
        {
            Hue = 0x455;
            LootType = LootType.Blessed;
            SkillBonuses.SetValues(0, SkillName.Poisoning, 30.0);
            Attributes.BonusMana = 8;
            Attributes.BonusStam = 8;
            Attributes.DefendChance = 15;
            Attributes.EnhancePotions = 15;
            Attributes.Luck = 300;
        }

        public AcidProofRobe(Serial serial)
            : base(serial)
        {
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();           
        }
    }
}
using System;

namespace Server.Items
{
    public class CrystallineRing : GoldRing, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public CrystallineRing()
        {
            this.Hue = 0x480;
			
            this.Attributes.RegenHits = 5;
            this.Attributes.RegenMana = 3;
            this.Attributes.SpellDamage = 20;
			
            this.SkillBonuses.SetValues(0, SkillName.Magery, 20.0);
            this.SkillBonuses.SetValues(1, SkillName.Focus, 20.0);
        }

        public CrystallineRing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075096;
            }
        }// Crystalline Ring
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}
using System;

namespace Server.Items
{
    public class AlchemistsBauble : GoldBracelet
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public AlchemistsBauble()
        {
            this.Hue = 0x290;
            this.SkillBonuses.SetValues(0, SkillName.Magery, 10.0);
            this.Attributes.EnhancePotions = 30;
            this.Attributes.LowerRegCost = 20;
            this.Resistances.Poison = 10;
        }

        public AlchemistsBauble(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070638;
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
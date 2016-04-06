using System;

namespace Server.Items
{
    public class PendantOfTheMagi : GoldNecklace
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public PendantOfTheMagi()
        {
            this.Hue = 0x48D;
            this.Attributes.BonusInt = 10;
            this.Attributes.RegenMana = 3;
            this.Attributes.SpellDamage = 5;
            this.Attributes.LowerManaCost = 10;
            this.Attributes.LowerRegCost = 30;
        }

        public PendantOfTheMagi(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072937;
            }
        }// Pendant of the Magi
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
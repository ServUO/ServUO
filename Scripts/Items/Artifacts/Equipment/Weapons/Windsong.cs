using System;

namespace Server.Items
{
    public class Windsong : MagicalShortbow
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public Windsong()
            : base()
        {
            Hue = 172;			
            Attributes.WeaponDamage = 35;
            WeaponAttributes.SelfRepair = 3;			
            Velocity = 25;			
        }

        public Windsong(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075031;
            }
        }// Windsong
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
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
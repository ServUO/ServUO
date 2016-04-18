using System;

namespace Server.Items
{
    public class DiseasedMachete : ElvenMachete
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DiseasedMachete()
        {
            this.WeaponAttributes.HitPoisonArea = 25;
        }

        public DiseasedMachete(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073536;
            }
        }// Diseased Machete
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
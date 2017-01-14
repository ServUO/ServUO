using System;

namespace Server.Items
{
    public class MacheteOfDefense : ElvenMachete
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public MacheteOfDefense()
        {
            this.Attributes.DefendChance = 5;
        }

        public MacheteOfDefense(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073535;
            }
        }// machete of defense
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
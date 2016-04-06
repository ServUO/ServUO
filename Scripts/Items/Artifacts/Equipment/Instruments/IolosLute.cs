using System;

namespace Server.Items
{
    public class IolosLute : Lute
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public IolosLute()
        {
            this.Hue = 0x47E;
            this.Slayer = SlayerName.Silver;
            //Slayer2 = SlayerName.DaemonDismissal;
            this.Slayer2 = SlayerName.Exorcism;
        }

        public IolosLute(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063479;
            }
        }
        public override int InitMinUses
        {
            get
            {
                return 1600;
            }
        }
        public override int InitMaxUses
        {
            get
            {
                return 1600;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
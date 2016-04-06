using System;

namespace Server.Items
{
    public class GwennosHarp : LapHarp
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public GwennosHarp()
        {
            this.Hue = 0x47E;
            this.Slayer = SlayerName.Repond;
            this.Slayer2 = SlayerName.ReptilianDeath;
        }

        public GwennosHarp(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063480;
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
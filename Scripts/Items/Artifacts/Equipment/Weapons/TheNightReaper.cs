using System;

namespace Server.Items
{
    public class TheNightReaper : RepeatingCrossbow
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public TheNightReaper()
        {
            this.ItemID = 0x26CD;
            this.Hue = 0x41C;

            this.Slayer = SlayerName.Exorcism;
            this.Attributes.NightSight = 1;
            this.Attributes.WeaponSpeed = 25;
            this.Attributes.WeaponDamage = 55;
        }

        public TheNightReaper(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072912;
            }
        }// The Night Reaper
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
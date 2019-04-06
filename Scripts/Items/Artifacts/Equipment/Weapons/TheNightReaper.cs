using System;

namespace Server.Items
{
    public class TheNightReaper : RepeatingCrossbow
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public TheNightReaper()
        {
            Hue = 0x41C;
            Slayer = SlayerName.Exorcism;
            Attributes.NightSight = 1;
            Attributes.WeaponSpeed = 25;
            Attributes.WeaponDamage = 55;
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
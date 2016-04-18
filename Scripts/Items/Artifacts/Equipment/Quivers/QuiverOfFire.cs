using System;

namespace Server.Items
{
    public class QuiverOfFire : ElvenQuiver
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public QuiverOfFire()
            : base()
        {
            this.Hue = 0x4E7;
        }

        public QuiverOfFire(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073109;
            }
        }// quiver of fire
        public override void AlterBowDamage(ref int phys, ref int fire, ref int cold, ref int pois, ref int nrgy, ref int chaos, ref int direct)
        {
            cold = pois = nrgy = chaos = direct = 0;
            phys = fire = 50;
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
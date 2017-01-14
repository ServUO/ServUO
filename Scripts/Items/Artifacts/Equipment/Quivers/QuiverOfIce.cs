using System;

namespace Server.Items
{
    public class QuiverOfIce : ElvenQuiver
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public QuiverOfIce()
            : base()
        {
            this.Hue = 0x4ED;
        }

        public QuiverOfIce(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073110;
            }
        }// quiver of ice
        public override void AlterBowDamage(ref int phys, ref int fire, ref int cold, ref int pois, ref int nrgy, ref int chaos, ref int direct)
        {
            fire = pois = nrgy = chaos = direct = 0;
            phys = cold = 50;
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
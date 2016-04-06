using System;

namespace Server.Items
{
    public class QuiverOfRage : BaseQuiver
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public QuiverOfRage()
            : base()
        {
            this.Hue = 0x24C;

            this.WeightReduction = 25;
            this.DamageIncrease = 10;
        }

        public QuiverOfRage(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075038;
            }
        }// Quiver of Rage
        public override void AlterBowDamage(ref int phys, ref int fire, ref int cold, ref int pois, ref int nrgy, ref int chaos, ref int direct)
        {
            chaos = direct = 0;
            phys = fire = cold = pois = nrgy = 20;
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
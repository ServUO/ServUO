using System;

namespace Server.Items
{
    public class QuiverOfElements : BaseQuiver
    {
        [Constructable]
        public QuiverOfElements()
            : base()
        {
            this.Hue = 0xEB;
            this.WeightReduction = 50;
        }

        public QuiverOfElements(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075040;
            }
        }// Quiver of the Elements
        public override void AlterBowDamage(ref int phys, ref int fire, ref int cold, ref int pois, ref int nrgy, ref int chaos, ref int direct)
        {
            phys = fire = cold = pois = nrgy = direct = 0;
            chaos = 100;
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
using System;

namespace Server.Items
{
    public class KrampusMinionTalons : BaseShoes
    {
        public override int LabelNumber { get { return 1125644; } } // krampus minion talons

        [Constructable]
        public KrampusMinionTalons()
            : this(0)
        {
            Weight = 2.0;
        }

        [Constructable]
        public KrampusMinionTalons(int hue)
            : base(0xA294, 1153)
        {
        }

        public KrampusMinionTalons(Serial serial)
            : base(serial)
        {
        }

        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RegularLeather;
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

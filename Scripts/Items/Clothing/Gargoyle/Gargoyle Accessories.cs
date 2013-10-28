using System;

namespace Server.Items
{
    [FlipableAttribute(0x450D, 0x450D)]
    public class GargoyleTailMale : BaseWaist
    {
        [Constructable]
        public GargoyleTailMale()
            : this(0)
        {
        }

        [Constructable]
        public GargoyleTailMale(int hue)
            : base(0x450D, hue)
        {
            this.Weight = 2.0;
        }

        public GargoyleTailMale(Serial serial)
            : base(serial)
        {
        }

        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
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

    [FlipableAttribute(0x44C1, 0x44C2)]
    public class GargoyleTailFemale : BaseWaist
    {
        [Constructable]
        public GargoyleTailFemale()
            : this(0)
        {
        }

        [Constructable]
        public GargoyleTailFemale(int hue)
            : base(0x44C1, hue)
        {
            this.Weight = 2.0;
        }

        public GargoyleTailFemale(Serial serial)
            : base(serial)
        {
        }

        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
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
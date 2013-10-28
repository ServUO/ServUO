using System;

namespace Server.Items
{
    [FlipableAttribute(0x41D8, 0x41D9)]
    public class LeatherTalons : BaseShoes
    {
        [Constructable]
        public LeatherTalons()
            : this(0)
        {
        }

        [Constructable]
        public LeatherTalons(int hue)
            : base(0x41D8, hue)
        {
            this.Weight = 3.0;
        }

        public LeatherTalons(Serial serial)
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
using System;
using System.Text;

namespace Server.Items
{
    public class VendorTile : Item
    {
        [Constructable]
        public VendorTile()
            : base(0x3F08)
        {
            Name = "a vendor tile";
            Movable = false;
        }

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

        public override TimeSpan DecayTime
        {
            get
            {
                return TimeSpan.FromDays(10000);
            }
        }

        public VendorTile(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}

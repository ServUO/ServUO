using System;
using Server;

namespace Server.Items
{
    /* 
    FUN FACT: Although never officially confirmed, it is thought this item is named after a player, Admiral Ruffie,
	who long championed the idea of more sea-based content in Ultima Online, years before High Seas was released. 
    */
    public class AdmiralHeartyRum : BeverageBottle
    {
        [Constructable]
        public AdmiralHeartyRum()
            : base(BeverageType.Wine)
        {
            this.Hue = 0x66C;
            this.Name = "The Admiral's Hearty Rum";
        }

        public AdmiralHeartyRum(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight
        {
            get
            {
                return 1;
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

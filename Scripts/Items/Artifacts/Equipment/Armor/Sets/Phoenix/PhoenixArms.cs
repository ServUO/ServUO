using System;

namespace Server.Items
{
    public class PhoenixArms : RingmailArms
    {
        [Constructable]
        public PhoenixArms()
        {
            Hue = 0x8E;
			LootType = LootType.Blessed;     
        }

        public PhoenixArms(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041607;
            }
        }// ringmail sleeves of the phoenix
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
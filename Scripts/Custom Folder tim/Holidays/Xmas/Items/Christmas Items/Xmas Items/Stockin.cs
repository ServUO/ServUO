using System;
using Server;
using Server.Items;
using System.Collections;
using Server.Multis;
using Server.Network;

namespace Server.Items
{

    public class RedStockin : Bag
    {

        public override int MaxWeight { get { return 0; } }
        public override int DefaultDropSound { get { return 0x42; } }


        [Constructable]
        public RedStockin()
        {
            Movable = true;
            GumpID = 259;
            Weight = 1.0;
            ItemID = Utility.RandomList(0x2BDB, 0x2BDC); //Red Stockin
        }

        public RedStockin(Serial serial)
            : base(serial)
        {
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
//Green Stockin
        public class GreenStockin : Bag
        {

            public override int MaxWeight { get { return 0; } }
            public override int DefaultDropSound { get { return 0x42; } }


            [Constructable]
            public GreenStockin()
            {
                Movable = true;
                GumpID = 259;
                Weight = 1.0;
                ItemID = Utility.RandomList(0x2BD9, 0x2BDA); //Green Stockin
            }

            public GreenStockin(Serial serial)
                : base(serial)
            {
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
}



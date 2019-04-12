using System;

namespace Server.Items
{
    public class KrampusCoinPurse : Bag
    {
        public KrampusCoinPurse(Mobile m)
        {
            Name = "Krampus' Coin Purse"; // No Cliloc!

            for (int i = 0; i < 25; i++)
            {
                DropItemStacked(Loot.RandomGem());
            }

            DropItem(new Gold(Utility.RandomMinMax(2000, 5000)));

            if (.25 > Utility.RandomDouble())
            {
                if (Utility.RandomBool())
                {
                    DropItem(new DirtySnowballs());
                }
                else
                {
                    if (m.Karma > 0)
                    {
                        DropItem(new GoldBranch());
                    }
                    else
                    {
                        DropItem(new SilverBranch());
                    }
                }
            }
        }

        public KrampusCoinPurse(Serial serial)
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

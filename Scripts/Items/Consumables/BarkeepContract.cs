using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class BarkeepContract : Item
    {
        public override int LabelNumber => 1153779;  // a barkeep contract

        [Constructable]
        public BarkeepContract()
            : base(0x14F0)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public BarkeepContract(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                from.SendLocalizedMessage(503248); // Your godly powers allow you to place this vendor whereever you wish.

                Mobile v = new PlayerBarkeeper(from, BaseHouse.FindHouseAt(from))
                {
                    Direction = from.Direction & Direction.Mask
                };
                v.MoveToWorld(from.Location, from.Map);

                Delete();
            }
            else
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house == null || !house.IsOwner(from))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, false, "You are not the full owner of this house.");
                }
                else if (!house.CanPlaceNewBarkeep())
                {
                    from.SendLocalizedMessage(1062490); // That action would exceed the maximum number of barkeeps for this house.
                }
                else
                {
                    bool vendor, contract;
                    BaseHouse.IsThereVendor(from.Location, from.Map, out vendor, out contract);

                    if (vendor)
                    {
                        from.SendLocalizedMessage(1062677); // You cannot place a vendor or barkeep at this location.
                    }
                    else if (contract)
                    {
                        from.SendLocalizedMessage(1062678); // You cannot place a vendor or barkeep on top of a rental contract!
                    }
                    else
                    {
                        Mobile v = new PlayerBarkeeper(from, house)
                        {
                            Direction = from.Direction & Direction.Mask
                        };
                        v.MoveToWorld(from.Location, from.Map);

                        Delete();
                    }
                }
            }
        }
    }
}
using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public class ContractOfEmployment : Item
    {
		public override int LabelNumber => 1041243;// a contract of employment
		
        [Constructable]
        public ContractOfEmployment()
            : base(0x14F0)
        {
            Weight = 1.0;
        }

        public ContractOfEmployment(Serial serial)
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

                Mobile v = new PlayerVendor(from, BaseHouse.FindHouseAt(from))
                {
                    Direction = from.Direction & Direction.Mask
                };
                v.MoveToWorld(from.Location, from.Map);

                v.SayTo(from, 503246); // Ah! it feels good to be working again.

                EventSink.InvokePlacePlayerVendor(new PlacePlayerVendorEventArgs(from, v));

                Delete();
            }
            else
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house == null)
                {
                    from.SendLocalizedMessage(503240); // Vendors can only be placed in houses.	
                }
                else if (!house.IsOwner(from))
                {
                    from.SendLocalizedMessage(1062423); // Only the house owner can directly place vendors.  Please ask the house owner to offer you a vendor contract so that you may place a vendor in this house.
                }
                else if (!house.Public || !house.CanPlaceNewVendor())
                {
                    from.SendLocalizedMessage(503241); // You cannot place this vendor or barkeep.  Make sure the house is public and has sufficient storage available.
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
                        Mobile v = new PlayerVendor(from, house)
                        {
                            Direction = from.Direction & Direction.Mask
                        };
                        v.MoveToWorld(from.Location, from.Map);

                        v.SayTo(from, 503246); // Ah! it feels good to be working again.

                        EventSink.InvokePlacePlayerVendor(new PlacePlayerVendorEventArgs(from, v));

                        Delete();
                    }
                }
            }
        }
    }
}

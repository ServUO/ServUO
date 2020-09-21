using Server.Multis;

namespace Server.Items
{
    [Flipable(0xA12E, 0xA12F)]
    public class HolidayWreath : Item, IDyable
    {
        public override int LabelNumber => 1029004;  // wreath
        public override bool IsArtifact => true;

        public int MadeID { get; set; }

        [Constructable]
        public HolidayWreath()
            : base(0xA12E)
        {
            MadeID = Utility.Random(1114138, 13);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1158828, string.Format("#{0}", MadeID)); // Made From Handpicked Trees Near ~1_WHERE~
        }

        public HolidayWreath(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(MadeID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            MadeID = reader.ReadInt();
        }

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsCoOwner(from))
            {
                if (from.InRange(GetWorldLocation(), 1))
                {
                    Hue = sender.DyedHue;
                    return true;
                }
                else
                {
                    from.SendLocalizedMessage(500295); // You are too far away to do that.
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}

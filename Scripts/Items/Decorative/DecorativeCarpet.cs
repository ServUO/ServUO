using Server.Multis;
using Server.Regions;

namespace Server.Items
{
    public class DecorativeCarpet : Item, IDyable
    {
        public override int Hue
        {
            get
            {
                if (Movable && IsInsideHouse())
                    return 253;

                return base.Hue;
            }
        }

        [Constructable]
        public DecorativeCarpet(int itemId)
            : base(itemId)
        {
            Weight = 1.0;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Movable && IsInsideHouse())
                list.Add(1113267); // (Double Click to Lockdown)
        }

        public override bool DisplayWeight => Movable;

        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            if (!base.OnDroppedToWorld(from, p))
                return false;

            InvalidateProperties();
            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            HouseRegion region = Region.Find(Location, Map) as HouseRegion;

            if (region != null)
            {
                BaseHouse house = region.House;

                if (house.IsOwner(from) || house.IsCoOwner(from))
                {
                    Movable = !Movable;

                    if (Movable)
                        house.Carpets.Remove(this);
                    else
                        house.Carpets.Add(this);

                    InvalidateProperties();
                }
                else
                {
                    // Only the home owner may lock this down.
                    from.SendLocalizedMessage(1113268);
                }
            }
        }

        public bool IsInsideHouse()
        {
            return Region.Find(Location, Map).IsPartOf<HouseRegion>();
        }

        public DecorativeCarpet(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            /*int version = */
            reader.ReadInt();
        }

        #region IDyable Members
        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
            {
                return false;
            }

            Hue = sender.DyedHue;

            return true;
        }
        #endregion
    }
}
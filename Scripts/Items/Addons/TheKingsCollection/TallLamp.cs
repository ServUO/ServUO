using Server.Multis;
using Server.Targeting;
using System;

namespace Server.Items
{
    public class TallLamp : BaseLight, IFlipable, IAddon
    {
        public override int LabelNumber => 1098410;  // tall lamp

        public override int LitItemID => ItemID == 0x4C52 ? 0x4C53 : 0x4C55;
        public override int UnlitItemID => ItemID == 0x4C53 ? 0x4C52 : 0x4C54;

        public int NorthID => Burning ? 0x4C53 : 0x4C52;
        public int WestID => Burning ? 0x4C55 : 0x4C54;

        [Constructable]
        public TallLamp()
            : base(0x4C52)
        {
            Movable = false;
            Duration = TimeSpan.Zero;
            Burning = false;
            Light = LightType.Circle300;
            Weight = 0.0;
        }

        public Item Deed => new TallLampDeed();

        public bool CouldFit(IPoint3D p, Map map)
        {
            return map.CanFit((Point3D)p, 20);
        }

        void IChopable.OnChop(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && (house.IsOwner(from) || (house.Addons.ContainsKey(this) && house.Addons[this] == from)))
            {
                Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.

                Delete();

                house.Addons.Remove(this);

                from.AddToBackpack(Deed);
            }
            else
            {
                from.SendLocalizedMessage(1113134); // You can only redeed items in your own house!
            }
        }

        public void OnFlip(Mobile from)
        {
            if (ItemID == NorthID)
                ItemID = WestID;
            else if (ItemID == WestID)
                ItemID = NorthID;
        }

        public TallLamp(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class TallLampDeed : Item
    {
        public override int LabelNumber => 1098410;  // tall lamp

        [Constructable]
        public TallLampDeed()
            : base(0x14F0)
        {
            LootType = LootType.Blessed;
        }

        public TallLampDeed(Serial serial)
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
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && house.IsCoOwner(from))
            {
                from.BeginTarget(-1, true, TargetFlags.None, Placement_OnTarget, null);
            }
            else
            {
                from.SendLocalizedMessage(502092); // You must be in your house to do this.
            }
        }

        public void Placement_OnTarget(Mobile from, object targeted, object state)
        {
            IPoint3D p = targeted as IPoint3D;
            Map map = from.Map;

            if (p == null || map == null || Deleted)
                return;

            if (IsChildOf(from.Backpack))
            {
                Point3D loc = new Point3D(p);
                BaseHouse house = BaseHouse.FindHouseAt(loc, from.Map, 16);

                if (house != null && house.IsCoOwner(from))
                {
                    Item addon = new TallLamp();

                    addon.MoveToWorld(loc, from.Map);

                    house.Addons[addon] = from;
                    Delete();
                }
                else
                {
                    from.SendLocalizedMessage(1042036); // That location is not in your house.
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }
    }
}

using Server;
using System;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public enum CannonType
    {
        Light, 
        Heavy,
        //Large
    }

    public class ShipCannonDeed : Item
    {
        public virtual CannonType CannonType { get { return CannonType.Light; } }

        public ShipCannonDeed() : base(5362)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                BaseBoat boat = BaseBoat.FindBoatAt(from, from.Map);

                if (boat != null && boat is BaseGalleon)
                {
                    if(((BaseGalleon)boat).Cannons.Count >= ((BaseGalleon)boat).MaxCannons)
                        from.SendMessage("You have already deployed the maximum amount of cannons for this galleon!");
                    else if (((BaseGalleon)boat).GetSecurityLevel(from) >= SecurityLevel.Officer)
                        from.Target = new InternalTarget(this, boat);
                    else
                        from.SendMessage("You must be at least an officer of this vessle to place a cannon.");
                }
                else
                    from.SendLocalizedMessage(1116625); //You must be on the ship to deploy a weapon.
            }
        }

        private class InternalTarget : Target
        {
            private ShipCannonDeed m_Deed;
            private BaseBoat m_Boat;

            public InternalTarget(ShipCannonDeed deed, BaseBoat boat) : base(3, false, TargetFlags.None)
            {
                m_Deed = deed;
                m_Boat = boat;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Map map = from.Map;

                if (targeted is IPoint3D)
                {
                    Point3D pnt = new Point3D((IPoint3D)targeted);

                    BaseBoat boat = BaseBoat.FindBoatAt(new Point2D(pnt.X, pnt.Y), map);

                    if (boat != null && boat is BaseGalleon && ((BaseGalleon)boat).GetSecurityLevel(from) >= SecurityLevel.Officer && m_Boat == boat)
                        ((BaseGalleon)boat).TryAddCannon(from, pnt, m_Deed);
                }
            }
        }

        public ShipCannonDeed(Serial serial) : base(serial) { }

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

    public class LightShipCannonDeed : ShipCannonDeed
    {
        public override CannonType CannonType { get { return CannonType.Light; } }
        public override int LabelNumber { get { return 1095793; } }

        [Constructable]
        public LightShipCannonDeed()
        {
            Hue = 1142;
        }

        public LightShipCannonDeed(Serial serial) : base(serial) { }

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

    public class HeavyShipCannonDeed : ShipCannonDeed
    {
        public override CannonType CannonType { get { return CannonType.Heavy; } }
        public override int LabelNumber { get { return 1095794; } }

        [Constructable]
        public HeavyShipCannonDeed()
        {
            Hue = 1146;
        }

        public HeavyShipCannonDeed(Serial serial) : base(serial) { }

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
using Server.ContextMenus;
using Server.Mobiles;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Items
{
    public class EndlessDecanter : Pitcher
    {
        private bool m_Linked = false;
        private Point3D m_LinkLocation;
        private Map m_LinkMap;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Linked { get { return m_Linked; } set { m_Linked = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D LinkLocation { get { return m_LinkLocation; } set { m_LinkLocation = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map LinkMap { get { return m_LinkMap; } set { m_LinkMap = value; } }

        public override int LabelNumber => 1115929; // Endless Decanter of Water

        public override int ComputeItemID()
        {
            return 0x0FF6;
        }

        [Constructable]
        public EndlessDecanter() : base(BeverageType.Water)
        {
            Weight = 2.0;
            Hue = 0x399;
        }

        public EndlessDecanter(Serial serial) : base(serial)
        {
        }

        public static void HandleThrow(BaseBeverage beverage, WaterElemental elemental, Mobile thrower)
        {
            if (!beverage.IsFull)
            {
                thrower.SendLocalizedMessage(1113038);  // It is not full. 
            }
            else if (!thrower.InRange(elemental.Location, 5))
            {
                thrower.SendLocalizedMessage(500295);   // You are too far away to do that.
            }
            else if (!elemental.HasDecanter)
            {
                thrower.SendLocalizedMessage(1115895);  // It seems that this water elemental no longer has a magical decanter...
            }
            else if (0.1 > Utility.RandomDouble())
            {
                thrower.RevealingAction();
                elemental.Damage(1, thrower);

                elemental.HasDecanter = false;
                beverage.Delete();
                thrower.AddToBackpack(new EndlessDecanter());
                thrower.SendLocalizedMessage(1115897);  // The water elemental has thrown a magical decanter back to you!
            }
            else
            {
                thrower.RevealingAction();
                elemental.Damage(1, thrower);

                beverage.Delete();
                thrower.PlaySound(0x040);
                thrower.SendLocalizedMessage(1115896);  // The water pitcher has shattered.
            }
        }

        public override void QuantityChanged()
        {
            if (Content == BeverageType.Water && Quantity == 0 && m_Linked && RootParent is Mobile)
            {
                if (((Mobile)RootParent).InRange(m_LinkLocation, 10) && ((Mobile)RootParent).Map == m_LinkMap)
                {
                    Quantity = MaxQuantity;

                    ((Mobile)RootParent).SendLocalizedMessage(1115901);  // The decanter has automatically been filled from the linked water trough.
                    ((Mobile)RootParent).PlaySound(0x4E);
                }
                else
                {
                    ((Mobile)RootParent).SendLocalizedMessage(1115972);  // The decanter’s refill attempt failed because the linked water trough is not in the area.
                }
            }
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1115889);  // Auto Water Refill

            if (m_Linked)
                list.Add(1115893);  // Linked
            else
                list.Add(1115894);  // Unlinked
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive)
            {
                list.Add(new LinkEntry(from, this));

                if (m_Linked)
                    list.Add(new UnlinkEntry(from, this));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_Linked);
            writer.Write(m_LinkLocation);
            writer.Write(m_LinkMap);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    m_Linked = reader.ReadBool();
                    m_LinkLocation = reader.ReadPoint3D();
                    m_LinkMap = reader.ReadMap();

                    break;
            }
        }

        private class LinkEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly EndlessDecanter m_Decanter;

            public LinkEntry(Mobile from, EndlessDecanter decanter) : base(1115891, 0) // Link
            {
                m_From = from;
                m_Decanter = decanter;
            }

            public override void OnClick()
            {
                if (m_Decanter.Deleted || !m_Decanter.Movable || !m_From.CheckAlive() || !m_Decanter.CheckItemUse(m_From))
                    return;

                m_From.SendLocalizedMessage(1115892);   // Target a water trough you wish to link.

                m_From.BeginTarget(10, false, TargetFlags.None, Link_OnTarget);
            }

            private void Link_OnTarget(Mobile from, object targ)
            {
                int itemID = 0;
                Point3D location = new Point3D();
                Map map = Map.Felucca;

                if (targ is StaticTarget)
                {
                    itemID = ((StaticTarget)targ).ItemID;
                    location = ((StaticTarget)targ).Location;
                    map = from.Map;
                }
                else if (targ is Item)
                {
                    itemID = ((Item)targ).ItemID;
                    location = ((Item)targ).Location;
                    map = ((Item)targ).Map;
                }

                if (itemID >= 0xB41 && itemID <= 0xB44)
                {
                    m_Decanter.Linked = true;
                    m_Decanter.LinkLocation = location;
                    m_Decanter.LinkMap = map;

                    from.SendLocalizedMessage(1115899); // That water trough has been linked to this decanter.

                    if (m_Decanter.Quantity == 0 && m_Decanter.Content == BeverageType.Water)
                    {
                        m_Decanter.QuantityChanged();
                        m_Decanter.InvalidateProperties();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1115900); // Invalid target. Please target a water trough.
                }
            }
        }

        private class UnlinkEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly EndlessDecanter m_Decanter;

            public UnlinkEntry(Mobile from, EndlessDecanter decanter) : base(1115930, 0) // Unlink
            {
                m_From = from;
                m_Decanter = decanter;
            }

            public override void OnClick()
            {
                if (m_Decanter.Deleted || !m_Decanter.Movable || !m_From.CheckAlive() || !m_Decanter.CheckItemUse(m_From))
                    return;

                m_From.SendLocalizedMessage(1115898);   // The link between this decanter and the water trough has been removed.
                m_Decanter.Linked = false;
            }
        }
    }
}

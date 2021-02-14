using Server.ContextMenus;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Items
{
    public class GalleonContainer : WoodenChest, IChopable
    {
        private BaseGalleon m_Galleon;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon => m_Galleon;

        public override bool IsDecoContainer => false;

        public override string DefaultName => "Ship Container";

        public override int DefaultMaxWeight => 1250;

        public GalleonContainer(BaseGalleon galleon)
        {
            m_Galleon = galleon;
            Movable = false;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            list.Add(new RelocateContainerEntry(this, m_Galleon));
        }

        private class RelocateContainerEntry : ContextMenuEntry
        {
            private readonly GalleonContainer m_Container;
            private readonly BaseGalleon m_Galleon;

            public RelocateContainerEntry(GalleonContainer container, BaseGalleon galleon)
                : base(1061829, 3)
            {
                m_Galleon = galleon;
                m_Container = container;
            }

            public override void OnClick()
            {
                Owner.From.Target = new RelocateTarget(m_Container, m_Galleon);
                Owner.From.SendMessage("Where do you wish to relocate the ship container?");
            }
        }

        private class RelocateTarget : Target
        {
            private readonly GalleonContainer m_Container;
            private readonly BaseGalleon m_Galleon;

            public RelocateTarget(GalleonContainer container, BaseGalleon galleon)
                : base(12, false, TargetFlags.None)
            {
                m_Container = container;
                m_Galleon = galleon;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is IPoint3D point3D && from.Map != null)
                {
                    BaseBoat boat = BaseBoat.FindBoatAt(point3D, from.Map);

                    if (boat != null && boat == m_Galleon && IsSurface(point3D))
                    {
                        IPooledEnumerable eable = from.Map.GetObjectsInRange(new Point3D(point3D), 0);

                        foreach (object o in eable)
                        {
                            if (o is Mobile || o is Item)
                            {
                                from.SendMessage("You cannot place the ship container there, try again.");
                                from.Target = new RelocateTarget(m_Container, m_Galleon);
                                eable.Free();
                                return;
                            }
                        }
                        eable.Free();

                        StaticTarget st = (StaticTarget)point3D;
                        int z;

                        z = st.Z;

                        m_Container.MoveToWorld(new Point3D(point3D.X, point3D.Y, z), from.Map);
                    }
                    else
                    {
                        from.SendMessage("You cannot place the ship container there, try again.");
                        from.Target = new RelocateTarget(m_Container, m_Galleon);
                    }
                }
            }

            public bool IsSurface(IPoint3D pnt)
            {
                if (pnt is StaticTarget st && (st.Flags & TileFlag.Surface) > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Galleon == null || from.AccessLevel > AccessLevel.Player)
                base.OnDoubleClick(from);
            else if (!m_Galleon.Contains(from))
            {
                if (m_Galleon.TillerMan != null)
                    m_Galleon.TillerManSay("You must be on the ship to open the container.");
            }
            else if (m_Galleon.Owner is PlayerMobile && !m_Galleon.Scuttled && m_Galleon.GetSecurityLevel(from) < SecurityLevel.Crewman)
                from.SendMessage("You must be at least a crewman to access the ship container.");
            else
                base.OnDoubleClick(from);
        }

        public void OnChop(Mobile from)
        {
            if (m_Galleon != null && m_Galleon.Owner == from)
            {
                Effects.PlaySound(Location, Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.
                Destroy();
            }
        }

        public GalleonContainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_Galleon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            m_Galleon = reader.ReadItem() as BaseGalleon;
        }
    }
}

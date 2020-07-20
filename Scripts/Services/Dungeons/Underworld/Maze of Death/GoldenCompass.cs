using Server.Gumps;
using Server.Network;
using Server.Regions;

namespace Server.Items
{
    public class GoldenCompass : BaseDecayingItem
    {
        private int m_Span;

        public override int Lifespan => m_Span;
        public override bool UseSeconds => false;

        public override int LabelNumber => 1113578;  // a golden compass

        [Constructable]
        public GoldenCompass()
            : base(0x1CB)
        {
            Weight = 1;
            Hue = 1177;
            m_Span = 0;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (from.Region != null && from.Region.IsPartOf<MazeOfDeathRegion>())
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1113585); // The compass' arrows flicker. You must be near the right location.
                }
                else
                {
                    from.SendLocalizedMessage(1155663); // Nothing happens.
                }
            }
            else if (RootParent == null && !Movable && !IsLockedDown && !IsSecure)
            {
                if (from.InRange(GetWorldLocation(), 3))
                {
                    if (from.Backpack != null && m_Span == 0)
                    {
                        if (from.Backpack.FindItemByType(typeof(GoldenCompass)) == null)
                        {
                            GoldenCompass gc = new GoldenCompass();

                            if (from.PlaceInBackpack(gc))
                            {
                                gc.StartTimer();
                                from.Backpack.DropItem(gc);
                                from.SendLocalizedMessage(1072223); // An item has been placed in your backpack.
                                gc.SendTimeRemainingMessage(from);
                            }
                            else
                            {
                                gc.Delete();
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(501885); // You already own one of those!
                        }
                    }
                }
                else
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                }
            }
        }

        public void StartTimer()
        {
            TimeLeft = 7200;
            m_Span = 7200;
            Movable = true;
            InvalidateProperties();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            Mobile m = RootParent as Mobile;

            if (m != null)
                m.CloseGump(typeof(CompassDirectionGump));
        }

        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            from.SendLocalizedMessage(1076256); // That item cannot be traded.

            return false;
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            from.SendLocalizedMessage(1076254); // That item cannot be dropped.

            return false;
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            from.SendLocalizedMessage(1076254); // That item cannot be dropped.

            return false;
        }

        public GoldenCompass(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_Span);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            m_Span = reader.ReadInt();

            if (m_Span > 0)
            {
                StartTimer();
            }
        }
    }
}

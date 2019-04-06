using Server;
using System;
using Server.Gumps;
using Server.Regions;
using Server.Mobiles;

namespace Server.Items
{
	public class GoldenCompass : BaseDecayingItem
	{
		private int m_Span;
		
		public override int Lifespan { get { return m_Span; } }
        public override int LabelNumber { get { return 1113578; } } // a golden compass
		
		[Constructable]
		public GoldenCompass() : base(459)
		{
            Weight = 1;
			Hue = 0x501;
			m_Span = 0;
            Movable = false;
		}

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) && from.Region != null && from.Region.IsPartOf<MazeOfDeathRegion>())
            {
                from.CloseGump(typeof(CompassDirectionGump));
                from.SendGump(new CompassDirectionGump(from));
            }
            else if (RootParent == null && from.InRange(GetWorldLocation(), 3) && !Movable && !IsLockedDown && !IsSecure)
            {
                if (from.Backpack != null && m_Span == 0 && from.Backpack.FindItemByType(typeof(GoldenCompass)) == null)
                {
                    GoldenCompass c = new GoldenCompass();
                    c.StartTimer();
                    from.Backpack.DropItem(c);
                    from.SendLocalizedMessage(1113584); // Please return what you borrow!
                }
            }
        }

        public override void StartTimer()
        {
            TimeLeft = 1800;
            m_Span = 1800;
            Movable = true;
            base.StartTimer();
            InvalidateProperties();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            Mobile m = this.RootParent as Mobile;

            if (m != null)
                m.CloseGump(typeof(Server.Gumps.CompassDirectionGump));
        }
		
		public GoldenCompass(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
			writer.Write(m_Span);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
			m_Span = reader.ReadInt();

            if(m_Span > 0)
            {
                StartTimer();
            }
		}
	}
}
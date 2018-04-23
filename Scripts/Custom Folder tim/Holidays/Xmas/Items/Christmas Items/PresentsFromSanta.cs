using System;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
	[Flipable( 0x232A, 0x232B )]
	public class PresentsFromSanta : GiftBox
	{
		private Mobile m_Owner;
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; InvalidateProperties(); }
        }
		public override int DefaultGumpID { get { return 0x777A; } }
      
		
		[Constructable]
		public PresentsFromSanta()
		{
			Name = "A Gift from Santa";

		}
		public override void OnDoubleClick(Mobile from)
        {
        	if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (Owner == null && IsChildOf(from.Backpack))
            {
                Owner = from;
                base.OnDoubleClick(from);
            }
            
            else if (Owner == from || from.AccessLevel > Owner.AccessLevel)
                base.OnDoubleClick(from);
            else
                from.SendMessage("This is not your gift.");
        }
		public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1072304, Owner == null ? "nobody" : Owner.Name);

        }

		public PresentsFromSanta( Serial serial ) : base( serial )
		{
		}
	

		public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
            writer.Write(m_Owner != null);
            if (m_Owner != null) writer.Write(m_Owner);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
            if (reader.ReadBool()) m_Owner = reader.ReadMobile();

        }
	}
}

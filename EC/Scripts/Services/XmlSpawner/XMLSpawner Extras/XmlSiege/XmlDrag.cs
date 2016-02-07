using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlDrag : XmlAttachment
	{
		private Mobile m_DraggedBy = null;    // mobile doing the dragging
		private Point3D m_currentloc = Point3D.Zero;
		private Map m_currentmap = null;
		private int m_Distance = -3;

		private InternalTimer m_Timer;

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile DraggedBy { get { return m_DraggedBy; } set { m_DraggedBy = value; if (m_DraggedBy != null) { DoTimer(); } } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Distance { get { return m_Distance; } set { m_Distance = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D CurrentLoc { get { return m_currentloc; } set { m_currentloc = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Map CurrentMap { get { return m_currentmap; } set { m_currentmap = value; } }


		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlDrag(ASerial serial)
			: base(serial)
		{
		}

		[Attachable]
		public XmlDrag()
		{
		}

		[Attachable]
		public XmlDrag(Mobile draggedby)
		{
			DraggedBy = draggedby;
		}

		[Attachable]
		public XmlDrag(string name, Mobile draggedby)
		{
			Name = name;
			DraggedBy = draggedby;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0);
			// version 0
			writer.Write(m_DraggedBy);
			writer.Write(m_Distance);

		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			// version 0
			m_DraggedBy = reader.ReadMobile();
			m_Distance = reader.ReadInt();
		}

		public override void OnAttach()
		{
			base.OnAttach();

			if (AttachedTo is Mobile || AttachedTo is Item)
			{
				DoTimer();
			}
			else
				Delete();
		}

		public override void OnReattach()
		{
			base.OnReattach();

			DoTimer();
		}

		public override void OnDelete()
		{
			base.OnDelete();

			if (m_Timer != null)
				m_Timer.Stop();
		}

		public void DoTimer()
		{
			if (m_Timer != null)
				m_Timer.Stop();

			m_Timer = new InternalTimer(this);
			m_Timer.Start();
		}

		// added the duration timer that begins on spawning
		private class InternalTimer : Timer
		{
			private XmlDrag m_attachment;

			public InternalTimer(XmlDrag attachment)
				: base(TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500))
			{
				Priority = TimerPriority.FiftyMS;
				m_attachment = attachment;
			}

			protected override void OnTick()
			{
				if (m_attachment == null) return;

				Mobile draggedby = m_attachment.DraggedBy;
				object parent = m_attachment.AttachedTo;

				if (parent == null || !(parent is Mobile || parent is Item) || draggedby == null || draggedby.Deleted || draggedby == parent)
				{
					Stop();
					return;
				}

				// get the location of the mobile dragging

				Point3D newloc = draggedby.Location;
				Map newmap = draggedby.Map;

				if (newmap == null || newmap == Map.Internal)
				{
					// if the mobile dragging has an invalid map, then disconnect
					m_attachment.DraggedBy = null;
					Stop();
					return;
				}

				// update the location of the dragged object if the parent has moved
				if (newloc != m_attachment.CurrentLoc || newmap != m_attachment.CurrentMap)
				{
					m_attachment.CurrentLoc = newloc;
					m_attachment.CurrentMap = newmap;

					int x = newloc.X;
					int y = newloc.Y;
					int lag = m_attachment.Distance;
					// compute the new location for the dragged object
					switch (draggedby.Direction & Direction.Mask)
					{
						case Direction.North: y -= lag; break;
						case Direction.Right: x += lag; y -= lag; break;
						case Direction.East: x += lag; break;
						case Direction.Down: x += lag; y += lag; break;
						case Direction.South: y += lag; break;
						case Direction.Left: x -= lag; y += lag; break;
						case Direction.West: x -= lag; break;
						case Direction.Up: x -= lag; y -= lag; break;
					}

					if (parent is Mobile)
					{
						((Mobile)parent).Location = new Point3D(x, y, newloc.Z);
						((Mobile)parent).Map = newmap;
					}
					else
						if (parent is Item)
						{
							((Item)parent).Location = new Point3D(x, y, newloc.Z);
							((Item)parent).Map = newmap;
						}
				}


			}
		}

		public override string OnIdentify(Mobile from)
		{
			if (from == null || from.AccessLevel == AccessLevel.Player) return null;

			if (Expiration > TimeSpan.Zero)
			{
				return String.Format("{2}: Dragged by {0} expires in {1} mins", DraggedBy, Expiration.TotalMinutes, Name);
			}
			else
			{
				return String.Format("{1}: Dragged by {0}", DraggedBy, Name);
			}
		}
	}
}

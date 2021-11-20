using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlSaveItem : XmlAttachment
	{
		private class SaveItemPack : Container
		{
			public override int MaxWeight { get { return 0; }}

			public SaveItemPack() : base( 0x9B2 )
			{
			}

			public SaveItemPack( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int) 0 );
			}

			public override void Deserialize(GenericReader reader)
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();
			}
		}

		private Item m_SavedItem;
		private Container m_Container;
		private Mobile m_WasOwnedBy;

		[CommandProperty( AccessLevel.GameMaster )]
		public Container Container
		{
			get { return m_Container; } 
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
			public Item SavedItem 
		{ 
			get
			{ 
				// if the item has been moved off of the internal map, then forget about it
				if(m_SavedItem != null && (m_SavedItem.Parent != m_Container || m_SavedItem.Deleted))
				{
					m_WasOwnedBy = null;
					m_SavedItem = null;
				}

				return m_SavedItem; 
			} 
			set 
			{ 
				// delete any existing item before assigning a new value
				if(SavedItem != null)
				{
                    SafeItemDelete(m_SavedItem);
					//m_SavedItem.Delete();
					m_SavedItem = null;
				}

				// dont allow saving the item if it is attached to it
				if(value != AttachedTo)
				{
					m_SavedItem = value; 
				}

				// automatically internalize any saved item
				if(m_SavedItem != null)
				{
					AddToContainer(m_SavedItem);
					
				}
			} 
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RestoreItem
		{ 
			get{ return false; } 
			set 
			{ 
				if(value == true && SavedItem != null && AttachedTo is IEntity && ((IEntity)AttachedTo).Map != Map.Internal && ((IEntity)AttachedTo).Map != null)
				{

					// move the item to the location of the object the attachment is attached to
					if(AttachedTo is Item)
					{
						m_SavedItem.Map = ((Item)AttachedTo).Map;
						m_SavedItem.Location = ((Item)AttachedTo).Location;
						m_SavedItem.Parent = ((Item)AttachedTo).Parent;
					} else
						if(AttachedTo is Mobile)
					{
						m_SavedItem.Map = ((Mobile)AttachedTo).Map;
						m_SavedItem.Location = ((Mobile)AttachedTo).Location;
						m_SavedItem.Parent = null;
					} 


					m_SavedItem = null;
					m_WasOwnedBy = null;

				}
			} 
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile WasOwnedBy { get{ return m_WasOwnedBy; } set { m_WasOwnedBy = value; } }

		private void AddToContainer(Item item)
		{
			if(item == null) return;

			if(m_Container == null)
			{
				m_Container = new SaveItemPack();
			}

			// need to place in a container to prevent internal map cleanup of the item
			m_Container.DropItem(item);
			m_Container.Internalize();
		}

		public Item GetItem()
		{
			Item returneditem = SavedItem;

			m_SavedItem = null;
			m_WasOwnedBy = null;

			return returneditem;
		}

		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments
       
		// a serial constructor is REQUIRED
		public XmlSaveItem(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlSaveItem()
		{
			m_Container = new SaveItemPack();
		}

		[Attachable]
		public XmlSaveItem(string name)
		{
			Name = name;
		}


		public XmlSaveItem(string name, Item saveditem)
		{
			Name = name;
			SavedItem = saveditem;

		}

		public XmlSaveItem(string name, Item saveditem, Mobile wasownedby)
		{
			Name = name;
			SavedItem = saveditem;
			WasOwnedBy = wasownedby;
		}

		public override void OnDelete()
		{
			base.OnDelete();

			// delete the item
			if(SavedItem != null)
			{
				//SavedItem.Delete();
                SafeItemDelete(SavedItem);
			}

			if(m_Container != null)
			{
                SafeItemDelete(m_Container);
				//m_Container.Delete();
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 0 );
			// version 0
			if(SavedItem != null)
			{
				writer.Write(m_SavedItem);
			} 
			else
			{
				writer.Write((Item)null);
			}
			writer.Write(m_WasOwnedBy);
			writer.Write(m_Container);


		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			// version 0
			m_SavedItem = reader.ReadItem();
			m_WasOwnedBy = reader.ReadMobile();
			m_Container = (Container)reader.ReadItem();

			AddToContainer(m_SavedItem);
		}

		public override string OnIdentify(Mobile from)
		{
			if(from == null || from.AccessLevel == AccessLevel.Player) return null;

			if(Expiration > TimeSpan.Zero)
			{
				return String.Format("{2}: Item {0} expires in {1} mins",SavedItem, Expiration.TotalMinutes, Name);
			} 
			else
			{
				return String.Format("{1}: Item {0}",SavedItem, Name);
			}
		}
	}
}

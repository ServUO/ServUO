using Server;
using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Multis
{
	public class HouseTeleporterTile : HouseTeleporter
	{
		private int _Charges;
		private SecureLevel _Level;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public HouseTeleporter Link
		{
			get
			{
				if(Target != null && Target.Deleted)
					Target = null;
				
				return Target as HouseTeleporter;
			}
			set
			{
				Target = value;
			}
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int Charges
		{
			get { return _Charges; }
			set { _Charges = value; InvalidateProperties(); } 
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public SecureLevel Level
		{
			get { return _Level; }
			set { _Level = value; InvalidateProperties(); } 
		}
		
		[Constructable]
		public HouseTeleporterTile() : this(null, -1)
		{
		}
		
		[Constructable]
		public HouseTeleporterTile(int charges) : this(null, charges)
		{
		}
		
		public HouseTeleporterTile(HouseTeleporter link, int charges) : base(id, link)
		{
			_Charges = charges;
		}
		
		// TODO: Change this to virtual on base class
		public override void CheckAccess(Mobile m)
		{
			BaseHouse house = BaseHouse.FindHouseAt(this);
			
			if(house == null || _Link == null || !IsLockedDown || !_Link.IsLockedDown) // TODO: Messages for these?
			{
				return false;
			}
			
			BaseHouse linkHouse = BaseHouse.FindHouseAt(_Link);
			
			if(house.HasSecureAccess(m, Level))
			{
				if(linkHouse.HasAccess(m) || linkHouse.Public)
				{
					return true;
				}
				else
				{
					// TODO: Message?
					return false;
				}
			}

			// TODO: Message?
			return false;
		}
		
		public override void OnDoubleClick(Mobile m)
		{
			if(IsChildOf(m.Backpack))
			{
				m.SendLocalizedMessage(1114918); // Select a House Teleporter to link to.
				m.BeginTarget(-1, false, Server.Targeting.TargetFlags.None, (from, targeted) =>
				{
					if(targeted is HouseTeleporterTile)
					{
						var tile = targeted as HouseTeleporterTile;
						
						if(tile.IsChildOf(m.Backpack))
						{
							tile.Link = this;
							Link = tile;
						
							from.SendLocalizedMessage(1114919); // The two House Teleporters are now linked.
						}
						else
						{
							from.SendLocalizedMessage(1114917); // This must be in your backpack to link it.
						}
					}
				});
			}
			else
			{
				m.SendLocalizedMessage(1114917); // This must be in your backpack to link it.
			}
		}
		
		public HouseTeleporterTile(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			
			writer.Write(_Charges);	
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			_Charges = reader.ReadInt();
		}
	}
}

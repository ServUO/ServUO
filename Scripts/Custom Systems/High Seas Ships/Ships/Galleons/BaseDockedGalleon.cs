using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;

namespace Server.Multis
{
	public abstract class BaseDockedGalleon : Item
	{
		private int _multiID_South, _multiID_East, _multiID_West, _multiID_North;
		private Direction _chosenDirection;		

		[CommandProperty(AccessLevel.GameMaster)]
		public int MultiID{ get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D Offset{ get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string ShipName{ get; set; }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public abstract BaseGalleon Galleon{ get; }		

		public BaseDockedGalleon(int id, Point3D offset, BaseGalleon galleon) : base(0x14F4)
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
			_multiID_North = id;
			_multiID_South = id + 2;
			_multiID_East = id + 1;
			_multiID_West = id + 3;
			_chosenDirection = Direction.North;
			MultiID = id;			
			Offset = offset;
			ShipName = galleon.ShipName;			
		}

		public BaseDockedGalleon(Serial serial) : base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			from.SendGump(new GalleonPlacementGump(from, null, this));
		}		
		
		public void PlacementDirection(Mobile from, Direction chosenDirection)
		{
			_chosenDirection = chosenDirection;
		
			switch (_chosenDirection)
			{		
				case Direction.West:
				{
					MultiID = _multiID_West;	

					break;
				}	

				case Direction.South:
				{
					MultiID = _multiID_South;	

					break;					
				}

				case Direction.East:
				{
					MultiID = _multiID_East;
					
					break;
				}	

				case Direction.North:
				{
					MultiID = _multiID_North;
					
					break;
				}					
			}
			
			ShipPlacement(from);
		}
		
		public void ShipPlacement(Mobile from)
		{
			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendLocalizedMessage(502482); // Where do you wish to place the ship?
				from.Target = new InternalTarget(this);
			}
		}

		public override void AddNameProperty(ObjectPropertyList list)
		{
			if (ShipName != null)
				list.Add(ShipName);
			else
				base.AddNameProperty(list);
		}

		public override void OnSingleClick(Mobile from)
		{
			if (ShipName != null)
				LabelTo(from, ShipName);
			else
				base.OnSingleClick(from);
		}

		public void OnPlacement(Mobile from, Point3D p)
		{
			if (Deleted)
			{
				return;
			}
			else if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
			}
			else
			{
				Map map = from.Map;

				if (map == null)
					return;

				BaseGalleon galleon = Galleon;

				if (galleon == null)
					return;

				p = new Point3D(p.X - Offset.X, p.Y - Offset.Y, p.Z - Offset.Z);

				if (galleon.CanFit( p, map, galleon.ItemID ) && map != Map.Ilshenar && map != Map.Malas)
				{
					Delete();
					galleon.Owner = from;
					galleon.ShipName = ShipName;
					uint keyValue = galleon.CreateKeys(from);

					if (galleon.Ropes != null)
						for (int i = 0; i < galleon.Ropes.Count; ++i)
							galleon.Ropes[i].KeyValue = keyValue;

					galleon.MoveToWorld(p, map);
					galleon.SetFacing(_chosenDirection);						
				}
				else
				{
					galleon.Delete();
					from.SendLocalizedMessage(1043284); // A ship can not be created here.
				}
			}
		}
		
		#region Serialization
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write((int) 1); // version

			writer.Write(MultiID);
			writer.Write(Offset);
			writer.Write(ShipName);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{				
				case 1:
				{
					MultiID = reader.ReadInt();
					_multiID_North = MultiID;
					_multiID_East = MultiID + 1;
					_multiID_South = MultiID + 2;
					_multiID_West = MultiID + 3;					
					Offset = reader.ReadPoint3D();
					ShipName = reader.ReadString();
					
					goto case 0;
				}
				
				case 0:
				{
					if ( version == 0 )
						reader.ReadUInt();

					break;
				}
			}

			if (LootType == LootType.Newbied)
				LootType = LootType.Blessed;

			if (Weight == 0.0)
				Weight = 1.0;
		}
        #endregion		

		private class InternalTarget : MultiTarget
		{
			private BaseDockedGalleon _model;

			public InternalTarget(BaseDockedGalleon model) : base(model.MultiID, model.Offset)
			{
				_model = model;
			}

			protected override void OnTarget(Mobile from, object o)
			{
				IPoint3D ip = o as IPoint3D;

				if (ip != null)
				{
					if (ip is Item)
						ip = ((Item)ip).GetWorldTop();

					Point3D p = new Point3D( ip );

					Region region = Region.Find( p, from.Map );

					if (region.IsPartOf(typeof(DungeonRegion)))
						from.SendLocalizedMessage(502488); // You can not place a ship inside a dungeon.
					else if (region.IsPartOf(typeof(HouseRegion)) || region.IsPartOf(typeof(ChampionSpawnRegion)))
						from.SendLocalizedMessage(1042549); // A boat may not be placed in this area.
					else
						_model.OnPlacement(from, p);
				}
			}
		}
	}
}
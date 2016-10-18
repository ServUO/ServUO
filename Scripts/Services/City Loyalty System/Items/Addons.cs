using Server;
using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Network;
using Server.Multis;

namespace Server.Items
{
	public abstract class InterchangeableAddonDeed : BaseAddonDeed
	{
		// This will need to be further implemented in any derived class
		public override BaseAddon Addon { get { return null; } }

        public abstract int EastID { get; }
        public abstract int SouthID { get; }

		public InterchangeableAddonDeed()
		{
		}
		
		public override void OnDoubleClick(Mobile from)
		{
			if ( IsChildOf( from.Backpack ) )
				from.Target = new InternalTarget( this );
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}
		
		private class InternalTarget : Target
		{
			public InterchangeableAddonDeed Deed { get; set; }
			
			public InternalTarget(InterchangeableAddonDeed deed) : base( -1, true, TargetFlags.None )
			{
				Deed = deed;
			}
			
			protected override void OnTarget( Mobile from, object targeted )
			{
				IPoint3D p = targeted as IPoint3D;
				Map map = from.Map;
				
				if(p == null || map == null || Deed.Deleted)
					return;
					
				AddonFitResult result = AddonFitResult.Valid;
					
				if(Deed.IsChildOf(from.Backpack))
				{
					BaseHouse house = null;
					
					if ( !map.CanFit( p.X, p.Y, p.Z, 16, false, true, false ) )
						result = AddonFitResult.Blocked;
					else if ( !BaseAddon.CheckHouse( from, new Point3D(p), map, 16, ref house ) )
						result = AddonFitResult.NotInHouse;
					else
					{
						bool east = BaseAddon.IsWall(p.X - 1, p.Y, p.Z, map);
						bool south = BaseAddon.IsWall(p.X, p.Y - 1, p.Z, map);
						
						if(!south && !east)
							result = AddonFitResult.NoWall;
                        else if (south && east)
                            from.SendGump(new AddonInterchangeableGump(from, Deed, p, map));
                        else
                        {
                            BaseAddon addon = Deed.DeployAddon(east, p, map);
                            house.Addons.Add(addon);

                            Deed.Delete();
                        }
					}

                    switch (result)
                    {
                        default: break;
                        case AddonFitResult.Blocked: from.SendLocalizedMessage(500269); break; // You cannot build that there.
                        case AddonFitResult.NotInHouse: from.SendLocalizedMessage(500274); break; // You can only place this in a house that you own!
                        case AddonFitResult.NoWall: from.SendLocalizedMessage(500268); break; // This object needs to be mounted on something.
                    }
				}
				else
				{
					from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				}
			}
		}
		
		public virtual BaseAddon DeployAddon(bool east, IPoint3D p, Map map)
		{
			BaseAddon addon = Addon;
			
			if(addon != null)
			{
				if(addon is InterchangeableAddon)
					((InterchangeableAddon)addon).EastFacing = east;
				
				Server.Spells.SpellHelper.GetSurfaceTop( ref p );
				
				addon.MoveToWorld(new Point3D(p), map);
			}

            return addon;
		}

        public InterchangeableAddonDeed(Serial serial)
            : base(serial)
        {
        }
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}

	public abstract class InterchangeableAddon : BaseAddon
	{
		private bool _EastFacing;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public bool EastFacing
		{
			get
			{
				return _EastFacing;
			}
			set
			{
				if(Components.Count == 1)
				{	
					if(value)
					{
						Components[0].ItemID = EastID;
					}
					else
					{
						Components[0].ItemID = SouthID;
					}
				}
				
				_EastFacing = value;
			}
		}
		
		public abstract int EastID { get; }
		public abstract int SouthID { get; }
		
		// This will need to be further implemented in any derived class
		public override BaseAddonDeed Deed { get { return null; } }
		
		public InterchangeableAddon(bool eastface = true, int loc = 0)
		{
			if(loc != 0)
			    AddComponent(new LocalizedAddonComponent(eastface ? EastID : SouthID, loc), 0, 0, 0);
			else
				AddComponent(new AddonComponent(eastface ? EastID : SouthID), 0, 0, 0);
		}
		
		public InterchangeableAddon(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
	
	public class AddonInterchangeableGump : Gump
	{
		public InterchangeableAddonDeed Deed { get; private set; }
		public IPoint3D Point { get; private set; }
        public Map Map { get; private set; }
		
		public AddonInterchangeableGump(Mobile user, InterchangeableAddonDeed deed, IPoint3D p, Map map) : base(50, 50)
		{
			Deed = deed;
			Point = p;
            Map = map;

            AddGumpLayout();
		}
		
		public void AddGumpLayout()
		{
			AddBackground(0, 0, 300, 200, 2600);
			
			AddHtmlLocalized(0, 15, 300, 16, 1152360, false, false); // <CENTER>Choose a banner:</CENTER>
			
			Rectangle2D b = ItemBounds.Table[Deed.EastID];
			AddItem(70 - b.Width / 2 - b.X, 75 - b.Height / 2 - b.Y, Deed.EastID);
			
			b = ItemBounds.Table[Deed.SouthID];
			AddItem(180 - b.Width / 2 - b.X, 75 - b.Height / 2 - b.Y, Deed.SouthID);

            AddButton(75, 50, 2103, 2104, 1, GumpButtonType.Reply, 0);
            AddButton(205, 50, 2103, 2104, 2, GumpButtonType.Reply, 0);
		}

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (!state.Mobile.InRange(Point, 3))
            {
                state.Mobile.SendLocalizedMessage(500251); // That location is too far away.
            }
            else if (info.ButtonID == 1)
            {
                Deed.DeployAddon(true, Point, Map);
            }
            else if (info.ButtonID == 2)
            {
                Deed.DeployAddon(false, Point, Map);
            }
        }
	}
}